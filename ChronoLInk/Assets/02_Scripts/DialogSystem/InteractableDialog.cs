using UnityEngine;
using Photon.Pun;

public class InteractableDialog : MonoBehaviourPun
{
    public int startDialogIndex;
    public int endDialogIndex;
    public bool isGlobalDialog = false;
    public bool playOnStart = false;

    private bool isPlayerInRange = false;
    private bool hasTriggered = false;

    private DialogSystem dialogSystem;

    private void Start()
    {
        dialogSystem = FindObjectOfType<DialogSystem>();

        if (playOnStart)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPC_StartDialog", RpcTarget.All, startDialogIndex, endDialogIndex, true);
            }
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (dialogSystem != null)
            {
                if (isGlobalDialog)
                {
                    if (!hasTriggered)
                    {
                        photonView.RPC("RPC_StartDialog", RpcTarget.All, startDialogIndex, endDialogIndex, true);
                    }
                }
                else
                {
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
        hasTriggered = true;

        if (dialogSystem != null)
        {
            dialogSystem.StartDialog(startIndex, endIndex, isGlobal);
        }
    }
}