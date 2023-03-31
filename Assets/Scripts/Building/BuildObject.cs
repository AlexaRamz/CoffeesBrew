using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildObject : MonoBehaviour
{
    public Build build;
    Item item;
    UI_Building buildUI;
    Image itemImage;
    Image selectionImage;

    void Start()
    {
        buildUI = FindObjectOfType<UI_Building>();
    }
    public void SetObject(Build thisBuild)
    {
        build = thisBuild;
        SetImage(build.rotations[0].sprite);
    }
    public void SetItem(Item thisItem)
    {
        item = thisItem;
        SetImage(thisItem.asset);
    }
    void SetImage(Sprite image)
    {
        itemImage = transform.Find("Image").GetComponent<Image>();
        selectionImage = transform.Find("SelectionImage").GetComponent<Image>();
        itemImage.sprite = image;
    }
    void SelectObject()
    {
        selectionImage.enabled = true;
    }
    public void DeselectObject()
    {
        selectionImage.enabled = false;
    }
    public void ChooseObject()
    {
        if (build)
        {
            buildUI.SelectObject(this, build);
        }
        else if (item)
        {
            buildUI.SelectItem(this, item);
        }
        SelectObject();
    }
}
