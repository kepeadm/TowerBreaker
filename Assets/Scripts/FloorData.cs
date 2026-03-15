using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FloorData", menuName = "Stage/FloorData")]
public class FloorData : ScriptableObject
{
    [System.Serializable]
    public class EnemyEntry
    {
        public GameObject prefab;
        public float      spacing = 2f;
    }

    [Header("몬스터 패턴")]
    public List<EnemyEntry> pattern = new List<EnemyEntry>();

    [Header("스폰 위치")]
    public float   startX         = 20f;
    public float   floorY         = 0f;
    public Vector2 playerSpawnPos;
    public Vector3 cameraPos;
}