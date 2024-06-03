using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public static ActiveWeapon Instance { get; private set; }
    [SerializeField] private Sword sword;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        FollowMouseDirection();
    }
    public Sword GetActiveWeapon()
    {
        return sword;
    }
    private void FollowMouseDirection()
    {
        // Получаем позицию мыши в мировых координатах
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Вычисляем направление от игрока к мыши
        Vector3 direction = mousePosition - transform.position;

        // Разворачиваем оружие в зависимости от положения мыши относительно игрока
        if (direction.x < 0f)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
