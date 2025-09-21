using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 추가한 Using

using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun;


public class LobbyManager : MonoBehaviourPunCallbacks
{

    public InputField RoomNameInputField;
    public Button CreatRoonButton;
    public Button JoinRandomRoomButton;
    public Transform RoomListContent;
    public GameObject RoomListItemPrefab;

    private List<RoomInfo> roomList = new List<RoomInfo>();
    // Start is called before the first frame update
    void Start()
    {
        // 씬 시작시 로비에 자동 접속

        //OnConnectedToMaster 먼저 콜백후 호출 그다음 OnJoinedLobby() 가 호출

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버 로비 연결 성공");
        PhotonNetwork.JoinLobby();

    }
    public override void OnRoomListUpdate(List<RoomInfo> _roomList)
    {

        Debug.Log("로비 방목록 업데이트");
        roomList = _roomList;
        //UpdateRoomList();

    }

    void UpdateRoomList()
    {
        // 기존 방 목록 UI를 모두 삭제
        foreach(Transform child in RoomListContent)
        {
            Destroy(child.gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
