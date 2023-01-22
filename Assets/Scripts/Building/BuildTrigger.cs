using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BuildTrigger : MonoBehaviour
{
    //Handles build actions on interact such as decorating and movement
    public BuildInfo info;
    Inventory plrInv;
    public GameObject[] decor;
    Movement2D plr;
    BuildingSystem buildSys;

    void Start()
    {
        plr = FindObjectOfType<Movement2D>();
        plrInv = FindObjectOfType<Inventory>();
    }
    public void SetObject(BuildInfo thisInfo, Vector2Int gridPos)
    {
        buildSys = FindObjectOfType<BuildingSystem>();
        info.SetInfo(thisInfo.build, thisInfo.rot);
        info.gridPos = gridPos;
        //decor
 
        Build build = info.build;
        gameObject.name = build.name;
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        render.sprite = info.GetRotation().sprite;
        //UpdateSprite();
        render.flipX = info.GetRotation().flipped;
        SetTrigger();
        if (build.Type == Build.ObjectType.Furniture)
        {
            SetCollider();
        }
        else if (build.Type == Build.ObjectType.FloorDecor)
        {
            render.sortingOrder = -1;
        }
        if (build.canDecorate)
        {
            if (!gameObject.GetComponent<SortingGroup>())
            {
                gameObject.AddComponent<SortingGroup>();
            }
            SetPlacements();
        }
    }
    public void SetColor(Color32 color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
    public bool HasRuleBuilds()
    {
        return info.GetRotation().ruleBuilds.Length > 0;
    }
    public bool UpdateSprite()
    {
        buildSys = FindObjectOfType<BuildingSystem>();
        if (HasRuleBuilds())
        {
            GameObject[] adjacentObjects = buildSys.GetAdjacentObjects(info.gridPos);
            GameObject[] cornerObjects = buildSys.GetCornerObjects(info.gridPos);
            foreach (RuleBuild ruleBuild in info.GetRotation().ruleBuilds)
            {
                bool CheckRule(GameObject[] objects, RuleBuild.BuildType[] rules)
                {
                    int i = 0;
                    foreach (RuleBuild.BuildType rule in rules)
                    {
                        GameObject obj = objects[i];
                        if (rule == RuleBuild.BuildType.Self && (obj == null || (obj.GetComponent<BuildTrigger>() && obj.GetComponent<BuildTrigger>().info.build != info.build)) || (rule == RuleBuild.BuildType.Empty && obj != null) || (rule == RuleBuild.BuildType.Other && obj != null && obj.GetComponent<BuildTrigger>() && obj.GetComponent<BuildTrigger>().info.build == info.build))
                        {
                            return false;
                        }
                        i++;
                    }

                    return true;
                }
                if (CheckRule(adjacentObjects, ruleBuild.adjacentBuilds) && CheckRule(cornerObjects, ruleBuild.cornerBuilds))
                {
                    GetComponent<SpriteRenderer>().sprite = ruleBuild.sprite;
                    GetComponent<SpriteRenderer>().flipX = ruleBuild.flipped;
                    return true;
                }
            }
        }
        return false;
    }
    public Vector2Int GetPos()
    {
        return info.gridPos;
    }
    public void SetPos(Vector2Int pos)
    {
        info.gridPos = pos;
    }
    void SetCollider()
    {
        if (GetComponent<PolygonCollider2D>() != null)
        {
            Destroy(GetComponent<PolygonCollider2D>());
        }
        gameObject.AddComponent<PolygonCollider2D>();
    }
    void SetTrigger()
    {
        if (GetComponent<BoxCollider2D>() != null)
        {
            Destroy(GetComponent<BoxCollider2D>());
        }
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
    }
    void SetPlacements()
    {
        if (decor.Length == 0)
        {
            decor = new GameObject[info.GetPlacementAmount()];
            info.SetPlacementArray();
        }
    }
    public void ClearPlacement(int place)
    {
        info.decor[place] = null;
        Destroy(decor[place]);
    }
    void PlaceDecor(Item item, Placement thisPlacement, int place)
    {
        GameObject obj = new GameObject("Decor");
        obj.transform.parent = transform;
        obj.transform.position = transform.position + (Vector3Int)thisPlacement.position + (Vector3)thisPlacement.offset;
        BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        SpriteRenderer render = obj.AddComponent<SpriteRenderer>();
        render.sortingOrder = 1;
        render.sprite = item.asset;
        info.decor[place] = item;
        decor[place] = obj;
    }
    public int GetDecorPlacement(GameObject thisObj)
    {
        int i = 0;
        foreach (GameObject obj in decor)
        {
            if (thisObj == obj)
            {
                return i;
            }
            i++;
        }
        return -1;
    }
    void Update()
    {
        if (plrInv != null && info.build && info.build.canDecorate)
        {
            if (Input.GetKeyDown(KeyCode.Return) && plr.isInteractingWithObject(gameObject))
            {
                int place = 0;
                Placement[] placements = info.GetPlacements();
                for (int i = 0; i < placements.Length; i++)
                {
                    if (plr.GetInteractArrayPos() == info.gridPos + placements[i].position)
                    {
                        place = i;
                        break;
                    }
                }

                Item current = info.decor[place];
                if (current != null)
                {
                    if (plrInv.CollectItem(new ItemInfo { item = current, amount = 1 }))
                    {
                        ClearPlacement(place);
                    }
                }
                else
                {
                    Item item = plrInv.GetCurrentItem();
                    if (item != null)
                    {
                        plrInv.DepleteCurrentItem();
                        PlaceDecor(item, info.GetPlacement(place), place);
                    }
                }
            }
        }
    }
}

