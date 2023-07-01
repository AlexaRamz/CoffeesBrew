using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public BuildingSystem buildSys;
    Movement2D movement;
    public Tilemap tilemap;
    public Canvas currentUI;
    Inventory plrInv;

    private void Start()
    {
        movement = GetComponent<Movement2D>();
        plrInv = GetComponent<Inventory>();
    }
    public bool SetCurrentUI(Canvas thisUI)
    {
        if (!currentUI && thisUI)
        {
            plrInv.HideInterface();
            currentUI = thisUI;
            return true;
        }
        else if (currentUI && !thisUI)
        {
            plrInv.ShowInterface();
            currentUI = null;
            return true;
        }
        return false;
    }

    public bool isInteractingWith(Vector3 pos)
    {
        Vector3Int targetedPos = tilemap.WorldToCell(movement.GetInteractArea());
        return targetedPos == tilemap.WorldToCell(pos);
    }
    public bool isInteractingWith(Vector3 pos, Vector2Int objectSize)
    {
        Vector3Int targetedPos = tilemap.WorldToCell(movement.GetInteractArea());
        Vector3Int originPos = tilemap.WorldToCell(pos);
        for (int i = 0; i < objectSize.x; i++) {
            for (int j = 0; j < objectSize.y; j++)
            {
                if (new Vector3Int(originPos.x + i, originPos.y + j, originPos.z) == targetedPos)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool isInteractingWithObject(GameObject obj)
    {
        if (buildSys)
        {
            Vector2Int arrayPos = GetInteractArrayPos();
            GameObject targetedObj = buildSys.GetValue(arrayPos.x, arrayPos.y);
            return targetedObj == obj;
        }
        return false;
    }
    public Vector2Int GetInteractArrayPos()
    {
        return buildSys.WorldToArray(movement.GetInteractArea());
    }


    // Input handler
    [HideInInspector] public GameObject pointerOn;
    public GameObject GetPointerOn()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero);
        List<GameObject> Objects = new List<GameObject>();
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null && (hits[i].collider.gameObject.GetComponent<BuildTrigger>() || hits[i].collider.gameObject.GetComponent<Interactable>()))
            {
                Objects.Add(hits[i].collider.gameObject);
            }
        }
        Objects = Objects.OrderBy(e => e.transform.position.y).ToList();
        GameObject hit = null;
        if (Objects.Count > 0)
        {
            hit = Objects[0];
        }
        return hit;
    }
    public bool CheckOnUI()
    {
        // Check on UI
        if (currentUI != null)
        {
            GraphicRaycaster gr = currentUI.gameObject.GetComponent<GraphicRaycaster>();
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(ped, results);
            return results.Count != 0;
        }
        return false;
    }
    public delegate void MyDelegate();
    public MyDelegate onMouseDown;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !CheckOnUI())
        {
            pointerOn = GetPointerOn();
            onMouseDown?.Invoke();
        }
        if (Input.GetMouseButtonUp(0))
        {
            pointerOn = null;
        }
    }
}
