using Cinemachine.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject targetPanel;
    private bool isInRange = false;

    void Start()
    {
         
        
      targetPanel.SetActive(false);
        
    }

    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.Q))
        {
            // 3. E키가 눌렸는지 확인
            TogglePanel();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 2. 플레이어가 범위에 들어왔는지 확인
            isInRange = true;
        }
       
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;

            if (targetPanel != null && targetPanel.activeSelf)
            {
                targetPanel.SetActive(false);
            }
        }
    }

    void TogglePanel()
    {
        if (targetPanel != null)
        {
            bool isActive = targetPanel.activeSelf;
            targetPanel.SetActive(!isActive);
        }
    }

}
