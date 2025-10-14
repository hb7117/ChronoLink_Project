using UnityEngine;

// 이 스크립트는 이제 순수하게 UI만 담당하므로 Photon 관련 코드가 모두 필요 없습니다.
public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject optionsPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (pausePanel.activeSelf)
        {
            Resume();
        }
        else
        {
            pausePanel.SetActive(true);
            optionsPanel.SetActive(false);
        }
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
    }

    public void ShowOptions()
    {
        optionsPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    public void BackToPauseMenu()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

   
    public void LeaveToLobby()
    {
        // GameManager의 싱글톤 인스턴스를 통해 방 나가기 함수를 호출합니다.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LeaveGameAndReturnToLobby();
        }
        else
        {
            Debug.LogError("GameManager 인스턴스를 찾을 수 없습니다!");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

