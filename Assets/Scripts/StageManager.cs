using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [Header("층 데이터")]
    [SerializeField] private FloorData[] floors;

    [Header("참조")]
    [SerializeField] private Transform spawnRoot;
    [SerializeField] private GameObject stagePanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Transform  playerTransform;
    [SerializeField] private Transform  cameraTransform;


    private int  _currentFloor = 0;
    private bool _isCleared    = false;
    private List<GameObject> _activeEnemies = new List<GameObject>();

    void Awake() => Instance = this;

    void Start() => StartCoroutine(WaitAndSpawn());

    private IEnumerator WaitAndSpawn()
    {
        while (ObjectPool.Instance == null)
            yield return null;

        yield return StartCoroutine(SpawnFloor(_currentFloor));
    }

    // ── 층 스폰 ───────────────────────────────────
    public IEnumerator SpawnFloor(int floorIndex)
    {
        if (floorIndex >= floors.Length) yield break;

        _isCleared = false;
        FloorData floor  = floors[floorIndex];
        float     spawnX = floor.startX;

        _activeEnemies.Clear();

        // 플레이어 스폰 위치
        if (playerTransform != null)
            playerTransform.position = floor.playerSpawnPos;

        // 카메라 위치
        if (cameraTransform != null)
        {
            Vector3 camPos = floor.cameraPos;
            camPos.z = -10f;
            cameraTransform.position = camPos;
        }

        foreach (var entry in floor.pattern)
        {
            if (entry.prefab == null) continue;

            Vector3    pos = new Vector3(spawnX, floor.floorY, 0);
            GameObject obj = ObjectPool.Instance.Get(
                entry.prefab, pos, Quaternion.identity);

            obj.GetComponent<EnemyBase>()?.ResetEnemy();
            _activeEnemies.Add(obj);

            spawnX += entry.spacing;
            yield return null;
        }
        PublishEnemyCount();
    }

    // ── 다음 층으로 (StagePanel 버튼에 연결) ──────
    public void NextFloor()
    {
        if (stagePanel != null) stagePanel.SetActive(false);

        _currentFloor++;
        StartCoroutine(SpawnFloor(_currentFloor));
    }

    // ── 나가기 (StagePanel / EndPanel 버튼에 연결) ─
    public void ExitToLobby()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("OutGame");
    }

    // ── 클리어 체크 ───────────────────────────────
    public bool IsFloorClear()
    {
        if (_activeEnemies.Count == 0) return false;
        foreach (var enemy in _activeEnemies)
            if (enemy != null && enemy.activeSelf) return false;
        return true;
    }

    private int _lastEnemyCount = -1;

    void Update()
    {
        int current = GetRemainingCount();
        if (current != _lastEnemyCount)
        {
            _lastEnemyCount = current;
            PublishEnemyCount();
        }

        if (_isCleared) return;
        if (!IsFloorClear()) return;

        _isCleared = true;

        bool isLastFloor = (_currentFloor >= floors.Length - 1);
        if (isLastFloor)
        {
            Debug.Log("[Stage] 모든 스테이지 클리어!");
            if (endPanel != null) endPanel.SetActive(true);
        }
        else
        {
            Debug.Log($"[Stage] {_currentFloor + 1}F 클리어!");
            if (stagePanel != null) stagePanel.SetActive(true);
        }
    }
    public int GetRemainingCount()
    {
        int count = 0;
        foreach (var enemy in _activeEnemies)
            if (enemy != null && enemy.activeSelf) count++;
        return count;
    }

    private void PublishEnemyCount()
    {
        EventBus.Publish(new OnEnemyCountChanged { remaining = GetRemainingCount() });
    }
}

public struct OnEnemyCountChanged { public int remaining; }