using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    Inventory plrInv;
    GraphicRaycaster gr;
    public Transform buildObjects;
    public GameObject buildTemplate;
    public GameObject template;

    BuildInfo currentInfo;

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

    public List<Build> furnitureList = new List<Build>();
    public List<Build> wallDecList = new List<Build>();
    public List<GameObject> selectedObjects = new List<GameObject>();

    public GameObject OptionUI;

    void Start()
    {
        plrInv = FindObjectOfType<Inventory>();

        Transform UI = transform.Find("BuildingMenu");
        gr = UI.GetComponent<GraphicRaycaster>();
        objectUI = UI.GetComponent<UI_Building>();

        gridOrigin = (Vector2Int)tilemap.origin;
        width = tilemap.size.x;
        height = tilemap.size.y;
        gridArray = new GameObject[width, height];
        wallArray = new GameObject[width, height];
        currentArray = gridArray;

        currentTilemap = tilemap;

        currentInfo = new BuildInfo();

        //Add existing
        foreach (Transform buildObj in buildObjects)
        {
            BuildTrigger buildTrigger = buildObj.GetComponent<BuildTrigger>();
            if (buildTrigger && buildTrigger.info.build)
            {
                Vector2Int arrayPos = WorldToArray(buildObj.transform.position);
                buildTrigger.SetObject(buildTrigger.info, arrayPos);
                SetValue(arrayPos, buildObj.gameObject, buildTrigger.info.GetRotation().size);
            }
            else
            {
                Vector2Int arrayPos = WorldToArray(buildObj.transform.position);
                SetValue(arrayPos, buildObj.gameObject, new Vector2Int(1, 1));
            }
        }
    }
    public void RotateObject()
    {
        if (canPlace == false)
        {
            List<GameObject> newObjects = new List<GameObject>();
            foreach (GameObject obj in selectedObjects)
            {
                if (obj.GetComponent<BuildTrigger>())
                {
                    BuildInfo info = obj.GetComponent<BuildTrigger>().info;
                    Vector2Int gridPos = info.gridPos;
                    if (info.CanRotate() && !CheckOverlap(gridPos, info.GetNextRotation().size, obj))
                    {
                        SetValue(gridPos, null, info.GetRotation().size);
                        info.AdvanceRotation();
                        currentInfo.SetInfo(info.build, info.rot);
                        newObjects.Add(PlaceItem(obj.transform.position));
                        Destroy(obj);
                    }
                }
            }
            selectedObjects = newObjects;
        }
    }
    public void StoreBuild()
    {
        if (canPlace == false)
        {
            while (selectedObjects.Count != 0)
            {
                GameObject obj = selectedObjects[selectedObjects.Count - 1];
                if (obj && obj.GetComponent<BuildTrigger>())
                {
                    BuildInfo info = obj.GetComponent<BuildTrigger>().info;
                    SetValue(info.gridPos, null, info.GetRotation().size);
                    selectedObjects.RemoveAt(selectedObjects.Count - 1);
                    Destroy(obj);
                }
                else if (obj.transform.parent.GetComponent<BuildTrigger>())
                {
                    BuildTrigger trigger = obj.transform.parent.GetComponent<BuildTrigger>();
                    int place = trigger.GetDecorPlacement(obj);
                    if (place != -1)
                    {
                        trigger.ClearPlacement(place);
                    }
                }
            }
        }
        selectedObjects = new List<GameObject>();
        ResetOptions();
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
    private bool CheckOverlap(Vector2Int origin, Vector2Int size, GameObject ignoreObj = null)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                int x = origin.x + i;
                int y = origin.y + j;
                GameObject obj = GetValue(x, y);
                if (x >= width || y >= height || (obj != null && obj != ignoreObj) || currentTilemap.GetTile(new Vector3Int(x + gridOrigin.x, y + gridOrigin.y, 0)) == null)
                {
                    return true;
                }
            }
        }
        return false;
    }
    void SetTemplate()
    {
        template.GetComponent<SpriteRenderer>().sprite = currentInfo.GetRotation().sprite;
        template.GetComponent<SpriteRenderer>().flipX = currentInfo.GetRotation().flipped;
    }
    public void ResetOptions()
    {
        OptionUI.GetComponent<RectTransform>().position = new Vector3(-10, -10, 0);
        OptionUI.GetComponent<Image>().enabled = false;
        ClearSelected();
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
        ClearTemplates();
    }
    public void ChangeObject(Build build)
    {
        canPlace = true;
        template.GetComponent<SpriteRenderer>().enabled = true;
        ResetOptions();
        currentInfo.ResetInfo();
        currentInfo.build = build;
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
        currentInfo.AdvanceRotation();
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

    void SetValue(Vector2Int arrayPos, GameObject value, Vector2Int size)
    {
        int x = arrayPos.x;
        int y = arrayPos.y;
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
        Object.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        yield return new WaitForSeconds(0.1f);
        Object.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    GameObject PlaceItem(Vector3 worldPos)
    {
        Vector2Int arrayPos = WorldToArray(worldPos);
        GameObject Object;
        Rotation thisRotation = currentInfo.GetRotation();
        if (thisRotation.Object)
        {
            Object = Instantiate(currentInfo.GetRotation().Object, worldPos, Quaternion.identity);
            if (!Object.GetComponent<BuildTrigger>())
            {
                Object.AddComponent<BuildTrigger>();
            }
        }
        else
        {
            Object = Instantiate(buildTemplate, worldPos, Quaternion.identity);
        }
        Object.transform.SetParent(buildObjects);
        BuildTrigger trigger = Object.GetComponent<BuildTrigger>();
        trigger.SetObject(currentInfo, arrayPos);
        SetValue(arrayPos, Object, thisRotation.size);
        StartCoroutine(Pop(Object));

        return Object;
    }
    bool CheckAll(Vector2 startPos, Vector2 endPos)
    {
        float startX;
        float endX;
        float startY;
        float endY;

        if (startPos.x > endPos.x)
        {
            startX = endPos.x;
            endX = startPos.x;
        }
        else
        {
            startX = startPos.x;
            endX = endPos.x;
        }
        if (startPos.y > endPos.y)
        {
            startY = endPos.y;
            endY = startPos.y;
        }
        else
        {
            startY = startPos.y;
            endY = endPos.y;
        }
        for (float i = startX; i <= endX; i++)
        {
            for (float j = startY; j <= endY; j++)
            {
                Vector2Int arrayPos = WorldToArray(new Vector3(i, j, 0));
                if (!CheckArrayPos(arrayPos.x, arrayPos.y) || CheckOverlap(arrayPos, currentInfo.GetRotation().size) || currentTilemap.GetTile(currentTilemap.WorldToCell(new Vector3(i, j, 0))) == null)
                {
                    return false;
                }
            }
        }
        return true;
    }
    void PlaceAll(Vector2 startPos, Vector2 endPos)
    {
        float startX;
        float endX;
        float startY;
        float endY;

        if (startPos.x > endPos.x)
        {
            startX = endPos.x;
            endX = startPos.x;
        }
        else
        {
            startX = startPos.x;
            endX = endPos.x;
        }
        if (startPos.y > endPos.y)
        {
            startY = endPos.y;
            endY = startPos.y;
        }
        else
        {
            startY = startPos.y;
            endY = endPos.y;
        }
        for (float i = startX; i <= endX; i++)
        {
            for (float j = startY; j <= endY; j++)
            {
                PlaceItem(new Vector3(i, j, 0));
            }
        }
    }

    //Manage templates while placing
    public List<GameObject> templates = new List<GameObject>();
    void ClearTemplates()
    {
        foreach (GameObject template in templates)
        {
            Destroy(template);
        }
    }
    void UpdateTemplates(Vector2 startPos, Vector2 endPos, Color32 color)
    {
        ClearTemplates();
        float startX;
        float endX;
        float startY;
        float endY;

        if (startPos.x > endPos.x)
        {
            startX = endPos.x;
            endX = startPos.x;
        }
        else
        {
            startX = startPos.x;
            endX = endPos.x;
        }
        if (startPos.y > endPos.y)
        {
            startY = endPos.y;
            endY = startPos.y;
        }
        else
        {
            startY = startPos.y;
            endY = endPos.y;
        }
        for (float i = startX; i <= endX; i++)
        {
            for (float j = startY; j <= endY; j++)
            {
                GameObject Object = new GameObject();
                Object.transform.position = new Vector3(i, j, 0);
                SpriteRenderer render = Object.AddComponent<SpriteRenderer>();
                render.spriteSortPoint = SpriteSortPoint.Pivot;
                Rotation thisRotation = currentInfo.GetRotation();
                render.color = color;
                render.flipX = thisRotation.flipped;
                render.sprite = thisRotation.sprite;
                templates.Add(Object);
            }
        }
    }

    public Vector2Int WorldToArray(Vector3 pos)
    {
        return (Vector2Int)currentTilemap.WorldToCell(pos) - gridOrigin;
    }

    const int unreachableInt = -100;
    Vector3Int previousTile = new Vector3Int(unreachableInt, unreachableInt, 0);
    Vector2 startPos;
    bool placing;
    bool selecting;
    bool available;
    bool clickingUI;

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

            //calculate world and array coordinates of mouse
            Vector3Int selectedTile = currentTilemap.WorldToCell(point);
            Vector2Int arrayPos = WorldToArray(point);
            int x = arrayPos.x;
            int y = arrayPos.y;

            Vector3 gridPos = currentTilemap.CellToWorld(selectedTile) + new Vector3(0.5f, 0.5f, 0f);

            //determine whether on tile in previous update
            bool onPrevious = false;
            if (selectedTile == previousTile)
            {
                onPrevious = true;
            }
            else
            {
                onPrevious = false;
                previousTile = selectedTile;
            }

            if (canPlace)
            {
                //update the template tiles and check available only each time the tile changes
                if (!onPrevious)
                {
                    template.transform.position = gridPos;
                    if (placing)
                    {
                        template.GetComponent<SpriteRenderer>().enabled = false;
                        if (CheckAll(startPos, (Vector2)gridPos))
                        {
                            available = true;
                            UpdateTemplates(startPos, (Vector2)gridPos, new Color32(255, 255, 255, 135));
                        }
                        else
                        {
                            available = false;
                            UpdateTemplates(startPos, (Vector2)gridPos, new Color32(255, 0, 0, 135));
                        }
                    }
                    else
                    {
                        template.GetComponent<SpriteRenderer>().enabled = true;
                        if (CheckAll((Vector2)gridPos, (Vector2)gridPos))
                        {
                            available = true;
                            template.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 135);
                        }
                        else
                        {
                            available = false;
                            template.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 135);
                        }
                    }
                }
                
                //If mouse down, start placing. If mouse up place objects
                if (Input.GetMouseButtonDown(0) && available && !onUI)
                {
                    startPos = (Vector2)gridPos;
                    placing = true;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    placing = false;
                    ClearTemplates();
                    if (available && !onUI)
                    {
                        PlaceAll(startPos, (Vector2)gridPos);
                        template.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
                if (Input.GetKeyUp("r"))
                {
                    RotateBuild();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (!onUI)
                    {
                        clickingUI = false;
                        startPos = (Vector2)gridPos;
                        previousTile = new Vector3Int(unreachableInt, unreachableInt, 0);
                        selecting = true;
                        ResetOptions();
                    }
                    else
                    {
                        clickingUI = true;
                    }
                }
                else if (Input.GetMouseButtonUp(0) && !onUI && !clickingUI)
                {
                    selecting = false;
                    if (GetValue(x, y) == null)
                    {
                        ResetOptions();
                    }
                    else
                    {
                        OptionUI.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(new Vector3(point.x + 1f, point.y + 1.5f, 0));
                        OptionUI.GetComponent<Image>().enabled = true;
                    }
                }
                if (!onPrevious)
                {
                    if (selecting)
                    {
                        SelectAll(startPos, (Vector2)gridPos);
                        UpdateSelected(new Color32(140, 255, 140, 255));
                    }
                }
            }
        }
    }
    //public void SelectObject(GameObject thisObject)
    //    StartCoroutine(Pop(thisObject));

        //PlaceDecor(item, trigger.GetNearestPlacement(pos)
    void SelectAll(Vector2 start, Vector2 end)
    {
        ClearSelected();
        int startX;
        int endX;
        int startY;
        int endY;

        Vector2Int startPos = WorldToArray(start);
        Vector2Int endPos = WorldToArray(end);

        if (startPos.x > endPos.x)
        {
            startX = endPos.x;
            endX = startPos.x;
        }
        else
        {
            startX = startPos.x;
            endX = endPos.x;
        }
        if (startPos.y > endPos.y)
        {
            startY = endPos.y;
            endY = startPos.y;
        }
        else
        {
            startY = startPos.y;
            endY = endPos.y;
        }
        for (int i = startX; i <= endX; i++)
        {
            for (int j = startY; j <= endY; j++)
            {
                GameObject obj = GetValue(i, j);
                if (obj)
                {
                    selectedObjects.Add(obj);
                }
            }
        }
    }
    void ClearSelected()
    {
        UpdateSelected(new Color32(255, 255, 255, 255));
        selectedObjects = new List<GameObject>();
    }
    void UpdateSelected(Color32 color)
    {
        foreach (GameObject obj in selectedObjects)
        {
            if (obj && obj.GetComponent<BuildTrigger>())
            {
                obj.GetComponent<BuildTrigger>().SetColor(color);
            }
        }
    }
}
