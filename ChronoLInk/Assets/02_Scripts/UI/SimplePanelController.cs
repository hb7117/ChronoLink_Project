using UnityEngine;

public class SimplePanelController : MonoBehaviour
{
    // 1. 기존 변수 이름을 'lockPanel'로 변경 (이게 자물쇠 패널)
    public GameObject lockPanel;

    // 2. (추가) 성공 시 띄울 패널 변수
    public GameObject successPanel;

    // 3. (추가) 퍼즐 해결 여부 변수
    public bool isPuzzleSolved = false;

    private bool isInRange = false;

    void Start()
    {
        // 시작 시 두 패널 모두 끔
        if (lockPanel != null) lockPanel.SetActive(false);
        if (successPanel != null) successPanel.SetActive(false);
    }

    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            TogglePanel();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            // 범위를 벗어나면 모든 패널을 닫음
            if (lockPanel != null) lockPanel.SetActive(false);
            if (successPanel != null) successPanel.SetActive(false);
        }
    }

    // 4. (수정) TogglePanel 함수 로직 변경
    void TogglePanel()
    {
        // 1. 현재 활성화된 패널이 있는지 확인 (둘 중 하나라도 켜져있는가?)
        bool anyPanelActive = (lockPanel != null && lockPanel.activeSelf) ||
                              (successPanel != null && successPanel.activeSelf);

        if (anyPanelActive)
        {
            // 2. 패널이 켜져있다면 -> 끈다 (E키는 토글이니까)
            if (lockPanel != null) lockPanel.SetActive(false);
            if (successPanel != null) successPanel.SetActive(false);
        }
        else
        {
            // 3. 패널이 꺼져있다면 -> 켠다
            if (isPuzzleSolved)
            {
                // 3-1. 퍼즐이 풀렸으면: 성공 패널 켜기
                if (successPanel != null) successPanel.SetActive(true);
            }
            else
            {
                // 3-2. 퍼즐이 안 풀렸으면: 자물쇠 패널 켜기
                if (lockPanel != null) lockPanel.SetActive(true);
            }
        }
    }
}