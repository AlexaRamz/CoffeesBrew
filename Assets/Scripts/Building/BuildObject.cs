using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildObject : MonoBehaviour
{
    public Build build;
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
        itemImage = transform.Find("Image").GetComponent<Image>();
        selectionImage = transform.Find("SelectionImage").GetComponent<Image>();
        itemImage.sprite = build.rotations[0].sprite;
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
        buildUI.SelectObject(this, build);
        SelectObject();
    }
}
