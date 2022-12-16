using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    bool inRange = false;
    ShopSystem shopSys;
    public Shop thisShop;

    void Start()
    {
        shopSys = FindObjectOfType<ShopSystem>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = false;
            shopSys.CloseMenu();
        }
    }
    void Update()
    { 
        if (inRange && shopSys != null && thisShop != null)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                shopSys.ToggleMenu(thisShop);
            }
        }
    }
}
