using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchDrop : MonoBehaviour
{
    float speed = -200f;
    public float endHeight;
    public TreeHarvestGame game;
    Transform basket;
    void Start()
    {
        basket = game.basket;
    }
    Vector3 rot;
    void Update()
    {
        float pos;
        pos = speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y + pos, transform.position.z);

        if (gameObject.GetComponent<RectTransform>().anchoredPosition.y <= endHeight)
        {
            Destroy(gameObject);
        }
        transform.Rotate(new Vector3(0, 0, 1));

        rot += Vector3.forward * 180 * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rot);

        Vector3 dropPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
        if (Mathf.Abs(dropPos.x - basket.GetComponent<RectTransform>().anchoredPosition.x) < 30 && Mathf.Abs(dropPos.y - basket.GetComponent<RectTransform>().anchoredPosition.y) < 85)
        {
            game.Collect();
            Destroy(gameObject);
        }
    }
}
