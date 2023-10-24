using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Order
{
    public NPC customer;
    public List<ItemInfo> items = new List<ItemInfo>();

    public Order(NPC thisCustomer)
    {
        customer = thisCustomer;
    }
    public void AddItem(Item item, int amount)
    {
        items.Add(new ItemInfo(item, amount));
    }
}
