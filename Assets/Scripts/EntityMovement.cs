using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    //Rigidbody2D rb;
    public float moveSpeed = 2f;
    public float fastMoveSpeed = 4f;
    public Transform patrolPoint;
    public float patrolRange = 4f;
    IEnumerator currentCoroutine;

    public enum MoveState
    {
        Default, // Decide between wander or patrol
        Wander,
        Patrol,
        Flee,
        Alarmed,
        Follow,
        Idle
    }
    public MoveState moveState;

    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    public void SetState(MoveState newState)
    {
        hasStarted = destinationSet = false;
        alarmTimeLeft = alarmTime;
        currentSpeed = 0f;
        SetMovementAnim(dir, 0f);
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        if (newState == MoveState.Default)
        {
            if (patrolPoint != null)
            {
                moveState = MoveState.Patrol;
                return;
            }
            else
            {
                moveState = MoveState.Wander;
                return;
            }
        }
        moveState = newState;
    }
    public void SetState(string newState)
    {
        SetState((MoveState)System.Enum.Parse(typeof(MoveState), newState));
    }

    Vector2 destination;
    bool destinationSet;
    public float currentSpeed;

    Vector2 RandomPoint(Vector3 origin, float minDistance, float maxDistance)
    {
        Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        return (Vector2)origin + randomDir * Random.Range(minDistance, maxDistance);
    }
    bool hasStarted;
    IEnumerator Wander_C(float minWait, float maxWait)
    {
        float waitTime = Random.Range(minWait, maxWait);
        yield return new WaitForSeconds(waitTime);
        hasStarted = false;
        destinationSet = true;
    }

    // Flee
    public Transform fleeFrom;

    // Alarmed
    public float alarmTime = 3f;
    float alarmTimeLeft;

    //Follow
    public Transform follow;
    public float spaceAway = 1.5f;

    void FixedUpdate()
    {
        if (moveState == MoveState.Wander)
        {
            if (!hasStarted && !destinationSet)
            {
                hasStarted = true;
                destination = RandomPoint(transform.position, 1f, 8f);
                currentSpeed = moveSpeed;
                
                currentCoroutine = Wander_C(2f, 8f);
                StartCoroutine(currentCoroutine);
            }
        }
        else if (moveState == MoveState.Patrol)
        {
            if (patrolPoint != null)
            {
                if (!hasStarted && !destinationSet)
                {
                    hasStarted = true;
                    destination = RandomPoint(patrolPoint.position, 0f, patrolRange);
                    currentSpeed = moveSpeed;

                    currentCoroutine = Wander_C(2f, 8f);
                    StartCoroutine(currentCoroutine);
                }
            }
        }
        else if (moveState == MoveState.Flee)
        {
            if (fleeFrom != null)
            {
                destination = fleeFrom.position;
                currentSpeed = -fastMoveSpeed;
                destinationSet = true;
                //rb.MovePosition(rb.position + -plrDir * fastMoveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                SetState(MoveState.Alarmed);
            }
        }
        else if (moveState == MoveState.Alarmed)
        {
            if (!destinationSet)
            {
                destination = RandomPoint(transform.position, 1f, 4f);
                currentSpeed = fastMoveSpeed;
                destinationSet = true;
            }
            alarmTimeLeft -= Time.deltaTime;
            if (alarmTimeLeft <= 0f)
            {
                SetState(MoveState.Default);
            }
        }
        else if (moveState == MoveState.Follow)
        {
            if (follow != null)
            {
                if (Vector3.Distance(transform.position, follow.position) > spaceAway)
                {
                    destination = follow.position;
                    currentSpeed = moveSpeed;
                    destinationSet = true;
                }
                else
                {
                    destinationSet = false;
                    currentSpeed = 0f;
                    SetMovementAnim(dir, 0f);
                }
            }
            else
            {
                SetState(MoveState.Default);
            }
        }

        if (destinationSet)
        {
            MoveTowards(destination, currentSpeed);
        }
    }
    
    Vector2 dir;
    void MoveTowards(Vector3 moveTo, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, moveTo, speed * Time.fixedDeltaTime);
        if (Vector3.Distance(transform.position, destination) < 0.001f)
        {
            destinationSet = false;
            currentSpeed = 0f;
            SetMovementAnim(dir, 0f);
        }
        else
        {
            dir = (Vector2)(moveTo - transform.position).normalized * Mathf.Sign(speed);
            SetMovementAnim(dir, Mathf.Abs(speed));
        }
    }
    Animator anim;
    void SetMovementAnim(Vector2 dir, float speed)
    {
        anim.SetFloat("Horizontal", dir.x);
        anim.SetFloat("Vertical", dir.y);
        anim.SetFloat("Speed", speed);
    }

}
