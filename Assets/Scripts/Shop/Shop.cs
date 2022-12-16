using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop", menuName = "Shop")]
public class Shop : ScriptableObject
{
    public Item[] items;
    public Dialogue[] dialogues;
}
