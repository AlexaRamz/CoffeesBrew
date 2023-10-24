using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageManager : MonoBehaviour, IMenu
{
    public Canvas canvas;
    public InventoryUI storageUI;
    public InventoryUI barUI;
    public InventoryUI invUI;
    public List<ItemInfo> itemInv;
    Inventory plrInv;
    PlayerManager plr;
    StorageTrigger thisStorage;

    void Start()
    {
        plrInv = FindObjectOfType<Inventory>();
        plr = FindObjectOfType<PlayerManager>();
    }
    public void OpenMenu(StorageTrigger storage, List<ItemInfo> items, int maxInv)
    {
        if (!plr.SetCurrentUI(this)) return;
        ClearItems();
        CreateItemList(maxInv);
        thisStorage = storage;
        int i = 0;
        foreach (ItemInfo it in items)
        {
            itemInv[i] = it;
            i++;
        }
        storageUI.SetInventory(items);
        canvas.enabled = true;
        plrInv.storing = true;
        plrInv.SetInventories();
    }
    public void CloseMenu()
    {
        if (!plr.SetCurrentUI(null)) return;
        Debug.Log("close");
        plrInv.storing = false;
        canvas.enabled = false;
        storageUI.ClearInventory();
        ClearItems();
    }
    public Canvas GetCanvas()
    {
        return canvas;
    }
    public ItemInfo GetItem(int slot)
    {
        return itemInv[slot - 1];
    }
    public void RemoveItem(int slot)
    {
        itemInv[slot - 1] = null;
        UpdateInventory();
    }
    public void ChangeItem(ItemInfo item, int slot)
    {
        itemInv[slot - 1] = item;
        UpdateInventory();
    }
    public void UpdateInventory()
    {
        storageUI.SetInventory(itemInv);
        thisStorage.items = itemInv;
    }
    void ClearItems()
    {
        itemInv = new List<ItemInfo>();
        thisStorage = null;
    }
    void CreateItemList(int maxInv)
    {
        for (int i = 0; i < maxInv; i++)
        {
            itemInv.Add(new ItemInfo());
        }
    }
}
