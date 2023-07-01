using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Building : MonoBehaviour
{
    public BuildingSystem buildSys;
    PlayerManager plr;
    public GameObject slotTemplate;
    public Transform container;
    public ArrowScrolling scroll;
    [SerializeField] private Image doneButton;
    Canvas canvas;

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
    public void OpenBuilding() // Called by building system
    {
        if (plr.SetCurrentUI(canvas))
        {
            CancelPlace();
            canvas.enabled = true;
        }
    }
    public void CloseBuilding() // Called by close button
    {
        if (plr.SetCurrentUI(null))
        {
            CancelPlace();
            canvas.enabled = false;
            buildSys.CloseBuilding();
        }
    }
    public void CancelPlace()
    {
        ClearSelection();
        buildSys.CancelPlace();
        doneButton.enabled = false;
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
    public void SetItems(List<Item> itemList)
    {
        ClearObjects();
        scroll.UpdateScroll();
        foreach (Item item in itemList)
        {
            Instantiate(slotTemplate, container).GetComponent<BuildObject>().SetItem(item);
        }
    }
}
