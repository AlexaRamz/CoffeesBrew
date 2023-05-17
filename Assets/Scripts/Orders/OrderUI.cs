using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    OrderSystem orderSys;
    Canvas canvas;
    public GameObject itemTemplate;
    public Transform container;
    public OrderButton mainButton;
    public GameObject customerImage;

    void Start()
    {
        orderSys = FindObjectOfType<OrderSystem>();
        canvas = GetComponent<Canvas>();

    }

    public void MainAction() // Called when click on main button
    {
        if (orderSys.takingOrder)
        {
            TakeOrder();
        }
        else if (orderSys.customerCount > 0)
        {
            ClearItems();
            orderSys.NextOrder();
            mainButton.SetState(OrderButton.OrderState.Take);
            customerImage.GetComponent<Animator>().ResetTrigger("Slide");
            customerImage.GetComponent<Animator>().SetTrigger("Slide");
        }
        else
        {
            CloseMenu();
        }
    }
    void SetNextAction()
    {
        if (orderSys.customerCount == 0)
        {
            mainButton.SetState(OrderButton.OrderState.Done);
        }
        else
        {
            mainButton.SetState(OrderButton.OrderState.Next);
        }
    }
    void TakeOrder()
    {
        orderSys.TakeOrder();
    }
    public void ClearItems()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
    IEnumerator ListOrders(Order order)
    {
        foreach (OrderItem item in order.items)
        {
            yield return new WaitForSeconds(0.5f);
            Transform displayItem = Instantiate(itemTemplate, container).transform;
            displayItem.Find("Image").GetComponent<Image>().sprite = item.item.asset;
            displayItem.Find("Text").GetComponent<Text>().text = "x" + item.amount.ToString();
        }
        yield return new WaitForSeconds(0.7f);
        SetNextAction();
    }
    public void DisplayOrder(Order order)
    {
        ClearItems();
        // Main button: Disable and "loading" mode
        mainButton.SetState(OrderButton.OrderState.Loading);
        StartCoroutine(ListOrders(order));
    }
    public void LoadPortrait(Sprite image)
    {

    }
    public void OpenMenu()
    {
        canvas.enabled = true;
    }
    public void CloseMenu() // Called when click on close button
    {
        orderSys.CloseMenu();
        canvas.enabled = false;
    }
}
