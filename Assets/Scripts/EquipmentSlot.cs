using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private PlayerData playerData;

    private float _lastClickTime;
    private const float DoubleClickInterval = 0.3f;

    void OnEnable()
    {
        EventBus.Subscribe<OnWeaponChanged>(OnWeaponChanged);
        RefreshIcon(playerData?.EquippedWeapon);
    }

    void OnDisable() => EventBus.Unsubscribe<OnWeaponChanged>(OnWeaponChanged);

    private void OnWeaponChanged(OnWeaponChanged e) => RefreshIcon(e.weapon);

    private void RefreshIcon(ItemData weapon)
    {
        if (itemIcon == null) return;

        if (weapon == null || string.IsNullOrEmpty(weapon.spritePath))
        {
            itemIcon.sprite  = null;
            itemIcon.enabled = false;
            return;
        }

        itemIcon.sprite  = Resources.Load<Sprite>(weapon.spritePath);
        itemIcon.enabled = true;
        itemIcon.color   = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        float now = Time.unscaledTime;

        if (now - _lastClickTime < DoubleClickInterval)
        {
            playerData?.UnequipWeapon();
            _lastClickTime = 0;
        }
        else
        {
            _lastClickTime = now;
        }
    }
}