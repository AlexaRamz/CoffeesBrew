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
    public OnTriggerDo interactTrigger;
    public OnTriggerDo pointerTrigger;

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

    Interactable GetInteractingWith()
    {
        List<GameObject> objectsInRange = interactTrigger.GetObjectsInRange();
        Interactable closestObject = null;
        float closestDistance = 100f;
        foreach (GameObject obj in objectsInRange) // Priotize interactable closest to the facing direction and within 90 degrees
        {
            if (obj.GetComponent<Interactable>() != null)
            {
                float distance = Vector2.Distance((obj.transform.position - transform.position).normalized, movement.GetFacingDirection());
                if (distance < closestDistance) // distance < Mathf.Sqrt(2)
                {
                    closestObject = obj.GetComponent<Interactable>();
                    closestDistance = distance;
                }
            }
        }
        return closestObject;
    }

    public Vector2Int GetInteractArrayPos()
    {
        return buildSys.WorldToArray(movement.GetInteractArea());
    }


    // Input handler
    Interactable pointerOn;
    Interactable previousPointerOn;
    public GameObject GetPointerOn()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero);
        List<GameObject> Objects = new List<GameObject>();
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null && hits[i].collider.gameObject.GetComponent<Interactable>())
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
            GraphicRaycaster gr = currentMenu.GetCanvas().GetComponent<GraphicRaycaster>();
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
            List<GameObject> objectsInRange = pointerTrigger.GetObjectsInRange();
            foreach (GameObject obj in objectsInRange)
            {
                if (obj == pointerOn.gameObject)
                {
                    return true;
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
                pointerOn = previousPointerOn = hit.GetComponent<Interactable>();
            }
            else
            {
                pointerOn = null;
            }
            if (pointerOn != null)
            {
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
                if (movement.currentSpeed != 0f)
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
            if (previousPointerOn)
            {
                previousPointerOn.InteractOff();
            }
            pointerOn.Interact();
            pointerOn.Interact(Interactable.InputType.OnClick);
            UpdateCursor();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (previousPointerOn)
            {
                previousPointerOn.InteractOff();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            if (currentMenu != null)
            {
                currentMenu.CloseMenu();
            }
            CursorOff();
        }
        if (Input.GetKeyDown(KeyCode.Return) && plrState == PlayerState.Normal)
        {
            Interactable interact = GetInteractingWith();
            if (interact != null)
            {
                previousPointerOn = interact;
                interact.Interact();
                interact.Interact(Interactable.InputType.OnKey);
                Vector2 dir = (Vector2)(interact.transform.position - transform.position).normalized;
                if (movement.currentSpeed == 0f)
                {
                    movement.SetFacing(new Vector2Int(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y)));
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (previousPointerOn)
            {
                previousPointerOn.InteractOff();
            }
        }
    }
}
