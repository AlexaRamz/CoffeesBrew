using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : Interactable
{
    public string changeTo;
    public Transform teleportTo;
    SceneChangeManager manager;
    public bool onTrigger;

    void Start()
    {
        manager = FindObjectOfType<SceneChangeManager>();
    }
    public override void Interact()
    {
        if (!onTrigger)
        {
            ChangeScene();
        }
    }
    public void ChangeScene()
    {
        if (teleportTo != null)
        {
            manager.StartCoroutine(manager.TeleportTo(teleportTo.position));
        }
        else
        {
            manager.StartCoroutine(manager.LoadScene(changeTo));
        }
    }
}
