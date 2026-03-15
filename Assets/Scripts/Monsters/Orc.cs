using UnityEngine;

public class Orc : EnemyBase
{
    [SerializeField] private float attackDetectRange = 1.5f;
    [SerializeField] private string playerTag = "Player";

    private PlayerHealth _playerHealth;

    protected override void Awake()
    {
        base.Awake();
        maxHp       = 20f;
        damage      = 15f;
        walkSpeed   = 2f;
        attackSpeed = 1.5f;
        dropExp     = 1;
        dropGold    = 2;

        var player = GameObject.FindWithTag(playerTag);
        if (player != null)
            _playerHealth = player.GetComponent<PlayerHealth>();
    }

    protected override void PerformAttack()
    {
        if (_playerHealth == null) return;

        float dist = Vector2.Distance(transform.position, _playerHealth.transform.position);
        if (dist <= attackDetectRange)
        {
            _playerHealth.TakeDamage(1);
            Debug.Log("[Orc] 근접 공격!");
        }
    }
}
