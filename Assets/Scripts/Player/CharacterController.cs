using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class CharacterController : MonoBehaviour
{
    //General Properties
    public Direction direction = Direction.E;
    Rigidbody2D rb;
    public LayerMask floorLayer; //Use this to set which layers will be checked for colliders to see whether the character is grounded.

    //Movement & Jumping
    Vector2 moveVector, currentForceOfGravity;
    bool isGrounded, jumpIsQueued;
    float coyoteTime;
    public Transform leftFoot, rightFoot; //Create two empties and position them at the bottom corners of the character for the groundcheck.
    public float moveSpeed = 3000f, jumpPower = 5000f, maxCoyoteTime = .15f, customGravity = 1000f;

    //Graphics related
    public SpriteRenderer sr;
    public Animator animator;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0; //We use custom gravity to ensure the smoothest terrain traverlsal possible.
    }
    void Update()
    {
        coyoteTime += Time.deltaTime;
        ReadControls();
        UpdateSpriteAndAnimations();
    }

    void FixedUpdate()
    {
        isGrounded = GroundCheck();
        CalcGravity();

        if (jumpIsQueued)
        {
            rb.AddForce(Vector2.up * jumpPower * Time.fixedDeltaTime, ForceMode2D.Impulse);
            jumpIsQueued = false;
        }
        rb.AddForce((moveVector + currentForceOfGravity) * Time.fixedDeltaTime);
    }

    void ReadControls()
    {
        moveVector = Vector2.zero;

        // Movement
        if (Keyboard.current.leftArrowKey.isPressed) 
        {
            moveVector.x = -moveSpeed;
            if (Keyboard.current.upArrowKey.isPressed) direction = Direction.NW;
            else if (Keyboard.current.downArrowKey.isPressed) direction = Direction.SW;
            else direction = Direction.W;
        }
        else if (Keyboard.current.rightArrowKey.isPressed)
        {
            moveVector.x = moveSpeed;
            if (Keyboard.current.upArrowKey.isPressed) direction = Direction.NE;
            else if (Keyboard.current.downArrowKey.isPressed) direction = Direction.SE;
            else direction = Direction.E;
        }
        if (moveVector.x != 0 && Keyboard.current.upArrowKey.isPressed) direction = Direction.N;
        else if (moveVector.x != 0 && Keyboard.current.downArrowKey.isPressed) direction = Direction.S;

        // A-Button
        // This is a bool so the input can not be read multiple times before the next physics frame (even if this is very unlikely to happen).
        if (Keyboard.current.xKey.wasPressedThisFrame && isGrounded && !jumpIsQueued) jumpIsQueued = true; 

        // B-Button
        if (Keyboard.current.cKey.wasPressedThisFrame) Hook(); //This should live in its own script.
    }

    private void Hook()
    {
        throw new NotImplementedException();
    }

    void UpdateSpriteAndAnimations()
    {
        if(direction == Direction.E || direction == Direction.NE || direction == Direction.SE) sr.flipX = false;
        else if(direction == Direction.W || direction == Direction.NW || direction == Direction.SW) sr.flipX = true;
        //Plug in animation setfloats and setbools here!
    }

    private bool GroundCheck()
    {
        if (Physics2D.Linecast(transform.position, leftFoot.position, floorLayer)
            || Physics2D.Linecast(transform.position, rightFoot.position, floorLayer))
        {
            coyoteTime = 0f;
            return true;
        }
        else if (coyoteTime <= maxCoyoteTime) return true;
        else return false;
    }

    private void CalcGravity()
    {
        if (!isGrounded) currentForceOfGravity.y -= customGravity * Time.fixedDeltaTime;
        else currentForceOfGravity = Vector2.zero;
    }
}
