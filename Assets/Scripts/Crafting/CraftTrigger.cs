using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftTrigger : MonoBehaviour
{
    public CraftStation thisCrafting;
    CraftingSystem craftSys;
    Movement2D plr;

    void Start()
    {
        craftSys = FindObjectOfType<CraftingSystem>();
        plr = FindObjectOfType<Movement2D>();
    }
    void Update()
    {
        if (craftSys != null && thisCrafting != null)
        {
            if (Input.GetKeyDown(KeyCode.Return) && plr.isInteractingWith(gameObject))
            {
                craftSys.ToggleMenu(thisCrafting);
            }
        }
    }
}
