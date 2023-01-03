using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2D : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    Vector2 movement;
    string lastDir = "none";

    Animator anim;
    float movementSpeed;
    public bool plrActive = true;

    enum Direction { Up, Down, Left, Right};
    Direction facing = Direction.Up;

    BuildingSystem buildSys;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        buildSys = FindObjectOfType<BuildingSystem>();
    }
    public bool isInteractingWith(Vector3 pos)
    {
        Vector3Int targetedPos = buildSys.tilemap.WorldToCell(GetInteractArea());
        return targetedPos == buildSys.tilemap.WorldToCell(pos);
    }
    public bool isInteractingWithObject(GameObject obj)
    {
        Vector2Int arrayPos = GetInteractArrayPos();
        GameObject targetedObj = buildSys.GetValue(arrayPos.x, arrayPos.y);
        return targetedObj == obj;
    }
    public Vector2Int GetInteractArrayPos()
    {
        return buildSys.WorldToArray(GetInteractArea());
    }
    Vector3 GetInteractArea()
    {
        Vector3 pos = transform.position;
        switch (facing)
        {
            case Direction.Up:
                return pos + new Vector3(0, 1, 0);
            case Direction.Down:
                return pos + new Vector3(0, -1, 0);
            case Direction.Left:
                return pos + new Vector3(-1, 0, 0);
            case Direction.Right:
                return pos + new Vector3(1, 0, 0);
            default:
                return Vector3.zero;
        }
    }
    void Update()
    {
        if (plrActive)
        {
            if (Input.GetKeyDown("up"))
            {
                facing = Direction.Up;
                lastDir = "vertical";
            }
            else if (Input.GetKeyDown("down"))
            {
                facing = Direction.Down;
                lastDir = "vertical";
            }
            else if (Input.GetKeyDown("left"))
            {
                facing = Direction.Left;
                lastDir = "horizontal";
            }
            else if (Input.GetKeyDown("right"))
            {
                facing = Direction.Right;
                lastDir = "horizontal";
            }
        }
    }
    void FixedUpdate()
    {
        if (plrActive)
        {
            if (Input.GetAxisRaw("Horizontal") == 0f && Input.GetAxisRaw("Vertical") != 0f)
            {
                lastDir = "vertical";
            }
            else if (Input.GetAxisRaw("Horizontal") != 0f && Input.GetAxisRaw("Vertical") == 0f)
            {
                lastDir = "horizontal";
            }
            Animate();
            ProcessInputs();
        }
    }

    void ProcessInputs()
    {
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (lastDir == "horizontal")
        {
            movement.y = 0f;
        }
        else if (lastDir == "vertical")
        {
            movement.x = 0f;
        }
        movementSpeed = Mathf.Clamp(movement.magnitude, 0.0f, 1.0f);
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
    void Animate()
    {
        if (movement != Vector2.zero)
        {
            anim.SetFloat("Horizontal", movement.x);
            anim.SetFloat("Vertical", movement.y);
        }
        anim.SetFloat("Speed", movementSpeed);
    }
}
