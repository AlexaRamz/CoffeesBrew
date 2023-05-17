using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderQueue : MonoBehaviour
{
    OrderSystem orderSys;
    public GameObject orderTemplate;
    public Transform container;

    private void Start()
    {
        orderSys = FindObjectOfType<OrderSystem>();
    }
    void ClearOrders()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
    public void UpdateOrders(List<Order> orderList)
    {
        ClearOrders();
        int index = 0;
        foreach (Order order in orderList)
        {
            Instantiate(orderTemplate, container).GetComponent<OrderObject>().SetOrder(order, index);
            index++;
        }
    }
    public void DeleteOrder(int index)
    {
        orderSys.DeleteOrder(index);
    }
}
