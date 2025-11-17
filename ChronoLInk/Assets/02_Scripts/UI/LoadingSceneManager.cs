using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class LoadingSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Slider progressbar;
    [SerializeField] private Text loadingText;

    private string whereScene;
    private AsyncOperation asyncLoad;
    private PhotonView photonView;

    private List<int> loadedPlayers = new List<int>();

    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView == null)
        {
            Debug.LogError("LoadingSceneManager에 PhotonView 컴포넌트가 없습니다! RPC가 작동하지 않습니다.");
        }
    }

    void Start()
    {
        SetDestinationScene();
        StartCoroutine(LoadingGameSceneAsync());
    }

    void SetDestinationScene()
    {
        string previousScene = SceneHistory.previousSceneName;
        // ...
        switch (previousScene)
        {
            case "LobbyScene":
                whereScene = "GameScene";
                break;
            case "GameScene":
                whereScene = "GameScene2"; // "NextStageScene"을 실제 다음 씬 이름으로 변경
                break;
            default:
                whereScene = "LobbyScene";
                break;
        }
    }

    IEnumerator LoadingGameSceneAsync()
    {
        asyncLoad = SceneManager.LoadSceneAsync(whereScene);
        asyncLoad.allowSceneActivation = false;
        loadingText.text = "Loading...";

        while (asyncLoad.progress < 0.9f)
        {
            progressbar.value = asyncLoad.progress;
            yield return null;
        }

        progressbar.value = 1f;

        loadingText.text = "다른 플레이어를 기다리는 중...";

        Debug.Log("로딩 완료. PlayerReadyRPC를 마스터 클라이언트로 전송합니다.");
        photonView.RPC("PlayerReadyRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    void PlayerReadyRPC(int actorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (!loadedPlayers.Contains(actorNumber))
        {
            loadedPlayers.Add(actorNumber);
            Debug.Log($"플레이어 {actorNumber} 로딩 완료. (현재 {loadedPlayers.Count} / {PhotonNetwork.CurrentRoom.PlayerCount})");
        }

        if (loadedPlayers.Count == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("모든 플레이어 로딩 완료! 씬을 동시에 활성화합니다.");

            loadedPlayers.Clear();

            photonView.RPC("ActivateSceneRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    void ActivateSceneRPC()
    {
        PhotonNetwork.IsMessageQueueRunning = false;

        asyncLoad.allowSceneActivation = true;
    }
}