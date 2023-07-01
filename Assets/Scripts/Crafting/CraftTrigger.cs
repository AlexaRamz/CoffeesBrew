using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftTrigger : MonoBehaviour
{
    public CraftStation thisCrafting;
    CraftingSystem craftSys;

    void Start()
    {
        craftSys = FindObjectOfType<CraftingSystem>();
    }
    public void OpenCrafting()
    {
        if (craftSys && thisCrafting)
        {
            craftSys.ToggleMenu(thisCrafting);
        }
    }
}
