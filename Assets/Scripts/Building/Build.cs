using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rotation
{
    public Sprite sprite;
    public GameObject Object;
    public bool flipped;
    public Vector2Int size = new Vector2Int(1, 1);
    public Vector2[] placements;
}
[CreateAssetMenu(fileName = "New Build", menuName = "Build")]
public class Build : ScriptableObject
{
    public Rotation[] rotations;
    public enum ObjectType
    {
        Furniture,
        WallDecor,
        TableDecor,
        FloorDecor,
    }
    public ObjectType Type;
    public bool canDecorate = false;
    //canDecorate is true assumes all rotations have placements
}

[System.Serializable]
public class BuildInfo
{
    public Build build;
    public int rot;
    public Vector2Int gridPos;
    public Item[] decor;

    public Rotation GetRotation()
    {
        return build.rotations[rot];
    }
    public Vector2[] GetPlacements()
    {
        return GetRotation().placements;
    }
    public int GetPlacementAmount()
    {
        return GetRotation().placements.Length;
    }
    public void SetPlacementArray()
    {
        decor = new Item[GetPlacementAmount()];
    }
}
