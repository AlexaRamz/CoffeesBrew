using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IMenu
{
    public List<ItemInfo> itemInv;
    public InventoryUI barUI;
    public InventoryUI hotUI;
    public InventoryUI invUI;
    int maxInv = 24;
    int barCount = 6;
    public Canvas invCanvas;
    public Canvas hotCanvas;

    public int money;
    public ItemInfo currentItem;
    public int currentSlot = 1;
    Transform holder;

    Animator anim;

    public bool moving = false;
    public GameObject movingSlot;
    string movingFromInv;
    public int movingFromSlot;
    ItemInfo movingItem;

    public ItemInfo firstToolObject;

    StorageManager storageManager;
    public bool storing;

    Movement2D plrMovement;
    PlayerManager plr;

    private void Start()
    {
        storageManager = GameObject.Find("StorageSystem").GetComponent<StorageManager>();
        plrMovement = GetComponent<Movement2D>();
        anim = GetComponent<Animator>();
        holder = transform.Find("ItemHolder");
        for (int i = 0; i < maxInv; i++)
        {
            itemInv.Add(new ItemInfo());
        }
        CollectItem(firstToolObject);
        SetInventories();
        plr = FindObjectOfType<PlayerManager>();
    }
    bool open = false;
    public void OpenMenu()
    {
        if (!open && plr.SetCurrentUI(this))
        {
            invCanvas.enabled = true;
            open = true;
        }
    }
    public void CloseMenu()
    {
        if (open && plr.SetCurrentUI(null))
        {
            CancelMoving();
            invCanvas.enabled = false;
            open = false;

        }
    }
    public void ToggleInv()
    {
        if (open)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }
    public Canvas GetCanvas()
    {
        return invCanvas;
    }
    public void ShowInterface()
    {
        hotCanvas.enabled = true;
    }
    public void HideInterface()
    {
        hotCanvas.enabled = false;
    }
    public bool Spend(int amount, ItemInfo item = null)
    {
        if (money >= amount && CollectItem(item))
        {
            money -= amount;
            return true;
        }
        return false;
    }
    public void SetInventories()
    {
        if (storing)
        {
            SetStorageInv();
        }
        barUI.SetInventory(itemInv.GetRange(0, 6), 1);
        hotUI.SetInventory(itemInv.GetRange(0, 6), 1);
        invUI.SetInventory(itemInv.GetRange(6, itemInv.Count - 6), 7);
        ChangeSlot(currentSlot);
    }
    void SetStorageInv()
    {
        storageManager.UpdateInventory();
        storageManager.barUI.SetInventory(itemInv.GetRange(0, 6), 1);
        storageManager.invUI.SetInventory(itemInv.GetRange(6, itemInv.Count - 6), 7);
    }
    public void ApplyMoving(int toSlot, string toInv)
    {
        if (movingFromInv == "storage")
        {
            storageManager.RemoveItem(movingFromSlot);
        }
        else
        {
            RemoveItem(itemInv, movingFromSlot);
        }
        if (toInv == "storage")
        {
            storageManager.ChangeItem(movingItem, toSlot);
        }
        else
        {
            itemInv[toSlot - 1] = movingItem;
        }
        CancelMoving();
    }
    public void CancelMoving()
    {
        movingFromSlot = 0;
        movingItem = null;
        movingSlot.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        movingSlot.GetComponent<Image>().sprite = null;
        moving = false;
        SetInventories();
    }
    public void ChangeMoving(int slot, string inv)
    {
        movingFromSlot = slot;
        movingFromInv = inv;
        ItemInfo item = GetItem(slot, inv);
        movingSlot.GetComponent<Image>().color = new Color32(255, 255, 255, 135);
        movingSlot.GetComponent<Image>().sprite = item.item.asset;
        movingItem = item;
        moving = true;
    }
    public void ChangeItem(ItemInfo item)
    {
        currentItem = item;
        HoldItem();
    }
    public void MoveItem(ItemInfo item, string fromInv, int slot)
    {
        int startSlot = 1;
        int endSlot = maxInv;
        if (storing)
        {
            if (fromInv == "storage")
            {
                if (AddItem(itemInv, item, startSlot, endSlot) == true)
                {
                    storageManager.RemoveItem(slot);
                }
            }
            else
            {
                endSlot = storageManager.itemInv.Count;
                if (AddItem(storageManager.itemInv, item, startSlot, endSlot) == true)
                {
                    RemoveItem(itemInv, slot);
                }
            }
        }
        else
        {
            if (fromInv == "hotbar")
            {
                startSlot = barCount + 1;
            }
            else if (fromInv == "inventory")
            {
                endSlot = barCount;
            }
            if (AddItem(itemInv, item, startSlot, endSlot) == true)
            {
                RemoveItem(itemInv, slot);
            }
        }
        SetInventories();
    }
    void RemoveItem(List<ItemInfo> inventory, int i)
    {
        inventory[i - 1] = null;
        plr.UpdateCursor();
    }
    public bool AddItem(List<ItemInfo> inventory, ItemInfo item, int startSlot=1, int endSlot=24)
    {
        for (int i = startSlot - 1; i < endSlot; i++)
        {
            ItemInfo Item = inventory[i];
            if (Item == null || Item.item == null)
            {
                inventory[i] = item;
                return true;
            }
            else if (Item.item.name == item.item.name && Item.item.stackMax >= (Item.amount + item.amount))
            {
                Item.amount += item.amount;
                return true;
            }
        }
        plr.UpdateCursor();
        Debug.Log("Inventory full!");
        return false;
    }
    public bool DepleteItem(int slot, int amount)
    {
        ItemInfo item = itemInv[slot - 1];
        if (item.amount - amount < 0)
        {
            return false;
        }
        item.amount -= amount;
        if (item.amount == 0)
        {
            RemoveItem(itemInv, slot);
        }
        SetInventories();
        return true;
    }
    public Item GetCurrentItem()
    {
        if (currentItem != null)
        {
            return currentItem.item;
        }
        return null;
    }
    public int GetCurrentItemAmount()
    {
        if (currentItem != null)
        {
            return currentItem.amount;
        }
        return 0;
    }
    public int GetItemAmount(int slot)
    {
        ItemInfo item = GetItem(slot);
        if (item != null)
        {
            return item.amount;
        }
        return 0;
    }
    public bool DepleteCurrentItem()
    {
        return DepleteItem(currentSlot, 1);
    }
    public ItemInfo GetItem(int slot, string inv = "")
    {
        if (inv == "storage")
        {
            return storageManager.GetItem(slot);
        }
        return itemInv[slot - 1];
    }
    //Item transfer functions
    public bool CollectItem(ItemInfo item)
    {
        if (AddItem(itemInv, item))
        {
            SetInventories();
            return true;
        }
        return false;
    }
    public bool CheckItem(ItemInfo item)
    {
        Item it = item.item;
        int amount = item.amount;
        int count = 0;
        foreach (ItemInfo i in itemInv)
        {
            if (i != null && i.item == it)
            {
                count += i.amount;
                if (count >= amount)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool GiveItem(ItemInfo item)
    {
        if (CheckItem(item))
        {
            Give(item);
            SetInventories();
            return true;
        }
        return false;
    }
    void Give(ItemInfo item)
    {
        Item it = item.item;
        int amount = item.amount;
        int count = amount;
        int i = 0;
        while (i < itemInv.Count)
        {
            ItemInfo ite = itemInv[i];
            if (ite != null && ite.item == it)
            {
                if (ite.amount >= count)
                {
                    ite.amount -= count;
                    if (ite.amount == 0)
                    {
                        itemInv[i] = null;
                    }
                    return;
                }
                count -= ite.amount;
                itemInv[i] = null;
            }
            i++;
        }
    }
    public bool CheckItems(List<ItemInfo> items)
    {
        foreach (ItemInfo it in items)
        {
            if (CheckItem(it) == false)
            {
                return false;
            }
        }
        return true;
    }
    public bool GiveItems(List<ItemInfo> items)
    {
        if (CheckItems(items))
        {
            foreach (ItemInfo it in items)
            {
                Give(it);
            }
            SetInventories();
            return true;
        }
        return false;
    }


    public void ChangeSlot(int slot)
    {
        currentSlot = slot;
        if (slot <= barCount)
        {
            ChangeItem(itemInv[slot - 1]);
        }
    }
    public void SelectSlot(int slot)
    {
        hotUI.SelectSlot(slot);
        ChangeSlot(slot);
        plr.UpdateCursor();
    }
    public void HoldItem()
    {
        SpriteRenderer renderer = holder.GetComponent<SpriteRenderer>();
        if (currentItem != null && currentItem.item != null)
        {
            renderer.sprite = currentItem.item.asset;
            renderer.enabled = true;
            anim.SetBool("Holding", true);
            plrMovement.SetHolding(true);
        }
        else
        {
            renderer.enabled = false;
            anim.SetBool("Holding", false);
            plrMovement.SetHolding(false);
        }
    }
    void Update()
    {
        if (moving)
        {
            movingSlot.transform.position = Input.mousePosition;
        }
        if (Input.GetKeyDown("c"))
        {
            ToggleInv();
        }
        else if (Input.GetKeyDown("1"))
        {
            SelectSlot(1);
        }
        else if (Input.GetKeyDown("2"))
        {
            SelectSlot(2);
        }
        else if (Input.GetKeyDown("3"))
        {
            SelectSlot(3);
        }
        else if (Input.GetKeyDown("4"))
        {
            SelectSlot(4);
        }
        else if (Input.GetKeyDown("5"))
        {
            SelectSlot(5);
        }
        else if (Input.GetKeyDown("6"))
        {
            SelectSlot(6);
        }
        else if (Input.GetKeyDown("z"))
        {
            if (currentSlot > 1)
            {
                SelectSlot(currentSlot - 1);
            }
            else
            {
                SelectSlot(barCount);
            }
        }
        else if (Input.GetKeyDown("x"))
        {
            if (currentSlot < barCount)
            {
                SelectSlot(currentSlot + 1);
            }
            else
            {
                SelectSlot(1);
            }
        }
    }
}
