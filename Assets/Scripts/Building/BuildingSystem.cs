using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.Rendering;

[System.Serializable]
    public class BuildAmount
    {
        public Build build;
        public int amount = 1;
    }
public class BuildingSystem : MonoBehaviour
{
    PlayerManager plr;
    public Transform buildObjects;
    public GameObject buildTemplate;
    public GameObject template;

    BuildInfo currentInfo;
    Item currentItem;

    public bool canBuild = true;
    bool canPlace = false;
    public Tilemap tilemap;
    public Tilemap walls;
    Tilemap currentTilemap;

    GameObject[,] gridArray;
    GameObject[,] wallArray;
    int width = 20;
    int height = 20;
    Vector2Int gridOrigin;
    GameObject[,] currentArray;

    public UI_Building objectUI;

    public List<Build> wallDecList = new List<Build>();
    public List<Item> itemList = new List<Item>();
    List<GameObject> selectedObjects = new List<GameObject>();

    BuildTrigger selected;
    GameObject selectedItem;

    [System.Serializable]
    public class BuildCategory
    {
        public Build.ObjectType category;
        public List<BuildAmount> builds = new List<BuildAmount>();
        // Future: inventory of builds (build and amount)
        // Add method
        // Remove method
    }
    public List<BuildCategory> categories = new List<BuildCategory>(); // Match tabs to categories by index


