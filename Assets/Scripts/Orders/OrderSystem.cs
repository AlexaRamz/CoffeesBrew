using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSystem : MonoBehaviour
{
    OrderUI orderUI;
    public List<NPC> customerList;
    Queue<NPC> customers = new Queue<NPC>();
    public List<Order> orderList = new List<Order>();
    public List<Item> menu = new List<Item>();

    public bool takingOrder = true;
    NPC currentCustomer;
    Order currentOrder;

    OrderQueue orderQueue;
    Inventory plr;

    void Start()
    {
        orderUI = transform.Find("OrderMenu").GetComponent<OrderUI>();
        orderQueue = FindObjectOfType<OrderQueue>();

        foreach (NPC customer in customerList)
        {
            customers.Enqueue(customer);
        }
        plr = GameObject.Find("Player").GetComponent<Inventory>();
    }

    Order GenerateOrder(NPC customer)
    {
        // Randomize order from customer prefs: items and amounts
        // currentCustomer.items
        Order newOrder = new Order(customer);
        if (menu.Count > 0)
        {
            Item item = menu[Random.Range(0, menu.Count)];
            int amount = Random.Range(1, 3);
            newOrder.AddItem(item, amount);

            item = menu[Random.Range(0, menu.Count)];
            amount = Random.Range(1, 3);
            newOrder.AddItem(item, amount);
        }
        return newOrder;
    }
    public void TakeOrder()
    {
        if (customers.Count > 0)
        {
            currentOrder = GenerateOrder(currentCustomer);
            orderList.Add(currentOrder);
            takingOrder = false;
            customers.Dequeue();
            orderUI.DisplayOrder(currentOrder);
            orderUI.UpdateNum(customers.Count);
        }
    }
    public void DeleteOrder(int index)
    {
        orderList.RemoveAt(index);
        orderQueue.UpdateOrders(orderList);
    }
    public void OpenMenu()
    {
        NextOrder(false);
        orderUI.OpenMenu();
    }
    public void CloseMenu()
    {
        orderQueue.UpdateOrders(orderList);
    }
    public int GetCustomerCount()
    {
        return customers.Count;
    }
    public void NextOrder(bool slide = true)
    {
        if (customers.Count > 0)
        {
            takingOrder = true;
            currentCustomer = customers.Dequeue();
            orderUI.LoadPortrait(currentCustomer.customerInfo.portrait);
            orderUI.UpdateNum(customers.Count);
        }
    }
    public Order CheckOrderFulfill(NPC thisCustomer)
    {
        for (int i = 0; i < orderList.Count; i++)
        {
            Order order = orderList[i];
            if (order.customer == thisCustomer && plr.GiveItems(order.items))
            {
                orderList.RemoveAt(i);
                return order;
            }
        }
        return null;
    }
}
