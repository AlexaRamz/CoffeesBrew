using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderObject : MonoBehaviour
{
    OrderQueue queueUI;
    int index;

    private void Start()
    {
        queueUI = FindObjectOfType<OrderQueue>();
    }
    public void SetOrder(Order order, int thisIndex)
    {
        GetComponent<Image>().sprite = order.items[0].item.asset;
        index = thisIndex;
        int amount = order.items[0].amount;
        if (amount > 1)
        {
            transform.Find("Text").GetComponent<Text>().text = "x" + amount.ToString();
        }
    }
    public void DeleteOrder()
    {
        queueUI.DeleteOrder(index);
    }
}
