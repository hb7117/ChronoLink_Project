using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameMenuController : MonoBehaviourPunCallbacks
{
    public GameObject menuPanel;
    public GameObject optionPanel;

    private bool isMenuOpen = false;

    private void Start()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionPanel != null && optionPanel.activeSelf)
            {
                optionPanel.SetActive(false);
                menuPanel.SetActive(true);
            }
            else
            {
                ToggleMenu();
            }
        }
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (menuPanel != null)
            menuPanel.SetActive(isMenuOpen);

        if (isMenuOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void OnClickContinue()
    {
        ToggleMenu();
    }

    public void OnClickOption()
    {
        if (optionPanel != null)
        {
            menuPanel.SetActive(false);
            optionPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Option Panel is not assigned.");
        }
    }

    public void OnClickBackFromOption()
    {
        if (optionPanel != null) optionPanel.SetActive(false);
        if (menuPanel != null) menuPanel.SetActive(true);
    }

    public void OnClickQuitGame()
    {
        Application.Quit();
    }

    public void OnClickReturnToLobby()
    {
        Time.timeScale = 1f;
        PhotonNetwork.OfflineMode = false;

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene("LobbyScene");
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("LobbyScene");
    }
}