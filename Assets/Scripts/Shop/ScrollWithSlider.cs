using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollWithSlider : MonoBehaviour
{
    //Used for ScrollRect to update Slider without causing infinite loop when Slider tries to update ScrollRect back
    public Slider slider;
    public void UpdateSliderValue()
    {
        float scrollPosition = GetComponent<ScrollRect>().verticalNormalizedPosition;
        slider.SetValueWithoutNotify(scrollPosition);
    }
}
