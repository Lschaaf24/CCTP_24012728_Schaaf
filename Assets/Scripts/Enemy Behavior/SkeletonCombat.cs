using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonCombat : EnemyCombat
{
    private EnemyHealthBar enemyHealthBar;
    private Animator animator;
    public GameObject projectilePrefab;
    public Transform firePoint;

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

        if (CanAttack(distanceToPlayer))
        {
            TriggerAttack();
            animator.SetTrigger("Fire");
        }
    }

    void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 direction = (GameObject.FindGameObjectWithTag("Player").transform.position - firePoint.position).normalized; 

        projectile.GetComponent<Projectile>().initializeProjectile(direction, stats.attackDamage);
    }
}
