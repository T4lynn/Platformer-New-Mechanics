using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows.Speech;

public enum PlayerDirection
{
    left, right
}

public enum PlayerState
{
    idle, walking, jumping, dead, climbing
}


public class PlayerController : MonoBehaviour
{
   
    // a coroutine that runs when the player hit's 'E'. The coroutine increases the max speed to let the player 'sprint'. 
    public IEnumerator dashSpeed()
    {
        maxSpeed = 10f;
        yield return new WaitForSeconds(5);
        maxSpeed = 5f;
    }

  // launch pad variables. 
    public bool launchPad;
    public Vector2 bounceForce = new Vector2(0, 1000);

    [SerializeField] private Rigidbody2D body;
    private PlayerDirection currentDirection = PlayerDirection.right;
    public PlayerState currentState = PlayerState.idle;
    public PlayerState previousState = PlayerState.idle;

    [Header("Horizontal")]
    public float maxSpeed = 5f;
    public float accelerationTime = 0.25f;
    public float decelerationTime = 0.15f;

    [Header("Vertical")]
    public float apexHeight = 3f;
    public float apexTime = 0.5f;

    [Header("Ground Checking")]
    public float groundCheckOffset = 0.5f;
    public Vector2 groundCheckSize = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;

    private float accelerationRate;
    private float decelerationRate;

    private float gravity;
    private float initialJumpSpeed;

    private bool isGrounded = false;
    public bool isDead = false;

    private Vector2 velocity;

    //added variables involved with climbing. 
    [Header("Climbing Check")]
    public float climbOffset = 0.5f;
    public Vector2 climbSize = new(0.4f, 0.1f);
    public LayerMask climbMask;
    [SerializeField] private bool climbing;

    public void Start()
    {
        body.gravityScale = 0;

        accelerationRate = maxSpeed / accelerationTime;
        decelerationRate = maxSpeed / decelerationTime;

        gravity = -2 * apexHeight / (apexTime * apexTime);
        initialJumpSpeed = 2 * apexHeight / apexTime;
    }

    public void Update()
    {
        previousState = currentState;

        CheckForGround();
        checkClimbing(); // checks for whether the player is close enough to a wall to climb.
        if (climbing)
        {
           // Debug.Log("climbing now");
        }

            Vector2 playerInput = new Vector2();
        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.y = Input.GetAxisRaw("Vertical"); // added a verical input axis.

        if (isDead)
        {
            currentState = PlayerState.dead;
        }

        switch (currentState)
        {
            case PlayerState.dead:
                // do nothing - we ded.
                break;
            case PlayerState.idle:
                if (!isGrounded && !climbing) currentState = PlayerState.jumping;
                else if (velocity.x != 0) currentState = PlayerState.walking;
                else if (climbing) { currentState = PlayerState.climbing; }
                break;
            case PlayerState.walking:
                if (!isGrounded && !climbing) currentState = PlayerState.jumping;
                else if (velocity.x == 0 && !climbing) currentState = PlayerState.idle;
                else if (climbing) { currentState = PlayerState.climbing; }
                break;
            case PlayerState.jumping:
                if (isGrounded)
                {
                    if (velocity.x != 0) currentState = PlayerState.walking;
                    else currentState = PlayerState.idle;
                }
                if (climbing) { currentState = PlayerState.climbing; }
                break;

                //added a climbing state.
            case PlayerState.climbing:
                if (velocity.x != 0) { currentState = PlayerState.walking; }
                else if (!isGrounded && !climbing) { currentState = PlayerState.jumping; }
                else { currentState = PlayerState.idle; }
                break;
        }

        MovementUpdate(playerInput);
        JumpUpdate();

        if (!isGrounded && !climbing)
        { velocity.y += gravity * Time.deltaTime; }
        else if (launchPad) { return; } //if the player is on the launch pad, the code returns instead of setting the velocity. 
        else
        { velocity.y = 0; }

        body.velocity = velocity;

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(dashSpeed());
        }
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        //Debug.Log(playerInput.y + "is the player input");
        if (playerInput.x < 0)
            currentDirection = PlayerDirection.left;
        else if (playerInput.x > 0)
            currentDirection = PlayerDirection.right;

        if (playerInput.x != 0)
        {
            velocity.x += accelerationRate * playerInput.x * Time.deltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        }
        else
        {
            if (velocity.x > 0)
            {
                velocity.x -= decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Max(velocity.x, 0);
            }
            else if (velocity.x < 0)
            {
                velocity.x += decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Min(velocity.x, 0);
            }
        }
        ////added on new code.
        //If the player is close enough to a wall, the player can use their y input to move up and down the wall. 
        if (climbing == true)
        {
            
          // Debug.Log("climbing now");
            Vector2 Position = new Vector2(transform.position.x, transform.position.y);

                Position.y += playerInput.y * 1  *Time.deltaTime;
                transform.position = Position;  
        }
    }

    private void JumpUpdate()
    {
        if (isGrounded && Input.GetButton("Jump"))
        {
            velocity.y = initialJumpSpeed;
            isGrounded = false;
        }
    }

    private void CheckForGround()
    {
        isGrounded = Physics2D.OverlapBox(
            transform.position + Vector3.down * groundCheckOffset,
            groundCheckSize,
            0,
            groundCheckMask);
    }
    //Checks to see if the player is close enough to a wall to climb using an overlap box (nearly the same as the isgrounded
    //function. 
    public void checkClimbing()
    {
        climbing =  Physics2D.OverlapBox(
            transform.position + Vector3.right * climbOffset, climbSize, 0, climbMask); 
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.down * groundCheckOffset, groundCheckSize);
        Gizmos.DrawWireCube(transform.position + Vector3.right * climbOffset, climbSize);
    }

    public bool IsWalking()
    {
        return velocity.x != 0;
    }
    public bool IsGrounded()
    {
        return isGrounded;
    }

    //Checks to see if the player is on the bouncy pad. If the player is, it adds a force. 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("triggered");
        body.AddForce(bounceForce, ForceMode2D.Force);
    }

    public PlayerDirection GetFacingDirection()
    {
        return currentDirection;
    }

    

}