    void Start()
    {
        plr = FindObjectOfType<PlayerManager>();

        // Calculate build array sizes based on the respective tilemaps
        gridOrigin = (Vector2Int)tilemap.origin;
        width = tilemap.size.x;
        height = tilemap.size.y;
        gridArray = new GameObject[width, height];
        wallArray = new GameObject[width, height];
        currentArray = gridArray;

        currentTilemap = tilemap;

        currentInfo = new BuildInfo();

        AddExisting();
    }
    void AddExisting()
    {
        // Add any exising builds in the build area
        // FUTURE: Track tiles with non-build colliders: you cannot place anything here.
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

    // UI Control
    public void ResetOptions()
    {
        objectUI.ResetOptionsPos();
        ClearSelected();
    }
    public void OpenBuilding()
    {
        objectUI.OpenBuilding();
        canBuild = true;
        ChangeCategory(0);
    }
    public void CloseBuilding()
    {
        canBuild = false;
        ResetOptions();
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
        currentItem = null;
        currentInfo.ResetInfo();
        currentInfo.SetInfo(build);
        SetTemplate(template);
        previousPos = Vector3.zero;
    }
    public void ChangeItem(Item item)
    {
        canPlace = true;
        template.GetComponent<SpriteRenderer>().enabled = true;
        ResetOptions();
        currentInfo.ResetInfo();
        currentItem = item;
        SetTemplate(template);
        previousPos = Vector3.zero;
    }
    public void ChangeCategory(string category)
    {
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
        if (category == "ItemDecor")
        {
            currentArray = gridArray;
            currentTilemap = tilemap;
            objectUI.SetItems(itemList);
            if (canPlace == true)
            {
                ChangeItem(itemList[0]);
            }
        }
    }
    public void ChangeCategory(int index)
    {
        if (index >= 0 && index < categories.Count)
        {
            List<BuildAmount> builds = categories[index].builds;
            currentArray = gridArray;
            currentTilemap = tilemap;
            objectUI.SetObjects(builds);
            if (canPlace == true)
            {
                ChangeObject(builds[0].build);
            }
        }
        else
        {
            Debug.Log("invalid category");
        }
    }
    void RotateBuild()
    {
        currentInfo.AdvanceRotation();
        SetTemplate(template);
        previousPos = Vector3.zero;
    }

    // Object editing
    public void RotateObject()
    {
        if (!canPlace)
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
        if (!canPlace)
        {
            if (selected)
            {
                selected.RemoveDecor(selectedItem);
                selected = null;
                selectedItem = null;
            }
            else
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
        }
        selectedObjects = new List<GameObject>();
        ResetOptions();
    }

    // Array
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
    public Vector2Int WorldToArray(Vector3 pos)
    {
        return (Vector2Int)currentTilemap.WorldToCell(pos) - gridOrigin;
    }
    public Vector3 ArrayToWorld(Vector2Int pos)
    {
        return currentTilemap.CellToWorld((Vector3Int)(pos + gridOrigin)) + new Vector3(0.5f, 0.5f, 0f);
    }


    // Template
    void SetTemplate(GameObject templ)
    {
        SetTemplate(templ, new Color32(255, 0, 0, 135));
    }
    void SetTemplate(GameObject templ, Color32 color)
    {
        SpriteRenderer render = templ.GetComponent<SpriteRenderer>();
        render.spriteSortPoint = SpriteSortPoint.Pivot;
        if (currentInfo.build)
        {
            Rotation thisRotation = currentInfo.GetRotation();
            render.sprite = thisRotation.sprite;
            render.flipX = thisRotation.flipped;
            template.GetComponent<SortingGroup>().enabled = false;
        }
        else if (currentItem)
        {
            render.sprite = currentItem.asset;
            render.flipX = false;
            template.GetComponent<SortingGroup>().enabled = true;
        }
        render.color = color;
    }
    void SetTemplateColor()
    {
        SetTemplateColor(new Color32(255, 255, 255, 135));
    }
    void SetTemplateColor(Color32 color)
    {
        SpriteRenderer render = template.GetComponent<SpriteRenderer>();
        render.color = color;
        render.enabled = true;
    }

    // Updating surrounding objects
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

    // Object placing
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
    List<GameObject> templates = new List<GameObject>();
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
                Object.AddComponent<SpriteRenderer>();
                SetTemplate(Object, color);
                templates.Add(Object);
            }
        }
    }
    GameObject GetPointerOnItem(Vector3 mouseWorldPos)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero);
        List<GameObject> Objects = new List<GameObject>();
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.tag == "Decor Item")
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
        return hit;
    }

    Vector2 startPos;
    bool placing;
    bool selecting;
    bool available;

    int nearestPlace;
    Vector3 placementPos;
    BuildTrigger trigger;
    bool decorable;

    Vector2Int CalculateArrayPos()
    {
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //calculate world and array coordinates of mouse (grid dependent)
        Vector3Int selectedTile = currentTilemap.WorldToCell(point);
        return WorldToArray(point);
    }
    const int unreachableInt = -100;
    Vector3 previousPos = new Vector3(unreachableInt, unreachableInt, 0);
    bool OnPrevious(Vector3 gridPos)
    {
        //determine whether on tile in previous update
        if (gridPos == previousPos)
        {
            return true;
        }
        else
        {
            previousPos = gridPos;
            return false;
        }
    }
    void Update()
    {
        if (canBuild)
        {
            if (canPlace)
            {
                Vector2Int arrayPos = CalculateArrayPos();
                Vector3 gridPos = ArrayToWorld(arrayPos);

                if (currentInfo.build) // Place build
                {
                    // Update the template tiles and check available only each time the tile changes
                    if (!OnPrevious(gridPos))
                    {
                        template.transform.position = gridPos;
                        if (placing)
                        {
                            Debug.Log("placing");
                            template.GetComponent<SpriteRenderer>().enabled = false;
                            if (CheckAll(startPos, (Vector2)gridPos))
                            {
                                available = true;
                                Debug.Log("available");
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
                            if (CheckAll((Vector2)gridPos, (Vector2)gridPos))
                            {
                                available = true;
                                SetTemplateColor();
                            }
                            else
                            {
                                available = false;
                                SetTemplateColor(new Color32(255, 0, 0, 135));
                            }
                        }
                    }

                    //If mouse down, start placing. If mouse up place objects
                    if (Input.GetMouseButtonDown(0) && !plr.CheckOnUI())
                    {
                        startPos = (Vector2)gridPos;
                        previousPos = new Vector3(unreachableInt, unreachableInt, 0);
                        placing = true;
                    }
                    else if (Input.GetMouseButtonUp(0) && placing)
                    {
                        placing = false;
                        ClearTemplates();
                        if (available && !plr.CheckOnUI())
                        {
                            PlaceAll(startPos, (Vector2)gridPos);
                            template.GetComponent<SpriteRenderer>().enabled = false;
                            available = false;
                        }
                    }
                    if (Input.GetKeyUp("r"))
                    {
                        RotateBuild();
                    }
                }
                else if (currentItem) // Place item decor
                {
                    //calculate world and array coordinates of mouse (sprite/trigger dependent)
                    GameObject hit = plr.GetPointerOn();
                    if (hit && hit.GetComponent<BuildTrigger>())
                    {
                        arrayPos = hit.GetComponent<BuildTrigger>().info.gridPos;
                        gridPos = ArrayToWorld(arrayPos);
                    }

                    if (!OnPrevious(gridPos))
                    {
                        GameObject obj = GetValue(arrayPos.x, arrayPos.y);
                        if (obj && obj.GetComponent<BuildTrigger>() && obj.GetComponent<BuildTrigger>().CanDecorate())
                        {
                            available = false;
                            decorable = true;

                            trigger = obj.GetComponent<BuildTrigger>();
                            nearestPlace = trigger.GetNearestPlace(gridPos);

                            placementPos = trigger.GetPlacePos(nearestPlace);
                            if (trigger.PlaceEmpty(nearestPlace))
                            {
                                SetTemplateColor();
                                available = true;
                            }
                            else
                            {
                                SetTemplateColor(new Color32(255, 0, 0, 135));
                            }
                        }
                        else
                        {
                            available = false;
                            decorable = false;
                            SetTemplateColor(new Color32(255, 0, 0, 135));
                        }
                    }

                    // If on a build and placement where you can place decor, template shows in proper placement, otherwise red and in grid position
                    if (decorable)
                    {
                        template.transform.position = placementPos;
                        if (available)
                        {
                            if (Input.GetMouseButtonDown(0) && !plr.CheckOnUI())
                            {
                                trigger.PlaceDecor(currentItem, nearestPlace);
                                template.GetComponent<SpriteRenderer>().enabled = false;
                                available = false;
                            }
                        }
                    }
                    else
                    {
                        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        template.transform.position = new Vector3(point.x, point.y, 0);
                    }
                }
            }
            else
            {
                // Selecting

                //calculate world and array coordinates of mouse (sprite/trigger dependent)
                Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GameObject hit = GetPointerOnItem(point);
                if (hit && !selecting && !plr.CheckOnUI())
                {
                    selected = hit.transform.parent.GetComponent<BuildTrigger>();
                    if (Input.GetMouseButtonDown(0))
                    {
                        ClearSelected();
                        selectedItem = hit;
                        objectUI.SetOptionsPos(Camera.main.WorldToScreenPoint(new Vector3(point.x + 1f, point.y + 1.5f, 0)));
                    }
                }
                else
                {
                    hit = plr.GetPointerOn();
                    Vector2Int arrayPos = CalculateArrayPos();
                    Vector3 gridPos = ArrayToWorld(arrayPos);

                    if (hit != null && hit.GetComponent<BuildTrigger>())
                    {
                        arrayPos = hit.GetComponent<BuildTrigger>().info.gridPos;
                        gridPos = ArrayToWorld(arrayPos);
                    }

                    if (Input.GetMouseButtonDown(0) && !plr.CheckOnUI())
                    {
                        startPos = (Vector2)gridPos;
                        previousPos = new Vector3(unreachableInt, unreachableInt, 0);
                        selecting = true;
                        ResetOptions();
                    }
                    else if (Input.GetMouseButtonUp(0) && selecting)
                    {
                        selecting = false;
                        if (GetValue(arrayPos.x, arrayPos.y) == null)
                        {
                            ResetOptions();
                        }
                        else
                        {
                            objectUI.SetOptionsPos(Camera.main.WorldToScreenPoint(new Vector3(point.x + 1f, point.y + 1.5f, 0)));
                        }
                    }
                    if (!OnPrevious(gridPos))
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
    }

    // Object selecting
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
