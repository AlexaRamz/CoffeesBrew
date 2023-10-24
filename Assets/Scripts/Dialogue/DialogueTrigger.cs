using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : Interactable
{
    public NPC speaker;
    DialogueManager dialogueSys;

    void Start()
    {
        dialogueSys = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
    }
    public override void Interact()
    {
        if (dialogueSys.talking == false && !CheckOrderFulfill())
        {
            dialogueSys.StartDialogue(speaker.GetDialogue(), speaker);
        }
    }
    bool CheckOrderFulfill()
    {
        Order foundOrder = GameObject.Find("OrderSystem").GetComponent<OrderSystem>().CheckOrderFulfill(speaker);
        if (foundOrder != null)
        {
            dialogueSys.StartDialogue(speaker.GetDialogue(NPC.DialogueType.OrderFulfill), speaker);
            return true;
        }
        return false;
    }
}
