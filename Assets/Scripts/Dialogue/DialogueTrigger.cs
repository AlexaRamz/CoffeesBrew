using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    bool inRange = false;
    public Dialogue[] dialogues;
    DialogueManager dialogueSys;
    public bool isBubble = true;
    public bool randomized;
    void Start()
    {
        dialogueSys = FindObjectOfType<DialogueManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = false;
        }
    }
    void PauseAnim()
    {
        gameObject.transform.Find("Image").GetComponent<Animator>().SetBool("Pause", true);
    }
    public void Disappear()
    {
        if (isBubble)
        {
            gameObject.transform.Find("Image").GetComponent<Animator>().SetTrigger("Poof");
            gameObject.transform.Find("Image").GetComponent<Animator>().SetBool("Pause", false);
            this.enabled = false;
        }
    }
    void Update()
    {
        if (inRange)
        {
            if (Input.GetKeyDown(KeyCode.Return) && dialogueSys.talking == false)
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
}
