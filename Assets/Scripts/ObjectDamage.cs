using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDamage : MonoBehaviour
{
    public int maxHealth;
    public int health;
    Slider slider;
    Transform canvas;
    Transform healthBar;
    public bool useHealthBar = true;
    public GameObject structure;
    IEnumerator coroutine;
    bool shake;

    bool barActive;
    float startingPosX;
    Transform image;

    public GameObject breakingPrefab;
    public GameObject drop;

    public bool canDrop = true;
    public int dropQuantity = 1;

    void Start()
    {
        health = maxHealth;
        canvas = GameObject.Find("ObjectBar").transform;
        healthBar = canvas.Find("HealthBar");
        slider = healthBar.GetComponent<Slider>();
        if (useHealthBar)
        {
            SetHealth();
            coroutine = ShowBar();
            shake = false;
        }

        image = transform.Find("Image");
    }
    void Spawn()
    {
        Instantiate(structure, transform.position, Quaternion.identity);
    }
    void Drop()
    {
        if (canDrop)
        {
            for (int i = 1; i <= dropQuantity; i++)
            {
                Instantiate(drop, new Vector3(transform.position.x + Random.Range(-0.1f, 0.1f), transform.position.y + Random.Range(-0.1f, 0.1f), transform.position.z), Quaternion.identity);
            }
        }
    }
    void SetHealth()
    {
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = health;
        }
        barActive = false;
        startingPosX = transform.position.x;
    }
    IEnumerator ShowBar()
    {
        yield return new WaitForSeconds(0.75f);
        barActive = false;
        ResetBar();
        shake = false;
        transform.position = new Vector3(startingPosX, transform.position.y, transform.position.z);
    }
    public void ResetBar()
    {
        canvas.GetComponent<Canvas>().enabled = false;
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (useHealthBar)
            {
                ResetBar();
            }
            Instantiate(breakingPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            if (drop != null)
            {
                Drop();
            }
            if (structure != null)
            {
                Spawn();
            }
            Destroy(gameObject);
        }
        if (useHealthBar)
        {
            SetHealth();
            shake = true;
            barActive = true;
            StopCoroutine(coroutine);
            coroutine = ShowBar();
            StartCoroutine(coroutine);
        }
    }

    void Update()
    {
        if (barActive && useHealthBar)
        {
            healthBar.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            healthBar.transform.parent.GetComponent<Canvas>().enabled = true;
        }
        if (shake)
        {
            var speed = 40;
            var amount = 0.02f;
            image.position = new Vector3(startingPosX + Mathf.Sin(Time.time * speed) * amount, transform.position.y, transform.position.z);
        }
    }
}
