using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerDo : MonoBehaviour
{
    public UnityEvent actionOnEnter;
    public UnityEvent actionOnExit;
    public List<string> tags;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.isTrigger)
        {
            if (tags.Count == 0)
            {
                actionOnEnter.Invoke();
            }
            foreach (string thisTag in tags)
            {
                if (col.gameObject.tag == thisTag)
                {
                    actionOnEnter.Invoke();
                    return;
                }
            }
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (!col.isTrigger)
        {
            actionOnExit.Invoke();
        }
    }
}
