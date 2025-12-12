using UnityEngine;

public class BodyPartSlot : MonoBehaviour
{
    [Header("설정")]
    public string requiredTag;  
    public BodyPartManager manager;  

    [HideInInspector] public bool isCorrectItemPlaced = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(requiredTag))
        {
            Debug.Log($"[Slot] {requiredTag} 아이템이 배치되었습니다!");
            isCorrectItemPlaced = true;

            // 매니저에게 "나 채워졌어!"라고 보고
            if (manager != null) manager.CheckPuzzleCompletion();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(requiredTag))
        {
            Debug.Log($"[Slot] {requiredTag} 아이템이 제거되었습니다.");
            isCorrectItemPlaced = false;
        }
    }
}