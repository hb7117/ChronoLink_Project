using UnityEngine;
using Photon.Pun;
using System.Collections; // 코루틴 사용

public class TimeObject : MonoBehaviourPun
{
    public string timeObjectID = "Unique_ID_01";
    public bool isPastObject = true;

    // OnEnable은 오브젝트가 활성화될 때마다 호출됩니다.
    void OnEnable()
    {
        // 미래 오브젝트만 등록 시도
        if (!isPastObject)
        {
            // GameManager가 바로 준비되지 않았을 수 있으므로,
            // 잠시 기다렸다가 등록을 시도하는 코루틴을 실행합니다.
            StartCoroutine(RegisterWithGameManager());
        }
    }

    IEnumerator RegisterWithGameManager()
    {
        // GameManager.Instance가 준비될 때까지 최대 1초 대기 (프레임마다 확인)
        float timer = 0f;
        while (GameManager.Instance == null && timer < 1.0f)
        {
            yield return null; // 다음 프레임까지 대기
            timer += Time.deltaTime;
        }

        // GameManager를 찾았으면 등록
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterTimeObject(timeObjectID, this);
            Debug.Log($"TimeObject '{timeObjectID}' attempting registration via OnEnable coroutine."); // 등록 시도 로그
        }
        else
        {
            Debug.LogError($"TimeObject '{timeObjectID}': GameManager.Instance not found after waiting!");
        }
    }

    // 오브젝트가 파괴될 때 GameManager에서 등록 해제 (선택 사항이지만 권장)
    // void OnDisable()
    // {
    //     if (!isPastObject && GameManager.Instance != null)
    //     {
    //         GameManager.Instance.UnregisterTimeObject(timeObjectID); // GameManager에 Unregister 함수 필요
    //     }
    // }
}