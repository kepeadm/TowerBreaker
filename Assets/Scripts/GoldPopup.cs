using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GoldPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image goldIcon;
    [SerializeField] private float riseHeight = 60f;
    [SerializeField] private float duration   = 1.2f;

    private RectTransform _rect;
    private CanvasGroup _cg;
    private Vector2 _originPos;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _originPos = _rect.anchoredPosition;
        if (amountText == null)
            amountText = GetComponentInChildren<TextMeshProUGUI>();

        if (amountText == null)
            Debug.LogError("[GoldPopup] amountText가 연결되지 않았습니다.", gameObject);

        _cg = GetComponent<CanvasGroup>();
        if (_cg == null)
            _cg = gameObject.AddComponent<CanvasGroup>();

        gameObject.SetActive(false);
    }

    public void Show(int amount)
    {
        if (amountText == null) return;
        gameObject.SetActive(true);

        StopAllCoroutines();
        _rect.anchoredPosition = _originPos;
        amountText.text = $"+{amount}";
        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        float elapsed = 0f;
        _cg.alpha = 1f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            _rect.anchoredPosition = _originPos + Vector2.up * (riseHeight * t);
            _cg.alpha = 1f - t;
            elapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
        _rect.anchoredPosition = _originPos;
        _cg.alpha = 1f;
    }
}