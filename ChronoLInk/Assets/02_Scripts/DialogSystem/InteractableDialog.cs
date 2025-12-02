using UnityEngine;
using Photon.Pun;

public class InteractableDialog : MonoBehaviourPun
{
    [Header("Dialog Settings")]
    public int startDialogIndex;
    public int endDialogIndex;
    public bool isGlobalDialog = false; // 체크하면 모두가 GlobalPanel, 끄면 각자 역할에 맞는 Panel

    private bool isPlayerInRange = false;
    private DialogSystem dialogSystem;

    private void Start()
    {
        dialogSystem = FindObjectOfType<DialogSystem>();
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (dialogSystem != null)
            {
                if (isGlobalDialog)
                {
                    // 1. 글로벌 대화: RPC로 모두에게 알림 -> 모두 GlobalPanel이 열림
                    photonView.RPC("RPC_StartDialog", RpcTarget.All, startDialogIndex, endDialogIndex, true);
                }
                else
                {
                    // 2. 개인 대화: 나만 실행 -> 내 역할(Past/Future)에 따라 PastPanel/FuturePanel 열림
                    // RPC를 쏘지 않고 직접 호출합니다.
                    dialogSystem.StartDialog(startDialogIndex, endDialogIndex, false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PhotonView otherPv = other.GetComponent<PhotonView>();
        // 태그와 IsMine 확인
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

    // 글로벌일 때만 호출되는 RPC
    [PunRPC]
    public void RPC_StartDialog(int startIndex, int endIndex, bool isGlobal)
    {
        if (dialogSystem != null)
        {
            dialogSystem.StartDialog(startIndex, endIndex, isGlobal);
        }
    }
}