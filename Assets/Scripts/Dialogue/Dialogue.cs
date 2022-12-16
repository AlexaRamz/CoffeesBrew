using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Response
{
    public string Name;

    public Dialogue newDialogue;
}
[System.Serializable]
public class IconSet
{
    public Sprite[] icons;
}
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public IconSet[] icons;

    [TextArea(3, 10)]
    public string[] sentences;
    //responses, if any, appear after final sentence
    public Response[] responses;
}
