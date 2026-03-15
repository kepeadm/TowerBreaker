using UnityEngine;
using System.Collections;

public class Slime : EnemyBase
{
    [Header("슬라임 공격 설정")]
    [SerializeField] private float attackDetectRange = 1.5f; // 넉넉하게
    [SerializeField] private string playerTag = "Player";

    private PlayerHealth _playerHealth;

    protected override void Awake()
    {
        base.Awake();
        maxHp       = 10f;
        damage      = 5f;
        walkSpeed   = 1.5f;
        attackSpeed = 1.2f;
        dropExp     = 1;
        dropGold    = 1;

        // PlayerHealth 캐시
        var player = GameObject.FindWithTag(playerTag);
        if (player != null)
            _playerHealth = player.GetComponent<PlayerHealth>();
    }

    protected override void PerformAttack()
    {
        if (_playerHealth == null) return;

        float dist = Vector2.Distance(
            transform.position,
            _playerHealth.transform.position);

        Debug.Log($"[Slime] 플레이어 거리: {dist}");

        if (dist <= attackDetectRange)
        {
            _playerHealth.TakeDamage(1);
            Debug.Log("[Slime] 플레이어 공격!");
        }
    }
}