using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour, IMenu
{
    Queue<TextIconSet> sentences;
    Dialogue currentDialogue;
    NPC currentSpeaker;
    string currentSentence;
    PlayerManager plr;

    [HideInInspector] public bool talking = false;
    bool responding = false;
    bool typing;
    public float typeSpeed = 25f;
    public float pauseTime = 0.3f;
    public int maxLetters = 30;
    IEnumerator currentCoroutine;

    public Canvas mainUI, responseUI;
    public GameObject textBox, responseContainer;
    public Text textDisplay;
    public Image iconDisplay;
    public GameObject responseTemplate;

    void Start()
    {
        sentences = new Queue<TextIconSet>();
        plr = GameObject.Find("Player").GetComponent<PlayerManager>();
    }
    public void StartDialogue(Dialogue dialogue, NPC speaker)
    {
        if (dialogue == null || speaker == null) return;
        OpenMenu();
        currentDialogue = dialogue;
        currentSpeaker = speaker;
        StartCoroutine(StartDelay());

        foreach (TextIconSet sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
    void OpenMenu()
    {
        if (!talking && plr.SetCurrentUI(this))
        {
            talking = mainUI.enabled = true;
        }
        //LeanTween.scale(textBox, new Vector3(1f, 1f, 1f), 0.2f).setEase(LeanTweenType.easeInCubic);
    }
    public void CloseMenu()
    {
        EndDialogue();
        mainUI.enabled = false;
        StartCoroutine(EndDelay());
        //textBox.transform.localScale = new Vector3(0, 0, 0);
    }
    void EndDialogue()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        talking = false;
        currentDialogue = null;
        sentences.Clear();
        textDisplay.text = "";
        iconDisplay.sprite = null;
        ClearResponses();
    }
    IEnumerator StartDelay() // Provides a delay before opened
    {
        yield return new WaitForSeconds(0.001f);
        talking = true;
    }
    IEnumerator EndDelay() // Provides a delay before player can open again
    {
        yield return new WaitForSeconds(0.001f);
        plr.SetCurrentUI(null);
    }
    public Canvas GetCanvas()
    {
        return textBox.GetComponent<Canvas>();
    }


    // ---------- TEXT ----------
    List<char> pauseChars = new List<char> { '.', ',', '!', '?' };
    IEnumerator TypeText(string sentence)
    {
        typing = true;
        var lines = sentence.Split('\n');
        List<string> newLines = new List<string>();
        //Add newline at the last space before max letters reached
        foreach (string line in lines)
        {
            string newLine = line;
            if (maxLetters != 0 && line.Length > maxLetters)
            {
                for (int i = 0; i < maxLetters; i++)
                {
                    if (line.Substring(maxLetters - i, 1) == " ")
                    {
                        newLine = newLine.Insert(maxLetters - i + 1, "\n");
                        break;
                    }
                }
            }
            newLines.Add(newLine);
        }
        currentSentence = string.Join("\n", newLines);
        int lineCount = 0;
        foreach (string line in newLines)
        {
            int charIndex = 0;
            string originalText = textDisplay.text;
            while (charIndex < line.Length)
            {
                if (line[charIndex] != ' ')
                {
                    yield return new WaitForSeconds(1 / typeSpeed);
                }

                textDisplay.text = originalText + line.Substring(0, charIndex + 1);
                if (pauseChars.Contains(line[charIndex]) && (charIndex == line.Length - 1 || line[charIndex + 1] == ' ' || line[charIndex + 1] == '.'))
                {
                    yield return new WaitForSeconds(pauseTime);
                }
                charIndex += 1;
            }
            lineCount += 1;
            if (lineCount != newLines.Count)
            {
                textDisplay.text = originalText + line.Substring(0, charIndex) + "\n";
                yield return new WaitForSeconds(pauseTime);
            }
        }
        typing = false;
        SetResponses();
    }
    void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            if (!responding)
            {
                CloseMenu();
            }
        }
        else
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            textDisplay.text = "";
            TextIconSet sentence = sentences.Dequeue();
            currentSentence = sentence.text;
            currentCoroutine = TypeText(currentSentence);
            StartCoroutine(currentCoroutine);
            Sprite icon = currentSpeaker.GetPortrait(sentence.emotion);
            if (icon != null)
            {
                iconDisplay.sprite = icon;
            }
        }
    }
    void SkipToEnd()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        typing = false;
        textDisplay.text = currentSentence;
        SetResponses();
    }

    // ---------- RESPONSES ----------
    void SetResponses()
    {
        if (sentences.Count == 0 && currentDialogue.responses.Count != 0)
        {
            responding = responseUI.enabled = true;
            for (int i = 0; i < currentDialogue.responses.Count; i++)
            {
                int index = i;
                GameObject obj = Instantiate(responseTemplate, responseContainer.transform);
                obj.GetComponent<ResponseButton>().SetResponse(this, currentDialogue.responses[i].text, index);
            }
            UpdateResponseSelection(0);
        }
    }
    int responseSelectionIndex = 0;
    public void UpdateResponseSelection(int i)
    {
        ClearResponseSelection();
        responseSelectionIndex = i;
        responseContainer.transform.GetChild(i).GetComponent<ResponseButton>().Select();
    }
    void ClearResponseSelection()
    {
        foreach (Transform child in responseContainer.transform)
        {
            child.GetComponent<ResponseButton>().Deselect();
        }
    }
    public void Respond(int i)
    {
        ClearResponses();
        talking = false;
        StartDialogue(currentDialogue.responses[i].nextDialogue, currentSpeaker);
    }
    void ClearResponses()
    {
        responseUI.enabled = false;
        responding = false;
        foreach (Transform child in responseContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void Update()
    {
        if (talking)
        {
            if (!responding)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
                {
                    if (typing)
                    {
                        SkipToEnd();
                    }
                    else
                    {
                        DisplayNextSentence();
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    int newSelectionIndex = responseSelectionIndex - 1;
                    if (newSelectionIndex < 0) newSelectionIndex = currentDialogue.responses.Count - 1;
                    UpdateResponseSelection(newSelectionIndex);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    int newSelectionIndex = responseSelectionIndex + 1;
                    if (newSelectionIndex >= currentDialogue.responses.Count) newSelectionIndex = 0;
                    UpdateResponseSelection(newSelectionIndex);
                }
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Respond(responseSelectionIndex);
                }
            }
        }
    }
}
