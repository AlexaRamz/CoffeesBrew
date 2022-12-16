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
    GameObject plr;
    BuildingSystem buildSys;

    public void SetObject(Build build, int rot, int x, int y)
    {
        buildSys = FindObjectOfType<BuildingSystem>();
        info.build = build;
        info.rot = rot;
        info.gridPos = new Vector2Int(x, y);
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        render.sprite = info.GetRotation().sprite;
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
            gameObject.AddComponent<SortingGroup>();
            SetPlacements();
        }
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
        decor = new GameObject[info.GetPlacementAmount()];
        info.SetPlacementArray();
    }
    bool inRange;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = true;
            plr = collision.gameObject;
            plrInv = collision.gameObject.GetComponent<Inventory>();
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = false;
            plr = null;
            plrInv = null;
        }
    }
    void ClearPlacement(int place)
    {
        info.decor[place] = null;
        Destroy(decor[place]);
    }
    void Update()
    {
        if (inRange && plrInv != null && info.build.canDecorate)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Vector2[] placements = info.GetPlacements();
                Vector2 nearestPlacement = Vector2.zero;
                float nearestDistance = float.MaxValue;
                int place = 0;
                for (int i = 0; i < placements.Length; i++)
                {
                    Vector2 placement = placements[i];
                    Vector3 pos = new Vector3(transform.position.x + placement.x, transform.position.y + placement.y, transform.position.z);
                    float distance = (pos - plr.transform.position).sqrMagnitude;
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestPlacement = pos;
                        place = i;
                        Debug.Log(distance);
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
                        GameObject obj = new GameObject("Decor");
                        obj.transform.parent = transform;
                        obj.transform.position = nearestPlacement;
                        SpriteRenderer render = obj.AddComponent<SpriteRenderer>();
                        render.sortingOrder = 1;
                        render.sprite = item.asset;
                        info.decor[place] = item;
                        decor[place] = obj;
                    }
                }
            }
        }
    }
}

