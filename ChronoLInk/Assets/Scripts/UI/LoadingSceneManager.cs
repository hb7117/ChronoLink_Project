using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;

// PunRPC를 사용하기 위해 MonoBehaviourPunCallbacks로 변경합니다.
public class LoadingSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Slider progressbar;
    [SerializeField] private Text loadingText;

    private AsyncOperation asyncLoad;
    private PhotonView photonView;

    void Awake()
    {
        // RPC 통신을 위해 PhotonView 컴포넌트가 필요합니다.
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        StartCoroutine(LoadingGameSceneAsync());
    }

    IEnumerator LoadingGameSceneAsync()
    {
        asyncLoad = SceneManager.LoadSceneAsync("GameScene");
        asyncLoad.allowSceneActivation = false;
        loadingText.text = "Loading...";

        // 씬 로딩이 90%까지 완료될 때까지 대기
        while (asyncLoad.progress < 0.9f)
        {
            progressbar.value = asyncLoad.progress;
            yield return null;
        }

        progressbar.value = 1f;
        loadingText.text = "아무 키나 눌러주세요";

        // 키 입력 대기
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        loadingText.text = "다른 플레이어를 기다리는 중...";

        // ★★★ 새로운 로직: 방장에게 내가 준비되었다고 알립니다 ★★★
        photonView.RPC("PlayerReadyRPC", RpcTarget.MasterClient);
    }

    // 이 함수는 오직 방장(MasterClient)의 컴퓨터에서만 실행됩니다.
    [PunRPC]
    void PlayerReadyRPC()
    {
        // 방장이 아니라면 즉시 종료
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // 준비된 플레이어 수를 카운트하기 위해 룸 커스텀 프로퍼티를 사용
        int readyCount = 0;
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("readyCount"))
        {
            readyCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["readyCount"];
        }

        readyCount++;

        // 룸 프로퍼티 업데이트
        var newProps = new ExitGames.Client.Photon.Hashtable();
        newProps["readyCount"] = readyCount;
        PhotonNetwork.CurrentRoom.SetCustomProperties(newProps);

        // 준비된 플레이어 수와 현재 방의 플레이어 수가 같다면, 모든 플레이어에게 씬을 활성화하라고 알림
        if (readyCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("모든 플레이어 준비 완료! 씬을 동시에 활성화합니다.");
            photonView.RPC("ActivateSceneRPC", RpcTarget.All);
        }
    }

    // 이 함수는 모든 플레이어의 컴퓨터에서 실행됩니다.
    [PunRPC]
    void ActivateSceneRPC()
    {
        // 씬을 활성화하여 GameScene으로 진입
        asyncLoad.allowSceneActivation = true;
    }
}