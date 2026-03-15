using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI goldText;

    void OnEnable()
    {
        EventBus.Subscribe<OnExpChanged>(OnExpChanged);
        EventBus.Subscribe<OnGoldChanged>(OnGoldChanged);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<OnExpChanged>(OnExpChanged);
        EventBus.Unsubscribe<OnGoldChanged>(OnGoldChanged);
    }

    private void OnExpChanged(OnExpChanged e)
    {
        expText.text = $"LV.{e.level} [{e.exp}/{e.maxExp}]";
    }

    private void OnGoldChanged(OnGoldChanged e)
    {
        goldText.text = $"{e.amount}";
    }
}