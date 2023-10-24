using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectOnInteract : Interactable
{
    public ItemInfo item;
    public Sprite holdingSprite;
    public Sprite normalSprite;
    public bool holding;
    public bool pop = true;
    Inventory plrInv;
    SpriteRenderer renderr;
    private void Start()
    {
        renderr = GetComponent<SpriteRenderer>();
        plrInv = FindObjectOfType<Inventory>();
    }
    void SetHoldingSprite()
    {
        renderr.sprite = holdingSprite;
    }
    void SetNormalSprite()
    {
        renderr.sprite = normalSprite;
    }
    void DropItems()
    {
        if (holding)
        {
            if (pop)
            {
                for (int i = 0; i < item.amount; i++)
                {
                    GameObject collect = Instantiate(Resources.Load("Collectable") as GameObject, new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z), Quaternion.identity);
                    collect.GetComponent<Collect>().item = item.item;
                }
                SetNormalSprite();
                holding = false;
            }
            else if (plrInv && plrInv.CollectItem(item))
            {
                SetNormalSprite();
                holding = false;
            }
        }
    }
    public override void Interact()
    {
        DropItems();
    }
}
