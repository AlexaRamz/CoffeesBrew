using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Building : MonoBehaviour, IMenu
{
    public BuildingSystem buildSys;
    PlayerManager plr;
    public GameObject slotTemplate;
    public Transform container;
    public ArrowScrolling scroll;
    [SerializeField] private Image doneButton;
    Canvas canvas;
    public GameObject OptionUI;

    void Start()
    {
        plr = FindObjectOfType<PlayerManager>();
        canvas = GetComponent<Canvas>();
    }
    BuildObject currentSelection;
    void ClearSelection()
    {
        if (currentSelection)
        {
            currentSelection.DeselectObject();
        }
    }
    bool open = false;
    public void OpenBuilding() // Called by building system
    {
        if (!open && plr.SetCurrentUI(this))
        {
            CancelPlace();
            canvas.enabled = true;
            open = true;
        }
    }
    public void CloseMenu()
    {
        if (open && plr.SetCurrentUI(null))
        {
            CancelPlace();
            canvas.enabled = false;
            buildSys.CloseBuilding();
            open = false;
        }
    }
    public GraphicRaycaster GetGraphicRaycaster()
    {
        return canvas.GetComponent<GraphicRaycaster>();
    }
    public void CancelPlace()
    {
        ClearSelection();
        buildSys.CancelPlace();
        doneButton.enabled = false;
    }
    public void SelectCategory(int index) // Called by tab buttons
    {
        buildSys.ChangeCategory(index);
    }
    public void SelectObject(BuildObject selection, Build thisBuild)
    {
        ClearSelection();
        currentSelection = selection;
        doneButton.enabled = true;
        buildSys.ChangeObject(thisBuild);
    }
    public void SelectItem(BuildObject selection, Item thisItem)
    {
        ClearSelection();
        currentSelection = selection;
        doneButton.enabled = true;
        buildSys.ChangeItem(thisItem);
    }

    void ClearObjects()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
    public void SetObjects(List<Build> objectList)
    {
        ClearObjects();
        scroll.UpdateScroll();
        foreach (Build build in objectList)
        {
            Instantiate(slotTemplate, container).GetComponent<BuildObject>().SetObject(build);
        }
    }
    public void SetObjects(List<BuildAmount> objectList)
    {
        ClearObjects();
        scroll.UpdateScroll();
        foreach (BuildAmount info in objectList)
        {
            Instantiate(slotTemplate, container).GetComponent<BuildObject>().SetObject(info.build);
        }
    }
    public void SetItems(List<Item> itemList)
    {
        ClearObjects();
        scroll.UpdateScroll();
        foreach (Item item in itemList)
        {
            Instantiate(slotTemplate, container).GetComponent<BuildObject>().SetItem(item);
        }
    }
    public void ResetOptionsPos()
    {
        OptionUI.GetComponent<RectTransform>().position = new Vector3(-10, -10, 0);
        OptionUI.GetComponent<Image>().enabled = false;
    }
    public void SetOptionsPos(Vector3 pos)
    {
        OptionUI.GetComponent<RectTransform>().position = pos;
        OptionUI.GetComponent<Image>().enabled = true;
    }
}
