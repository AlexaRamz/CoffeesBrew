using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    PlayerManager plr;
    public Vector2Int objectSize = new Vector2Int(1, 1);
    public KeyCode interactKey = KeyCode.Return;
    public bool onClick = true;
    public UnityEvent interactAction;

    void Start()
    {
        plr = FindObjectOfType<PlayerManager>();
        // this interactable *subscribes* its own function to the onClick event
        plr.onMouseDown += InteractOnClick;
    }
    void Update()
    {
        if (Input.GetKeyDown(interactKey) && plr.isInteractingWith(transform.position, objectSize))
        {
            interactAction.Invoke();
        }
    }
    void InteractOnClick()
    {
        if (onClick)
        {
            if (plr.pointerOn == gameObject)
            {
                interactAction.Invoke();
            }
        }
    }
}
