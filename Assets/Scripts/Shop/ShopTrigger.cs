using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    ShopSystem shopSys;
    public Shop thisShop;
    Movement2D plr;

    void Start()
    {
        shopSys = FindObjectOfType<ShopSystem>();
        plr = FindObjectOfType<Movement2D>();
    }
    void Update()
    { 
        if (shopSys != null && thisShop != null)
        {
            if (Input.GetKeyDown(KeyCode.Return) && plr.isInteractingWith(transform.position))
            {
                shopSys.ToggleMenu(thisShop);
            }
        }
    }
}
