using UnityEngine;

public class SkeletonArcher : EnemyBase
{
    [SerializeField] private float attackDetectRange = 5f;
    [SerializeField] private string playerTag = "Player";

    private PlayerHealth _playerHealth;

    protected override void Awake()
    {
        base.Awake();
        maxHp       = 50f;
        damage      = 12f;
        walkSpeed   = 1.8f;
        attackSpeed = 2f;
        dropExp     = 3;
        dropGold    = 5;

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
            Debug.Log("[SkeletonArcher] 원거리 공격!");
        }
    }
}