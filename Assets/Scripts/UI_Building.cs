using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Building : MonoBehaviour
{
    public GameObject slotTemplate;
    public GameObject empty;
    public Transform container;
    public ArrowScrolling scroll;
    void Start()
    {

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
            Rotation rotation = build.rotations[0];
            RectTransform itemSlot = Instantiate(slotTemplate, container).GetComponent<RectTransform>();
            itemSlot.Find("Image").GetComponent<Image>().sprite = rotation.sprite;
            itemSlot.GetComponent<BuildObject>().build = build;
            count += 1;
        }
        for (int i = 0; i < (int)Mathf.Ceil(count / 5f) * 5 - count; i++)
        {
            RectTransform itemSlot = Instantiate(empty, container).GetComponent<RectTransform>();
        }
    }
}
