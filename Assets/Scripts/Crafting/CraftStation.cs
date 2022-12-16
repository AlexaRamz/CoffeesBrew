using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CraftStation", menuName = "CraftStation")]
public class CraftStation : ScriptableObject
{
    public Item[] items;
}
