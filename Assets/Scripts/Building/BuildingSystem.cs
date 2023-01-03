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
    GraphicRaycaster gr;
    public Transform buildObjects;
    public GameObject buildTemplate;
    public GameObject template;
    Build thisBuild;
    int rot;
    public bool canBuild = true;
    public bool canPlace = false;
    public Tilemap tilemap;
    public Tilemap walls;
    Tilemap currentTilemap;

    GameObject[,] gridArray;
    GameObject[,] wallArray;
    int width = 20;
    int height = 20;
    Vector2Int gridOrigin;
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
        gr = UI.GetComponent<GraphicRaycaster>();
        objectUI = UI.GetComponent<UI_Building>();
        doneButton = UI.Find("DoneButton").Find("Button").gameObject;

        gridOrigin = (Vector2Int)tilemap.origin;
        width = tilemap.size.x;
        height = tilemap.size.y;
        gridArray = new GameObject[width, height];
        wallArray = new GameObject[width, height];
        currentArray = gridArray;

        currentTilemap = tilemap;

        //Add existing
        foreach (Transform buildObj in buildObjects)
        {
            BuildTrigger buildTrigger = buildObj.GetComponent<BuildTrigger>();
            if (buildTrigger && buildTrigger.info.build)
            {
                Vector2Int arrayPos = WorldToArray(buildObj.transform.position);
                buildTrigger.SetObject(buildTrigger.info.build, buildTrigger.info.rot, arrayPos.x, arrayPos.y);
                SetValue(arrayPos.x, arrayPos.y, buildObj.gameObject, buildTrigger.info.GetRotation().size);
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
        if (canPlace == false && selected != null && selected.GetComponent<BuildTrigger>())
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
                Vector2Int size = build.GetRotation(ro).size;
                SetValue(gridPos.x, gridPos.y, null, info.GetRotation().size);
                if (CheckOverlap(gridPos.x, gridPos.y, size) == false)
                {
                    thisBuild = build;
                    rot = ro;
                    Vector3 newPos = selected.transform.position;
                    Destroy(selected);
                    selected = PlaceItem(newPos, gridPos.x, gridPos.y);
                }
                else
                {
                    SetValue(gridPos.x, gridPos.y, selected, thisBuild.GetRotation(rot).size);
                }
            }
        }
    }
    public void StoreBuild()
    {
        if (canPlace == false && selected != null)
        {
            if (selected.GetComponent<BuildTrigger>())
            {
                BuildInfo info = selected.GetComponent<BuildTrigger>().info;
                SetValue(info.gridPos.x, info.gridPos.y, null, info.build.GetRotation(rot).size);
                Destroy(selected);
            }
            else if (selected.transform.parent.GetComponent<BuildTrigger>())
            {
                BuildTrigger trigger = selected.transform.parent.GetComponent<BuildTrigger>();
                int place = trigger.GetDecorPlacement(selected);
                if (place != -1)
                {
                    trigger.ClearPlacement(place);
                }
            }
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
                if (x >= width || y >= height || GetValue(x, y) != null || currentTilemap.GetTile(new Vector3Int(x + gridOrigin.x, y + gridOrigin.y, 0)) == null)
                {
                    return true;
                }
            }
        }
        return false;
    }
    void SetTemplate()
    {
        template.GetComponent<SpriteRenderer>().sprite = thisBuild.GetRotation(rot).sprite;
        template.GetComponent<SpriteRenderer>().flipX = thisBuild.GetRotation(rot).flipped;
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
        SetTemplate();
        previousTile = new Vector3Int(0, 0, 0);
    }

    public GameObject[] GetAdjacentObjects(Vector2Int originPos)
    {
        GameObject[] adjacentBuilds = new GameObject[4];
        adjacentBuilds[0] = GetValue(originPos.x, originPos.y + 1);
        adjacentBuilds[1] = GetValue(originPos.x + 1, originPos.y);
        adjacentBuilds[2] = GetValue(originPos.x, originPos.y - 1);
        adjacentBuilds[3] = GetValue(originPos.x - 1, originPos.y);

        return adjacentBuilds;
    }
    public GameObject[] GetCornerObjects(Vector2Int originPos)
    {
        GameObject[] cornerBuilds = new GameObject[4];
        cornerBuilds[0] = GetValue(originPos.x - 1, originPos.y + 1);
        cornerBuilds[1] = GetValue(originPos.x + 1, originPos.y + 1);
        cornerBuilds[2] = GetValue(originPos.x + 1, originPos.y - 1);
        cornerBuilds[3] = GetValue(originPos.x - 1, originPos.y - 1);

        return cornerBuilds;
    }
    public GameObject[] GetSurroundingObjects(Vector2Int originPos, Vector2Int size)
    {
        int arraySize = 2 * size.x + 2 * size.y + 4;
        GameObject[] adjacentBuilds = new GameObject[arraySize];
        int i = 0;
        adjacentBuilds[0] = GetValue(originPos.x - 1, originPos.y + size.y);
        i++;
        for (int j = 0; j < size.x; j++)
        {
            adjacentBuilds[i] = GetValue(originPos.x + j, originPos.y + size.y);
            i++;
        }
        adjacentBuilds[i] = GetValue(originPos.x + size.x, originPos.y + size.y);
        i++;
        for (int j = size.y - 1; j >= 0; j--)
        {
            adjacentBuilds[i] = GetValue(originPos.x + size.x, originPos.y + j);
            i++;
        }
        adjacentBuilds[i] = GetValue(originPos.x + size.x, originPos.y - 1);
        i++;
        for (int j = size.x - 1; j >= 0; j--)
        {
            adjacentBuilds[i] = GetValue(originPos.x + j, originPos.y - 1);
            i++;
        }
        adjacentBuilds[i] = GetValue(originPos.x - 1, originPos.y - 1);
        i++;
        for (int j = 0; j < size.y; j++)
        {
            adjacentBuilds[i] = GetValue(originPos.x - 1, originPos.y + j);
            i++;
        }

        return adjacentBuilds;
    }

    void SetValue(int x, int y, GameObject value, Vector2Int size)
    {
        if (CheckArrayPos(x, y))
        {
            currentArray[x, y] = value;
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    currentArray[x + i, y + j] = value;
                }
            }
            if (value != null && value.GetComponent<BuildTrigger>())
            {
                value.GetComponent<BuildTrigger>().UpdateSprite();
            } 
            foreach (GameObject obj in GetSurroundingObjects(new Vector2Int(x, y), size))
            {
                if (obj != null && obj.GetComponent<BuildTrigger>())
                {
                    obj.GetComponent<BuildTrigger>().UpdateSprite();
                }
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
        GameObject Object;
        Rotation thisRotation = thisBuild.GetRotation(rot);
        if (thisRotation.Object)
        {
            Object = Instantiate(thisRotation.Object, gridPos, Quaternion.identity);
            if (!Object.GetComponent<BuildTrigger>())
            {
                Object.AddComponent<BuildTrigger>();
            }
        }
        else
        {
            Object = Instantiate(buildTemplate, gridPos, Quaternion.identity);
        }
        Object.transform.SetParent(buildObjects);
        Object.GetComponent<BuildTrigger>().SetObject(thisBuild, rot, x, y);
        SetValue(x, y, Object, thisRotation.size);
        Object.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        StartCoroutine(Pop(Object));
        return Object;
    }
    public Vector2Int WorldToArray(Vector3 pos)
    {
        return (Vector2Int)currentTilemap.WorldToCell(pos) - gridOrigin;
    }

    const int unreachableInt = -100;
    Vector3Int previousTile = new Vector3Int(unreachableInt, unreachableInt, 0);
    void Update()
    {
        if (canBuild)
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check on UI
            bool onUI = false;
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(ped, results);
            onUI = results.Count != 0;

            if (canPlace)
            {
                Vector3Int selectedTile = currentTilemap.WorldToCell(point);
                Vector2Int arrayPos = WorldToArray(point);
                int x = arrayPos.x;
                int y = arrayPos.y;

                Vector3 gridPos = currentTilemap.CellToWorld(selectedTile) + new Vector3(0.5f, 0.5f, 0f);
                template.transform.position = gridPos;

                bool onPrevious = false;
                if (selectedTile == previousTile)
                {
                    onPrevious = true;
                    template.GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                {
                    onPrevious = false;
                    previousTile = new Vector3Int(unreachableInt, unreachableInt, 0);
                    template.GetComponent<SpriteRenderer>().enabled = true;
                }

                bool available = false;
                if (CheckArrayPos(x, y) && !CheckOverlap(x, y, thisBuild.GetRotation(rot).size) && currentTilemap.GetTile(selectedTile) != null)
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
                        if (hits[i].collider != null && (hits[i].collider.gameObject.GetComponent<BuildTrigger>() || hits[i].collider.transform.parent.GetComponent<BuildTrigger>()))
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

        //PlaceDecor(item, trigger.GetNearestPlacement(pos)
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
