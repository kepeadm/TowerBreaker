using UnityEngine;
using TMPro;

public class RemainUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI remainText;

    void Awake()
    {
        if (remainText == null)
            remainText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void OnEnable()  => EventBus.Subscribe<OnEnemyCountChanged>(OnCountChanged);
    void OnDisable() => EventBus.Unsubscribe<OnEnemyCountChanged>(OnCountChanged);

    private void OnCountChanged(OnEnemyCountChanged e)
    {
        remainText.text = $"남은 적: {e.remaining}";
    }
}