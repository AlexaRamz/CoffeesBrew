using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeManager : MonoBehaviour
{
    Image fade;
    public float fadeTime = 0.25f;
    Transform plr;

    void Start()
    {
        plr = GameObject.Find("Player").transform;
    }
    void Awake()
    {
        fade = transform.Find("Overlay").transform.Find("Fade").GetComponent<Image>();
        StartCoroutine(FadeToClear());
    }
    public IEnumerator FadeToBlack()
    {
        fade.color = Color.clear;
        float rate = 1.0f / fadeTime;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            fade.color = Color.Lerp(Color.clear, Color.black, progress);

            progress += rate * Time.deltaTime;

            yield return null;
        }
    }
    public IEnumerator FadeToClear()
    {
        fade.color = Color.black;
        float rate = 1.0f / fadeTime;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            fade.color = Color.Lerp(Color.black, Color.clear, progress);

            progress += rate * Time.deltaTime;

            yield return null;
        }
    }
    public IEnumerator LoadScene(string changeTo)
    {
        yield return StartCoroutine(FadeToBlack());
        SceneManager.LoadScene(changeTo);
    }
    public IEnumerator TeleportTo(Vector2 pos)
    {
        yield return StartCoroutine(FadeToBlack());
        plr.position = pos;
        StartCoroutine(FadeToClear());
    }
}
