using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("문 설정")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private string requiredKeyID = "Key_Bedroom";

    [Header("애니메이션 설정")]
    [SerializeField] private float openAngle = -90.0f; // 앞으로 열리도록 -90
    [SerializeField] private float openSpeed = 2.0f;

    private bool isOpen = false;
    private bool isOpening = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    // PlayerInteraction 스크립트가 이 함수를 호출합니다.
    public void TryOpenDoor(Inventory playerInventory)
    {
        if (isOpen || isOpening) return;

        if (isLocked)
        {
            // 잠긴 문
            if (playerInventory != null && playerInventory.HasItem(requiredKeyID))
            {
                // 열쇠 있음 -> 열쇠 소모 및 문 열기
                playerInventory.RemoveItem(requiredKeyID);
                isLocked = false;
                StartCoroutine(OpenDoorCoroutine());
            }
            else
            {
                // 열쇠 없음
                Debug.Log("문이 잠겨있다... " + requiredKeyID + "가 필요해.");
            }
        }
        else
        {
            // 잠기지 않은 문
            Debug.Log("문을 엽니다.");
            StartCoroutine(OpenDoorCoroutine());
        }
    }

    // 문 여는 애니메이션 (코루틴)
    private IEnumerator OpenDoorCoroutine()
    {
        isOpening = true;
        float time = 0;
        Quaternion startRotation = transform.rotation;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, openRotation, time);
            time += Time.deltaTime * openSpeed;
            yield return null;
        }

        transform.rotation = openRotation;
        isOpen = true;
        isOpening = false;
    }
}