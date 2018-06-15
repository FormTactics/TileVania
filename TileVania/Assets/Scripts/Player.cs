using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class Player : MonoBehaviour {

    // Config
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(25f, 25f);

    // States
    bool isAlive = true;

    // Cached Component References
    Rigidbody2D Rigidbody2D;
    Animator Animator;
    CapsuleCollider2D BodyCollider2D;
    BoxCollider2D FeetCollider2D;
    


    float gravityScaleAtStart;


    // Messages, then Methods

    void Start () {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        BodyCollider2D = GetComponent<CapsuleCollider2D>();
        FeetCollider2D = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = Rigidbody2D.gravityScale;
        
	}
	
	
	void Update () {

        if (!isAlive)
        {
            return;
        }

        Run();
        FlipSprite();
        Jump();
        ClimbLadder();
        Die();
        //JumpFromLadder();
	}
	
	private void Run(){
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // -1 to +1
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, Rigidbody2D.velocity.y);
        Rigidbody2D.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(Rigidbody2D.velocity.x) > Mathf.Epsilon;
        Animator.SetBool("isRunning", playerHasHorizontalSpeed);
        
    }

    private void FlipSprite ()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(Rigidbody2D.velocity.x) > Mathf.Epsilon;

        // If the player is moving Horizontally
        if(playerHasHorizontalSpeed)
        {
            // Reverse the current scaling of the x-axis
            transform.localScale = new Vector2(Mathf.Sign(Rigidbody2D.velocity.x), 1f);
        }
    }

    private void Jump ()
    {
        if (!FeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

        if(CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            Rigidbody2D.velocity += jumpVelocityToAdd;
        }
    }

    private void ClimbLadder ()
    {
        bool playerHasVerticalInput = Mathf.Abs(Rigidbody2D.velocity.x) > Mathf.Epsilon;

        if (!FeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            Animator.SetBool("isClimbing", false);
            Rigidbody2D.gravityScale = gravityScaleAtStart;
            return;
        }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            Rigidbody2D.velocity += jumpVelocityToAdd;
            
        }
        else 
        {
            float upOrDown = CrossPlatformInputManager.GetAxis("Vertical");
            Vector2 climbing = new Vector2(Rigidbody2D.velocity.x * 0.3f, upOrDown * climbSpeed);
            Rigidbody2D.velocity = climbing;
            Animator.SetBool("isClimbing", true);
            Rigidbody2D.gravityScale = 0f;
            
        }

       
    }

    private void Die ()
    {
        if (BodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy","Hazards")))
        {
            isAlive = false;
            Animator.SetBool("isDead", true);
            Rigidbody2D.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
            

        }
    }

}
