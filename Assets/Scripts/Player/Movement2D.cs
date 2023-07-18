using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Movement2D : MonoBehaviour
{
    public float moveSpeed = 5f;
    Rigidbody2D rb;
    Vector2 movement;
    string lastDir = "none";

    Animator anim;
    public float movementSpeed;
    bool plrActive = true;

    public enum Direction { Up, Down, Left, Right};
    [HideInInspector] public Direction facing = Direction.Up;

    public Collider2D upTrigger;
    public Collider2D downTrigger;
    public Collider2D leftTrigger;
    public Collider2D rightTrigger;

    public SortingGroup itemHolder;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    public Vector3 GetInteractArea()
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
    public void SetPlrActive(bool active)
    {
        plrActive = active;
        if (!plrActive)
        {
            ProcessInputs();
            Animate();
        }
    }
    // Determine whether there is a collision in direction currently facing
    bool colliding = false;
    public void SetCollidingBool(bool b)
    {
        colliding = b;
    }
    void SetTriggerDirection()
    {
        upTrigger.enabled = false;
        downTrigger.enabled = false;
        leftTrigger.enabled = false;
        rightTrigger.enabled = false;

        switch (facing)
        {
            case Direction.Up:
                upTrigger.enabled = true;
                return;
            case Direction.Down:
                downTrigger.enabled = true;
                return;
            case Direction.Left:
                leftTrigger.enabled = true;
                return;
            case Direction.Right:
                rightTrigger.enabled = true;
                return;
        }
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
        SetTriggerDirection();
    }
    void SetFacing()
    {
        if (plrActive)
        {
            Vector2 inputs = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (lastDir == "vertical")
            {
                if (inputs.x > 0f)
                {
                    facing = Direction.Right;
                }
                else if (inputs.x < 0f)
                {
                    facing = Direction.Left;
                }
            }
            else if (lastDir == "horizontal")
            {
                if (inputs.y > 0f)
                {
                    facing = Direction.Up;
                }
                else if (inputs.y < 0f)
                {
                    facing = Direction.Down;
                }
            }
            UpdateHoldingSortOrder();
            SetTriggerDirection();
        }
    }
    void Update()
    {
        if (plrActive)
        {
            if (Input.GetKeyDown("up"))
            {
                lastDir = "vertical";
                SetFacing(Direction.Up);
            }
            else if (Input.GetKeyDown("down"))
            {
                lastDir = "vertical";
                SetFacing(Direction.Down);
            }
            else if (Input.GetKeyDown("left"))
            {
                lastDir = "horizontal";
                SetFacing(Direction.Left);
            }
            else if (Input.GetKeyDown("right"))
            {
                lastDir = "horizontal";
                SetFacing(Direction.Right);
            }
            if (Input.GetKeyUp("up") || Input.GetKeyUp("down") || Input.GetKeyUp("right") || Input.GetKeyUp("left"))
            {
                SetFacing();
            }
        }
    }
    void FixedUpdate()
    {
        if (plrActive)
        {
            ProcessInputs();
            Animate();
        }
    }

    void ProcessInputs()
    {
        if (plrActive && !colliding)
        {
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (movement.x == 0f && movement.y != 0f)
            {
                lastDir = "vertical";
            }
            else if (movement.x != 0f && movement.y == 0f)
            {
                lastDir = "horizontal";
            }

            // Pay attention to the direction of most recently pressed key
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
        else
        {
            movement = Vector2.zero;
            movementSpeed = 0f;
        }
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
