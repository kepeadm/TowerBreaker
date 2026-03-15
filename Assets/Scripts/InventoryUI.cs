using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private RectTransform layout;
    [SerializeField] private InventorySlot slotPrefab;

    [Header("버튼")]
    [SerializeField] private Button enhanceBtn;
    [SerializeField] private Button dismantleBtn;

    [Header("그리드 설정")]
    [SerializeField] private int columns     = 3;
    [SerializeField] private float cellSizeY = 135f;
    [SerializeField] private float spacingY  = 7f;
    [SerializeField] private float paddingY  = 0f;

    [Header("연출")]
    [SerializeField] private float intervalPerSlot = 0.05f;

    private InventorySlot _selectedSlot = null;

    void OnEnable()
    {
        SetButtonsAlpha(0f);
        enhanceBtn.onClick.AddListener(OnEnhanceClicked);
        dismantleBtn.onClick.AddListener(OnDismantleClicked);
        EventBus.Subscribe<OnInventoryChanged>(OnInventoryChanged);
        StartCoroutine(LoadSlots());
    }

    void OnDisable()
    {
        enhanceBtn.onClick.RemoveListener(OnEnhanceClicked);
        dismantleBtn.onClick.RemoveListener(OnDismantleClicked);
        EventBus.Unsubscribe<OnInventoryChanged>(OnInventoryChanged);
        StopAllCoroutines();
        _selectedSlot = null;
    }

    private void OnInventoryChanged(OnInventoryChanged e)
    {
        StopAllCoroutines();
        _selectedSlot = null;
        SetButtonsAlpha(0f);
        StartCoroutine(LoadSlots());
    }

    // ── 슬롯 로드 ─────────────────────────────────
    private IEnumerator LoadSlots()
    {
        foreach (Transform child in layout)
            Destroy(child.gameObject);

        yield return null;

        var inventory = playerData.Inventory;

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == null)
                continue;

            var slot = Instantiate(slotPrefab, layout);
            slot.Setup(inventory[i], OnSlotClicked, OnSlotDoubleClicked);

            if (inventory[i] == playerData.EquippedWeapon)
                slot.SetSelected(true);

            RefreshGridHeight(i + 1);
            yield return new WaitForSeconds(intervalPerSlot);
        }
    }

    // ── 슬롯 클릭 ─────────────────────────────────
    private void OnSlotClicked(InventorySlot slot)
    {
        if (_selectedSlot == slot)
        {
            _selectedSlot.SetSelected(false);
            _selectedSlot = null;
            SetButtonsAlpha(0f);
            return;
        }
        if (_selectedSlot != null)
            _selectedSlot.SetSelected(false);

        _selectedSlot = slot;
        _selectedSlot.SetSelected(true);
        SetButtonsAlpha(1f);
    }

    // ── 슬롯 더블클릭 → 장착 ─────────────────────
    private void OnSlotDoubleClicked(InventorySlot slot)
    {
        if (slot.ItemData == null) return;

        bool equipped = playerData.EquipWeapon(slot.ItemData);
        if (!equipped)
            Debug.Log("[InventoryUI] 무기만 장착할 수 있습니다.");

    }

    // ── 강화 버튼 ─────────────────────────────────
    private void OnEnhanceClicked()
    {
        if (_selectedSlot?.ItemData == null) return;
        // TODO: 강화 로직 연결
        Debug.Log($"[InventoryUI] 강화: {_selectedSlot.ItemData.name}");
    }

    [Header("분해 팝업")]
    [SerializeField] private GoldPopup goldPopup;

    private void OnDismantleClicked()
    {
        if (_selectedSlot?.ItemData == null) return;

        int refund = playerData.DismantleItem(_selectedSlot.ItemData);
        Debug.Log($"[InventoryUI] 분해: +{refund} 골드");
        goldPopup?.Show(refund);

        _selectedSlot = null;
        SetButtonsAlpha(0f);
    }

    // ── 버튼 알파 제어 ────────────────────────────
    private void SetButtonsAlpha(float alpha)
    {
        SetButtonAlpha(enhanceBtn,   alpha);
        SetButtonAlpha(dismantleBtn, alpha);
        enhanceBtn.interactable  = alpha > 0f;
        dismantleBtn.interactable = alpha > 0f;
    }

    private void SetButtonAlpha(Button btn, float alpha)
    {
        var c = btn.image.color;
        c.a = alpha;
        btn.image.color = c;
        var tmp = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmp != null)
        {
            var tc = tmp.color;
            tc.a = alpha;
            tmp.color = tc;
        }
    }

    private void RefreshGridHeight(int itemCount)
    {
        int rows = Mathf.CeilToInt((float)itemCount / columns);
        float h  = rows * (cellSizeY + spacingY) - spacingY + paddingY;
        layout.sizeDelta = new Vector2(layout.sizeDelta.x, h);
    }
}