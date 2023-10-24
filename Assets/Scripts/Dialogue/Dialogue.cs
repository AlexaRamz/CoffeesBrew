using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Response
{
    public string text;
    public Dialogue nextDialogue;
}
[System.Serializable]
public class TextIconSet
{
    [TextArea(3, 10)]
    public string text;
    public NPC.Emotion emotion;
}
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public List<TextIconSet> sentences;
    //responses, if any, appear after final sentence
    public List<Response> responses;
}
