using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenTrigger : MonoBehaviour
{
    public int cookTime;
    ProgressBarManager progressBar;
    
    bool cooking = false;
    bool inRange;
    Inventory plrInv;
    Item cookingItem;
    bool cooked = false;

    void Start()
    {
        progressBar = FindObjectOfType<ProgressBarManager>();
    }
    void CookingOn()
    {
        Item item = plrInv.GetCurrentItem();
        if (item != null && item.ovenCookTo != null)
        {
            cooking = true;
            progressBar.TurnOn(transform);
            plrInv.DepleteCurrentItem();
            cookingItem = item;
            GetComponent<Animator>().SetBool("Cooking", true);
            StartCoroutine(Progress());
        }
    }
    void CookingOff()
    {
        cooking = false;
        GetComponent<Animator>().SetBool("Cooking", false);
        cooked = true;
        progressBar.TurnOff();
    }
    void CollectItem()
    {
        cooked = false;
        plrInv.CollectItem(new ItemInfo { item = cookingItem.ovenCookTo, amount = 1 });
        cookingItem = null;
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
        CookingOff();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = true;
            plrInv = collision.gameObject.GetComponent<Inventory>();
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = false;
            plrInv = null;
        }
    }
    void Update()
    {
        if (inRange && cooking == false && plrInv != null)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (cooked == false)
                {
                    CookingOn();
                }
                else
                {
                    CollectItem();
                }
            }
        }
    }
}
