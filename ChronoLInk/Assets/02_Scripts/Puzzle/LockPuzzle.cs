using UnityEngine;
using UnityEngine.UI; // UI 사용을 위해 필수

public class LockPuzzle : MonoBehaviour
{
    // 1. 상호작용 오브젝트에 붙어있는 SimplePanelController
    private SimplePanelController panelController;

    // 2. UI 요소들
    public Text[] numberTexts;   // 3개의 숫자 텍스트
    public Button[] upButtons;   // 3개의 Up 버튼
    public Button[] downButtons; // 3개의 Down 버튼
    public Button confirmButton; // 확인 버튼

    // 3. 정답 설정
    private int[] currentNumbers = { 0, 0, 0 };
    private int[] correctCombination = { 8, 8, 8 }; // ⭐️ 정답을 여기에서 수정하세요 (예: 101)

    void Start()
    {
        // 각 버튼에 클릭 이벤트 리스너(기능) 추가
        for (int i = 0; i < numberTexts.Length; i++)
        {
            int index = i; // 클로저 문제 해결
            upButtons[index].onClick.AddListener(() => ChangeNumber(index, 1));
            downButtons[index].onClick.AddListener(() => ChangeNumber(index, -1));
        }

        // 확인 버튼 리스너
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(CheckCombination);
        }

        UpdateDisplay(); // 초기 숫자 표시
    }

    // 숫자 변경 함수
    void ChangeNumber(int index, int amount)
    {
        currentNumbers[index] = (currentNumbers[index] + amount + 10) % 10; // 0-9 순환
        UpdateDisplay();
    }

    // UI 텍스트 업데이트
    void UpdateDisplay()
    {
        for (int i = 0; i < numberTexts.Length; i++)
        {
            numberTexts[i].text = currentNumbers[i].ToString();
        }
    }

    // 정답 확인 함수
    void CheckCombination()
    {
        bool isCorrect = true;
        for (int i = 0; i < numberTexts.Length; i++)
        {
            if (currentNumbers[i] != correctCombination[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            Debug.Log("자물쇠 잠금 해제!");
            SolvePuzzle();
        }
        else
        {
            Debug.Log("비밀번호가 틀렸습니다!");
            // (선택) 틀렸을 때 시각적 피드백 (예: 텍스트 빨간색으로)
        }
    }

    // 퍼즐 해결 처리
    void SolvePuzzle()
    {
        if (panelController != null)
        {
            // 1. SimplePanelController에 퍼즐이 풀렸다고 알림
            panelController.isPuzzleSolved = true;

            // 2. 현재 자물쇠 패널 즉시 끄기
            this.gameObject.SetActive(false); // this.gameObject는 LockPanel임

            // 3. 성공 패널 즉시 켜기
            if (panelController.successPanel != null)
            {
                panelController.successPanel.SetActive(true);
            }
        }
    }
}