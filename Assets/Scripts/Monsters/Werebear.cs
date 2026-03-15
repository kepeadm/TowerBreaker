using UnityEngine;

public class Werebear : EnemyBase
{
    [SerializeField] private float attackDetectRange = 2f;
    [SerializeField] private string playerTag = "Player";

    private PlayerHealth _playerHealth;

    protected override void Awake()
    {
        base.Awake();
        maxHp       = 200f;
        damage      = 30f;
        walkSpeed   = 2.5f;
        attackSpeed = 2f;
        dropExp     = 50;
        dropGold    = 50;

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
            Debug.Log("[Werebear] 기본 공격");
        }

        // TODO: Werebear 특수공격 구현 (광역 공격)
        // 예: 주변 범위 전체 타격 + 강한 넉백
    }
}