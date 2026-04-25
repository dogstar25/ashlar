using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject quitConfirmPanel;

    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "Game";

    private void Start()
    {
        ShowMainMenu();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingsPanel.activeSelf || quitConfirmPanel.activeSelf)
            {
                ShowMainMenu();
            }
            else
            {
                ShowQuitConfirm();
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        quitConfirmPanel.SetActive(false);
    }

    public void ShowQuitConfirm()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        quitConfirmPanel.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}