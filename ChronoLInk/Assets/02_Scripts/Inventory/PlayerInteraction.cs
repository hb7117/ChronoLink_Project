using UnityEngine;
using Photon.Pun;
using System.Linq;

// 💡 님이 가지고 계신 PlayerInteraction 스크립트입니다.
[RequireComponent(typeof(Inventory))] // 💡 [추가] 인벤토리가 필수임을 명시
public class PlayerInteraction : MonoBehaviour
{
    [Header("아이템 줍기 설정 (E 키)")] // 💡 이름 변경
    public float pickupRadius = 2f;
    public LayerMask itemLayer;
    public KeyCode pickupKey = KeyCode.E; // 💡 이름 변경

    // --- [여기에 '문' 관련 변수들 추가] ---
    [Header("문 상호작용 설정 (F 키)")]
    [SerializeField] private float doorInteractDistance = 3.0f; // 문 상호작용 거리
    [SerializeField] private LayerMask doorLayer;             // "Door" 레이어 설정용
    [SerializeField] private KeyCode doorInteractKey = KeyCode.F;   // 문 상호작용 키
    private Camera playerCamera; // 💡 Raycast를 쏘기 위한 카메라
    // --- [여기까지 추가] ---

    private Inventory inventory;
    private PhotonView photonView;
    private ItemObject currentInteractableItem;

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

        // 💡 [추가] 카메라 정보 가져오기
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // --- 1. 아이템 줍기 (기존 E키 로직) ---
        CheckForInteractableItems(); // E키로 주울 아이템이 있는지 확인

        if (currentInteractableItem != null && Input.GetKeyDown(pickupKey)) // 'pickupKey' (E)
        {
            TryPickupItem();
        }

        // --- 2. 문 상호작용 (새로운 F키 로직) ---
        if (Input.GetKeyDown(doorInteractKey)) // 'doorInteractKey' (F)
        {
            TryInteractWithDoor(); // 💡 새로 추가된 함수 호출
        }

        // --- 3. 아이템 버리기 (기존 1~6키 로직) ---
        if (inventory == null) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) { inventory.DropItem(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { inventory.DropItem(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { inventory.DropItem(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { inventory.DropItem(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { inventory.DropItem(4); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { inventory.DropItem(5); }
    }

    // --- [기존 아이템 줍기 함수들] ---
    private void CheckForInteractableItems()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRadius, itemLayer);

        ItemObject closestItem = null;
        float closestDistance = float.MaxValue;

        if (hitColliders.Length > 0)
        {
            foreach (var hit in hitColliders)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    ItemObject item = hit.GetComponent<ItemObject>();
                    if (item != null)
                    {
                        closestDistance = distance;
                        closestItem = item;
                    }
                }
            }
        }
        currentInteractableItem = closestItem;
    }

    private void TryPickupItem()
    {
        if (currentInteractableItem == null) return;

        PhotonView itemPhotonView = currentInteractableItem.GetComponent<PhotonView>();

        inventory.AddItem(currentInteractableItem.itemData);

        if (itemPhotonView != null)
        {
            if (itemPhotonView.IsMine || PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(currentInteractableItem.gameObject);
            }
            else
            {
                photonView.RPC("RPC_RequestDestroyItem", RpcTarget.MasterClient, itemPhotonView.ViewID);
            }
        }
        else
        {
            Destroy(currentInteractableItem.gameObject);
        }
        currentInteractableItem = null;
    }

    [PunRPC]
    void RPC_RequestDestroyItem(int itemPhotonViewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        PhotonView itemPV = PhotonView.Find(itemPhotonViewID);
        if (itemPV != null)
        {
            PhotonNetwork.Destroy(itemPV.gameObject);
        }
    }

    // --- [💡 여기에 '문 상호작용' 함수 추가] ---
    void TryInteractWithDoor()
    {
        // 💡 [수정] 카메라 위치가 아닌, 플레이어 위치에서 플레이어 정면으로 쏘도록 변경
        // 1. 레이저의 시작점: 플레이어 위치 (y값을 0.5f 정도 올려서 허리 높이에서 쏘게 함)
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        // 2. 레이저의 방향: 플레이어의 정면 (transform.forward)
        Ray ray = new Ray(rayStart, transform.forward);

        RaycastHit hit;

        // [디버깅] 씬(Scene) 뷰에서 3초간 빨간색 광선이 보이게 함 (매우 유용!)
        Debug.DrawRay(ray.origin, ray.direction * doorInteractDistance, Color.red, 3.0f);

        if (Physics.Raycast(ray, out hit, doorInteractDistance, doorLayer))
        {
            // [디버깅] 광선에 맞은 물체의 이름을 콘솔에 출력
            Debug.Log("Raycast Hit: " + hit.collider.gameObject.name);

            // 맞은 물체 또는 그 부모에서 Door 스크립트를 찾음
            Door door = hit.collider.GetComponentInParent<Door>();

            if (door != null)
            {
                // 문을 찾았으면, 내 인벤토리를 넘겨주며 문 열기 시도
                door.TryOpenDoor(inventory);
            }
        }
        else
        {
            // [디버깅] 아무것도 맞지 않았을 때 로그
            Debug.Log("Raycast Hit Nothing on Door Layer.");
        }
    }
}