using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageTrigger : MonoBehaviour
{
    StorageManager manager;
    public List<ItemInfo> items;
    public int maxInventory = 30;

    void Start()
    {
        manager = FindObjectOfType<StorageManager>();
    }
    public void OpenStorage()
    {
        manager.ToggleMenu(this, items, maxInventory);
    }
}
