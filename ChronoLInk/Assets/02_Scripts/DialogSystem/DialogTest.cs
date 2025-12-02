using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTest : MonoBehaviour
{
    [SerializeField]
    private DialogSystem dialogSystem;
    private int dialogIndex;
    private int endDialogIndex;
    private bool isGlobal;

    //private IEnumerator Start()
    //{
    //     
    //    dialogSystem.StartDialog(dialogIndex, endDialogIndex,isGlobal);
    //
    //   
    //    yield return new WaitUntil(() => dialogSystem.currentDialogIndex == -1);
    //
    //   
    //  
    //}
    IEnumerator RunDialogTest()
    {
        // 이미 대화 중이면 중복 실행 방지 (선택 사항)
        if (dialogSystem.currentDialogIndex != -1) yield break;

        dialogSystem.StartDialog(dialogIndex, endDialogIndex, isGlobal);

        yield return new WaitUntil(() => dialogSystem.currentDialogIndex == -1);

        Debug.Log("테스트 대화가 종료되었습니다.");
    }
}