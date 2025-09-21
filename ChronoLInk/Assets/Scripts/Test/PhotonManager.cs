using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PhotonManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        //photonServerSettings �� ������ appID �� ������ ���� ����

        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("������ ���� ���� ����");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� ���� ����");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
