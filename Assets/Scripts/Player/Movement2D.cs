using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Movement2D : MonoBehaviour
{
    public float walkSpeed = 5f;
    Rigidbody2D rb;
    Animator anim;
    
    bool plrActive = true;
    Vector2 movementDir;
    [HideInInspector] public float currentSpeed = 0f;

    public enum Direction { Up, Down, Left, Right};
    [HideInInspector] public Direction facing = Direction.Down;
    enum Axis { Horizontal, Vertical, None };
    Axis lastAxis = Axis.None; // Helps to predict facing direction for diagonal movement

    public SortingGroup itemHolder;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lastPosition = transform.position;
    }
    public void SetPlrActive(bool active)
    {
        plrActive = active;
        if (!plrActive)
        {
            ProcessInputs();
            Animate();
        }
    }
    public Vector2 GetFacingDirection()
    {
        switch (facing)
        {
            case Direction.Up:
                return new Vector2(0, 1);
            case Direction.Down:
                return new Vector2(0, -1);
            case Direction.Left:
                return new Vector2(-1, 0);
            case Direction.Right:
                return new Vector2(1, 0);
            default:
                return Vector3.zero;
        }
    }
    public Vector3 GetInteractArea()
    {
        Vector3 pos = transform.position;
        return pos + (Vector3)GetFacingDirection();
    }
    bool holding = false;
    public void SetHolding(bool hold)
    {
        holding = hold;
        UpdateHoldingSortOrder();
    }
    void UpdateHoldingSortOrder()
    {
        if (holding && (facing == Direction.Left || facing == Direction.Right))
        {
            itemHolder.sortingOrder = -1;
        }
        else
        {
            itemHolder.sortingOrder = 0;
        }
    }
    void SetFacing(Direction dir)
    {
        facing = dir;
        UpdateHoldingSortOrder();
    }
    public void SetFacing(Vector2Int dir)
    {
        if (dir == new Vector2Int(0, 1))
        {
            SetFacing(Direction.Up);
        }
        else if (dir == new Vector2Int(0, -1))
        {
            SetFacing(Direction.Down);
        }
        else if (dir == new Vector2Int(-1, 0))
        {
            SetFacing(Direction.Left);
        }
        else if (dir == new Vector2Int(1, 0))
        {
            SetFacing(Direction.Right);
        }
        Animate();
    }

    void FixedUpdate()
    {
        if (plrActive)
        {
            ProcessInputs();
            Animate();
        }
    }
    void UpdateFacingDirection()
    {
        if (movementDir.x == 0f)
        {
            if (movementDir.y > 0f)
            {
                facing = Direction.Up;
                lastAxis = Axis.Vertical;
            }
            else if (movementDir.y < 0f)
            {
                facing = Direction.Down;
                lastAxis = Axis.Vertical;
            }
        }
        else if (movementDir.y == 0f)
        {
            if (movementDir.x > 0f)
            {
                facing = Direction.Right;
                lastAxis = Axis.Horizontal;
            }
            else if (movementDir.x < 0f)
            {
                facing = Direction.Left;
                lastAxis = Axis.Horizontal;
            }
        }
        else if (movementDir == Vector2.zero)
        {
            lastAxis = Axis.None;
        }
        else
        {
            if (lastAxis == Axis.Horizontal)
            {
                if (movementDir.x > 0f)
                {
                    facing = Direction.Right;
                }
                else if (movementDir.x < 0f)
                {
                    facing = Direction.Left;
                }
            }
            else if (lastAxis == Axis.Vertical)
            {
                if (movementDir.y > 0f)
                {
                    facing = Direction.Up;
                }
                else if (movementDir.y < 0f)
                {
                    facing = Direction.Down;
                }
            }
        }
        UpdateHoldingSortOrder();
    }
    float stoppingTolerance = 0.001f;
    bool colliding = false;
    Vector2 lastPosition;
    void ProcessInputs()
    {
        if (plrActive)
        {
            colliding = Vector2.Distance(transform.position, lastPosition) < stoppingTolerance;
            lastPosition = transform.position;

            movementDir = (new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))).normalized;
            if (movementDir != Vector2.zero)
            {
                currentSpeed = walkSpeed;
                UpdateFacingDirection();
                rb.MovePosition(rb.position + movementDir * walkSpeed * Time.fixedDeltaTime);
            }
            else
            {
                currentSpeed = 0f;
            }
        }
        else
        {
            movementDir = Vector2.zero;
            currentSpeed = 0f;
        }
    }
    void Animate()
    {
        Vector2 dir = GetFacingDirection();
        anim.SetFloat("Horizontal", dir.x);
        anim.SetFloat("Vertical", dir.y);
        if (!colliding)
        {
            anim.SetFloat("Speed", currentSpeed);
        }
        else
        {
            anim.SetFloat("Speed", 0f);
        }
    }
}
