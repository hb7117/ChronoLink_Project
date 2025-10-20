using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Image icon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawSlot(ItemData itemdata)
    {
        icon.sprite = itemdata.itemIcon;
        icon.enabled=true;
    }
    public void ClearSlot()
    {
        icon.sprite = null;
        icon.enabled=false; 
    }
}
