using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcCombat : EnemyCombat
{
    [SerializeField]private Transform attackPoint;
    [SerializeField] private LayerMask playerLayers;
    private EnemyHealthBar enemyHealthBar;
    [SerializeField] private GameObject parryParticle;
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
    public void PerformAttackOrc()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, stats.attackRange, playerLayers);

        foreach (Collider2D hit in hitPlayer)
        {
            Debug.Log("Player hit");
            PlayerCombat playerCombat = hit.GetComponent<PlayerCombat>();

            if(playerCombat != null)
            {
                if (playerCombat.isParryActive())
                {
                    animator.SetTrigger("Stun");
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
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, stats.attackRange);
    }
}
