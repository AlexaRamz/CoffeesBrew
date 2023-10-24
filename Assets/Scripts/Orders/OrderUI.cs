using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour, IMenu
{
    OrderSystem orderSys;
    Canvas canvas;
    public Animator anim;
    public GameObject itemTemplate;
    public Transform container;
    public OrderButton mainButton;
    public GameObject customerImage;
    Animator customerAnim;
    public Text customerNum;
    public Image customerNumIcon;
    public GameObject chatButton;
    SceneChangeManager effectManager;
    PlayerManager plr;

    void Start()
    {
        orderSys = FindObjectOfType<OrderSystem>();
        canvas = GetComponent<Canvas>();
        effectManager = FindObjectOfType<SceneChangeManager>();
        customerAnim = customerImage.GetComponent<Animator>();
        plr = FindObjectOfType<PlayerManager>();
    }

    public void OpenMenu()
    {
        if (!plr.SetCurrentUI(this)) return;
        StartCoroutine(OpenMenuC());
        IEnumerator OpenMenuC()
        {
            anim.SetBool("Hide", true);
            yield return StartCoroutine(effectManager.FadeToBlack());
            canvas.enabled = true;
            anim.SetBool("Hide", false);
            yield return StartCoroutine(effectManager.FadeToClear());
        }
    }
    public void CloseMenu() // Called when click on close button
    {
        if (!plr.SetCurrentUI(null)) return;
        mainButton.SetState(OrderButton.OrderState.Off);
        StartCoroutine(CloseMenuC());
        IEnumerator CloseMenuC()
        {
            yield return StartCoroutine(effectManager.FadeToBlack());
            orderSys.CloseMenu();
            canvas.enabled = false;
            ClearPortrait();
            yield return StartCoroutine(effectManager.FadeToClear());
        }
    }
    public Canvas GetCanvas()
    {
        return canvas;
    }
    public void MainAction() // Called when click on main button
    {
        if (orderSys.takingOrder)
        {
            orderSys.TakeOrder();
        }
        else if (orderSys.GetCustomerCount() > 0)
        {
            ClearItems();
            orderSys.NextOrder();
            mainButton.SetState(OrderButton.OrderState.Take);
        }
        else
        {
            CloseMenu();
        }
    }
    void SetNextAction()
    {
        chatButton.GetComponent<Button>().enabled = true;
        chatButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        chatButton.transform.Find("Image").GetComponent<Image>().color = new Color32(255, 255, 255, 255);

        if (orderSys.GetCustomerCount() == 0)
        {
            mainButton.SetState(OrderButton.OrderState.Done);
        }
        else
        {
            mainButton.SetState(OrderButton.OrderState.Next);
        }
    }
    public void UpdateNum(int num)
    {
        customerNum.text = num.ToString();
        if (num == 0)
        {
            customerNumIcon.enabled = false;
            customerNum.enabled = false;
        }
        else
        {
            customerNum.enabled = true;
            customerNum.enabled = true;
        }
    }
    public void ClearItems()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
    IEnumerator ListOrders(Order order)
    {
        foreach (ItemInfo item in order.items)
        {
            yield return new WaitForSeconds(0.5f);
            Transform displayItem = Instantiate(itemTemplate, container).transform;
            displayItem.Find("Image").GetComponent<Image>().sprite = item.item.asset;
            displayItem.Find("Text").GetComponent<Text>().text = "x" + item.amount.ToString();
        }
        yield return new WaitForSeconds(0.7f);
        SetNextAction();
    }
    public void DisplayOrder(Order order)
    {
        ClearItems();
        // Disable Chat button
        chatButton.GetComponent<Button>().enabled = false;
        chatButton.GetComponent<Image>().color = new Color32(180, 180, 180, 255);
        chatButton.transform.Find("Image").GetComponent<Image>().color = new Color32(180, 180, 180, 255);
        // Main button: Disable and "loading" mode
        mainButton.SetState(OrderButton.OrderState.Loading);
        StartCoroutine(ListOrders(order));
    }
    void ClearPortrait()
    {
        Image Image = customerImage.transform.Find("Customer").GetComponent<Image>();
        Image.sprite = null;
        Image.enabled = false;

        Image = customerImage.transform.Find("Customer2").GetComponent<Image>();
        Image.sprite = null;
        Image.enabled = false;
    }
    bool portrait1;
    public void LoadPortrait(Sprite image)
    {
        customerAnim.ResetTrigger("Slide");
        customerAnim.SetTrigger("Slide");
        Image Image;
        if (portrait1)
        {
            Image = customerImage.transform.Find("Customer").GetComponent<Image>();
            Image.sprite = image;
            Image.enabled = true;
            portrait1 = false;
        }
        else
        {
            Image = customerImage.transform.Find("Customer2").GetComponent<Image>();
            Image.sprite = image;
            Image.enabled = true;
            portrait1 = true;
        }
    }
    public void OpenChat() // Call when click on chat button
    {
        anim.SetBool("Hide", true);
        customerAnim.SetBool("Center", true);
        IEnumerator Delay()
        {
            yield return new WaitForSeconds(2f);
            CloseChat();
        }
        StartCoroutine(Delay());
    }
    public void CloseChat()
    {
        anim.SetBool("Hide", false);
        customerAnim.SetBool("Center", false);
    }
}
