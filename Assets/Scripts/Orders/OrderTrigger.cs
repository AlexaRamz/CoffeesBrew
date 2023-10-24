using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderTrigger : Interactable
{
    OrderSystem orderSys;

    void Start()
    {
        orderSys = GameObject.Find("OrderSystem").GetComponent<OrderSystem>();
    }
    public override void Interact()
    {
        orderSys.OpenMenu();
    }
}
