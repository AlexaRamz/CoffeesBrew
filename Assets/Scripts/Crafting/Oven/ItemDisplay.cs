using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDisplay : MyUI
{
    [SerializeField] private MyImage itemImage;
    public void SetItem(Item item)
    {
        if (item != null)
        {
            itemImage.SetSprite(item.asset);
        }
        else
        {
            itemImage.SetSprite(null);
        }
    }
    public void Animate()
    {
        Animator anim = GetComponent<Animator>();
        anim.ResetTrigger("PopUp");
        anim.SetTrigger("PopUp");
    }
}
