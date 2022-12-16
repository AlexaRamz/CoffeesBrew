using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    Inventory invManager;
    public StorageManager storageManager;
    public GameObject slotTemplate;
    public string inv;
    public Canvas hoverDesc;

    void Start()
    {
        invManager = FindObjectOfType<Inventory>();
    }
    public void ChangeSlot(int slot)
    {
        //Change slot selection for main hot bar
        invManager.ChangeSlot(slot);
    }
    public void SelectSlot(int slot)
    {
        ResetSlot();
        UseItem Slot = transform.GetChild(slot - 1).GetComponent<UseItem>();
        Slot.SelectSlot();
    }
    void ResetSlot()
    {
        foreach (Transform slot in transform)
        {
            slot.GetComponent<UseItem>().DeselectSlot();
        }
    }
    public void SetDesc(Item item, Vector3 pos)
    {
        string text = item.name + '\n' + item.description;
        hoverDesc.transform.Find("Text").GetComponent<Text>().text = text;
        hoverDesc.transform.position = new Vector3(pos.x + hoverDesc.GetComponent<RectTransform>().sizeDelta.x / 2, pos.y + hoverDesc.GetComponent<RectTransform>().sizeDelta.y / 2, pos.z);
        hoverDesc.enabled = true;
    }
    public void ResetDesc()
    {
        hoverDesc.enabled = false;
        hoverDesc.transform.Find("Text").GetComponent<Text>().text = "";
    }
    public void ClearInventory()
    {
        foreach (Transform slot in transform)
        {
            Destroy(slot.gameObject);
        }
    }
    public void SetInventory(List<ItemInfo> inventory, int startIndex = 1)
    {
        ClearInventory();
        int slot = startIndex;
        foreach (ItemInfo item in inventory)
        {
            UseItem itemSlot = Instantiate(slotTemplate, transform).GetComponent<UseItem>();
            itemSlot.SetItem(item, slot, this);
            if (inv == "hotbar2" && slot == invManager.currentSlot)
            {
                itemSlot.SelectSlot();
            }
            slot += 1;
        }
    }
}
