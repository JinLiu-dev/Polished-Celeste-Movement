using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovementPolish : MonoBehaviour
{
    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    private AnimationScript anim;
    private GameObject visual;

    [Space]
    [Header("Stats")]
    public float speed = 7;
    public float jumpForce = 12;
    public float slideSpeed = 1;
    public float wallJumpLerp = 5;
    public float dashSpeed = 40;

    public float hangTime = 0;
    // public float coyoteTime = 0.075f;
    public float suqashFactor = 45f;
    public float jumpBufferTime = 0.045f;
    public float bufferedTime = -1f;

    public float DMcoyoteTime = 0;
    public float DMcoyoteWall = 0;
    private float dashY = 0;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;

    public bool goingUp;

    [Space]

    private bool groundTouch;
    private bool hasDashed;

    public int side = 1;

    [Space]
    [Header("Polish")]
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationScript>();
        visual = GameObject.Find ("Player/Visual");
    }

    // Update is called once per frame
    void Update()
    {
        //Squash and Stretch
        if(rb.velocity.y != 0)
        {
            visual.transform.localScale = new Vector3(1f, 1f - rb.velocity.y / suqashFactor, 1f);
        }
        else
        {
            visual.transform.localScale = new Vector3(1f, 1f , 1f);
        }
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        if(coll.onWall && !coll.onGround && goingUp)
        {
            if (x > 0 && coll.onRightWall)
            {
                x = 0;
            }
            if(x < 0 && !coll.onRightWall)
            {
                x = 0;
            }
        }

        if(goingUp)
        {
            DMcoyoteTime = 0;
            DMcoyoteWall = 0;
        }

        Vector2 dir = new Vector2(x, y);
        Vector2 rawdir = new Vector2(xRaw, yRaw);

        Walk(dir, rawdir);
        anim.SetHorizontalMovement(x, y, rb.velocity.y);
        if(!coll.onGround && !coll.onWall)
        {
            hangTime += Time.deltaTime;
        }
        else
        {
            if(((hangTime - bufferedTime) < jumpBufferTime) && coll.onGround)
            {
                anim.SetTrigger("jump");
                Jump(Vector2.up, false);
            }
            bufferedTime = -1f;
            hangTime = 0;
        }

        // if colliding w wall and middle mouse is pressed
        // enter wallgrab start
        if (coll.onWall && Input.GetButton("Fire3") && canMove)
        {
            if(side != coll.wallSide)
            {
                anim.Flip(side*-1);
            }
            wallGrab = true;
            wallSlide = false;
        }

        // if you release middle mouse, not on the wall or can't move
        // set wall variable to false
        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        // if on the ground and not dashing allow jumping
        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<FloatyJumping>().enabled = false;
            GetComponent<BetterJumping>().enabled = true;
        }
        if (coll.onWall && isDashing && dashY == 0)
        {
            rb.velocity += new Vector2(0, dashSpeed / 2);
        }

        // if wall grab and not dashing
        // modify movement to only up and down
        // else set grav to 3
        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if(x > .2f || x < -.2f)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }

            float speedModifier = y > 0 ? .5f : 1;

            rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
        }
        else
        {
            rb.gravityScale = 3;
        }

        // if on a wall and not on the ground
        // if not grabbing
        // then wallsliding
        if(coll.onWall && !coll.onGround && !goingUp)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        // if not colliding w wall or on ground
        // wall slide is false
        if (!coll.onWall || coll.onGround)
        {
            wallSlide = false;
        }

        // if on ground add coyote time
        if(coll.onGround) {
            DMcoyoteTime = 30f;
        }

        if(coll.onWall) {
            DMcoyoteWall = 10f;
        }

        // if no more coyete is available, not on the ground, not goingup/jumping, and coyoteTime is 0
        if(!coll.onGround && DMcoyoteTime > 0f) {
            DMcoyoteTime -= 1f;
        }

        if(!coll.onWall && DMcoyoteWall > 0f) {
            DMcoyoteWall -= 1f;
        }

        // if trigger to jump is pressed jump
        // if on ground jump
        // if on wall walljump
        // if (Input.GetButtonDown("Jump"))
        // {
        //     anim.SetTrigger("jump");
            // if(rb.velocity.y < 0)
            // {
            //     bufferedTime = hangTime;
            // }

        //     if (coll.onGround  || (hangTime < coyoteTime && hangTime > 0)){
        //         StopCoroutine(DisableWallSlide(0));
        //         StartCoroutine(DisableWallSlide(.4f));
        //         Jump(Vector2.up, false);
        //         hangTime += coyoteTime;
        //     }

        //     if (coll.onWall && !coll.onGround)
        //         WallJump();
        // }

        if (Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("jump");
            if(rb.velocity.y < 0)
            {
                bufferedTime = hangTime;
            }
            if ((coll.onGround || DMcoyoteTime > 0f)&& !goingUp)
            {
                //Debug.Log("jumping");
                StopCoroutine(DisableWallSlide(0));
                StartCoroutine(DisableWallSlide(.3f));
                Jump(Vector2.up, false);
                DMcoyoteTime = 0f;
            }
            if ((coll.onWall || DMcoyoteWall > 0f) && !coll.onGround)
            {
                StopCoroutine(DisableWallSlide(0));
                StartCoroutine(DisableWallSlide(.3f));
                WallJump();
            }
        }

        // if trying to dash and not dashed yet, dash
        if (Input.GetButtonDown("Fire1") && !hasDashed)
        {
            if(xRaw != 0 || yRaw != 0)
            {
                dashY = yRaw;
                Dash(xRaw, yRaw);
            }
        }

        // if touching the ground and the bool is wrong set bool
        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        // if not on ground and bool is wrong set bool
        if(!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        WallParticle(y);

        // if in a locked direction animation stop here
        if (wallGrab || wallSlide || !canMove)
            return;

        // else make sure direction animation is correct
        if(x > 0)
        {
            side = 1;
            anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            anim.Flip(side);
        }


    }

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;

        side = anim.sr.flipX ? -1 : 1;

        jumpParticle.Play();
    }

    private void Dash(float x, float y)
    {
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

        hasDashed = true;

        anim.SetTrigger("dash");

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        FindObjectOfType<GhostTrail>().ShowGhost();
        StartCoroutine(GroundDash());
        DOVirtual.Float(14, 0, .8f, RigidbodyDrag);

        dashParticle.Play();
        rb.gravityScale = 0;
        GetComponent<BetterJumping>().enabled = false;
        wallJumped = true;
        isDashing = true;

        yield return new WaitForSeconds(.3f);

        dashParticle.Stop();
        rb.gravityScale = 3;
        GetComponent<BetterJumping>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }

    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        wallJumped = true;
    }

    private void WallSlide()
    {
        if(coll.wallSide != side)
        {
            anim.Flip(side * -1);
        }

        if (!canMove)
        {
            return;
        }

        bool pushingWall = false;
        if((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    private void Walk(Vector2 dir, Vector2 rawdir)
    {
        if (!canMove)
        {
            return;
        }

        if (wallGrab)
        {
            return;
        }

        if (!wallJumped)
        {
            if(rb.velocity.x > 0 && rawdir.x < 0
              || rb.velocity.x < 0 && rawdir.x > 0 ){
              rb.velocity = new Vector2(0, rb.velocity.y);
            }
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);

        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir, bool wall)
    {
        slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;

        particle.Play();
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    IEnumerator DisableWallSlide(float time)
    {
        // Debug.Log("GoingUp");
        goingUp = true;
        yield return new WaitForSeconds(time);
        goingUp = false;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    void WallParticle(float vertical)
    {
        var main = slideParticle.main;

        if (wallSlide || (wallGrab && vertical < 0))
        {
            slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }

    int ParticleSide()
    {
        int particleSide = coll.onRightWall ? 1 : -1;
        return particleSide;
    }
}
