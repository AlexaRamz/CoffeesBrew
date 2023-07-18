using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Item/Food")]
public class Food : Item
{
    public enum FoodType
    {
        Dessert,
        Drink,
        Uncooked,
        Fruit,
        Vegetable,
        Protein,
    }
    public FoodType[] foodType;
    public Food ovenCookTo;

    public override Food GetFood()
    {
        return ovenCookTo;
    }
    // Eat
}
