using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Linq;

public class BuildingSystem : MonoBehaviour
{
    Inventory plrInv;
    public GraphicRaycaster gr;
    public Transform buildObjects;
    public GameObject buildTemplate;
    public GameObject template;
    Build thisBuild;
    Rotation thisRotation;
    int rot;
    public bool canBuild = true;
    public bool canPlace = false;
    public Tilemap tilemap;
    public Tilemap walls;
    public Tilemap currentTilemap;

    GameObject[,] gridArray;
    GameObject[,] wallArray;
    int width = 20;
    int height = 20;
    int gridOriginX = -10;
    int gridOriginY = -10;
    GameObject[,] currentArray;

    UI_Building objectUI;
    GameObject doneButton;

    public List<Build> furnitureList = new List<Build>();
    public List<Build> wallDecList = new List<Build>();
    GameObject selected = null;

    public GameObject OptionUI;

    void Start()
    {
        plrInv = FindObjectOfType<Inventory>();

        Transform UI = transform.Find("BuildingMenu");
        objectUI = UI.GetComponent<UI_Building>();
        doneButton = UI.Find("DoneButton").Find("Button").gameObject;

        //Chair, Table, TallPlant, Nightstand, Plant1, Carpet1
        //Painting1, Painting2, Shelf

        gridArray = new GameObject[width, height];
        wallArray = new GameObject[width, height];
        currentArray = gridArray;

        currentTilemap = tilemap;

        //Add existing
        foreach (Transform buildObj in buildObjects)
        {
            if (buildObj.GetComponent<BuildTrigger>())
            {
                BuildInfo info = buildObj.GetComponent<BuildTrigger>().info;
                Vector2Int size = info.build.rotations[info.rot].size;
                Vector2Int arrayPos = WorldToArray(buildObj.transform.position);
                SetValue(arrayPos.x, arrayPos.y, buildObj.gameObject, size);
                info.gridPos = arrayPos;
            }
            else
            {
                Vector2Int arrayPos = WorldToArray(buildObj.transform.position);
                SetValue(arrayPos.x, arrayPos.y, buildObj.gameObject, new Vector2Int(1, 1));
            }
        }
    }
    public void RotateObject()
    {
        if (canPlace == false && selected != null)
        {
            BuildInfo info = selected.GetComponent<BuildTrigger>().info;
            Build build = info.build;
            if (build.rotations.Length > 1)
            {
                int ro = info.rot;
                if (ro < (build.rotations.Length - 1))
                {
                    ro += 1;
                }
                else
                {
                    ro = 0;
                }
                Vector2Int gridPos = info.gridPos;
                Vector2Int size = build.rotations[ro].size;
                SetValue(gridPos.x, gridPos.y, null, thisRotation.size);
                if (CheckOverlap(gridPos.x, gridPos.y, size) == false)
                {
                    thisBuild = build;
                    thisRotation = build.rotations[ro];
                    rot = ro;
                    Vector3 newPos = selected.transform.position;
                    Destroy(selected);
                    selected = PlaceItem(newPos, gridPos.x, gridPos.y);
                    Debug.Log("place");
                }
                else
                {
                    SetValue(gridPos.x, gridPos.y, selected, thisRotation.size);
                }
            }
        }
    }
    public void StoreBuild()
    {
        if (canPlace == false && selected != null)
        {
            Destroy(selected);
        }
    }
    bool CheckArrayPos(int x, int y)
    {
        return (x >= 0 && y >= 0 && x < width && y < height);
    }
    public GameObject GetValue(int x, int y)
    {
        GameObject value = null;
        if (CheckArrayPos(x, y))
        {
            value = currentArray[x, y];
        }
        return value;
    }
    private bool CheckOverlap(int originX, int originY, Vector2Int size)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                int x = originX + i;
                int y = originY + j;
                if (x >= width || y >= height || GetValue(x, y) != null || currentTilemap.GetTile(new Vector3Int(x + gridOriginX, y + gridOriginY, 0)) == null)
                {
                    return true;
                }
            }
        }
        return false;
    }
    void SetTemplate()
    {
        template.GetComponent<SpriteRenderer>().sprite = thisRotation.sprite;
        template.GetComponent<SpriteRenderer>().flipX = thisRotation.flipped;
    }
    public void ResetOptions()
    {
        OptionUI.GetComponent<RectTransform>().position = new Vector3(-10, -10, 0);
        OptionUI.GetComponent<Image>().enabled = false;
        selected = null;
    }
    public void OpenBuilding()
    {
        CancelPlace();
        objectUI.gameObject.GetComponent<Canvas>().enabled = true;
        canBuild = true;
        ChangeCategory("Furniture");
        plrInv.HideInterface();
    }
    public void CloseBuilding()
    {
        CancelPlace();
        objectUI.gameObject.GetComponent<Canvas>().enabled = false;
        canBuild = false;
        ResetOptions();
        plrInv.ShowInterface();
    }
    public void CancelPlace()
    {
        canPlace = false;
        template.GetComponent<SpriteRenderer>().enabled = false;
        doneButton.GetComponent<Image>().enabled = false;
        doneButton.GetComponent<Button>().enabled = false;
    }
    public void ChangeObject(Build build)
    {
        canPlace = true;
        template.GetComponent<SpriteRenderer>().enabled = true;
        doneButton.GetComponent<Image>().enabled = true;
        doneButton.GetComponent<Button>().enabled = true;
        ResetOptions();
        thisBuild = build;
        thisRotation = build.rotations[0];
        rot = 0;
        SetTemplate();
        previousTile = new Vector3Int(0, 0, 0);
    }
    public void ChangeCategory(string category)
    {
        if (category == "Furniture")
        {
            currentArray = gridArray;
            currentTilemap = tilemap;
            objectUI.SetObjects(furnitureList);
            if (canPlace == true)
            {
                ChangeObject(furnitureList[0]);
            }
        }
        if (category == "WallDecor")
        {
            currentArray = wallArray;
            currentTilemap = walls;
            objectUI.SetObjects(wallDecList);
            if (canPlace == true)
            {
                ChangeObject(wallDecList[0]);
            }
        }
    }
    void RotateBuild()
    {
        if (rot < (thisBuild.rotations.Length - 1))
        {
            rot += 1;
        }
        else
        {
            rot = 0;
        }
        thisRotation = thisBuild.rotations[rot];
        SetTemplate();
        previousTile = new Vector3Int(0, 0, 0);
    }
    void SetValue(int x, int y, GameObject value, Vector2Int size) // work for 2x2 and so on
    {
        if (CheckArrayPos(x, y))
        {
            currentArray[x, y] = value;
            if (size.x > 1)
            {
                currentArray[x + (size.x - 1), y] = value;
            }
            if (size.y > 1)
            {
                currentArray[x, y + (size.y - 1)] = value;
            }
        }
    }
    IEnumerator Pop(GameObject Object)
    {
        yield return new WaitForSeconds(0.1f);
        Object.transform.localScale = new Vector3(1f, 1f, 1f);
    }
    GameObject PlaceItem(Vector3 gridPos, int x, int y)
    {
        GameObject Object = Instantiate(buildTemplate, gridPos, Quaternion.identity);
        Object.transform.SetParent(buildObjects);
        Object.GetComponent<BuildTrigger>().SetObject(thisBuild, rot, x, y);
        SetValue(x, y, Object, thisRotation.size);
        Object.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        StartCoroutine(Pop(Object));
        return Object;
    }
    public Vector2Int WorldToArray(Vector3 pos)
    {
        return (Vector2Int)currentTilemap.WorldToCell(pos) - new Vector2Int(gridOriginX, gridOriginY);
    }
    Vector3Int previousTile;
    void Update()
    {
        if (canBuild)
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            bool onUI = false;
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(ped, results);
            if (results.Count == 0)
            {
                onUI = false;
            }
            else
            {
                onUI = true;
            }
            if (canPlace)
            {
                Vector3Int selectedTile = currentTilemap.WorldToCell(point);
                Vector2Int arrayPos = WorldToArray(point);
                int x = arrayPos.x;
                int y = arrayPos.y;

                Vector3 tilePos = currentTilemap.CellToWorld(selectedTile);
                Vector3 gridPos = new Vector3(tilePos.x + 0.5f, tilePos.y + 0.5f, 0f);
                template.transform.position = gridPos;
                bool available = false;

                bool onPrevious = false;
                if (selectedTile == previousTile)
                {
                    onPrevious = true;
                    template.GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                {
                    onPrevious = false;
                    previousTile = new Vector3Int(0, 0, 0);
                    template.GetComponent<SpriteRenderer>().enabled = true;
                }

                Vector2Int rotSize = thisRotation.size;
                if (x >= 0 && y >= 0 && x < width && y < height && CheckOverlap(x, y, rotSize) == false && currentTilemap.GetTile(selectedTile) != null)
                {
                    available = true;
                    template.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 135);
                }
                else
                {
                    available = false;
                    if (onPrevious == false)
                    {
                        template.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 135);
                    }
                }

                if (Input.GetMouseButtonDown(0) && available == true && onUI == false)
                {
                    PlaceItem(gridPos, x, y);
                    previousTile = selectedTile;
                    onPrevious = true;
                }
                if (Input.GetKeyUp("r"))
                {
                    RotateBuild();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && onUI == false)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(point, Vector2.zero);
                    List<GameObject> Objects = new List<GameObject>();
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if (hits[i].collider != null && hits[i].collider.gameObject.GetComponent<BuildTrigger>())
                        {
                            Objects.Add(hits[i].collider.gameObject);
                        }
                    }
                    Objects = Objects.OrderBy(e => e.transform.position.y).ToList();
                    GameObject hit = null;
                    if (Objects.Count > 0)
                    {
                        hit = Objects[0];
                    }
                    if (hit != null)
                    {
                        Debug.Log("selected");
                        SelectObject(hit);
                    }
                    else
                    {
                        selected = null;
                    }
                }
            }
        }
    }
    public void SelectObject(GameObject thisObject)
    {
        selected = thisObject;
        thisObject.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        StartCoroutine(Pop(thisObject));
    }
    private void LateUpdate()
    {
        if (canBuild && canPlace == false)
        {
            if (selected != null)
            {
                Vector3 pos = selected.transform.position;
                Vector3 screenPos = new Vector3(pos.x + 1, pos.y + 1.5f, 0);
                OptionUI.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(screenPos);
                OptionUI.GetComponent<Image>().enabled = true;
            }
            else
            {
                ResetOptions();
            }
        }
    }
}
