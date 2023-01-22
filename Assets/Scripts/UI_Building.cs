using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Building : MonoBehaviour
{
    BuildingSystem buildSys;
    public GameObject slotTemplate;
    public Transform container;
    public ArrowScrolling scroll;
    public Image doneButton;

    void Start()
    {
        buildSys = FindObjectOfType<BuildingSystem>();
    }
    BuildObject currentSelection;
    void ClearSelection()
    {
        if (currentSelection)
        {
            currentSelection.DeselectObject();
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
        int count = 0;
        foreach (Build build in objectList)
        {
            Instantiate(slotTemplate, container).GetComponent<BuildObject>().SetObject(build);
            count += 1;
        }
    }
}
