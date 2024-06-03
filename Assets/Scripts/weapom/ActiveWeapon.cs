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
        // �������� ������� ���� � ������� �����������
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // ��������� ����������� �� ������ � ����
        Vector3 direction = mousePosition - transform.position;

        // ������������� ������ � ����������� �� ��������� ���� ������������ ������
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
