using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("스탯")]
    [SerializeField] protected float maxHp       = 100f;
    [SerializeField] protected float damage      = 10f;
    [SerializeField] protected float walkSpeed   = 2f;
    [SerializeField] protected float attackSpeed = 1.5f;

    [Header("드롭 아이템")]
    [SerializeField] protected List<DropEntry> drops = new List<DropEntry>();

    [Header("드롭 보상")]
    [SerializeField] protected int dropExp  = 10;
    [SerializeField] protected int dropGold = 5;

    [Header("공격 판정 타이밍")]
    [SerializeField] private float attackHitDelay = 0.3f;

    protected float       _currentHp;
    protected Rigidbody2D _rb;
    protected Animator    _anim;
    protected bool        _isDead;
    protected bool        _isAttacking; // 공격 중 여부
    protected bool        _isVisible;

    protected enum State { Attack, Walk }
    protected State _state = State.Attack;

    private static PlayerData _playerData;
    private Coroutine _aiCoroutine;

    protected virtual void Awake()
    {
        _rb        = GetComponent<Rigidbody2D>();
        _anim      = GetComponent<Animator>();
        _currentHp = maxHp;

        if (_playerData == null)
            _playerData = FindObjectOfType<PlayerData>();
    }

    protected virtual void Start()
    {
        _aiCoroutine = StartCoroutine(AIRoutine());
    }

    void OnBecameVisible()   => _isVisible = true;
    void OnBecameInvisible() => _isVisible = false;

    private IEnumerator AIRoutine()
    {
        while (!_isDead)
        {
            switch (_state)
            {
                case State.Attack:
                    yield return StartCoroutine(DoAttack());
                    _state = State.Walk;
                    break;
                case State.Walk:
                    yield return StartCoroutine(DoWalk());
                    _state = State.Attack;
                    break;
            }
        }
    }

    private IEnumerator DoWalk()
    {
        _anim.Play("walk", -1, 0f);
        _rb.linearVelocity = new Vector2(-walkSpeed, _rb.linearVelocity.y);
        yield return new WaitForSeconds(1.5f);
        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
    }

    private IEnumerator DoAttack()
    {
        if (!_isVisible)
        {
            yield return new WaitForSeconds(0.5f);
            yield break;
        }

        _isAttacking       = true;
        _rb.linearVelocity = Vector2.zero;
        _anim.Play("attack", -1, 0f);

        yield return new WaitForSeconds(attackHitDelay);

        PerformAttack();

        float remaining = attackSpeed - attackHitDelay;
        if (remaining > 0)
            yield return new WaitForSeconds(remaining);

        _isAttacking = false;
    }

    protected abstract void PerformAttack();

    // ── 피격 ─────────────────────────────────────
    public virtual void TakeDamage(float dmg, bool isCrit = false)
    {
        if (_isDead) return;

        _currentHp -= dmg;
        Debug.Log($"[{gameObject.name}] 피격 {dmg} / 남은 HP: {_currentHp}");

        if (_currentHp <= 0)
        {
            StartCoroutine(DieRoutine());
            return;
        }

        // 공격 중이면 피격 애니메이션 스킵
        if (!_isAttacking)
            StartCoroutine(HurtRoutine());
    }

    private IEnumerator HurtRoutine()
    {
        _anim.Play("hurt", -1, 0f);
        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator DieRoutine()
    {
        _isDead            = true;
        _isAttacking       = false;
        _rb.linearVelocity = Vector2.zero;

        // AI 코루틴 중단
        if (_aiCoroutine != null)
            StopCoroutine(_aiCoroutine);

        _anim.Play("death", -1, 0f);
        OnDrop();

        yield return new WaitForSeconds(1f);
        ObjectPool.Instance.Return(gameObject);
    }

    protected virtual void OnDrop()
    {
        if (_playerData == null) return;

        _playerData.AddExp(dropExp);
        _playerData.AddGold(dropGold);

        foreach (var entry in drops)
        {
            if (entry?.itemData == null) continue;
            if (Random.value <= entry.chance)
                _playerData.AddItem(entry.itemData);
        }
    }

    public virtual void ResetEnemy()
    {
        _isDead      = false;
        _isAttacking = false;
        _isVisible   = false;
        _currentHp   = maxHp;
        _state       = State.Attack;
        StopAllCoroutines();
        _aiCoroutine = StartCoroutine(AIRoutine());
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}