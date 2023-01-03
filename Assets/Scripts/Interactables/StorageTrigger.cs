using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageTrigger : MonoBehaviour
{
    StorageManager manager;
    public List<ItemInfo> items;
    public int maxInventory = 30;
    Movement2D plr;

    void Start()
    {
        manager = FindObjectOfType<StorageManager>();
        plr = FindObjectOfType<Movement2D>();
    }
    bool open;
    void Update()
    {
        if (manager != null)
        {
            if (Input.GetKeyDown(KeyCode.Return) && plr.isInteractingWith(transform.position))
            {
                if (open)
                {
                    manager.CloseMenu();
                    open = false;
                }
                else
                {
                    manager.OpenMenu(this, items, maxInventory);
                    open = true;
                }
            }
        }
    }
}
