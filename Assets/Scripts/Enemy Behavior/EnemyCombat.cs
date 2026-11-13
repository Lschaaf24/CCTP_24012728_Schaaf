using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    private float lastAttackTime = -Mathf.Infinity;
    public CharacterStats stats;
    protected Health health;

    protected bool isDead = false;
    public bool isStunned = false;


    void Start()
    {
        health = GetComponent<Health>();
        health.onDeath += onDeath;
    }

    public void onDeath()
    {
        if (isDead) return;
        isDead = true;

        GetComponent<EnemyMovement>().MovementDeath();
    }

    protected bool CanAttack(float distanceToPlayer)
    {
        return distanceToPlayer <= stats.attackRange && Time.time >= lastAttackTime + stats.attackCooldown && !isDead && !isStunned;
    }

    protected void TriggerAttack(){

        lastAttackTime = Time.time; 
    }

}
