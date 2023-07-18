using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnims : MonoBehaviour
{
    public float expandScale = 1.05f;
    public float shrinkScale = 0.95f;

    Sprite buttonSprite;
    void Start()
    {
        buttonSprite = GetComponent<Image>().sprite;
    }
    public void ExpandOn()
    {
        if (_enabled) GetComponent<RectTransform>().localScale = new Vector3(expandScale, expandScale, 1.0f);
    }
    public void ShrinkOn()
    {
        if (_enabled) GetComponent<RectTransform>().localScale = new Vector3(shrinkScale, shrinkScale, 1.0f);
    }
    public void SizeOff()
    {
        GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
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
        GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0, 0, -7));
    }
    public void RotateOff()
    {
        GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    public void PressedSprite(Sprite pressedSprite)
    {
        GetComponent<Image>().sprite = pressedSprite;
    }
    public void ResetSprite()
    {
        GetComponent<Image>().sprite = buttonSprite;
    }
    bool _enabled = true;
    //Basic Expanding Button
    bool inRange = false;
    bool pressing = false;
    public void Enable()
    {
        _enabled = true;
    }
    public void Disable()
    {
        _enabled = false;
    }
    public void PointerEnter()
    {
        inRange = true;
        ExpandOn();
    }
    public void PointerExit()
    {
        inRange = false;
        if (!pressing)
        {
            SizeOff();
        }
    }
    public void PointerDown()
    {
        pressing = true;
        ShrinkOn();
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
