using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BuildTrigger : Interactable
{
    //Handles build actions on interact such as decorating and movement
    public BuildInfo info;
    Inventory plrInv;
    public GameObject[] decor;
    PlayerManager plr;
    BuildingSystem buildSys;

    void Start()
    {
        plrInv = FindObjectOfType<Inventory>();
    }
    public void SetObject(BuildInfo thisInfo, Vector2Int gridPos)
    {
        plr = FindObjectOfType<PlayerManager>();

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
        if (build.category == Build.ObjectType.Table || build.category == Build.ObjectType.Seat || build.category == Build.ObjectType.Furniture)
        {
            SetCollider();
        }
        else if (build.category == Build.ObjectType.FloorDecor)
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
    public bool CanDecorate()
    {
        return info.build && info.build.canDecorate;
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
    public void RemoveDecor(GameObject thisItem)
    {
        int i = 0;
        foreach (GameObject item in decor)
        {
            if (item == thisItem)
            {
                ClearPlacement(i);
                return;
            }
            i++;
        }
    }
    public void PlaceDecor(Item item, int place)
    {
        GameObject obj = new GameObject("Decor");
        obj.transform.parent = transform;
        obj.transform.position = GetPlacePos(place); 
        SpriteRenderer render = obj.AddComponent<SpriteRenderer>();
        render.sortingOrder = 1;
        render.sprite = item.asset;
        BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        obj.tag = "Decor Item";
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
    public Vector3 GetPlacePos(int place)
    {
        Placement thisPlacement = info.GetPlacement(place);
        return transform.position + (Vector3Int)thisPlacement.position + (Vector3)thisPlacement.offset;
    }
    public int GetNearestPlace(Vector3 worldPos)
    {
        float min = 1000;
        int place = 0;
        Placement[] placements = info.GetPlacements();
        for (int i = 0; i < placements.Length; i++)
        {
            Placement thisPlacement = placements[i];
            float distance = Vector3.Distance(transform.position + (Vector3)(thisPlacement.position + thisPlacement.offset), worldPos);
            if (distance < min)
            {
                place = i;
                min = distance;
            }
        }
        return place;
    }
    public bool PlaceEmpty(int place)
    {
        return info.decor[place] == null;
    }
    int GetFacingPlace()
    {
        Placement[] placements = info.GetPlacements();
        for (int i = 0; i < placements.Length; i++)
        {
            if (plr.GetInteractArrayPos() == info.gridPos + placements[i].position)
            {
                return i;
            }
        }
        return 0;
    }
    public void Decorate(int place)
    {
        if (CanDecorate())
        {
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
                    PlaceDecor(item, place);
                }
            }
        }
    }
    // Decor Interactable
    public override void Interact(InputType input)
    {
        if (input == InputType.OnKey)
        {
            Decorate(GetFacingPlace());
        }
        else if (input == InputType.OnClick)
        {
            Decorate(GetNearestPlace(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
    }
    public override bool CanInteract()
    {
        if (CanDecorate())
        {
            Item current = info.decor[GetNearestPlace(Camera.main.ScreenToWorldPoint(Input.mousePosition))];
            if (current == null)
            {
                if (plrInv.GetCurrentItem() != null)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        return false;
    }
}

