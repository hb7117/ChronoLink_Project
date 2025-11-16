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
            Vector3 spawnPosition = (character == "Past") ? new Vector3(0f, 2f, 0f) : new Vector3(100f, 2f, 0f);
            PhotonNetwork.Instantiate(prefabToSpawn.name, spawnPosition, Quaternion.identity);
        }
    }
    #endregion


    public void RegisterTimeObject(string id, TimeObject obj)
    {
        if (!futureTimeObjects.ContainsKey(id))
        {

            futureTimeObjects.Add(id, obj);
        }
        else
        {
            futureTimeObjects[id] = obj;
        }
    }

    public void SyncTimeObject(string id, Vector3 pastLocalPosition)  
    {

        PhotonView pv = GetComponent<PhotonView>();

        if (pv != null)
        {
            pv.RPC("RPC_UpdateFutureObjectPosition", RpcTarget.All, id, pastLocalPosition);
        }
        
    }
 
    [PunRPC]
    void RPC_UpdateFutureObjectPosition(string id, Vector3 pastLocalPosition)  
    {

        TimeObject futureObj;
        if (futureTimeObjects.TryGetValue(id, out futureObj))
        {
            if (futureObj != null)
            {
                 
                Vector3 newFutureLocalPosition = new Vector3(
                    pastLocalPosition.x + 100f, // X축에 40 추가
                    pastLocalPosition.y,
                    pastLocalPosition.z
                );

                futureObj.transform.localPosition = newFutureLocalPosition;
            }
            else  
            {
                futureTimeObjects.Remove(id);
            }
        }
    }

    public void SyncDoorOpen(string doorID)
    {
        PhotonView pv = GetComponent<PhotonView>();
        if (pv != null)
        {
             
            pv.RPC("RPC_OpenFutureDoor", RpcTarget.All, doorID);
        }
         
    }

    
    [PunRPC]
    void RPC_OpenFutureDoor(string doorID)
    {
        
        TimeObject futureDoorObject;
        if (futureTimeObjects.TryGetValue(doorID, out futureDoorObject))
        {
            if (futureDoorObject != null)
            {
                
                DoorController door = futureDoorObject.GetComponent<DoorController>();
                if (door != null)
                {
                    door.OpenDoor();  
                }
               
            }
            else { futureTimeObjects.Remove(doorID); }
        }
        
    }
}