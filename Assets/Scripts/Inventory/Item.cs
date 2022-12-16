using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        Tool,
        Gem,
        Potion,
        Ingredient,
        Material,
        Food,
        Decor,
    }

    public string name;
    public ItemType itemType;
    public Sprite asset;
    public int stackMax = 99;
    public List<ItemInfo> materials = new List<ItemInfo>();
    public int amountToCraft = 1;
    public int price = 10;
    [TextArea(3, 10)]
    public string description;
    public Item ovenCookTo;
}

[System.Serializable]
public class ItemInfo
{
    public Item item;
    public int amount = 1;
}
