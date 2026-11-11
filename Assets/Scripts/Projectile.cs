using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float speed = 8f;
    private int damage;
    private Vector2 direction;
    private Quaternion rotation;
    private Rigidbody2D rb;
    private float angle;
    // Start is called before the first frame update

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    public void initializeProjectile(Vector2 dir, int dmg) 
    {
        direction = dir;
        damage = dmg;

        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


        Destroy(gameObject, 5f);
    }

    void Update()
    {
        rb.linearVelocity = direction * speed;

        rb.rotation = angle - 90f;
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            playerHealth.TakeDamage(damage, transform);
            GameObject healthBarObject = GameObject.FindGameObjectWithTag("HealthBar");
            HealthBar healthBar = healthBarObject.GetComponent<HealthBar>();
            healthBar.setHealth(playerHealth.currentHealth);
        
            Destroy(gameObject);
        }
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

    }
}
