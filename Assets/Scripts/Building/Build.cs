using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rotation
{
    public Sprite sprite;
    public GameObject Object;
    public bool flipped;
    public Vector2Int size;
    public Placement[] placements;
    public RuleBuild[] ruleBuilds; //Change sprite for this rotation depending on what's next to it
}
[System.Serializable]
public class RuleBuild
{
    public Sprite sprite;
    public bool flipped;
    public enum BuildType
    {
        Self,
        Empty, // null
        Any, // self and anything other than self including null
        Other, // not self, anything other than self including null
    }
    public BuildType[] adjacentBuilds = new BuildType[4] { BuildType.Any, BuildType.Any, BuildType.Any, BuildType.Any }; //Up, Right, Down, Left
    public BuildType[] cornerBuilds; //UpLeft, UpRight, DownRight, DownLeft
}
[System.Serializable]
public class Placement
{
    public Vector2 offset; //relative to position
    public Vector2Int position; //relative to objects origin position
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
    public bool canDecorate = false; //canDecorate is true assumes all rotations have placements
    public Rotation GetRotation(int rot)
    {
        return rotations[rot];
    }
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
        return build.GetRotation(rot);
    }
    public Placement[] GetPlacements()
    {
        return GetRotation().placements;
    }
    public Placement GetPlacement(int place)
    {
        return GetRotation().placements[place];
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
