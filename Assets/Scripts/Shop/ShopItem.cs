using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Item item;
    public ShopSystem shopSys;
    Image image;
    Text text;
    Image coinImage;
    bool canSelect;
    ButtonAnims anim;
    public void SetItem(Item it, ShopSystem sys)
    {
        item = it;
        shopSys = sys;
        image = transform.Find("Image").GetComponent<Image>();
        text = transform.Find("Text").GetComponent<Text>();
        coinImage = transform.Find("Coin").GetComponent<Image>();
        anim = GetComponent<ButtonAnims>();
        image.sprite = item.asset;
        text.text = item.price.ToString();
    }
    public void OnHoverEnter()
    {
        shopSys.UpdateDescription(item);
    }
    public void UpdateColor(int money)
    {
        if (money >= item.price)
        {
            Color32 normalCol = new Color32(255, 255, 255, 255);
            GetComponent<Image>().color = normalCol;
            image.color = normalCol;
            coinImage.color = normalCol;
            canSelect = true;
            anim.Enable();
        }
        else
        {
            Color32 grayCol = new Color32(150, 150, 150, 255);
            GetComponent<Image>().color = grayCol;
            image.color = grayCol;
            coinImage.color = grayCol;
            canSelect = false;
            anim.Disable();
        }
    }
    public void BuyItem()
    {
        if (canSelect)
        {
            shopSys.Buy(item);
        }
    }
}
