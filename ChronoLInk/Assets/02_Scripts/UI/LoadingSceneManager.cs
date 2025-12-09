using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class LoadingSceneManager : MonoBehaviourPunCallbacks
{
    [Header("UI 연결")]
    // [변경] Slider 대신 Image를 연결합니다. (Filled 타입이어야 함)
    [SerializeField] private Image loadingBarFill;
    [SerializeField] private Text loadingText;

    [Header("글리치 연출 연결 (필수)")]
    [SerializeField] private Image noiseImage;
    [SerializeField] private float glitchStartThreshold = 0.2f;
    [SerializeField] private float glitchDuration = 0.1f;
    [SerializeField] private float minGlitchInterval = 0.5f;
    [SerializeField] private float maxGlitchInterval = 2.0f;

    private string whereScene;
    private AsyncOperation asyncLoad;
    private PhotonView photonView;
    private List<int> loadedPlayers = new List<int>();

    // 글리치 제어 변수
    private float currentProgress = 0f;
    private float glitchTimer = 0f;
    private float nextGlitchTime = 0f;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView == null)
        {
            Debug.LogError("LoadingSceneManager에 PhotonView 컴포넌트가 없습니다!");
        }

        if (noiseImage != null)
        {
            noiseImage.enabled = false;
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

        switch (previousScene)
        {
            case "LobbyScene":
                whereScene = "TScene";
                break;
            case "TScene":
                whereScene = "GameScene";
                break;
            case "GameScene":
                whereScene = "GameScene2";
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

        float fakeLoadingTime = 0f;

        while (asyncLoad.progress < 0.9f)
        {
            fakeLoadingTime += Time.deltaTime;
            float realProgress = asyncLoad.progress;

            // [변경] Slider.value 대신 Image.fillAmount 사용
            if (loadingBarFill != null)
            {
                loadingBarFill.fillAmount = realProgress;
            }

            currentProgress = realProgress;
            HandleGlitchEffect();

            yield return null;
        }

        // 로딩 완료 시 100% 채우기
        if (loadingBarFill != null)
        {
            loadingBarFill.fillAmount = 1f;
        }
        currentProgress = 1f;

        StartCoroutine(LoopGlitchWhileWaiting());

        loadingText.text = "무전기 신호 대기중...";
        Debug.Log("로딩 완료. PlayerReadyRPC 전송.");

        photonView.RPC("PlayerReadyRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
    }


    void HandleGlitchEffect()
    {
        if (noiseImage == null) return;

        if (currentProgress >= glitchStartThreshold)
        {
            glitchTimer += Time.deltaTime;

            if (glitchTimer >= nextGlitchTime)
            {
                StartCoroutine(DoGlitchEffect());
                glitchTimer = 0f;
                nextGlitchTime = Random.Range(minGlitchInterval, maxGlitchInterval);
            }
        }
    }

    IEnumerator LoopGlitchWhileWaiting()
    {
        while (asyncLoad.allowSceneActivation == false)
        {
            HandleGlitchEffect();
            yield return null;
        }
    }

    IEnumerator DoGlitchEffect()
    {
        if (noiseImage != null)
        {
            noiseImage.enabled = true;
            yield return new WaitForSeconds(glitchDuration);
            noiseImage.enabled = false;
        }
    }

    [PunRPC]
    void PlayerReadyRPC(int actorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (!loadedPlayers.Contains(actorNumber))
        {
            loadedPlayers.Add(actorNumber);
            Debug.Log($"플레이어 {actorNumber} 로딩 완료. ({loadedPlayers.Count} / {PhotonNetwork.CurrentRoom.PlayerCount})");
        }

        if (loadedPlayers.Count == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("모든 플레이어 로딩 완료! 씬 활성화.");
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