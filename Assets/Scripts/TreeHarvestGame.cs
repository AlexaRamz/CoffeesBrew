using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeHarvestGame : MonoBehaviour
{
    Transform gameContainer;
    Transform spawn;
    public Transform basket;
    Transform dropContainer;
    Transform itemContainer;
    public Transform itemSlotTemplate;
    public RectTransform drop;
    public bool playing = false;
    bool canSpawn = true;
    public bool canToggle = true;
    bool canCollect = true;

    public Item item;
    public ItemInfo reward;

    int points;
    int maxPoints = 4;

    public Inventory inv;

    IEnumerator coroutine;

    void Start()
    {
        gameContainer = transform.Find("Minigame");
        basket = gameContainer.Find("Basket");
        spawn = transform.Find("Spawn");
        dropContainer = gameContainer.Find("DropContainer");
        itemContainer = transform.Find("Display").Find("ItemSlotContainer");
    }
    public void StartGame()
    {
        playing = true;
        canSpawn = true;
        canCollect = true;
        gameObject.GetComponent<Canvas>().enabled = true;
        Debug.Log("on");
        canToggle = false;
        StartCoroutine(ToggleDelay());
    }
    IEnumerator ToggleDelay()
    {
        yield return new WaitForSeconds(0.1f);
        canToggle = true;
    }
    public void CloseGame()
    {
        Debug.Log("off");
        playing = false;
        canSpawn = false;
        gameObject.GetComponent<Canvas>().enabled = false;
        ClearItems();
        ClearDrops();
        points = 0;
        canToggle = false;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        StartCoroutine(ToggleDelay());
    }
    void ClearItems()
    {
        foreach (Transform slot in itemContainer.transform)
        {
            Destroy(slot.gameObject);
        }
    }
    void ClearDrops()
    {
        foreach (Transform child in dropContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1f);
        ClearItems();
        CloseGame();
    }
    public void Collect()
    {
        if (canCollect)
        {
            Debug.Log("collected");
            points += 1;
            if (points > 0)
            {
                ClearItems();
                for (int i = 0; i < points; i++)
                {
                    RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemContainer).GetComponent<RectTransform>();
                    itemSlotRectTransform.Find("Image").GetComponent<Image>().sprite = item.asset;
                    itemSlotRectTransform.Find("Image").GetComponent<Image>().enabled = true;
                    Debug.Log("new");
                }
            }
            if (points == maxPoints)
            {
                points = 0;
                inv.DepleteCurrentItem();
                inv.CollectItem(reward);
                canCollect = false;
                StartCoroutine(EndGame());
            }
        }
    }
    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        if (playing)
        {
            float spawnWidth = spawn.GetComponent<RectTransform>().rect.width;
            Vector3 pos = new Vector3(spawn.GetComponent<RectTransform>().anchoredPosition.x + Random.Range(-1 * spawnWidth / 2, spawnWidth / 2), spawn.GetComponent<RectTransform>().anchoredPosition.y, 0);
            RectTransform myDrop = Instantiate(drop);
            myDrop.anchoredPosition = pos;
            myDrop.SetParent(dropContainer, false);
            myDrop.GetComponent<CatchDrop>().endHeight = transform.Find("End").GetComponent<RectTransform>().anchoredPosition.y;
            myDrop.GetComponent<CatchDrop>().game = this;
            canSpawn = true;
        }
    }
    void Update()
    {
        if (playing)
        {
            if (inv.currentItem != null && (inv.currentItem.item.name == "Basket" || inv.currentItem.item.name == "AppleBasket"))
            {
                Vector3 mousePos = Input.mousePosition;
                basket.GetComponent<RectTransform>().position = new Vector3(mousePos.x, basket.GetComponent<RectTransform>().position.y, 0);

                if (canSpawn)
                {
                    coroutine = Spawn();
                    StartCoroutine(coroutine);
                    canSpawn = false;
                }
                if (Input.GetKeyDown("e") && canToggle)
                {
                    CloseGame();
                    canToggle = false;
                    StartCoroutine(ToggleDelay());
                }
            }
            else
            {
                CloseGame();
                canToggle = false;
                StartCoroutine(ToggleDelay());
            }
        }
    }
}
