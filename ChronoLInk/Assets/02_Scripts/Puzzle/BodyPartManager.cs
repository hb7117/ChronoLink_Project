using UnityEngine;
using Photon.Pun;

public class BodyPartManager : MonoBehaviourPun
{
    [Header("슬롯 연결 (3개)")]
    public BodyPartSlot slotHead;
    public BodyPartSlot slotArm;
    public BodyPartSlot slotLeg;

    [Header("보상 오브젝트 설정")]
    // [변경] 프리팹 대신 씬에 미리 배치해 둔 오브젝트를 연결합니다.
    public GameObject sceneRewardObject;

    [Header("상태 확인용")]
    public bool isPuzzleSolved = false;

    private void Start()
    {
        // 게임 시작 시 보상 오브젝트가 켜져 있다면 강제로 끕니다.
        // (에디터에서 켜놓고 시작해도 자동으로 숨겨짐)
        if (sceneRewardObject != null)
        {
            sceneRewardObject.SetActive(false);
        }
    }

    public void CheckPuzzleCompletion()
    {
        if (isPuzzleSolved) return;

        // 3개 슬롯이 모두 정답인지 확인
        if (slotHead.isCorrectItemPlaced &&
            slotArm.isCorrectItemPlaced &&
            slotLeg.isCorrectItemPlaced)
        {
            Debug.Log("퍼즐 완성! 보상 오브젝트를 활성화합니다.");

            // 모든 플레이어의 화면에서 동시에 오브젝트를 켜기 위해 RPC 사용
            photonView.RPC("RPC_ActivateReward", RpcTarget.All);
        }
    }

    [PunRPC]
    void RPC_ActivateReward()
    {
        if (isPuzzleSolved) return; // 중복 실행 방지

        isPuzzleSolved = true;
        Debug.Log("보상 등장!");

        if (sceneRewardObject != null)
        {
            // 1. 오브젝트를 켭니다. (보이게 됨)
            sceneRewardObject.SetActive(true);

            // 2. 만약 물리 효과(떨어짐)를 확실하게 다시 주고 싶다면 (선택사항)
            Rigidbody rb = sceneRewardObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.WakeUp(); // 잠들어있던 물리 엔진 깨우기
            }
        }
        else
        {
            Debug.LogError("BodyPartManager: 씬에 배치된 보상 오브젝트(sceneRewardObject)가 연결되지 않았습니다!");
        }
    }
}