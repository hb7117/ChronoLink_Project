using UnityEngine;

// 💡 이 스크립트는 플레이어의 Inventory.cs와 같은 오브젝트에 있어야 합니다.
[RequireComponent(typeof(Inventory))]
public class PlayerInteract : MonoBehaviour
{
    [Header("상호작용 설정")]
    [SerializeField] private float interactDistance = 2.0f; // 상호작용 가능 거리
    [SerializeField] private LayerMask interactLayer;     // "Door" 레이어 등을 Inspector에서 설정
    [SerializeField] private KeyCode interactKey = KeyCode.F; // 상호작용 키

    private Inventory inventory;
    private Camera playerCamera; // 쿼터뷰 카메라 또는 메인 카메라

    void Start()
    {
        inventory = GetComponent<Inventory>(); // 내 인벤토리 가져오기
        playerCamera = Camera.main; // 메인 카메라를 사용
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        // 쿼터뷰에서는 화면 중앙 대신 마우스 클릭 위치로 Ray를 쏴야 할 수도 있습니다.
        // 우선은 플레이어 정면으로 쏘는 방식입니다.
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        // 💡 만약 플레이어 기준 정면을 원한다면:
        // Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            // 맞은 물체 또는 그 부모에서 Door 스크립트를 찾음
            Door door = hit.collider.GetComponentInParent<Door>();

            if (door != null)
            {
                // 문을 찾았으면, 내 인벤토리를 넘겨주며 문 열기 시도
                door.TryOpenDoor(inventory);
            }
        }
    }
}