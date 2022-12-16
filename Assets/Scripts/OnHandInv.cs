using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnHandInv : MonoBehaviour
{
    string main = "bar";
    ItemInfo item1;
    ItemInfo item2;
    public Sprite blank;
    public Inventory inv;

    void Start()
    {

    }
    public void ChangeItem1(ItemInfo item)
    {
        item1 = item;
        UpdateSlots();
    }
    public void ChangeItem2(ItemInfo item)
    {
        item2 = item;
        UpdateSlots();
        Debug.Log(main);
    }
    void UpdateSlots()
    {
        Transform slot1 = transform.Find("Slot1");
        Transform slot2 = transform.Find("Slot2");
        if (main == "bar")
        {
            if (item1 != null && item1.item != null && item1.item.name != null)
            {
                slot1.GetComponent<Image>().sprite = item1.item.asset;
            }
            else
            {
                slot1.GetComponent<Image>().sprite = blank;
            }
            if (item2 != null && item2.item != null && item2.item.name != null)
            {
                slot2.GetComponent<Image>().sprite = item2.item.asset;
            }
            else
            {
                slot2.GetComponent<Image>().sprite = blank;
            }
        }
        else if (main == "tool")
        {
            if (item2 != null && item2.item != null && item2.item.name != null)
            {
                slot1.GetComponent<Image>().sprite = item2.item.asset;
            }
            else
            {
                slot1.GetComponent<Image>().sprite = blank;
            }
            if (item1 != null && item1.item != null && item1.item.name != null)
            {
                slot2.GetComponent<Image>().sprite = item1.item.asset;
            }
            else
            {
                slot2.GetComponent<Image>().sprite = blank;
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown("v"))
        {
            if (main == "tool")
            {
                main = "bar";
                inv.HoldItem();
                UpdateSlots();
            }
            else
            {
                main = "tool";
                inv.HoldItem();
                UpdateSlots();
            }
        }
    }
}
