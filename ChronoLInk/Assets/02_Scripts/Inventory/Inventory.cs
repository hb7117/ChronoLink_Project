using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [Header("UI 설정")]
    public GameObject inventoryPanel;         
    public List<Slot> inventorySlots;        

    [Header("아이템 버리기")]
    public Transform dropPoint;               

    private List<ItemData> heldItems = new List<ItemData>(); 

    void Start()
    {
        
        if (inventoryPanel == null)
        {      
            inventoryPanel = GameObject.Find("InventoryPanel");
        }
        inventoryPanel.SetActive(false); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }

   
    public void AddItem(ItemData itemData)
    {
        if (heldItems.Count >= inventorySlots.Count)
        {
            Debug.Log("인벤토리가 꽉 찼습니다.");
            return;
        }
        heldItems.Add(itemData);
        UpdateInventoryUI();
    }

    
    public void DropItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= heldItems.Count) return;

        ItemData itemToDrop = heldItems[slotIndex];
        Instantiate(itemToDrop.itemPrefab, dropPoint.position, dropPoint.rotation);
        heldItems.RemoveAt(slotIndex);
        UpdateInventoryUI();
    }

    
    private void UpdateInventoryUI()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < heldItems.Count)
            {
                // 구닥다리 방식
                //inventorySlots[i].sprite = heldItems[i].itemIcon;
                //inventorySlots[i].enabled = true;

                // 새롭게 
                inventorySlots[i].DrawSlot(heldItems[i]);

            }
            else
            {
                inventorySlots[i].ClearSlot();
            }
        }
    }
}