using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSystem : MonoBehaviour
{
    OrderUI orderUI;
    public List<CustomerPref> customerList;
    Queue<CustomerPref> customers = new Queue<CustomerPref>();
    public List<Order> orderList = new List<Order>();
    public List<Item> menu = new List<Item>();

    public bool takingOrder = true;
    CustomerPref currentCustomer;
    // Portrait, dialogue, preferences (add food types to items)
    Order currentOrder;
    public int customerCount = 0;

    OrderQueue orderQueue;

    void Start()
    {
        orderUI = transform.Find("OrderMenu").GetComponent<OrderUI>();
        orderQueue = FindObjectOfType<OrderQueue>();

        customers.Enqueue(customerList[0]);
        customers.Enqueue(customerList[1]);
        customers.Enqueue(customerList[2]);
        customerCount = 3;
        
        //IEnumerator OpenDelay()
        //{
        //    yield return new WaitForSeconds(1f);
        //    OpenMenu();
        //}
        //StartCoroutine(OpenDelay());
    }

    Order GenerateOrder(CustomerPref customer)
    {
        // Randomize order from customer prefs: items and amounts
        // currentCustomer.items
        Order newOrder = new Order(customer.name);
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
            orderUI.UpdateNum(customerCount);
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

    public void NextOrder(bool slide = true)
    {
        if (customers.Count > 0)
        {
            takingOrder = true;
            currentCustomer = customers.Dequeue();
            orderUI.LoadPortrait(currentCustomer.portrait);
            orderUI.UpdateNum(customerCount);
        }
    }
}
