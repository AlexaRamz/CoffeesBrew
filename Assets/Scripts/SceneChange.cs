using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    public string changeTo;
    public bool onInteract = true;
    public Transform teleportTo;
    SceneChangeManager manager;

    void Start()
    {
        manager = FindObjectOfType<SceneChangeManager>();
    }
    bool inRange;
    bool Debounce = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (onInteract == false && Debounce == false)
            {
                Debounce = true;
                ChangeScene();
            }
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
    void Update()
    {
        if (onInteract && inRange)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ChangeScene();
            }
        }
    }
    void ChangeScene()
    {
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
