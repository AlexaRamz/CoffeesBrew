using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarManager : MonoBehaviour
{
    public Canvas barCanvas;
    public Slider bar;
    bool showing;
    Transform thisObject;

    void Start()
    {
        
    }
    public void TurnOn(Transform obj)
    {
        barCanvas.enabled = true;
        showing = true;
        thisObject = obj;
    }
    public void TurnOff()
    {
        barCanvas.enabled = false;
        showing = false;
        thisObject = null;
    }
    public void SetValue(float value)
    {
        bar.value = value;
    }
    void Update()
    {
        if (showing && thisObject != null)
        {
            bar.transform.position = Camera.main.WorldToScreenPoint(thisObject.position);
        }
    }
}
