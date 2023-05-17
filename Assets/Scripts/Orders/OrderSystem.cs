using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSystem : MonoBehaviour
{
    OrderUI orderUI;
    Queue<string> customers = new Queue<string>();
    public List<Order> orderList = new List<Order>();
    public List<Item> menu = new List<Item>();

    public bool takingOrder = true;
    string currentCustomer;
    // CustomerPref currentCustomer;
    Order currentOrder;
    public int customerCount = 0;

    OrderQueue orderQueue;

    void Start()
    {
        orderUI = transform.Find("OrderMenu").GetComponent<OrderUI>();
        orderQueue = FindObjectOfType<OrderQueue>();

        customers.Enqueue("Fern");
        customers.Enqueue("Bear");
        customers.Enqueue("Raccoon");
        customerCount = 3;
        NextOrder();
    }

    Order GenerateOrder(string customer)
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
        if (customerCount > 0)
        {
            currentOrder = GenerateOrder(currentCustomer);
            orderList.Add(currentOrder);
            takingOrder = false;
            customerCount -= 1;
            orderUI.DisplayOrder(currentOrder);
        }
    }
    public void DeleteOrder(int index)
    {
        orderList.RemoveAt(index);
        orderQueue.UpdateOrders(orderList);
    }
    public void OpenMenu()
    {
        NextOrder();
        orderUI.OpenMenu();
    }
    public void CloseMenu()
    {
        orderQueue.UpdateOrders(orderList);
    }

    public void NextOrder()
    {
        if (customers.Count > 0)
        {
            takingOrder = true;
            currentCustomer = customers.Dequeue();
            //currentCustomer = customerPrefs.GetCustomer(currentCustomer);
            //orderUI.LoadPortrait(currentCustomer.image);
        }
    }
}
