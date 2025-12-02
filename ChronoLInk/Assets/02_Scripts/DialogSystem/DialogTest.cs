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
        if (dialogSystem.currentDialogIndex != -1) yield break;

        dialogSystem.StartDialog(dialogIndex, endDialogIndex, isGlobal);

        yield return new WaitUntil(() => dialogSystem.currentDialogIndex == -1);

    }
}