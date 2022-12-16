using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacking : MonoBehaviour
{
    Animator anim;
    bool attacking;
    public int attackDamage;
    bool debounce;
    GameObject colliding;
    Inventory inv;
    string tool;

    //PlantingSystem buildSys;

    private void Start()
    {
        anim = transform.parent.GetComponent<Animator>();
        attacking = false;
        attackDamage = 5;
        debounce = false;
        inv = transform.parent.GetComponent<Inventory>();

        //buildSys = GameObject.Find("BuildingSystem").GetComponent<PlantingSystem>();
    }
    IEnumerator Debounce()
    {
        yield return new WaitForSeconds(0.1f);
        debounce = false;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        colliding = collision.gameObject;
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        colliding = null;
    }
    void Damage(GameObject Object)
    {
        Object.GetComponent<ObjectDamage>().TakeDamage(attackDamage);
        StartCoroutine(Debounce());
    }
    void ResetAnims()
    {
        anim.SetBool("Attacking", false);
        anim.SetBool("Harvest", false);
        attacking = false;
    }
    public void ResetActions()
    {
        ResetAnims();
        //buildSys.canPlace = false;
        //buildSys.ClearSelection();
    }
    void Update()
    {
        if (attacking && colliding != null && debounce == false)
        {
            if (tool == "Pick Axe" && colliding.tag == "Rock")
            {
                debounce = true;
                Damage(colliding);
            }
            else if (tool == "Hoe" && colliding.tag == "Crop")
            {
                debounce = true;
                Damage(colliding);
            }
        }
        if (Input.GetKeyDown("space"))
        {
            ResetAnims();
            ItemInfo item = inv.currentItem;
            if (item != null)
            {
                if (item.item.name == "Pick Axe")
                {
                    anim.SetBool("Attacking", true);
                    attacking = true;
                    tool = "Pick Axe";
                }
                else if (item.item.name == "Hoe")
                {
                    anim.SetBool("Harvest", true);
                    attacking = true;
                    tool = "Hoe";
                }
                else if (item.item.name == "Comb")
                {
                    //buildSys.TogglePlacing();
                }
            }
        }
        if (Input.GetKeyUp("space"))
        {
            ResetAnims();
        }
    }
}
