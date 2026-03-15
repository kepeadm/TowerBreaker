using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private Animator      animator;
    [SerializeField] private PlayerCombat  playerCombat;
    [SerializeField] private Rigidbody2D   rb;
    [SerializeField] private GameObject    deathPanel;

    [Header("하트 UI")]
    [SerializeField] private List<GameObject> hearts;

    [Header("넉백 설정")]
    [SerializeField] private float knockbackForce    = 8f;
    [SerializeField] private float knockbackDuration = 0.4f;

    private int  _currentHp;
    private bool _isDead;
    private bool _isKnockback;

    void Awake()
    {
        _currentHp = hearts.Count;
        animator   = animator ?? GetComponent<Animator>();
        rb         = rb       ?? GetComponent<Rigidbody2D>();

        // 연결 확인
        Debug.Log($"[PlayerHealth] 초기화 / HP: {_currentHp} / Hearts: {hearts.Count}");

        if (hearts.Count == 0)
            Debug.LogError("[PlayerHealth] hearts 리스트가 비어있습니다! Inspector에서 연결해주세요.");
        if (animator == null)
            Debug.LogError("[PlayerHealth] Animator가 없습니다!");
    }

    public void TakeDamage(int dmg = 1)
    {
        Debug.Log($"[PlayerHealth] TakeDamage 호출 / dmg:{dmg} / isDead:{_isDead} / isKnockback:{_isKnockback} / isDefending:{playerCombat?.IsDefending}");

        if (_isDead)    return;
        if (_isKnockback) return;
        if (playerCombat != null && playerCombat.IsDefending) return;

        for (int i = 0; i < dmg; i++)
        {
            if (_currentHp <= 0) break;
            _currentHp--;

            if (_currentHp < hearts.Count && hearts[_currentHp] != null)
            {
                Destroy(hearts[_currentHp]);
                hearts[_currentHp] = null;
            }
        }

        Debug.Log($"[PlayerHealth] 피격 후 HP: {_currentHp}");

        if (_currentHp <= 0)
            StartCoroutine(DieRoutine());
        else
            StartCoroutine(HurtRoutine());
    }

    private IEnumerator HurtRoutine()
    {
        _isKnockback = true;
        animator.Play("hurt", -1, 0f);

        // 왼쪽으로 넉백
        rb.linearVelocity = new Vector2(-knockbackForce, rb.linearVelocity.y);

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        _isKnockback      = false;

        animator.Play("idle", -1, 0f);
    }

    private IEnumerator DieRoutine()
    {
        _isDead            = true;
        rb.linearVelocity  = Vector2.zero;
        animator.Play("death", -1, 0f);

        yield return new WaitForSeconds(1f);

        if (deathPanel != null)
            deathPanel.SetActive(true);
    }
}