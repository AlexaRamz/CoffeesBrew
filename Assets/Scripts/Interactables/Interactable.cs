using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
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
    public virtual void InteractOff() // Key up or mouse click up
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
