using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject quitConfirmPanel;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused;

    private void Start()
    {
        ResumeGame();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleBackButton();
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        Time.timeScale = 0f;

        hudPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
    }

    public void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1f;

        hudPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        hudPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        quitConfirmPanel.SetActive(false);
    }

    public void ShowQuitConfirm()
    {
        hudPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(true);
    }

    public void ReturnToPauseMenu()
    {
        //hudPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void HandleBackButton()
    {
        if (settingsPanel.activeSelf || quitConfirmPanel.activeSelf)
        {
            ReturnToPauseMenu();
        }
        else if (pauseMenuPanel.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            PauseGame();

            // Later: save game state here.
            // SaveManager.Save();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            PauseGame();

            // Later: save game state here too.
        }
    }
}