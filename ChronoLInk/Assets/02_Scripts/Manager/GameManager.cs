using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가
using Photon.Pun;
using Photon.Voice.Unity; // 보이스 관리를 위해 추가
using System.Collections.Generic;

public class GameManager : MonoBehaviourPunCallbacks
{
    // 다른 스크립트에서 GameManager에 쉽게 접근할 수 있도록 Singleton 패턴을 적용합니다.
    public static GameManager Instance { get; private set; }

    [Header("플레이어 프리팹")]
    [SerializeField] private GameObject pastPlayerPrefab;
    [SerializeField] private GameObject futurePlayerPrefab;

    // 방장만 사용하는 변수들
    private List<int> readyPlayers = new List<int>();
    private bool isSpawningStarted = false;

    // 보이스 연결을 제어하기 위한 변수
    private VoiceConnection voiceConnection;

    #region 유니티 생명주기 및 싱글톤
    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 씬에 있는 VoiceConnection 컴포넌트를 찾습니다.
        voiceConnection = FindObjectOfType<VoiceConnection>();

        // 기존 플레이어 준비 신호 보내기
        photonView.RPC("SignalPlayerIsReady", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
    }
    #endregion

    #region 방 나가기 및 보이스 연결 해제 (새로운 핵심 로직)
    /// <summary>
    /// PauseMenu의 버튼 등 외부에서 호출할 함수입니다.
    /// 방을 나가는 전체 과정을 시작합니다.
    /// </summary>
    public void LeaveGameAndReturnToLobby()
    {
        // PUN 룸을 나가는 요청을 보냅니다.
        // 이후 작업은 OnLeftRoom 콜백 함수에서 자동으로 처리됩니다.
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// PhotonNetwork.LeaveRoom()이 성공하면 자동으로 호출되는 콜백 함수입니다.
    /// </summary>
    public override void OnLeftRoom()
    {
        // 보이스 연결을 끊습니다.
        // voiceConnection.Disconnect() 대신, 내부의 Client를 통해 직접 Disconnect를 호출합니다.
        if (voiceConnection != null && voiceConnection.Client != null && voiceConnection.Client.IsConnected)
        {
            voiceConnection.Client.Disconnect();
        }

        // 로비 씬으로 이동합니다.
        // "LobbyScene"은 실제 사용하는 로비 씬 이름으로 바꿔주세요.
        SceneManager.LoadScene("LobbyScene");
    }
    #endregion

    #region 기존 플레이어 스폰 로직
    [PunRPC]
    void SignalPlayerIsReady(int actorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (isSpawningStarted) return;
        if (!readyPlayers.Contains(actorNumber))
        {
            readyPlayers.Add(actorNumber);
        }
        if (readyPlayers.Count == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            isSpawningStarted = true;
            photonView.RPC("StartSpawningProcess", RpcTarget.All);
        }
    }

    [PunRPC]
    void StartSpawningProcess()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("character", out object characterValue))
        {
            string character = (string)characterValue;
            GameObject prefabToSpawn = (character == "Past") ? pastPlayerPrefab : futurePlayerPrefab;
            Vector3 spawnPosition = (character == "Past") ? new Vector3(-20f, 2f, 0f) : new Vector3(20f, 2f, 0f);
            PhotonNetwork.Instantiate(prefabToSpawn.name, spawnPosition, Quaternion.identity);
        }
    }
    #endregion
}

