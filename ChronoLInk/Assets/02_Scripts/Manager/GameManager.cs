using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("플레이어 프리팹")]
    [SerializeField] private GameObject pastPlayerPrefab;
    [SerializeField] private GameObject futurePlayerPrefab;

    // 방장만 사용하는 변수들     
    private List<int> readyPlayers = new List<int>();
    private bool isSpawningStarted = false;

    #region 스크립트 내용
    void Start()
    {
       
        photonView.RPC("SignalPlayerIsReady", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    
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
            Vector3 spawnPosition = (character == "Past") ? new Vector3(10f, 2f, 0f) : new Vector3(-10f, 2f, 0f);

            PhotonNetwork.Instantiate(prefabToSpawn.name, spawnPosition, Quaternion.identity);
        }
    }
    #endregion  // 
}