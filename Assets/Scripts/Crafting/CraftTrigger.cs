using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftTrigger : Interactable
{
    public CraftStation thisCrafting;
    CraftingSystem craftSys;

    void Start()
    {
        craftSys = FindObjectOfType<CraftingSystem>();
    }
    public override void Interact()
    {
        if (craftSys && thisCrafting)
        {
            craftSys.OpenMenu(thisCrafting);
        }
    }
}
