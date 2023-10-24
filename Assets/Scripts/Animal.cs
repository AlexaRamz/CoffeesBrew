using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : Interactable
{
    Animator anim;
    EntityMovement movement;
    void Start()
    {
        anim = GetComponent<Animator>();
        movement = GetComponent<EntityMovement>();
        movement.SetState(EntityMovement.MoveState.Default);
    }
    public void SetFollow()
    {
        //movement.fleeFrom = col.transform;
        //movement.SetState(EntityMovement.MoveState.Flee);
        movement.follow = GameObject.FindWithTag("Player").transform;
        movement.SetState(EntityMovement.MoveState.Follow);
    }
    public void CancelFollow()
    {
        movement.SetState(EntityMovement.MoveState.Default);
        //movement.fleeFrom = null;
    }
    // fleefrom = get closest enemy in range

    bool canPeck = false;
    void Update()
    {
        if ((movement.moveState == EntityMovement.MoveState.Wander || movement.moveState == EntityMovement.MoveState.Patrol) && movement.currentSpeed == 0f)
        {
            if (canPeck && Random.Range(-1f, 1f) < 0.3f)
            {
                anim.ResetTrigger("Peck");
                anim.SetTrigger("Peck");                
            }
            canPeck = false;
        }
        else
        {
            canPeck = true;
        }
    }
    bool interacting;
    bool actionOff = true;
    EntityMovement.MoveState previousState;
    IEnumerator ActionDelay()
    {
        yield return new WaitForSeconds(1f);
        if (!interacting)
        {
            PetOff();
        }
        actionOff = true;
    }
    void PetOn()
    {
        movement.SetState(EntityMovement.MoveState.Idle);
        anim.SetBool("Pet", true);
        StartCoroutine(ActionDelay());
    }
    void PetOff()
    {
        movement.SetState(previousState);
        anim.SetBool("Pet", false);
    }
    public override void Interact()
    {
        if (!interacting && actionOff)
        {
            interacting = true;
            actionOff = false;
            previousState = movement.moveState;
            PetOn();
        }
    }
    public override void InteractOff() // Cares about input type
    {
        interacting = false;
        if (actionOff)
        {
            PetOff();
        }
    }
}
