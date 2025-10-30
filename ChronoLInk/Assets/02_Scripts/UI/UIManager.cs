using Cinemachine.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject targetPanel; // 활성화/비활성화할 UI 패널
    private bool isInRange = false; // 플레이어가 오브젝트 범위 안에 있는지

    void Start()
    {
        // 시작할 때 패널을 비활성화 상태로 강제합니다.
        if (targetPanel != null)
        {
            targetPanel.SetActive(false);
        }
    }

    void Update()
    {
        // 플레이어가 범위 안에 있고, E 키를 눌렀을 때
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            TogglePanel();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // ⭐️ 중요: "Player" 태그를 가진 오브젝트만 감지
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // ⭐️ 중요: "Player" 태그를 가진 오브젝트만 감지
        if (other.CompareTag("Player"))
        {
            isInRange = false;

            // 플레이어가 범위를 벗어나면 패널을 강제로 닫습니다.
            if (targetPanel != null && targetPanel.activeSelf)
            {
                targetPanel.SetActive(false);
            }
        }
    }

    // 패널을 켜고 끄는 함수
    void TogglePanel()
    {
        if (targetPanel != null)
        {
            // 현재 패널의 활성화 상태를 반전시킵니다.
            bool isActive = targetPanel.activeSelf;
            targetPanel.SetActive(!isActive);
        }
    }

}
