using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class InteractionTextTrigger : MonoBehaviour
{
    public bool useQ_Zoom = false;
    public bool useE_Pickup = false;
    public bool useF_Interact = false;

    private static Text screenTextComponent;
    private static GameObject screenTextObject;

    void Start()
    {
        if (screenTextComponent == null)
        {
            GameObject foundObj = GameObject.Find("TriggerTeXt");

            if (foundObj != null)
            {
                screenTextObject = foundObj;
                screenTextComponent = foundObj.GetComponent<Text>();
                foundObj.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                ShowText();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                HideText();
            }
        }
    }

    void ShowText()
    {
        if (screenTextComponent == null) return;

        string finalMessage = "";

        // 줄바꿈 대신 띄어쓰기 3칸("   ")으로 구분
        if (useQ_Zoom)
        {
            finalMessage += "[ Q : 아이템 확대하기 ]   ";
        }

        if (useE_Pickup)
        {
            finalMessage += "[ E : 아이템 획득하기 ]   ";
        }

        if (useF_Interact)
        {
            finalMessage += "[ F : 상호작용하기 ]   ";
        }

        screenTextComponent.text = finalMessage;
        screenTextObject.SetActive(true);
    }

    void HideText()
    {
        if (screenTextObject != null)
        {
            screenTextObject.SetActive(false);
        }
    }
}