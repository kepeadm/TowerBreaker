using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData itemData;
}

[System.Serializable]
public class ItemData
{
    [Header("Item Data")]
    public string name           = "none";
    public string type           = "none";
    public float damage          = 0f;
    public float ciritial_chance = 0f;
    public float ciritial_damage = 0f;
    public int upgrade           = 0;
    public int min_gold          = 0;
    public int max_gold          = 0;
    public string spritePath     = "";

    [System.NonSerialized] public Sprite sprite;
    [System.NonSerialized] public Image image;
}


[System.Serializable]
public class ItemSaveData
{
    public string name;
    public string type;
    public float damage;
    public float ciritial_chance;
    public float ciritial_damage;
    public int upgrade;
    public int min_gold;
    public int max_gold;
    public string spritePath;

    public static ItemSaveData FromItemData(ItemData d) => new ItemSaveData
    {
        name            = d.name,
        type            = d.type,
        damage          = d.damage,
        ciritial_chance = d.ciritial_chance,
        ciritial_damage = d.ciritial_damage,
        upgrade         = d.upgrade,
        min_gold        = d.min_gold,
        max_gold        = d.max_gold,
        spritePath      = d.spritePath
    };

    public ItemData ToItemData() => new ItemData
    {
        name            = this.name,
        type            = this.type,
        damage          = this.damage,
        ciritial_chance = this.ciritial_chance,
        ciritial_damage = this.ciritial_damage,
        upgrade         = this.upgrade,
        min_gold        = this.min_gold,
        max_gold        = this.max_gold,
        spritePath      = this.spritePath,
        sprite          = string.IsNullOrEmpty(this.spritePath)
                              ? null
                              : Resources.Load<Sprite>(this.spritePath)
    };
}