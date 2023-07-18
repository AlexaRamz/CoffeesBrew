using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageTrigger : Interactable
{
    StorageManager manager;
    public List<ItemInfo> items;
    public int maxInventory = 30;

    void Start()
    {
        manager = FindObjectOfType<StorageManager>();
    }
    public override void Interact()
    {
        manager.OpenMenu(this, items, maxInventory);
    }
}
