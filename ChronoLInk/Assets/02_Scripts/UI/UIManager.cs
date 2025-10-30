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
        // 1. 스크립트가 시작되었는지, 타겟 패널이 할당되었는지 확인
        if (targetPanel == null)
        {
            Debug.LogError("!!!!!! [ " + this.name + " ] 스크립트의 Target Panel이 비어있습니다! Inspector에서 할당해주세요! !!!!!!");
        }
        else
        {
            Debug.Log("[ " + this.name + " ] 스크립트 시작. 타겟 패널: " + targetPanel.name);
            targetPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            // 3. E키가 눌렸는지 확인
            Debug.Log(">>> E키 입력 감지! TogglePanel() 호출합니다.");
            TogglePanel();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 2. 플레이어가 범위에 들어왔는지 확인
            isInRange = true;
            Debug.Log(">>> 'Player' 태그 감지! 범위 안에 들어왔습니다. (isInRange = true)");
        }
        else
        {
            // (보너스) 플레이어가 아닌 다른 것이 닿았는지 확인
            Debug.Log("...무언가 닿았지만 태그가 'Player'가 아닙니다. 닿은 오브젝트: " + other.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 4. 플레이어가 범위를 나갔는지 확인
            isInRange = false;
            Debug.Log("<<< 'Player'가 범위를 벗어났습니다. (isInRange = false)");

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
            Debug.Log(">>> 패널 활성화 상태 변경! 현재 상태: " + targetPanel.activeSelf);
        }
    }

}
