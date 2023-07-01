using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    public string changeTo;
    public Transform teleportTo;
    SceneChangeManager manager;

    void Start()
    {
        manager = FindObjectOfType<SceneChangeManager>();
    }
    bool Debounce = false;
    public void ChangeScene()
    {
        if (!Debounce)
        {
            Debounce = true;
            if (teleportTo != null)
            {
                manager.StartCoroutine(manager.TeleportTo(teleportTo.position));
            }
            else
            {
                manager.StartCoroutine(manager.LoadScene(changeTo));
            }
            Debounce = false;
        }
    }
}
