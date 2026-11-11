using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2.0f;
    public Animator animator;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    public float detectionRadius = 5.0f;
    public bool dead = false;
    public float nextWaypointDistance = 1f;
    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    Seeker seeker;
    Rigidbody2D rb;
    public EnemyCombat enemyCombat;
    private Health health;

    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform;

        InvokeRepeating("UpdatePath", 0f, 0.6f);

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemyCombat = GetComponent<EnemyCombat>();
        health = GetComponent<Health>();
    }

    void UpdatePath(){
        if(seeker.IsDone())
        seeker.StartPath(rb.position, player.position, OnPathComplete);
    }

    void OnPathComplete(Path p){
        if(!p.error){
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        if (path == null || dead || health.isKnockedBack || enemyCombat.isStunned) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
            
        }
        else
        {
            reachedEndOfPath = false;
        }


        if (player != null)
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized ;
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer > enemyCombat.stats.attackRange && distanceToPlayer < detectionRadius)
            {
                rb.linearVelocity = direction * speed;
                animator.SetBool("Running", true);

                if (direction.x > 0f)
                {
                    spriteRenderer.flipX = false;
                }
                else if (direction.x < 0f)
                {
                    spriteRenderer.flipX = true;
                }
                
                float distanceAlongPath = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            
                if (distanceAlongPath < nextWaypointDistance)
                {
                    currentWaypoint++;
                }
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                animator.SetBool("Running", false);
            }
        }
    }
        public void Die(){
        Debug.Log("Enemy died!");
        dead = true;
    }

}
