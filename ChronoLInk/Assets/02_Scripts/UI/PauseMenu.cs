using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [Header("공용 패널")]
    public GameObject pausePanel;

    [Header("캐릭터별 옵션 패널")]
    public GameObject pastOptionsPanel;
    public GameObject futureOptionsPanel;

    private bool isPaused = false;
    private string localPlayerCharacterType;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void RegisterLocalPlayer(string characterType)
    {
        localPlayerCharacterType = characterType;
        Debug.Log($"로컬 플레이어 등록 완료: {localPlayerCharacterType}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        if (isPaused) { Pause(); }
        else { Resume(); }
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        pastOptionsPanel.SetActive(false);
        futureOptionsPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Pause()
    {
        isPaused = true;

        pausePanel.SetActive(true);
        pastOptionsPanel.SetActive(false);
        futureOptionsPanel.SetActive(false);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowOptions()
    {
        pausePanel.SetActive(false);

        if (localPlayerCharacterType == "Past")
        {
            pastOptionsPanel.SetActive(true);
        }
        else if (localPlayerCharacterType == "Future")
        {
            futureOptionsPanel.SetActive(true);
        }
    }

    public void BackToPauseMenu()
    {
        pastOptionsPanel.SetActive(false);
        futureOptionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void LeaveToLobby()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LeaveGameAndReturnToLobby();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}