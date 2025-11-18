using UnityEngine;
using UnityEngine.UI;  

public class LockPuzzle : MonoBehaviour
{
     
    public SimplePanelController panelController;

    
    public Text[] numberTexts;    
    public Button[] upButtons;    
    public Button[] downButtons;  
    public Button confirmButton;  

    
    private int[] currentNumbers = { 0, 0, 0 };
    private int[] correctCombination = { 8, 8, 8 }; 

    void Start()
    {
         
        for (int i = 0; i < numberTexts.Length; i++)
        {
            int index = i;  
            upButtons[index].onClick.AddListener(() => ChangeNumber(index, 1));
            downButtons[index].onClick.AddListener(() => ChangeNumber(index, -1));
        }

         
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(CheckCombination);
        }

        UpdateDisplay();  
    }

    // 숫자 변경 함수
    void ChangeNumber(int index, int amount)
    {
        currentNumbers[index] = (currentNumbers[index] + amount + 10) % 10;  
        UpdateDisplay();
    }

     
    void UpdateDisplay()
    {
        for (int i = 0; i < numberTexts.Length; i++)
        {
            numberTexts[i].text = currentNumbers[i].ToString();
        }
    }

     
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
           
            SolvePuzzle();
        }
        else
        {
            Debug.Log("비밀번호가 틀렸습니다!");
             
        }
    }

    
    void SolvePuzzle()
    {
        if (panelController != null)
        {
            
            panelController.isPuzzleSolved = true;

           
            this.gameObject.SetActive(false); 

            
            if (panelController.successPanel != null)
            {
                panelController.successPanel.SetActive(true);
            }
        }
    }
}