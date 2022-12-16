using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    Response response;
    DialogManager manager;
    public void SetChoice(Response resp, DialogManager m)
    {
        response = resp;
        manager = m;
        transform.Find("Text").GetComponent<Text>().text = response.Name;
    }
    public void Unselect()
    {
        transform.Find("Image").GetComponent<Image>().enabled = false;
    }
    public void OnHoverEnter()
    {
        transform.Find("Image").GetComponent<Image>().enabled = true;
    }
    public void Select()
    {
        manager.SelectChoice(response);
    }
}
