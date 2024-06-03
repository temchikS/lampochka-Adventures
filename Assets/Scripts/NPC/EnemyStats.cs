using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int damage = 1; // Урон, наносимый врагом
    private int currentHealth;

    private CapsuleCollider2D enemyCollider;
    private float damageCooldown = 1f; // Время между нанесением урона
    private float nextDamageTime;

    private void Start()
    {
        currentHealth = maxHealth;
        enemyCollider = GetComponent<CapsuleCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            Player.instance.TakeDamage(damage);
            nextDamageTime = Time.time + damageCooldown;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        CheckDeath();
    }

    private void CheckDeath()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
