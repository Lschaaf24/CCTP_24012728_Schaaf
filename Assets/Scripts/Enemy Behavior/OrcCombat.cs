using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcCombat : EnemyCombat
{
    public Transform attackPoint;
    public LayerMask playerLayers;
    private EnemyHealthBar enemyHealthBar;
    public GameObject parryParticle;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        enemyHealthBar = gameObject.GetComponentInChildren<EnemyHealthBar>();
        enemyHealthBar.setMaxHealth(stats.maxHealth);

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Transform player = playerObject.transform;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (CanAttack(distanceToPlayer)){
            TriggerAttack();
            animator.SetTrigger("Attack");
        }

    }
    void PerformAttack()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, stats.attackRange, playerLayers);

        foreach (Collider2D hit in hitPlayer)
        {
            PlayerCombat playerCombat = hit.GetComponent<PlayerCombat>();

            if (playerCombat.isParryActive())
            {
                animator.SetTrigger("Stun");
                playerCombat.parryRoutine();
                Instantiate(parryParticle, attackPoint.position, Quaternion.identity);
            }
            else
            {
                Health playerHealth = hit.GetComponent<Health>();
                playerHealth.TakeDamage(stats.attackDamage, transform);

                GameObject healthBarObject = GameObject.FindGameObjectWithTag("HealthBar");
                HealthBar healthBar = healthBarObject.GetComponent<HealthBar>();
                healthBar.setHealth(playerHealth.currentHealth);
            }
            
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, stats.attackRange);
    }
}
