using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeypadPuzzle : MonoBehaviour
{
    public string correctPassword = "1016";
    private string currentInput = "";

    public Text displayText;

    public Image feedbackSuccessImage;
    public Image feedbackFailImage;

    // 5번: 문 오브젝트들
    // [수정] public GameObject doorToOpen; -> 2개의 변수로 변경
    public GameObject doorToOpen_Present; // 1. 첫 번째 문 (현재)
    public GameObject doorToOpen_Future;  // 2. 두 번째 문 (미래)

    public float openSpeed = 1.0f;
    public float openDistance = 3.0f;

    private bool isSolved = false;

    void Start()
    {
        if (feedbackSuccessImage != null) feedbackSuccessImage.gameObject.SetActive(false);
        if (feedbackFailImage != null) feedbackFailImage.gameObject.SetActive(false);
        if (displayText != null) displayText.text = "";
    }

    void OnEnable()
    {
        if (!isSolved)
        {
            ClearInput();
            if (feedbackSuccessImage != null) feedbackSuccessImage.gameObject.SetActive(false);
            if (feedbackFailImage != null) feedbackFailImage.gameObject.SetActive(false);
        }
    }

    public void OnNumberClick(string number)
    {
        if (isSolved) return;
        if (currentInput.Length >= correctPassword.Length) return;

        if (feedbackFailImage != null) feedbackFailImage.gameObject.SetActive(false);

        currentInput += number;
        if (displayText != null) displayText.text = currentInput;
    }

    public void OnEnterClick()
    {
        if (isSolved) return;

        if (currentInput == correctPassword)
        {
            Debug.Log("비밀번호 정답!");
            if (feedbackSuccessImage != null) feedbackSuccessImage.gameObject.SetActive(true);
            if (feedbackFailImage != null) feedbackFailImage.gameObject.SetActive(false);

            isSolved = true;

            // [수정] 함수 이름 변경
            OpenBothDoors();
        }
        else
        {
            Debug.Log("비밀번호 틀림!");
            if (feedbackFailImage != null) feedbackFailImage.gameObject.SetActive(true);
            StartCoroutine(ClearInputAfterDelay(1.0f));
        }
    }

    public void OnClearClick()
    {
        if (isSolved) return;

        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            if (displayText != null) displayText.text = currentInput;
            if (feedbackFailImage != null) feedbackFailImage.gameObject.SetActive(false);
        }
    }

    void ClearInput()
    {
        currentInput = "";
        if (displayText != null) displayText.text = "";
        if (feedbackFailImage != null) feedbackFailImage.gameObject.SetActive(false);
        if (feedbackSuccessImage != null) feedbackSuccessImage.gameObject.SetActive(false);
    }

    IEnumerator ClearInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClearInput();
    }

    // [수정] OpenTheDoor() -> OpenBothDoors()로 함수 이름 변경 및 로직 수정
    void OpenBothDoors()
    {
        bool doorOpened = false; // 문이 하나라도 열렸는지 확인

        // 현재 문 열기 (null이 아니면)
        if (doorToOpen_Present != null)
        {
            Debug.Log("현재 문을 엽니다!");
            StartCoroutine(MoveDoorLeft(doorToOpen_Present)); // 수정된 MoveDoorLeft 호출
            doorOpened = true;
        }

        // 미래 문 열기 (null이 아니면)
        if (doorToOpen_Future != null)
        {
            Debug.Log("미래 문을 엽니다!");
            StartCoroutine(MoveDoorLeft(doorToOpen_Future)); // 수정된 MoveDoorLeft 호출
            doorOpened = true;
        }

        // 문이 하나라도 열렸으면 2초 뒤 패널 닫기 예약
        if (doorOpened)
        {
            StartCoroutine(ClosePanelAfterDelay(2.0f));
        }
    }

    // [수정] MoveDoorLeft가 매개변수(parameter)를 받도록 수정
    IEnumerator MoveDoorLeft(GameObject door) // 어떤 문(door)을 열지 매개변수로 받음
    {
        Vector3 originalPosition = door.transform.position;
        Vector3 targetPosition = originalPosition + (Vector3.left * openDistance);
        float elapsedTime = 0;

        while (elapsedTime < openSpeed)
        {
            // [수정] doorToOpen -> door (매개변수로 받은 문을 움직임)
            door.transform.position = Vector3.Lerp(originalPosition, targetPosition, (elapsedTime / openSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        door.transform.position = targetPosition; // 정확한 위치 보정
    }

    IEnumerator ClosePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        this.gameObject.SetActive(false); // KeypadPanel 끄기
    }
}