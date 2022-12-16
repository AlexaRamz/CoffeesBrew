using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftItem : MonoBehaviour
{
    public CraftingSystem craftSys;
    public Item item;

    public void SetItem(Item it, CraftingSystem sys)
    {
        item = it;
        craftSys = sys;
        transform.GetComponent<Image>().sprite = item.asset;
    }
    public void Unselect()
    {
        gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }
    public void SelectItem()
    {
        craftSys.ResetSelection();
        craftSys.DisplayItem(item);
        gameObject.GetComponent<Image>().color = new Color32(140, 140, 140, 110);
    }
}
