using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarManager : MonoBehaviour
{
    [SerializeField] private Canvas barCanvas;
    [SerializeField] private Slider bar;

    void Start()
    {
        
    }
    public void TurnOn()
    {
        barCanvas.enabled = true;
    }
    public void TurnOff()
    {
        barCanvas.enabled = false;
    }
    public void SetValue(float value)
    {
        bar.value = value;
    }
}
