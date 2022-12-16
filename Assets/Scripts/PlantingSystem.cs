using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantingSystem : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile selected;
    public Tile unavailable;
    public bool canPlace;
    public GameObject sprout;

    public GameObject[,] gridArray;
    int width = 10;
    int height = 10;

    Vector3Int previousTile;
    bool justPlaced = false;

    public Inventory inv;

    void Start()
    {
        canPlace = false;
        gridArray = new GameObject[width, height];
    }
    public void TogglePlacing()
    {
        if (canPlace == false)
        {
            canPlace = true;
        }
        else
        {
            canPlace = false;
            ClearSelection();
        }
    }
    public void ClearSelection()
    {
        tilemap.ClearAllTiles();
    }
    void SetValue(int x, int y, GameObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
        }
    }
    private (GameObject, bool) GetValue(int x, int y)
    {
        GameObject value = null;
        bool inRange = false;
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            value = gridArray[x, y];
            inRange = true;
        }
        return (value, inRange);
    }
    void PlaceItem(Vector3 gridPos, int x, int y)
    {
        if (inv.GetCurrentItemAmount() > 0)
        {
            GameObject Object = Instantiate(sprout, gridPos, Quaternion.identity);
            SetValue(x, y, Object);
            inv.DepleteCurrentItem();
        }
    }
    void Update()
    {
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (canPlace)
        {
            Vector3Int selectedTile = tilemap.WorldToCell(point);
            ClearSelection();
            int x = selectedTile.x;
            int y = selectedTile.y;
            bool available = false;
            if (justPlaced == true && previousTile != selectedTile || justPlaced == false)
            {
                if (inv.GetCurrentItemAmount() > 0)
                {
                    if (GetValue(x, y) == (null, true))
                    {
                        tilemap.SetTile(selectedTile, selected);
                        available = true;
                    }
                    else
                    {
                        tilemap.SetTile(selectedTile, unavailable);
                        available = false;
                    }
                    justPlaced = false;
                }
                else
                {
                    canPlace = false;
                    ClearSelection();
                }
            }
            if (Input.GetMouseButtonDown(0) && available == true)
            {
                Vector3 tilePos = tilemap.CellToWorld(selectedTile);
                Vector3 gridPos = new Vector3(tilePos.x + 0.5f, tilePos.y + 0.5f, 0f);

                PlaceItem(gridPos, x, y);

                previousTile = selectedTile;
                justPlaced = true;
            }
        }
    }
}
