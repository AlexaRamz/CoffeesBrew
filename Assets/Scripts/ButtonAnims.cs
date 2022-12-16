using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnims : MonoBehaviour
{
    public bool isOn = true;
    public float expandScale = 1.05f;
    public float shrinkScale = 0.95f;
    public void ExpandOn()
    {
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(expandScale, expandScale, 1.0f);
    }
    public void ShrinkOn()
    {
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(shrinkScale, shrinkScale, 1.0f);
    }
    public void SizeOff()
    {
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }
    IEnumerator PopOff()
    {
        yield return new WaitForSeconds(0.1f);
        SizeOff();
    }
    public void PopOn()
    {
        ShrinkOn();
        StartCoroutine(PopOff());
    }
    public void RotateOn()
    {
        gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0, 0, -7));
    }
    public void RotateOff()
    {
        gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }
    //Basic Expanding Button
    bool inRange = false;
    bool pressing = false;
    public void PointerEnter()
    {
        if (isOn)
        {
            inRange = true;
            ExpandOn();
        }
    }
    public void PointerExit()
    {
        inRange = false;
        if (pressing == false)
        {
            SizeOff();
        }
    }
    public void PointerDown()
    {
        if (isOn)
        {
            pressing = true;
            ShrinkOn();
        }
    }
    public void PointerUp()
    {
        pressing = false;
        if (inRange)
        {
            ExpandOn();
        }
        else
        {
            SizeOff();
        }
    }
}
