using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public float knockback = 5.0f;
    public float knockbackDelay = 0.5f;
    private float lastAttackTime = -Mathf.Infinity;
    public CharacterStats stats;
    private Health health;
    private Animator animator;
    private bool isDead = false;
    public bool isAttacking = false;

    public float parryWindow = 0.1f;
    public float parryCooldown = 1.0f;
    private bool canParry = true;
    private bool parryActive = false;
    
    private playerMovement playerMovement;
    private HealthBar healthBar;


    void Start()
    {
        GameObject healthBarObject = GameObject.FindGameObjectWithTag("HealthBar");
        healthBar = healthBarObject.GetComponent<HealthBar>();

        health = GetComponent<Health>();
        healthBar.setMaxHealth(stats.maxHealth);

        health.onDeath += onDeath;

        playerMovement = gameObject.GetComponent<playerMovement>();
        animator = gameObject.GetComponent<Animator>();
    }

    public void onDeath()
    {
        if (isDead) return;
        isDead = true;
        playerMovement.Die();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("k") && Time.time >= lastAttackTime + stats.attackCooldown)
        {
            Attack();
        }

        if (Input.GetKeyDown("j") && canParry)
        {
            animator.SetTrigger("Parry");
            StartCoroutine(parryRoutine()); 
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");
        isAttacking = true;

    }

    void PerformAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, stats.attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();

            enemyHealth.TakeDamage(stats.attackDamage, transform);

            EnemyHealthBar enemyHealthBar = enemy.GetComponentInChildren<EnemyHealthBar>();
            if (enemyHealthBar != null)
            {
                enemyHealthBar.setHealth(enemyHealth.currentHealth);
            }
        }
        isAttacking = false;
    }

    public IEnumerator parryRoutine()
    {
        canParry = false;
        parryActive = true;


        yield return new WaitForSeconds(parryWindow);

        parryActive = false;

        yield return new WaitForSeconds(parryCooldown);
        canParry = true;
    }

    public bool isParryActive()
    {
        return parryActive;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, stats.attackRange);
    }

}   

