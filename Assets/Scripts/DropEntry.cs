using UnityEngine;

[System.Serializable]
public class DropEntry
{
    [Range(0f, 1f)]
    public float    chance;    // 드롭 확률 (0~1)
    public ItemData itemData;  // 드롭 아이템
}