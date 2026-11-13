using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    public float knockback = 5.0f;
    public float knockbackDelay = 0.5f;
    private float lastAttackTime = -Mathf.Infinity;
    public CharacterStats stats;
    private Health health;
    private Animator animator;
    private bool isDead = false;
    public bool isAttacking = false;

    [SerializeField]private float parryWindow = 0.1f;
    [SerializeField]private float parryCooldown = 1.0f;
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
        playerMovement.MovementDeath();
    }

    public void OnAttack(InputValue value)
    {
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");
        isAttacking = true;

    }

    public void PerformAttackPlayer()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, stats.attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            if(enemyHealth != null)
            {
                enemyHealth.TakeDamage(stats.attackDamage, transform);

                EnemyHealthBar enemyHealthBar = enemy.GetComponentInChildren<EnemyHealthBar>();
                if (enemyHealthBar != null)
                {
                    enemyHealthBar.setHealth(enemyHealth.currentHealth);
                }
            }
        }
        isAttacking = false;
    }

    public void OnParry(InputValue value)
    {
        if (canParry)
        {
            animator.SetTrigger("Parry");
            StartCoroutine(parryRoutine());
        }
    }

    public IEnumerator parryRoutine()
    {
        canParry = false;
        parryActive = true;

        Debug.Log("Parry active");
        yield return new WaitForSeconds(parryWindow);

        Debug.Log("Parry inactive");
        parryActive = false;

        yield return new WaitForSeconds(parryCooldown);
        canParry = true;
    }

    public bool isParryActive()
    {
        return parryActive;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, stats.attackRange);
    }

}   

