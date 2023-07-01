using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderButton : MonoBehaviour
{
    public enum OrderState {Take, Loading, Next, Done, Off};
    Sprite current_icon;
    Image background;
    Image image;
    Button button;
    Animator buttonAnim;
    Animator loadAnim;
    public Sprite takeIcon;
    public Sprite nextIcon;
    public Sprite doneIcon;

    private void Start()
    {
        background = GetComponent<Image>();
        image = transform.Find("Image").GetComponent<Image>();
        button = GetComponent<Button>();
        loadAnim = transform.Find("Load").GetComponent<Animator>();
        buttonAnim = GetComponent<Animator>();
        current_icon = takeIcon;
    }
    public void SetState(OrderState newState)
    {
        bool enabled = true;
        background.color = new Color32(255, 255, 255, 255);
        image.color = new Color32(255, 255, 255, 255);
        loadAnim.SetBool("Loading", false);
        switch (newState)
        {
            case OrderState.Take:
                button.enabled = enabled = true;
                current_icon = takeIcon;
                break;
            case OrderState.Loading:
                button.enabled = enabled = false;
                background.color = new Color32(180, 180, 180, 255);
                image.color = new Color32(255, 255, 255, 0);
                current_icon = null;
                loadAnim.SetBool("Loading", true);
                break;
            case OrderState.Next:
                button.enabled = enabled = true;
                current_icon = nextIcon;
                break;
            case OrderState.Done:
                button.enabled = enabled = true;
                current_icon = doneIcon;
                break;
            case OrderState.Off:
                button.enabled = enabled = false;
                break;
        }
        image.sprite = current_icon;
        buttonAnim.SetBool("Enabled", enabled);
    }
}
