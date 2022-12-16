using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestTree : MonoBehaviour
{
    bool inRange = false;
    Inventory inv;
    public GameObject drop;
    TreeHarvestGame game;

    void Start()
    {
        game = GameObject.Find("TreeHarvestGame").GetComponent<TreeHarvestGame>();
        inv = game.inv;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = false;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            if (inRange && game.canToggle && game.playing == false && inv.currentItem != null && inv.currentItem.item.name == "Basket")
            {
                game.StartGame();
            }
        }
    }
}
