using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{

    private Animator anim;
    private Movement move;
    private MovementPolish move2;
    private MovementDistinct move3;
    private MoveSelector ms;
    private Collision coll;
    [HideInInspector]
    public SpriteRenderer sr;

    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponentInParent<Collision>();
        move = GetComponentInParent<Movement>();
        move2 = GetComponentInParent<MovementPolish>();
        move3 = GetComponentInParent<MovementDistinct>();
        sr = GetComponent<SpriteRenderer>();
        ms = GetComponentInParent<MoveSelector>();
    }

    void Update()
    {
        anim.SetBool("onGround", coll.onGround);
        anim.SetBool("onWall", coll.onWall);
        anim.SetBool("onRightWall", coll.onRightWall);
        if(ms.moveStyle == 1)
        {
            anim.SetBool("wallGrab", move.wallGrab);
            anim.SetBool("wallSlide", move.wallSlide);
            anim.SetBool("canMove", move.canMove);
            anim.SetBool("isDashing", move.isDashing);
        }
        else if(ms.moveStyle == 2)
        {
            anim.SetBool("wallGrab", move2.wallGrab);
            anim.SetBool("wallSlide", move2.wallSlide);
            anim.SetBool("canMove", move2.canMove);
            anim.SetBool("isDashing", move2.isDashing);
        }
        else
        {
            anim.SetBool("wallGrab", move3.wallGrab);
            anim.SetBool("wallSlide", move3.wallSlide);
            anim.SetBool("canMove", move3.canMove);
            anim.SetBool("isDashing", move3.isDashing);
        }

    }

    public void SetHorizontalMovement(float x,float y, float yVel)
    {
        anim.SetFloat("HorizontalAxis", x);
        anim.SetFloat("VerticalAxis", y);
        anim.SetFloat("VerticalVelocity", yVel);
    }

    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }

    public void Flip(int side)
    {

        if (move.wallGrab || move.wallSlide)
        {
            if (side == -1 && sr.flipX)
                return;

            if (side == 1 && !sr.flipX)
            {
                return;
            }
        }

        bool state = (side == 1) ? false : true;
        sr.flipX = state;
    }
}
