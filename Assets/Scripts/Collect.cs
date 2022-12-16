using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collect : MonoBehaviour
{
    public Item item;
    void Start()
    {
        if (item != null)
        {
            transform.Find("Image").GetComponent<SpriteRenderer>().sprite = item.asset;
        }
    }
    bool collected;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected == false && item != null && collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<Inventory>().CollectItem(new ItemInfo { item = item, amount = 1 }))
            {
                collected = true;
                Destroy(gameObject);
            }
        }
    }
}
