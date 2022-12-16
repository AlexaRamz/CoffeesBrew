using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakParticles : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Animator>().SetTrigger("Break");
        StartCoroutine(DestroyDelay());
    }
    IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
    void Update()
    {
        
    }
}
