using UnityEngine;
using UnityEngine.EventSystems; 
using Photon.Pun;

public class PlayerInteraction : MonoBehaviour
{
    public float pickupDistance = 100f; 
    private Inventory inventory;
    private Camera playerCamera;
    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        inventory = GetComponent<Inventory>();
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryPickupItem();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { inventory.DropItem(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { inventory.DropItem(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { inventory.DropItem(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { inventory.DropItem(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { inventory.DropItem(4); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { inventory.DropItem(5); }
    }

    private void TryPickupItem()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            if (hit.collider.CompareTag("Item"))
            {
                ItemObject itemObject = hit.collider.GetComponent<ItemObject>();
                if (itemObject != null)
                {
                    inventory.AddItem(itemObject.itemData);
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }
}