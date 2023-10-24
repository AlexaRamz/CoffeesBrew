using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResponseButton : MonoBehaviour
{
    public Image star;
    public Text text;
    DialogueManager dialogueSys;
    int responseIndex;

    public void SetResponse(DialogueManager manager, string thisText, int thisIndex)
    {
        dialogueSys = manager;
        text.text = thisText;
        responseIndex = thisIndex;
    }
    public void Select()
    {
        star.enabled = true;
        text.color = new Color32(255, 255, 255, 255);
    }
    public void Deselect()
    {
        star.enabled = false;
        text.color = new Color32(255, 255, 255, 180);
    }
    public void Respond()
    {
        dialogueSys.Respond(responseIndex);
    }
    public void UpdateSelection()
    {
        dialogueSys.UpdateResponseSelection(responseIndex);
    }
}
