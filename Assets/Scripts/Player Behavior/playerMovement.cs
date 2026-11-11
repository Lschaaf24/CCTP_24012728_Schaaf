using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    public float moveSpeed = 3f; //Movement speed of player
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public Animator animator;
    public bool spriteFlip = false;
    public bool knockedBack = false;
    private bool Dead = false;

    public bool isDodging = false;
    private bool canDodge = true;
    public float dodgeSpeed = 10f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 1f;

    private PlayerCombat playerCombat;
    private Health health;
    public ParticleSystem runParticle;

    private Vector2 movementInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        playerCombat = gameObject.GetComponent<PlayerCombat>();
        health = gameObject.GetComponent<Health>();

    }
    // Update is called once per frame
    void Update()
    {
        if (health.isKnockedBack) return;
        if(Dead) return;
        if (playerCombat.isAttacking) return;
        if (!isDodging){

            rb.linearVelocity = new Vector2(movementInput.x * moveSpeed, movementInput.y * moveSpeed);
        }

        if (rb.linearVelocity != Vector2.zero)
        {
            runParticle.Play();
        }
       
        //if (Input.GetKeyDown(KeyCode.LeftShift) && canDodge)
        //{
        //    StartCoroutine(Dodge());
        //}

        runningAnimations();

    }

    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    public void onDodge(InputValue value)
    {
        if (value.isPressed && canDodge)
        {
            StartCoroutine(Dodge());
        }
    }

    public void Die(){
        Debug.Log("Player has died");
        rb.linearVelocity = Vector2.zero;
        Dead = true;
    }


    private void runningAnimations(){
        var particleDirection = runParticle.velocityOverLifetime;

        if (movementInput.x > 0)
        {
            spriteRenderer.flipX = false;
            //spriteFlip = false;
            particleDirection.x = -0.1f;
            runParticle.transform.localPosition = new Vector3(-0.25f, runParticle.transform.localPosition.y, 0);

        }
        else if (movementInput.x < 0)
        {
            spriteRenderer.flipX = true;
            //spriteFlip = true;
            particleDirection.x = 0.1f;
            runParticle.transform.localPosition = new Vector3(0.55f, runParticle.transform.localPosition.y, 0);

        }

        if (movementInput.x != 0 || movementInput.y != 0)
        {
            animator.SetBool("Running", true);
        }
        else
        {
            animator.SetBool("Running", false);
            
        }

    }

     private IEnumerator Dodge()
    {
        canDodge = false; 
        isDodging = true;

        if (animator != null)
        {
            animator.SetTrigger("Dodge");
            runParticle.Play();

        }

        Vector2 dodgeDirection = movementInput.normalized;

        float elapsedTime = 0f;
        while (elapsedTime < dodgeDuration)
        {
            rb.linearVelocity = dodgeDirection * dodgeSpeed;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isDodging = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }
}
