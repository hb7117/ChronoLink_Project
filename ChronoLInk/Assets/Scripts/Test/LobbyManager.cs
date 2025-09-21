using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �߰��� Using

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
        // �� ���۽� �κ� �ڵ� ����

        //OnConnectedToMaster ���� �ݹ��� ȣ�� �״��� OnJoinedLobby() �� ȣ��

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("������ ���� �κ� ���� ����");
        PhotonNetwork.JoinLobby();

    }
    public override void OnRoomListUpdate(List<RoomInfo> _roomList)
    {

        Debug.Log("�κ� ���� ������Ʈ");
        roomList = _roomList;
        //UpdateRoomList();

    }

    void UpdateRoomList()
    {
        // ���� �� ��� UI�� ��� ����
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
