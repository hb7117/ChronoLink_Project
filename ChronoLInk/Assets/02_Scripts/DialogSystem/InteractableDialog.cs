using UnityEngine;
using Photon.Pun;

public class InteractableDialog : MonoBehaviourPun
{
    [Header("Dialog Settings")]
    public int startDialogIndex;
    public int endDialogIndex;
    public bool isGlobalDialog = false;

    private bool isPlayerInRange = false;
    private bool hasTriggered = false; 

    private DialogSystem dialogSystem;

    private void Start()
    {
        dialogSystem = FindObjectOfType<DialogSystem>();
    }

    private void Update()
    {
        // [수정] hasTriggered가 false일 때만(아직 대화 안 했을 때만) 입력 받음
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && !hasTriggered)
        {
            if (dialogSystem != null)
            {
                if (isGlobalDialog)
                {
                    // 글로벌이면 RPC를 쏴서 "모든 사람"의 hasTriggered를 true로 만듦
                    photonView.RPC("RPC_StartDialog", RpcTarget.All, startDialogIndex, endDialogIndex, true);
                }
                else
                {
                    // 로컬이면 "나"만 hasTriggered를 true로 바꾸고 실행
                    hasTriggered = true;
                    dialogSystem.StartDialog(startDialogIndex, endDialogIndex, false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PhotonView otherPv = other.GetComponent<PhotonView>();
        if (other.CompareTag("Player") && otherPv != null && otherPv.IsMine)
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PhotonView otherPv = other.GetComponent<PhotonView>();
        if (other.CompareTag("Player") && otherPv != null && otherPv.IsMine)
        {
            isPlayerInRange = false;
        }
    }

    [PunRPC]
    public void RPC_StartDialog(int startIndex, int endIndex, bool isGlobal)
    {
        // [추가] RPC가 실행되었다는 건 누군가 대화를 시작했다는 뜻이므로
        // 모든 클라이언트에서 이 변수를 true로 바꿔서 중복 실행을 막음
        hasTriggered = true;

        if (dialogSystem != null)
        {
            dialogSystem.StartDialog(startIndex, endIndex, isGlobal);
        }
    }
}