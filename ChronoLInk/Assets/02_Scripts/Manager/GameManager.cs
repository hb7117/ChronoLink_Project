using Photon.Pun;
using Photon.Voice.Unity;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{

    public static GameManager Instance { get; private set; }

    [Header("플레이어 프리팹")]
    [SerializeField] private GameObject pastPlayerPrefab;
    [SerializeField] private GameObject futurePlayerPrefab;

    // 방장만 사용하는 변수들
    private List<int> readyPlayers = new List<int>();
    private bool isSpawningStarted = false;

    private Dictionary<string, TimeObject> futureTimeObjects = new Dictionary<string, TimeObject>();

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
        PhotonNetwork.IsMessageQueueRunning = true;

        voiceConnection = FindObjectOfType<VoiceConnection>();

        photonView.RPC("SignalPlayerIsReady", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);

    }
    #endregion

    #region 방 나가기 및 보이스 연결 해제 (새로운 핵심 로직)
    public void LeaveGameAndReturnToLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        if (voiceConnection != null && voiceConnection.Client != null && voiceConnection.Client.IsConnected)
        {
            voiceConnection.Client.Disconnect();
        }

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
            Vector3 spawnPosition = (character == "Past") ? new Vector3(0f, 2f, 0f) : new Vector3(40f, 2f, 0f);
            PhotonNetwork.Instantiate(prefabToSpawn.name, spawnPosition, Quaternion.identity);
        }
    }
    #endregion

    // --- 시간 개변 로직 ---

    public void RegisterTimeObject(string id, TimeObject obj)
    {
        if (!futureTimeObjects.ContainsKey(id))
        {

            futureTimeObjects.Add(id, obj);
            Debug.Log($"GameManager: 미래 오브젝트 '{id}' (InstanceID: {obj.GetInstanceID()}) 등록 완료.");
        }
        else
        {
            futureTimeObjects[id] = obj;
            Debug.LogWarning($"GameManager: 미래 오브젝트 ID '{id}'가 이미 존재하여 업데이트합니다.");
        }
    }

    public void SyncTimeObject(string id, Vector3 pastLocalPosition) // 💡 변수명 변경 (newLocalPosition -> pastLocalPosition)
    {
        Debug.Log($"GameManager.SyncTimeObject received request: ID '{id}', PastPos: {pastLocalPosition}. Checking PhotonView...");

        PhotonView pv = GetComponent<PhotonView>();

        if (pv != null)
        {
            Debug.Log("GameManager PhotonView found. Attempting to send RPC...");
            // 💡 과거의 위치(pastLocalPosition)를 그대로 RPC로 전송
            pv.RPC("RPC_UpdateFutureObjectPosition", RpcTarget.All, id, pastLocalPosition);
        }
        else
        {
            Debug.LogError("GameManager is MISSING PhotonView component! Cannot send SyncTimeObject RPC.");
        }
    }

    // 💡 [핵심 수정] 이 함수가 수정되었습니다.
    [PunRPC]
    void RPC_UpdateFutureObjectPosition(string id, Vector3 pastLocalPosition) // 💡 변수명 변경
    {
        Debug.Log($"RPC_UpdateFutureObjectPosition EXECUTED on client: {PhotonNetwork.LocalPlayer.NickName}. ID: '{id}', PastPos: {pastLocalPosition}");

        TimeObject futureObj;
        if (futureTimeObjects.TryGetValue(id, out futureObj))
        {
            if (futureObj != null)
            {
                // 💡 [수정] 과거 위치에 +40 오프셋을 더해 '미래 위치'를 계산합니다.
                Vector3 newFutureLocalPosition = new Vector3(
                    pastLocalPosition.x + 40f, // X축에 40 추가
                    pastLocalPosition.y,
                    pastLocalPosition.z
                );

                // 💡 [수정] 계산된 '미래 위치'로 설정합니다.
                futureObj.transform.localPosition = newFutureLocalPosition;
            }
            else // 파괴된 오브젝트 목록에서 제거
            {
                futureTimeObjects.Remove(id);
            }
        }
    }
}