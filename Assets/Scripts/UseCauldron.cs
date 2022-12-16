using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseCauldron : MonoBehaviour
{
    Animator anim;
    bool inRange = false;
    bool Debounce = false;
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inRange = false;
        }
    }
    IEnumerator AnimDelay()
    {
        yield return new WaitForSeconds(0.4f);
        Debounce = false;
    }
    void Update()
    {
        if (inRange)
        {
            if (Input.GetKeyDown("e"))
            {
                if (Debounce == false)
                {
                    Debounce = true;
                    anim.SetTrigger("Splash");
                    StartCoroutine(AnimDelay());
                }
            }
        }
    }
}
