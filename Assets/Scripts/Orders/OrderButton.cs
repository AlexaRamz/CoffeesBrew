using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderButton : MonoBehaviour
{
    public enum OrderState {Take, Loading, Next, Done};
    Sprite current_icon;
    Sprite current_pressed;
    Image image;
    Button button;
    Animator anim;
    public Sprite takeIcon;
    public Sprite takeIconPressed;
    public Sprite loadIcon;
    public Sprite nextIcon;
    public Sprite nextIconPressed;
    public Sprite doneIcon;
    public Sprite doneIconPressed;

    private void Start()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        anim = GetComponent<Animator>();
        current_pressed = takeIconPressed;
        current_icon = takeIcon;
    }
    bool enabled = true;
    public void SetState(OrderState newState)
    {
        image.color = new Color32(255, 255, 255, 255);
        anim.SetBool("Loading", false);
        switch (newState)
        {
            case OrderState.Take:
                button.enabled = enabled = true;
                current_pressed = takeIconPressed;
                current_icon = takeIcon;
                break;
            case OrderState.Loading:
                button.enabled = enabled = false;
                image.color = new Color32(180, 180, 180, 255);
                current_pressed = null;
                current_icon = loadIcon;
                anim.SetBool("Loading", true);
                break;
            case OrderState.Next:
                button.enabled = enabled = true;
                current_pressed = nextIconPressed;
                current_icon = nextIcon;
                break;
            case OrderState.Done:
                button.enabled = enabled = true;
                current_pressed = doneIconPressed;
                current_icon = doneIcon;
                break;
        }
        image.sprite = current_icon;
    }
    public void PointerDown()
    {
        if (enabled)
        {
            image.sprite = current_pressed;
        }
    }
    public void PointerUp()
    {
        if (enabled)
        {
            image.sprite = current_icon;
        }
    }

}
