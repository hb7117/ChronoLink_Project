using UnityEngine;
using Photon.Pun;
using System.Linq;

public class PlayerInteraction : MonoBehaviour
{
    
    public float pickupRadius = 2f;
    public LayerMask itemLayer;
    public KeyCode interactKey = KeyCode.E;

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
    }

  

    void Update()
    {
        if (!photonView.IsMine) return;

        CheckForInteractableItems();

        if (currentInteractableItem != null && Input.GetKeyDown(interactKey))
        {
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
}

