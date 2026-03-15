using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerData : MonoBehaviour
{
    private const string SAVE_KEY = "PlayerSaveData";

    [Header("Player Data")]
    [SerializeField] private int gold = 0;
    [SerializeField] private int level = 1;
    [SerializeField] private int exp = 0;
    [SerializeField] private int maxFloor = 0;
    [SerializeField] private List<ItemData> inventory = new List<ItemData>();
    [SerializeField] private List<Item> newBieKit = new List<Item>();

    [Header("기본 스탯")]
    [SerializeField] private float baseAttackPower = 1f;
    [SerializeField] private float baseCritChance  = 0.1f;
    [SerializeField] private float baseCritDamage  = 1.5f;


    private float _equipBonusAtk = 0f;


    public void ApplyWeaponBonus(ItemData weapon)
    {
        _equipBonusAtk = weapon?.damage ?? 0f;
    }

    // 레벨별 필요 경험치
    private static readonly int[] ExpTable = { 0, 10, 25, 45, 70, 100, 140, 190, 250, 320, 400 };

    public int Gold                 => gold;
    public int Level                => level;
    public int Exp                  => exp;
    public int MaxExp               => level < ExpTable.Length ? ExpTable[level] : ExpTable[^1];
    public List<ItemData> Inventory => inventory;

    public float AttackPower => baseAttackPower + _equipBonusAtk;
    public float CritChance  => baseCritChance;
    public float CritDamage  => baseCritDamage;

    public void AddItem(ItemData item)
    {
        inventory.Add(item);
        CleanInventory();
        EventBus.Publish(new OnInventoryChanged());
    }
    public void RemoveItem(ItemData item)
    {
        inventory.Remove(item);
        CleanInventory();
        EventBus.Publish(new OnInventoryChanged());
    }

    // ── Gold ──────────────────────────────────────
    public void AddGold(int amount)
    {
        gold += amount;
        EventBus.Publish(new OnGoldChanged { amount = gold });
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;
        gold -= amount;
        EventBus.Publish(new OnGoldChanged { amount = gold });
        return true;
    }

    // ── Exp / Level ───────────────────────────────
    public void AddExp(int amount)
    {
        exp += amount;
        while (exp >= MaxExp) // 연속 레벨업 처리
        {
            exp -= MaxExp;
            level++;
            Debug.Log($"[PlayerData] 레벨업! Lv.{level}");
        }
        EventBus.Publish(new OnExpChanged { level = level, exp = exp, maxExp = MaxExp });
    }

    // ── Save / Load ───────────────────────────────
    public void Save()
    {
        var saveData = new PlayerSaveData
        {
            gold            = this.gold,
            level           = this.level,
            exp             = this.exp,
            inventory       = inventory
                                .Where(i => i != null)
                                .Select(i => ItemSaveData.FromItemData(i))
                                .ToList(),
            equippedWeapon  = equippedWeapon != null  // 추가
                                ? ItemSaveData.FromItemData(equippedWeapon)
                                : null
        };
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
        Debug.Log($"[PlayerData] 저장 완료: {json}");
    }

    [Header("장착 장비")]
    [SerializeField] private ItemData equippedWeapon = null;
    public ItemData EquippedWeapon => equippedWeapon;

    public void getKit(){
        foreach(Item item in newBieKit){
            AddItem(item.itemData);
        }

        Debug.Log("[PlayerData] 기초지원 세트를 추가하였습니다.");
    }

    private void CleanInventory()
    {
        inventory.RemoveAll(i => i == null || i.name == "none" || string.IsNullOrEmpty(i.name));
    }

    public bool EquipWeapon(ItemData item)
    {
        if (item == null || item.type != "Weapon")
        {
            Debug.Log("[PlayerData] 무기만 장착할 수 있습니다.");
            return false;
        }

        // 같은 무기 재장착 방지
        if (equippedWeapon == item)
        {
            Debug.Log("[PlayerData] 이미 장착된 무기입니다.");
            return false;
        }

        // 새 무기를 인벤토리에서 먼저 제거
        inventory.Remove(item);

        // 기존 장착 무기 → 인벤토리로 반환
        if (equippedWeapon != null)
            inventory.Add(equippedWeapon);

        equippedWeapon = item;

        ApplyWeaponBonus(item);

        CleanInventory();
        EventBus.Publish(new OnWeaponChanged { weapon = equippedWeapon });
        EventBus.Publish(new OnInventoryChanged());
        Debug.Log($"[PlayerData] {item.name} 장착 완료");
        return true;
    }
    public void UnequipWeapon()
    {
        if (equippedWeapon == null) return;

        inventory.Add(equippedWeapon);
        equippedWeapon = null;
        ApplyWeaponBonus(null);

        CleanInventory();
        EventBus.Publish(new OnWeaponChanged { weapon = null });
        EventBus.Publish(new OnInventoryChanged());
        Debug.Log("[PlayerData] 무기 장착 해제");
    }

    public void Load()
    {
        gold       = 0;
        level      = 1;
        exp        = 0;
        maxFloor   = 0;
        inventory.Clear();
        equippedWeapon = null; // 초기화 추가

        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("[PlayerData] 저장 데이터 없음. 기본값 사용.");
            getKit();
        }
        else
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            var saveData = JsonUtility.FromJson<PlayerSaveData>(json);
            gold  = saveData.gold;
            level = saveData.level;
            exp   = saveData.exp;

            if (saveData.inventory != null)
                foreach (var itemSave in saveData.inventory)
                    inventory.Add(itemSave.ToItemData());

            // 장착 무기 복원
            if (saveData.equippedWeapon != null)
            {
                equippedWeapon = saveData.equippedWeapon.ToItemData();
                Debug.Log($"[PlayerData] 장착 무기 복원: {equippedWeapon.name}");
            }

            Debug.Log($"[PlayerData] 불러오기 완료 / Gold:{gold} Lv:{level} Exp:{exp}");
        }

        CleanInventory();
        EventBus.Publish(new OnGoldChanged  { amount = gold });
        EventBus.Publish(new OnExpChanged   { level = level, exp = exp, maxExp = MaxExp });
        EventBus.Publish(new OnWeaponChanged { weapon = equippedWeapon }); // 추가
    }

    public int DismantleItem(ItemData item)
    {
        if (item == null) return 0;

        int refund = UnityEngine.Random.Range(item.min_gold, item.max_gold + 1);
        inventory.Remove(item);
        AddGold(refund);
        CleanInventory();
        EventBus.Publish(new OnInventoryChanged());
        Debug.Log($"[PlayerData] {item.name} 분해 → +{refund} 골드");
        return refund;
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public int gold;
    public int level;
    public int exp;
    public int maxFloor;
    public List<ItemSaveData> inventory;
    public ItemSaveData equippedWeapon; // 추가
}

// 이벤트 구조체
public struct OnGoldChanged { public int amount; }
public struct OnExpChanged  { public int level; public int exp; public int maxExp; }

public struct OnWeaponChanged    { public ItemData weapon; }
public struct OnInventoryChanged { }