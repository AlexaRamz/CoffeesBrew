using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    Dialogue currentDialogue;
    Queue<string> sentences;
    Queue<IconSet> icons;
    Queue<Sprite> icons2;
    Response[] responses;

    public int maxLetters = 1;
    public Canvas responseUI;
    public Text choice1;
    public Text choice2;
    public Image star1;
    public Image star2;
    string choice = "choice1";

    public Canvas textBox;
    public Text textDisplay;
    public Image iconDisplay;
    IEnumerator currentCoroutine;
    Movement2D plr;

    public bool talking = false;
    bool typing;
    bool responding;
    string currentSentence;
    Sprite finalIcon;
    public float typeSpeed = 0.04f;
    public float pauseTime = 0.8f;

    public DialogueTrigger currentTrigger;

    void Start()
    {
        plr = FindObjectOfType<Movement2D>();
        sentences = new Queue<string>();
        icons = new Queue<IconSet>();
        icons2 = new Queue<Sprite>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        plr.plrActive = false;
        StartCoroutine(StartDelay());
        sentences.Clear();
        icons.Clear();
        icons2.Clear();
        textDisplay.text = "";
        iconDisplay.GetComponent<Image>().enabled = true;
        textBox.enabled = true;

        responses = dialogue.responses;

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        foreach (IconSet icon in dialogue.icons)
        {
            icons.Enqueue(icon);
        }
        DisplayNextSentence();
    }
    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.1f);
        talking = true;
    }
    IEnumerator EndDelay()
    {
        yield return new WaitForSeconds(0.1f);
        talking = false;
    }
    IEnumerator TypeText(string sentence)
    {
        typing = true;
        int n = maxLetters;
        var lines = sentence.Split('\n');
        List<string> newLines = new List<string>();
        foreach (string line in lines)
        {
            string newLine = line;
            if (n != 0 && line.Length > n)
            {
                bool found = false;
                for (int i = 0; i < n; i++)
                {
                    if (line.Substring(n - i, 1) == " " && found == false)
                    {
                        newLine = newLine.Insert(n - i + 1, "\n");
                        found = true;
                    }
                }
            }
            newLines.Add(newLine);
        }
        string newSentence = string.Join("\n", newLines);
        currentSentence = newSentence;
        int lineCount = 0;
        foreach (string line in newLines)
        {
            int charIndex = 0;
            string originalText = textDisplay.text;
            while (charIndex < line.Length)
            {
                yield return new WaitForSeconds(typeSpeed);
                //Add newline at the last space before max letters reached

                charIndex += 1;
                textDisplay.text = originalText + line.Substring(0, charIndex);
                if (line[charIndex - 1].ToString() == ",")
                {
                    yield return new WaitForSeconds(pauseTime);
                }
            }
            lineCount += 1;
            if (lineCount != newLines.Count)
            {
                textDisplay.text = originalText + line.Substring(0, charIndex) + "\n";
                yield return new WaitForSeconds(pauseTime);
                if (icons2.Count != 0)
                {
                    iconDisplay.GetComponent<Image>().sprite = icons2.Dequeue();
                }
            }
        }
        typing = false;
        Responses();
    }
    void Responses()
    {
        if (sentences.Count == 0 && responses.Length != 0)
        {
            responseUI.enabled = true;
            choice1.text = responses[0].Name;
            choice2.text = responses[1].Name;
            responding = true;
        }
    }
    void ResetResponses()
    {
        choice = "choice1";
        star1.enabled = true;
        star2.enabled = false;
        responseUI.enabled = false;
        responding = false;
    }
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        textDisplay.text = "";
        string sentence = sentences.Dequeue();
        currentSentence = sentence;
        currentCoroutine = TypeText(sentence);
        StartCoroutine(currentCoroutine);
        icons2.Clear();
        if (icons.Count != 0)
        {
            IconSet icon = icons.Dequeue();
            foreach (Sprite ic in icon.icons)
            {
                icons2.Enqueue(ic);
                finalIcon = ic;
            }
            iconDisplay.GetComponent<Image>().sprite = icons2.Dequeue();
        }
    }

    void EndDialogue()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        textDisplay.text = "";
        iconDisplay.GetComponent<Image>().sprite = null;
        iconDisplay.GetComponent<Image>().enabled = false;
        currentSentence = null;
        textBox.enabled = false;
        plr.plrActive = true;
        StartCoroutine(EndDelay());
        if (currentTrigger != null)
        {
            currentTrigger.Disappear();
            currentTrigger = null;
        }

        currentDialogue = null;
        ResetResponses();
    }
    void Update()
    {
        if (talking && Input.GetKeyDown(KeyCode.Return))
        {
            if (typing)
            {
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                }
                typing = false;
                textDisplay.text = currentSentence;
                iconDisplay.GetComponent<Image>().sprite = finalIcon;
                Responses();
            }
            else if (responding)
            {
                if (choice == "choice1")
                {
                    Dialogue nextDialogue = currentDialogue.responses[0].newDialogue;
                    ResetResponses();
                    currentDialogue = null;
                    StartDialogue(nextDialogue);
                }
                else if (choice == "choice2")
                {
                    Dialogue nextDialogue = currentDialogue.responses[1].newDialogue;
                    ResetResponses();
                    currentDialogue = null;
                    StartDialogue(nextDialogue);
                }
            }
            else
            {
                DisplayNextSentence();
            }
        }
        if (responding)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (choice == "choice1")
                {
                    choice = "choice2";
                    star1.enabled = false;
                    star2.enabled = true;
                }
                else if (choice == "choice2")
                {
                    choice = "choice1";
                    star1.enabled = true;
                    star2.enabled = false;
                }
            }
        }
    }
}
