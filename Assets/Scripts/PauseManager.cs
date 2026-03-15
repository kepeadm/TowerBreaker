using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private PlayerData playerData;
    public void OpenPause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
    public void ExitToLobby()
    {
        Time.timeScale = 1f;
        playerData?.Save();
        SceneManager.LoadScene("OutGame");
    }

    public void QuitGame()
    {
        playerData?.Save();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}