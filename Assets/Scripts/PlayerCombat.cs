using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCombat : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Animator animator;

    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private string wallTag = "Wall";

    [Header("전투 설정")]
    [SerializeField] private float attackSpeed = 0.5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private string enemyTag = "Enemy";

    [Header("투사체")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject staffPrefab;

    [Header("쿨타임 UI")]
    [SerializeField] private CooldownUI moveCooldownUI;
    [SerializeField] private CooldownUI defenseCooldownUI;

    [Header("공격 이펙트")]
    [SerializeField] private GameObject swordEF;
    [SerializeField] private GameObject bowEF;
    [SerializeField] private GameObject staffEF;

    [Header("방어 이펙트")]
    [SerializeField] private GameObject defEF;

    private Rigidbody2D _rb;
    private bool _isMoving;
    private bool _isDefending;
    private bool _isAutoAttacking;
    private float _moveCooldown;
    private float _defenseCooldown;

    private const float MoveCooldownTime    = 2f;
    private const float DefenseCooldownTime = 2f;
    private const float DefenseDuration     = 1f;

    public bool IsDefending => _isDefending;

    static readonly int HashMove   = Animator.StringToHash("isMoving");
    static readonly int HashDefend = Animator.StringToHash("isDefending");
    static readonly int HashAttack = Animator.StringToHash("Attack");

    void Awake()
    {
        _rb      = GetComponent<Rigidbody2D>();
        animator = animator ?? GetComponent<Animator>();
    }

    // ── 이동 ──────────────────────────────────────
    public void OnMoveButton()
    {
        if (_isMoving || Time.time < _moveCooldown) return;
        _moveCooldown = Time.time + MoveCooldownTime;
        moveCooldownUI?.StartCooldown();
        StartCoroutine(MoveRoutine());
    }

    private bool _hitWall = false;
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!_isMoving) return;
        //if (col.gameObject.CompareTag("Ground") || 
        //    col.gameObject.CompareTag("Floor")) return;

        foreach (ContactPoint2D contact in col.contacts)
        {
            if (contact.normal.x < -0.5f)
            {
                _hitWall = true;
                break;
            }
        }
    }

    private IEnumerator MoveRoutine()
    {
        _isMoving = true;
        _hitWall  = false;
        animator.SetBool(HashMove, true);

        while (!_hitWall)
        {
            _rb.linearVelocity = new Vector2(moveSpeed, _rb.linearVelocity.y);
            yield return null;
        }

        _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        animator.SetBool(HashMove, false);
        _isMoving = false;
        _hitWall  = false;
    }

    // ── 방어 ──────────────────────────────────────
    public void OnDefenseButton()
    {
        if (_isDefending || Time.time < _defenseCooldown) return;
        _defenseCooldown = Time.time + DefenseCooldownTime;
        defenseCooldownUI?.StartCooldown();
        StartCoroutine(DefenseRoutine());
    }

    private IEnumerator DefenseRoutine()
    {
        _isDefending       = true;
        _rb.linearVelocity = Vector2.zero;
        //animator.SetBool(HashDefend, true);

        // 방어 이펙트 ON
        if (defEF != null) defEF.SetActive(true);

        yield return new WaitForSeconds(DefenseDuration);

        _isDefending = false;
        //animator.SetBool(HashDefend, false);

        // 방어 이펙트 OFF
        if (defEF != null) defEF.SetActive(false);
    }

    // ── 공격 ──────────────────────────────────────
    public void OnAttackButtonDown()
    {
        StartCoroutine(SingleAttack());
        _isAutoAttacking = true;
        StartCoroutine(AutoAttackRoutine());
    }

    public void OnAttackButtonUp()
    {
        _isAutoAttacking = false;
    }

    private IEnumerator AutoAttackRoutine()
    {
        yield return new WaitForSeconds(attackSpeed);
        while (_isAutoAttacking)
        {
            StartCoroutine(SingleAttack());
            yield return new WaitForSeconds(attackSpeed);
        }
    }

    private IEnumerator SingleAttack()
    {
        string weaponName = playerData?.EquippedWeapon?.name?.ToLower() ?? "sword";

        switch (weaponName)
        {
            case "검": yield return StartCoroutine(AttackSword()); break;
            case "활":   yield return StartCoroutine(AttackBow());   break;
            case "스태프": yield return StartCoroutine(AttackStaff()); break;
            default:      yield return StartCoroutine(AttackSword()); break;
        }
    }
    private IEnumerator AttackSword()
    {
        animator.Play("attack1", -1, 0f);

        // 이펙트 ON
        if (swordEF != null) swordEF.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        // 이펙트 OFF
        if (swordEF != null) swordEF.SetActive(false);

        Vector2 dir      = new Vector2(transform.localScale.x, 0);
        Vector2 origin   = (Vector2)transform.position + dir * 0.5f;
        float   range    = 1.8f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range);
        Collider2D nearest     = null;
        float      nearestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag(enemyTag)) continue;
            if (hit.gameObject == gameObject) continue;
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < nearestDist) { nearestDist = dist; nearest = hit; }
        }

        if (nearest != null)
        {
            float atk     = playerData?.AttackPower ?? 10f;
            float crit    = playerData?.CritChance  ?? 0f;
            float critDmg = playerData?.CritDamage  ?? 1.5f;
            bool  isCrit  = Random.value < crit;
            float dmg     = isCrit ? atk * critDmg : atk;
            nearest.GetComponent<EnemyBase>()?.TakeDamage(dmg, isCrit);
            Debug.Log($"[Sword] {nearest.name} hit / dmg:{dmg} crit:{isCrit}");
        }
    }

    private IEnumerator AttackStaff()
    {
        animator.Play("attack2", -1, 0f);

        if (staffEF != null) staffEF.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        if (staffEF != null) staffEF.SetActive(false);

        Vector2 dir    = new Vector2(transform.localScale.x, 0);
        Vector2 origin = (Vector2)transform.position + dir * 0.5f;
        float   range  = 3f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range);
        float atk     = playerData?.AttackPower ?? 10f;
        float crit    = playerData?.CritChance  ?? 0f;
        float critDmg = playerData?.CritDamage  ?? 1.5f;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag(enemyTag)) continue;
            if (hit.gameObject == gameObject) continue;
            bool  isCrit = Random.value < crit;
            float dmg    = isCrit ? atk * critDmg : atk;
            hit.GetComponent<EnemyBase>()?.TakeDamage(dmg, isCrit);
            Debug.Log($"[Staff] {hit.name} hit / dmg:{dmg} crit:{isCrit}");
        }
    }

    private IEnumerator AttackBow()
    {
        animator.Play("attack2", -1, 0f);

        if (bowEF != null) bowEF.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        if (bowEF != null) bowEF.SetActive(false);

        Vector2 dir    = new Vector2(transform.localScale.x, 0);
        Vector2 origin = (Vector2)transform.position + dir * 0.5f;
        float   range  = 3f;

        Collider2D[] hits     = Physics2D.OverlapCircleAll(origin, range);
        Collider2D nearest     = null;
        float      nearestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag(enemyTag)) continue;
            if (hit.gameObject == gameObject) continue;
            Vector2 toEnemy = (Vector2)hit.transform.position - (Vector2)transform.position;
            if (Mathf.Sign(toEnemy.x) != Mathf.Sign(transform.localScale.x)) continue;
            float dist = toEnemy.magnitude;
            if (dist < nearestDist) { nearestDist = dist; nearest = hit; }
        }

        if (nearest != null)
        {
            float atk     = playerData?.AttackPower ?? 10f;
            float crit    = playerData?.CritChance  ?? 0f;
            float critDmg = playerData?.CritDamage  ?? 1.5f;
            bool  isCrit  = Random.value < crit;
            float dmg     = isCrit ? atk * critDmg : atk;
            nearest.GetComponent<EnemyBase>()?.TakeDamage(dmg, isCrit);
            Debug.Log($"[Bow] {nearest.name} hit / dmg:{dmg} crit:{isCrit}");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}