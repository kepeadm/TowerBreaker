using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image background;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image selectedOverlay;

    private ItemData _itemData;
    private float _lastClickTime;
    private const float DoubleClickInterval = 0.3f;
    public Action<InventorySlot> OnSlotClicked;
    public Action<InventorySlot> OnSlotDoubleClicked;

    public ItemData ItemData => _itemData;

    public void Setup(ItemData data, Action<InventorySlot> onClick, Action<InventorySlot> onDoubleClick)
    {
        _itemData         = data;
        OnSlotClicked     = onClick;
        OnSlotDoubleClicked = onDoubleClick;

        background.enabled = true;

        if (selectedOverlay != null)
            selectedOverlay.enabled = false;

        if (data == null || string.IsNullOrEmpty(data.spritePath))
        {
            itemIcon.sprite  = null;
            itemIcon.enabled = false;
            return;
        }

        itemIcon.sprite  = Resources.Load<Sprite>(data.spritePath);
        itemIcon.enabled = true;
        itemIcon.color   = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        float now = Time.unscaledTime;

        if (now - _lastClickTime < DoubleClickInterval)
        {
            OnSlotDoubleClicked?.Invoke(this);
            _lastClickTime = 0;
        }
        else
        {
            OnSlotClicked?.Invoke(this);
            _lastClickTime = now;
        }
    }

    // 선택 상태 표시
    public void SetSelected(bool selected)
    {
        if (selectedOverlay != null)
            selectedOverlay.enabled = selected;

        background.color = selected
            ? new Color(1f, 0.85f, 0.3f, 1f)
            : Color.white;
    }

    public void Clear()
    {
        _itemData        = null;
        itemIcon.sprite  = null;
        itemIcon.enabled = false;
        SetSelected(false);
    }
}