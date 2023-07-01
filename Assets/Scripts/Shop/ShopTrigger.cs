using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    ShopSystem shopSys;
    public Shop thisShop;

    void Start()
    {
        shopSys = FindObjectOfType<ShopSystem>();
    }
    public void OpenShop()
    { 
        if (shopSys && thisShop)
        {
            shopSys.ToggleMenu(thisShop);
        }
    }
}
