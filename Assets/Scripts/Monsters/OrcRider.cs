using UnityEngine;

public class OrcRider : EnemyBase
{
    [SerializeField] private float attackDetectRange = 2f;
    [SerializeField] private string playerTag = "Player";

    private PlayerHealth _playerHealth;

    protected override void Awake()
    {
        base.Awake();
        maxHp       = 100f;
        damage      = 20f;
        walkSpeed   = 3f;
        attackSpeed = 1.2f;
        dropExp     = 10;
        dropGold    = 10;

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
            Debug.Log("[OrcRider] 기본 공격");
        }

        // TODO: OrcRider 특수공격 구현 (돌진 공격)
        // 예: 일정 주기로 빠르게 돌진 후 강타
    }
}