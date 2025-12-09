using Cinemachine.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;  

public class UIManager : MonoBehaviour
{
    public GameObject targetPanel;
    private bool isInRange = false;

    void Start()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(false);
        }
    }

    void Update()
    {
         
        if (isInRange && Input.GetKeyDown(KeyCode.Q))
        {
            TogglePanel();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();

            if (pv != null && pv.IsMine)
            {
                isInRange = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();

            if (pv != null && pv.IsMine)
            {
                isInRange = false;

                if (targetPanel != null && targetPanel.activeSelf)
                {
                    targetPanel.SetActive(false);
                }
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