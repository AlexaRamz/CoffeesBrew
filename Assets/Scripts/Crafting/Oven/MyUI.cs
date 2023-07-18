using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUI : MonoBehaviour
{
    // Custom world space UI components class
    SpriteRenderer[] allRenderers;
    public virtual void Start()
    {
        allRenderers = GetComponentsInChildren<SpriteRenderer>();
    }
    public void Enable()
    {
        foreach (SpriteRenderer r in allRenderers)
        {
            r.enabled = true;
        }
    }
    public void Disable()
    {
        foreach (SpriteRenderer r in allRenderers)
        {
            r.enabled = false;
        }
    }
}
