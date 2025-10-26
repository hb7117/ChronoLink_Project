using UnityEngine;
using TMPro; // TextMeshPro를 사용한다면
using Photon.Pun;
// using UnityEngine.UI; // 일반 Text를 사용한다면

public class PlayerInteraction : MonoBehaviour
{
    // [Header("UI)")]
    // private GameObject interactUIObject;
    // private TextMeshProUGUI interactText; // 또는 public Text interactText;

    [Header("설정")]
    public float pickupRadius = 2f; // 이 값은 이제 사용되지 않으며, 콜라이더 크기로 대체됩니다.

    private Inventory inventory;
    private ItemObject currentInteractableItem;
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        inventory = GetComponent<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("PlayerInteraction: Inventory 스크립트를 찾을 수 없습니다!", this.gameObject);
        }

        /*
        interactUIObject = GameObject.FindGameObjectWithTag("InteractUI");
        if (interactUIObject != null)
        {
            interactText = interactUIObject.GetComponentInChildren<TextMeshProUGUI>();
            interactUIObject.SetActive(false); // UI를 숨긴 상태로 시작
            Debug.Log("PlayerInteraction: UI를 성공적으로 찾았습니다.");
        }
        else
        {
            Debug.LogWarning("PlayerInteraction: 'InteractUI' 태그를 가진 UI 오브젝트를 씬에서 찾을 수 없습니다. UI 기능이 작동하지 않습니다.", this.gameObject);
        }
        */
    }

    void Update()
    {
        if (currentInteractableItem != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E키 눌림! " + currentInteractableItem.name + " 획득 시도.");
            TryPickupItem();
        }

        if (inventory == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) { inventory.DropItem(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { inventory.DropItem(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { inventory.DropItem(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { inventory.DropItem(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { inventory.DropItem(4); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { inventory.DropItem(5); }
    }

    private void TryPickupItem()
    {
        if (currentInteractableItem == null) return;

        Debug.Log(currentInteractableItem.itemData.itemName + " 아이템을 인벤토리에 추가합니다.");

        inventory.AddItem(currentInteractableItem.itemData);

        PhotonView itemPhotonView = currentInteractableItem.GetComponent<PhotonView>();
        if (itemPhotonView != null)
        {
            PhotonNetwork.Destroy(currentInteractableItem.gameObject);
        }
        else
        {
            Destroy(currentInteractableItem.gameObject);
        }

        // HideInteractUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter 감지! 충돌한 오브젝트: " + other.name + ", 태그: " + other.tag);

        if (other.CompareTag("Item"))
        {
            Debug.Log("'Item' 태그 감지!");

            ItemObject item = other.GetComponent<ItemObject>(); 
            if (item != null)
            {
                Debug.Log("ItemObject 스크립트 찾음: " + item.itemData.itemName);
                currentInteractableItem = item;
                // ShowInteractUI();
            }
            else
            {
                Debug.LogWarning("'Item' 태그는 있지만 ItemObject 스크립트가 없습니다.", other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Debug.Log("OnTriggerExit 감지! " + other.name + " 범위 이탈");

            if (other.GetComponent<ItemObject>() == currentInteractableItem)
            {
                currentInteractableItem = null;
                // HideInteractUI();
            }
        }
    }

    /*
    private void ShowInteractUI()
    {
        if (interactUIObject == null || currentInteractableItem == null) return;

        // if (interactText != null)
        // {
        //     interactText.text = "E " + currentInteractableItem.itemData.itemName;
        // }
        
        interactUIObject.SetActive(true);
    }

    private void HideInteractUI()
    {
        if (interactUIObject != null)
        {
            interactUIObject.SetActive(false);
        }
    }
    */
}