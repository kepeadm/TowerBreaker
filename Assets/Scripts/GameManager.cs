using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;

    void Start()
    {
        if (playerData == null)
        {
            Debug.LogError("[GameManager] PlayerData가 연결되지 않았습니다.");
            return;
        }

        playerData.Load();
    }

    void OnApplicationQuit() => SaveGame();

    void OnApplicationPause(bool paused)
    {
        if (paused) SaveGame();
    }

    public void SaveGame()
    {
        playerData.Save();
        Debug.Log("[GameManager] 저장 완료");
    }

    public void ResetPlayerData()
    {
        PlayerPrefs.DeleteKey("PlayerSaveData");
        PlayerPrefs.Save();
        playerData.Load();
        Debug.Log("[GameManager] PlayerData 리셋 완료");
    }
    public void EnterDungeon()
    {
        playerData.Save();
        SceneManager.LoadScene("InGame");
    }
}