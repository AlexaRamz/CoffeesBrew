using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public Canvas storageCanvas;
    public InventoryUI storageUI;
    public InventoryUI barUI;
    public InventoryUI invUI;
    public List<ItemInfo> itemInv;
    Inventory plrInv;
    StorageTrigger thisStorage;

    void Start()
    {
        plrInv = FindObjectOfType<Inventory>();
    }
    public void OpenMenu(StorageTrigger storage, List<ItemInfo> items, int maxInv)
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
        plrInv.HideInterface();
        storageCanvas.enabled = true;
        plrInv.storing = true;
        plrInv.SetInventories();
    }
    public void CloseMenu()
    {
        plrInv.storing = false;
        storageCanvas.enabled = false;
        plrInv.ShowInterface();
        storageUI.ClearInventory();
        ClearItems();
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
