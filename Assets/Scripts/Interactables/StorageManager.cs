using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
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
    public void ToggleMenu(StorageTrigger storage, List<ItemInfo> items, int maxInv)
    {
        if (canvas.enabled)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu(storage, items, maxInv);
        }
    }
    public void OpenMenu(StorageTrigger storage, List<ItemInfo> items, int maxInv)
    {
        if (plr.SetCurrentUI(canvas))
        {
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
    }
    public void CloseMenu()
    {
        if (plr.SetCurrentUI(null))
        {
            plrInv.storing = false;
            canvas.enabled = false;
            storageUI.ClearInventory();
            ClearItems();
        } 
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
