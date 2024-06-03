using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackDuration = 0.1f;  // Длительность включения коллайдера

    public event EventHandler OnSwordSwing;

    private PolygonCollider2D _polygonCollider2D;

    private void Awake()
    {
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        AttackColliderTurnOff();
    }

    public void Attack()
    {
        StartCoroutine(PerformAttack());

        OnSwordSwing?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator PerformAttack()
    {
        AttackColliderTurnOn();
        yield return new WaitForSeconds(attackDuration);
        AttackColliderTurnOff();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out EnemyStats enemyEntity))
        {
            enemyEntity.TakeDamage(damage);
        }
    }

    public void AttackColliderTurnOff()
    {
        _polygonCollider2D.enabled = false;
    }

    private void AttackColliderTurnOn()
    {
        _polygonCollider2D.enabled = true;
    }
}
