using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Stats/Character")]
public class CharacterStats : ScriptableObject
{
    public int maxHealth;
    public int attackDamage;
    public float attackRange;
    public float attackCooldown;
    public float knockback;
}
