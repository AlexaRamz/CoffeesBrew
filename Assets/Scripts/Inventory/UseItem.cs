using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseItem : MonoBehaviour
{
    Inventory invManager;
    InventoryUI invUI;
    public int slotNum;

    public void SetItem(ItemInfo item, int slot, InventoryUI inv)
    {
        invManager = FindObjectOfType<Inventory>();
        slotNum = slot;
        invUI = inv;
        Image image = transform.Find("Image").GetComponent<Image>();
        if (item != null && item.item != null)
        {
            image.sprite = item.item.asset;
            image.color = new Color32(255, 255, 255, 255);
            Text txt = transform.Find("Text").GetComponent<Text>();
            if (item.amount > 1)
            {
                txt.text = item.amount.ToString();
            }
        }
        else
        {
            image.color = new Color32(255, 255, 255, 0);
        }
    }
    public void SelectSlot()
    {
        transform.Find("Slot").GetComponent<Image>().enabled = true;
    }
    public void DeselectSlot()
    {
        transform.Find("Slot").GetComponent<Image>().enabled = false;
    }
    public void UseThisItem()
    {
        if (invUI.inv == "hotbar2")
        {
            //Change main hotbar slot
            invManager.SelectSlot(slotNum);
        }
        else
        {
            //Move item
            ItemInfo item = invManager.GetItem(slotNum, invUI.inv);
            if (invManager.moving == false)
            {
                if (item != null && item.item != null)
                {
                    invManager.ChangeMoving(slotNum, invUI.inv);
                    DisappearImage();
                }
            }
            else
            {
                if (item == null || item.item == null)
                {
                    invManager.ApplyMoving(slotNum, invUI.inv);
                }
                else if (invManager.movingFromSlot == slotNum)
                {
                    invManager.CancelMoving();
                }
            }
        }
    }
    public void ResetImage()
    {
        transform.Find("Image").GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        transform.Find("Text").GetComponent<Text>().enabled = true;
    }
    public void DisappearImage()
    {
        transform.Find("Image").GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        transform.Find("Text").GetComponent<Text>().enabled = false;
    }
    public void MoveThisItem()
    {
        ItemInfo item = invManager.GetItem(slotNum, invUI.inv);
        if (item != null && item.item != null)
        {
            invManager.MoveItem(item, invUI.inv, slotNum);
        }
    }
    bool isOver;
    public void OnEnterHover()
    {
        isOver = true;
        if (invUI.hoverDesc != null)
        {
            ItemInfo item = invManager.GetItem(slotNum, invUI.inv);
            if (item != null && item.item != null)
            {
                invUI.SetDesc(item.item, transform.position);
            }
        }
    }
    public void OnExitHover()
    {
        isOver = false;
        if (invUI.hoverDesc != null)
        {
            invUI.ResetDesc();
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (isOver && invManager.moving == false)
            {
                MoveThisItem();
            }
        }
    }
}
