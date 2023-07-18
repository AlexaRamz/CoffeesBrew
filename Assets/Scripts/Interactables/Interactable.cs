using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public Vector2Int objectSize = new Vector2Int(1, 1);
    public KeyCode interactKey = KeyCode.Return;

    void Update()
    {
        if (Input.GetKeyDown(interactKey) && FindObjectOfType<PlayerManager>().isInteractingWith(transform.position, objectSize))
        {
            Interact();
            Interact(InputType.OnKey);
        }
    }
    public virtual bool CanInteract()
    {
        return true;
    }
    public void UpdateState() // Update the cursor when the "can interact" state of the interactable has changed
    {
        FindObjectOfType<PlayerManager>().UpdateCursor();
    }
    public virtual void Interact()
    {
        
    }
    public enum InputType
    {
        OnKey,
        OnClick,
    }
    public virtual void Interact(InputType input) // Cares about input type
    {

    }
}
