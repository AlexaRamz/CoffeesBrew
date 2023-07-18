using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenTrigger : Interactable
{
    public int cookTime;
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private ItemDisplay itemDisplay;

    bool cooking = false;
    Inventory plrInv;
    Item cookingItem;
    Item cookItemTo;
    bool cooked = false;

    void Start()
    {
        plrInv = GameObject.FindWithTag("Player").GetComponent<Inventory>();
    }
    public override void Interact()
    {
        if (!cooking)
        {
            if (!cooked)
            {
                CookingOn();
            }
            else
            {
                CollectItem();
            }
        }
    }
    void CookingOn()
    {
        Item item = plrInv.GetCurrentItem();
        if (item && item.GetFood() != null)
        {
            cooking = true;
            progressBar.Enable();
            plrInv.DepleteCurrentItem();
            cookingItem = item;
            cookItemTo = cookingItem.GetFood();
            GetComponent<Animator>().SetBool("Cooking", true);
            StartCoroutine(Progress());
        }
    }
    void DisplayOn()
    {
        itemDisplay.SetItem(cookItemTo);
        itemDisplay.Enable();
        itemDisplay.Animate();
    }
    void DisplayOff()
    {
        itemDisplay.Disable();
        itemDisplay.SetItem(null);
    }
    void CookingOff()
    {
        cooking = false;
        GetComponent<Animator>().SetBool("Cooking", false);
        cooked = true;
        progressBar.Disable();
        UpdateState();
    }
    void CollectItem()
    {
        cooked = false;
        plrInv.CollectItem(new ItemInfo { item = cookItemTo, amount = 1 });
        cookingItem = null;
        DisplayOff();
    }
    IEnumerator Progress()
    {
        float time = 0;
        while (time < cookTime)
        {
            progressBar.SetValue(time / cookTime);
            time += Time.deltaTime;
            yield return null;
        }
        DisplayOn();
        CookingOff();
    }
    public override bool CanInteract()
    {
        if (!cooking)
        {
            if (!cooked)
            {
                Item item = plrInv.GetCurrentItem();
                if (item && item.GetFood() != null)
                {
                    return true;
                }
            }
            else // else if cooked, can interact but send message if inventory full
            {
                return true;
            }
        }
        return false;
    }
}
