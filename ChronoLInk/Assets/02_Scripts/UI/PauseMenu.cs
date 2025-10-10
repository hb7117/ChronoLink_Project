using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviourPunCallbacks
{
    public GameObject pausePanel;
    public GameObject optionPanel;

    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionPanel.activeSelf)
            {
                //
            }
            else if (isPaused)
            {
                //
            }
            else
            {
                //
            }
        }
    }
    private void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true); // esc 누르면 판넬 나오게

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ResumeGame()// 게임 계속하기 바튼
    {
        isPaused = false;
        pausePanel.SetActive(false);
        optionPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
    public void OpenOptions() // 옵션 버튼
    {
        pausePanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionPanel.SetActive(false); // 옵션 패널은 닫고
        pausePanel.SetActive(true); // 다시 일시정지 패널을 엽니다.
    }

    // '로비로 나가기' 버튼에 연결할 함수입니다.
    public void LeaveToLobby()
    {
        // 먼저 현재 입장해 있는 포톤 룸을 나갑니다.
        Debug.Log("방에서 나갑니다...");
        PhotonNetwork.LeaveRoom();

        SceneManager.LoadScene("LobbyScene");
    }

    // LeaveRoom()이 성공적으로 완료되면 자동으로 호출되는 콜백 함수입니다.
    public override void OnLeftRoom()
    {
        // 방에서 완전히 나간 후에 로비 씬을 로드하는 것이 가장 안전합니다.
        // "LobbyScene" 부분은 실제 로비 씬의 이름으로 변경해주세요.
        
    }

    // '게임 종료' 버튼을 따로 만들 경우를 위한 함수입니다.
    public void QuitGame()
    {
        Debug.Log("게임을 종료합니다...");
        Application.Quit();
    }
}
