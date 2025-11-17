using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;  

public class EscapeZone : MonoBehaviourPunCallbacks
{
    private PhotonView photonView;

    private int playersAtExitCount = 0;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PhotonView pv = other.GetComponent<PhotonView>();

        if (pv != null && pv.IsMine)
        {
            Debug.Log("플레이어가 탈출 지점에 도달. 마스터 클라이언트에게 알립니다.");

            photonView.RPC("PlayerReachedExit", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);

            this.gameObject.GetComponent<Collider>().enabled = false;
        }
    }

    [PunRPC]
    void PlayerReachedExit(int actorNumber, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        
        int currentReadyCount = 0;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("ExitReadyCount", out object readyCountObj))
        {
            currentReadyCount = (int)readyCountObj;
        }

       

        currentReadyCount++;
        Debug.Log($"마스터 클라이언트: 플레이어 탈출 감지. 현재 카운트: {currentReadyCount} / {PhotonNetwork.CurrentRoom.PlayerCount}");

        var newProps = new ExitGames.Client.Photon.Hashtable();
        newProps["ExitReadyCount"] = currentReadyCount;
        PhotonNetwork.CurrentRoom.SetCustomProperties(newProps);

        if (currentReadyCount >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("모든 플레이어 탈출! 다음 씬으로 이동합니다.");

            newProps["ExitReadyCount"] = 0;
            PhotonNetwork.CurrentRoom.SetCustomProperties(newProps);

            photonView.RPC("LoadNextSceneRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    void LoadNextSceneRPC()
    {

        SceneHistory.previousSceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene("LoadingScene");
    }
}