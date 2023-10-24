using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CustomerInfo
{
    public Sprite portrait;
    public Dialogue dialogue;
    public List<Food> favorites;
    public enum DietType
    {
        Omnivore,
        Herbivore,
        Carnivore,
    }
    public DietType diet;
}
[CreateAssetMenu(fileName = "New NPC", menuName = "NPC")]
public class NPC : ScriptableObject
{
    public enum Emotion
    {
        Neutral,
        Happy,
        Sad,
        Angry,
        Shocked,
        Disgusted,
        Fearful,
        Embarrassed,
        Laughing,
        Pensive,
        Other,
    }
    [System.Serializable]
    public struct Portrait
    {
        public Emotion emotion;
        public Sprite sprite;
    }
    public List<Portrait> portraits;
    public List<Dialogue> dialogues;
    public List<Dialogue> orderFulfillDialogues;
    public CustomerInfo customerInfo;

    public Sprite GetPortrait(Emotion em = Emotion.Neutral)
    {
        Sprite thisSprite = null;
        foreach (Portrait portrait in portraits)
        {
            if (portrait.emotion == em)
            {
                return portrait.sprite;
            }
            else if (portrait.emotion == Emotion.Neutral)
            {
                thisSprite = portrait.sprite;
            }
        }
        return thisSprite;
    }
    public enum DialogueType
    {
        Default,
        OrderFulfill,
    }
    public Dialogue GetDialogue(DialogueType typ = DialogueType.Default)
    {
        switch (typ)
        {
            case DialogueType.Default:
                if (dialogues.Count > 0) return dialogues[Random.Range(0, dialogues.Count)];
                return null;
            case DialogueType.OrderFulfill:
                if (orderFulfillDialogues.Count > 0) return orderFulfillDialogues[Random.Range(0, orderFulfillDialogues.Count)];
                return null;
            default:
                return null;
        }
        
    }
}
