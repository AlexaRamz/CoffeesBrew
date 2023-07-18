using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : Interactable
{
    ShopSystem shopSys;
    public Shop thisShop;

    void Start()
    {
        shopSys = FindObjectOfType<ShopSystem>();
    }
    public override void Interact()
    { 
        if (shopSys && thisShop)
        {
            shopSys.OpenMenu(thisShop);
        }
    }
}
