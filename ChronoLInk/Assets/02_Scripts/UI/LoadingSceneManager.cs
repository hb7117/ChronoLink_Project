using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;

// PunRPC를 사용하기 위해 MonoBehaviourPunCallbacks를 사용합니다.
public class LoadingSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Slider progressbar;
    [SerializeField] private Text loadingText;

    // 로드할 씬의 이름을 담을 변수
    private string whereScene;
    private AsyncOperation asyncLoad;
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        // 로딩 코루틴을 시작하기 전에, 먼저 어느 씬으로 갈지 결정합니다.
        SetDestinationScene();

        // 결정된 씬으로 로딩을 시작합니다.
        StartCoroutine(LoadingGameSceneAsync());
    }

    /// <summary>
    /// 이전 씬의 이름에 따라 다음에 로드할 씬을 결정하는 함수
    /// </summary>
    void SetDestinationScene()
    {
        string previousScene = SceneHistory.previousSceneName;
        Debug.Log("이전 씬: " + previousScene);

        switch (previousScene)
        {
            case "Photon":
                whereScene = "GameScene";
                break;
            case "GameScene":
                whereScene = "GameScene1";
                break;
            // --- 여기에 새로운 씬 전환 규칙을 추가하세요 ---
            default:
                Debug.LogWarning("정의되지 않은 이전 씬입니다. 기본 씬(LobbyScene)으로 이동합니다.");
                whereScene = "Photon";
                break;
        }
    }

    IEnumerator LoadingGameSceneAsync()
    {
        // "GameScene" 이라고 고정된 부분을, 위에서 결정된 whereScene 변수로 교체합니다.
        asyncLoad = SceneManager.LoadSceneAsync(whereScene);

        asyncLoad.allowSceneActivation = false;
        loadingText.text = "Loading...";

        while (asyncLoad.progress < 0.9f)
        {
            progressbar.value = asyncLoad.progress;
            yield return null;
        }

        progressbar.value = 1f;
        loadingText.text = "아무 키나 눌러주세요";

        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        loadingText.text = "다른 플레이어를 기다리는 중...";

        photonView.RPC("PlayerReadyRPC", RpcTarget.MasterClient);
    }

    // --- 아래의 Photon RPC 관련 코드는 기존과 동일합니다 ---

    [PunRPC]
    void PlayerReadyRPC()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int readyCount = 0;
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("readyCount"))
        {
            readyCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["readyCount"];
        }
        readyCount++;

        var newProps = new ExitGames.Client.Photon.Hashtable();
        newProps["readyCount"] = readyCount;
        PhotonNetwork.CurrentRoom.SetCustomProperties(newProps);

        if (readyCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("모든 플레이어 준비 완료! 씬을 동시에 활성화합니다.");
            photonView.RPC("ActivateSceneRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    void ActivateSceneRPC()
    {
        asyncLoad.allowSceneActivation = true;
    }
}

