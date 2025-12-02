using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.SceneManagement;

public class QuickSingleStarter : MonoBehaviourPunCallbacks
{
    public GameObject lobbyPanelGroup;
    private ConnectAndLobbyManager savedLobbyManager; // 복구용 저장 변수

    public void StartSinglePlayPast()
    {
        Debug.Log(">>> [1] 싱글 플레이 진입 시도");

        // 0. 로비 매니저 찾아서 저장해두고 끄기
        savedLobbyManager = FindObjectOfType<ConnectAndLobbyManager>();
        if (savedLobbyManager != null)
        {
            savedLobbyManager.enabled = false; // 충돌 방지 위해 끄기
            Debug.Log(">>> [2] 기존 로비 매니저 잠시 중단됨");
        }

        // 0. UI 숨기기
        if (lobbyPanelGroup != null) lobbyPanelGroup.SetActive(false);

        StartCoroutine(ProcessOfflineMode());
    }

    IEnumerator ProcessOfflineMode()
    {
        // 1. 연결 끊기
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected)
            {
                yield return null;
            }
        }

        Debug.Log(">>> [3] 연결 해제 확인. 오프라인 모드 ON");

        // 2. 오프라인 모드 설정
        PhotonNetwork.OfflineMode = true;

        // 3. 캐릭터 설정
        var props = new ExitGames.Client.Photon.Hashtable();
        props["character"] = "Past";
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        // 4. 방 만들기
        Debug.Log(">>> [4] 싱글룸 생성 시도...");
        PhotonNetwork.CreateRoom("SingleRoom");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(">>> [5] 방 입장 성공! 씬 로딩 시도: LoadingScene");

        // 씬 이름이 정확한지 꼭 확인하세요! 대소문자 구별합니다.
        //PhotonNetwork.LoadLevel("LoadingScene");
        SceneManager.LoadScene("LoadingScene");
    }

    // [중요] 만약 실패했다면? -> 원상복구!
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($">>> [ERROR] 방 생성 실패: {message}");
        RecoverState();
    }

    // 상태 복구 함수
    private void RecoverState()
    {
        Debug.LogWarning("오류가 발생하여 상태를 복구합니다.");

        // 1. 오프라인 모드 끄기
        PhotonNetwork.OfflineMode = false;

        // 2. 로비 UI 다시 보이기
        if (lobbyPanelGroup != null) lobbyPanelGroup.SetActive(true);

        // 3. 로비 매니저 다시 켜기 (이게 있어야 입력이 다시 먹힘)
        if (savedLobbyManager != null)
        {
            savedLobbyManager.enabled = true;
            // 연결이 끊겼으니 다시 연결을 시도하게 하거나 초기화
            savedLobbyManager.AllOffUI(); // 기존 매니저에 있는 UI 초기화 함수 호출
            savedLobbyManager.OnClickLogo(); // 초기 화면으로 돌리기
        }
    }
}