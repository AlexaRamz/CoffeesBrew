using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropGrow : MonoBehaviour
{
    float growTime = 10f;
    public Sprite stage2;
    public Sprite crop;
    ObjectDamage objectDamage;

    void Start()
    {
        StartCoroutine(Grow());
        objectDamage = gameObject.GetComponent<ObjectDamage>();
        objectDamage.dropQuantity = 1;
    }
    IEnumerator Grow()
    {
        yield return new WaitForSeconds(growTime / 2);
        gameObject.GetComponent<SpriteRenderer>().sprite = stage2;
        StartCoroutine(Finish());
    }
    IEnumerator Finish()
    {
        yield return new WaitForSeconds(growTime / 2);
        gameObject.GetComponent<SpriteRenderer>().sprite = crop;
        objectDamage.dropQuantity = 2;
    }
    void Update()
    {
        
    }
}
