using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CustomerPref", menuName = "CustomerPref")]
public class CustomerPref : ScriptableObject
{
    public Sprite portrait;
    public Dialogue dialogue;
    public List<Item> favorites;
    public List<Item.FoodType> dietRestrictions;
}
