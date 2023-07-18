using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : Interactable
{
    public Dialogue[] dialogues;
    DialogueManager dialogueSys;
    public bool isBubble = true;
    public bool randomized;
    void Start()
    {
        dialogueSys = FindObjectOfType<DialogueManager>();
    }
    void PauseAnim()
    {
        gameObject.transform.Find("Image").GetComponent<Animator>().SetBool("Pause", true);
    }
    public void Disappear()
    {
        if (isBubble)
        {
            Animator anim = transform.Find("Image").GetComponent<Animator>();
            anim.SetTrigger("Poof");
            anim.SetBool("Pause", false);
            this.enabled = false;
        }
    }
    public override void Interact()
    {
        if (dialogueSys.talking == false)
        {
            Dialogue dialogue = dialogues[0];
            if (randomized && dialogues.Length > 1)
            {
                dialogue = dialogues[Random.Range(0, dialogues.Length)];
                Debug.Log("ddd");
            }
            dialogueSys.StartDialogue(dialogue);
            dialogueSys.currentTrigger = this;
            if (isBubble)
            {
                PauseAnim();
            }
        }
    }
}
