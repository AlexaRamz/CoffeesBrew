using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public enum PlayerState
    {
        Normal, 
        Frozen, // Movement and world interactions not allowed (typically when in a menu)
    }
    PlayerState plrState;

    public BuildingSystem buildSys;
    Movement2D movement;
    public Tilemap tilemap;
    public IMenu currentMenu;
    Inventory plrInv;
    public int interactRange = 3;

    private void Start()
    {
        movement = GetComponent<Movement2D>();
        plrInv = GetComponent<Inventory>();

        timeLeft = cursorActiveTime;
        Cursor.visible = false;
    }
    public bool SetCurrentUI(IMenu thisMenu)
    {
        if (currentMenu == null && thisMenu != null)
        {
            plrInv.HideInterface();
            currentMenu = thisMenu;
            plrState = PlayerState.Frozen;
            movement.SetPlrActive(false);
            cursor.sprite = normalCursor;
            cursor.color = new Color32(255, 255, 255, 255);

            return true;
        }
        else if (currentMenu != null && thisMenu == null)
        {
            plrInv.ShowInterface();
            currentMenu = null;
            plrState = PlayerState.Normal;
            movement.SetPlrActive(true);
            CursorOff();
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
        if (plrState == PlayerState.Normal)
        {
            Vector3Int targetedPos = tilemap.WorldToCell(movement.GetInteractArea());
            Vector3Int originPos = tilemap.WorldToCell(pos);
            for (int i = 0; i < objectSize.x; i++)
            {
                for (int j = 0; j < objectSize.y; j++)
                {
                    if (new Vector3Int(originPos.x + i, originPos.y + j, originPos.z) == targetedPos)
                    {
                        return true;
                    }
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
    Interactable pointerOn;
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
        if (currentMenu != null)
        {
            GraphicRaycaster gr = currentMenu.GetGraphicRaycaster();
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(ped, results);
            return results.Count != 0;
        }
        return false;
    }
    public float cursorActiveTime = 5f;
    float timeLeft;
    bool cursorMoving = false;
    bool cursorOn = true;
    public Image cursor;
    public Sprite interactCursor;
    public Sprite normalCursor;

    void CursorOn()
    {
        cursorOn = true;
        cursor.enabled = true;
        Cursor.lockState = CursorLockMode.None;
    }
    void CursorOff()
    {
        cursorOn = false;
        cursor.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    bool CanInteractWith()
    {
        bool InRange()
        {
            Vector2Int plrTile = (Vector2Int)tilemap.WorldToCell(transform.position);
            Vector2Int interactTile = (Vector2Int)tilemap.WorldToCell(pointerOn.transform.position);
            for (int x = 0; x < pointerOn.objectSize.x; x++)
            {
                for (int y = 0; y < pointerOn.objectSize.y; y++)
                {
                    if (Vector2Int.Distance(plrTile, new Vector2Int(interactTile.x + x, interactTile.y + y)) <= interactRange)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        return pointerOn != null && pointerOn.CanInteract() && (InRange() || (pointerOn.GetComponent<BuildTrigger>() && pointerOn.GetComponent<BuildTrigger>().info.build.category == Build.ObjectType.WallDecor));
    }
    public void UpdateCursor()
    {
        if (cursorOn && plrState == PlayerState.Normal && !CheckOnUI())
        {
            GameObject hit = GetPointerOn();
            if (hit != null)
            {
                pointerOn = hit.GetComponent<Interactable>();
            }
            else
            {
                pointerOn = null;
            }
            if (pointerOn != null)
            {
                Debug.Log("yes");
                cursor.sprite = interactCursor;
                if (CanInteractWith())
                {
                    cursor.color = new Color32(255, 255, 255, 255);
                }
                else
                {
                    cursor.color = new Color32(255, 255, 255, 128);
                }
            }
            else
            {
                Debug.Log("no");
                cursor.sprite = normalCursor;
                cursor.color = new Color32(255, 255, 255, 255);
            }
        }
        cursor.GetComponent<RectTransform>().position = Input.mousePosition;
    }
    private void Update()
    {
        if (Input.GetAxis("Mouse X") == 0f && Input.GetAxis("Mouse Y") == 0f)
        {
            if (cursorOn)
            {
                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0)
                {
                    cursorOn = false;
                    CursorOff();
                }
                if (movement.movementSpeed != 0f)
                {
                    UpdateCursor();
                }
            }
            if (cursorMoving)
            {
                cursorMoving = false;
            }
        }
        else
        {
            if (!cursorMoving)
            {
                cursorMoving = true;
                timeLeft = cursorActiveTime;
                CursorOn();
            }
            UpdateCursor();
        }

        if (Input.GetMouseButtonDown(0) && plrState == PlayerState.Normal && CanInteractWith() && !CheckOnUI())
        {
            pointerOn.Interact();
            pointerOn.Interact(Interactable.InputType.OnClick);
            UpdateCursor();
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            if (currentMenu != null)
            {
                currentMenu.CloseMenu();
            }
            CursorOff();
        }
    }
}
