using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowScrolling : MonoBehaviour
{
    public GameObject leftButton;
    public GameObject rightButton;
    public ScrollRect rect;
    public Transform container;
    int currentSlide = 1;
    void Start()
    {
        
    }
    public void UpdateScroll()
    {
        ChangeSlide(1);
    }
    public void ChangeSlide(int slide)
    {
        currentSlide = slide;
        int count = container.childCount;
        int slides = (int)Mathf.Ceil(count / 5);
        float pos;
        if (slide == slides)
        {
            pos = 1f;
        }
        else if (slide == 1)
        {
            pos = 0f;
        }
        else
        {
            pos = ((float)slide - 1) / ((float)slides - 1);
        }
        if (0f <= pos && pos <= 1f)
        {
            rect.horizontalNormalizedPosition = pos;
        }
    }
    public void ScrollRight()
    {
        int count = container.childCount;
        int slides = (int)Mathf.Ceil(count / 5f);
        if (currentSlide != slides)
        {
            ChangeSlide(currentSlide + 1);
        }
    }
    public void ScrollLeft()
    {
        if (currentSlide != 1)
        {
            ChangeSlide(currentSlide - 1);
        }
    }
    void Update()
    {
        
    }
}
