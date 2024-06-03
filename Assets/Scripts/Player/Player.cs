using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; }
    public int maxHealth = 20;
    public int health;
    [SerializeField] private float speed = 2f;
    private Vector2 movement;
    private Rigidbody2D rb;
    public GameObject weapon;
    private Animator animator;
    public GameObject healthPanel;
    private HealthUI healthUI;
    private bool isDead = false;

    private void Awake()
    {
        instance = this;
        animator = weapon.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        healthUI = healthPanel.GetComponent<HealthUI>();
        health = maxHealth;
        healthUI.UpdateHealth(health); // Инициализация UI при запуске
    }

    void Update()
    {
        if (isDead) return; // Проверяем, мертв ли игрок

        float movementY = Input.GetAxisRaw("Horizontal");
        float movementX = Input.GetAxisRaw("Vertical");
        movement = new Vector2(movementY, movementX);

        if (movement.sqrMagnitude > 1)
        {
            movement.Normalize();
        }
        if (Input.GetMouseButtonDown(0))
        {
            ActiveWeapon.Instance.GetActiveWeapon().Attack();
        }
    }

    void FixedUpdate()
    {
        if (isDead) return; // Проверяем, мертв ли игрок

        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    public void RevivePlayer()
    {
        health = maxHealth;
        isDead = false; // Игрок снова жив
    }

    public Vector2 Movement
    {
        get { return movement; }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthUI.UpdateHealth(health); // Обновление UI при получении урона
        if (health <= 0)
        {
            isDead = true; // Игрок мертв
            GameManager.instance.DeathPanel();
        }
        else
        {
            Debug.Log("Player took damage, remaining health: " + health);
        }
    }

    public int GetPlayerHealth()
    {
        return health;
    }
}
