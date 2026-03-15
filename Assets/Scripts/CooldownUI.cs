using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;   // 버튼 위 쿨타임 오버레이
    [SerializeField] private float cooldown = 2f;

    private float _endTime;

    public void StartCooldown()
    {
        _endTime = Time.time + cooldown;
    }

    void Update()
    {
        if (Time.time < _endTime)
        {
            fillImage.fillAmount = (_endTime - Time.time) / cooldown;
            fillImage.enabled    = true;
        }
        else
        {
            fillImage.fillAmount = 0f;
            fillImage.enabled    = false;
        }
    }
}