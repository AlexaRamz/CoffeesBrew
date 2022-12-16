using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectOnInteract : MonoBehaviour
{
    public ItemInfo item;
    public Sprite holdingSprite;
    public Sprite normalSprite;
    public bool holding;
    public bool pop = true;
    bool inRange = false;
    Inventory plrInv;
    SpriteRenderer spriteRender;
    private void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = true;
            plrInv = collision.gameObject.GetComponent<Inventory>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = false;
            plrInv = null;
        }
    }
    void SetHoldingSprite()
    {
        spriteRender.sprite = holdingSprite;
    }
    void SetNormalSprite()
    {
        spriteRender.sprite = normalSprite;
    }
    void Update()
    {
        if (inRange && holding && plrInv != null)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (pop)
                {
                    for (int i = 0; i < item.amount; i++)
                    {
                        GameObject collect = Instantiate(Resources.Load("Collectable") as GameObject, new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z), Quaternion.identity);
                        collect.GetComponent<Collect>().item = item.item;
                    }
                    SetNormalSprite();
                }
                else if (plrInv.CollectItem(item))
                {
                    holding = false;
                    SetNormalSprite();
                }
            }
        }
    }
}
