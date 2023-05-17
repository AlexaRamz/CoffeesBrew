using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrderItem
{
    public Item item;
    public int amount;
    
    public OrderItem(Item thisItem, int thisAmount)
    {
        item = thisItem;
        amount = thisAmount;
    }
}

[System.Serializable]
public class Order
{
    public string customerName;
    public List<OrderItem> items = new List<OrderItem>();

    public Order(string thisCustomer)
    {
        customerName = thisCustomer;
    }
    public void AddItem(Item item, int amount)
    {
        items.Add(new OrderItem(item, amount));
    }
}
