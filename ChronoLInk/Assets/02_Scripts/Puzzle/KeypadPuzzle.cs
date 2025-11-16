using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;  

public class KeypadPuzzle : MonoBehaviour
{
    public string correctPassword = "1016";
    private string currentInput = "";
    public Text displayText;

    public Image feedbackSuccessImage;
    public Image feedbackFailImage;

    public DoorController pastDoor;  
    public string timeObjectID;  

    private bool isSolved = false;

    void Start()
    {
        if (feedbackSuccessImage != null) feedbackSuccessImage.gameObject.SetActive(false);
        if (feedbackFailImage != null) feedbackFailImage.gameObject.SetActive(false);
        if (displayText != null) displayText.text = "";
    }

    void OnEnable()
    {
        if (isSolved)
        {
            if (feedbackSuccessImage != null) feedbackSuccessImage.gameObject.SetActive(true);  
            if (feedbackFailImage != null) feedbackFailImage.gameObject.SetActive(false);
            return;
        }

        ClearInput();
        if (feedbackSuccessImage != null) feedbackSuccessImage.gameObject.SetActive(false);
        if (feedbackFailImage != null) feedbackFailImage.gameObject.SetActive(false);
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
            if (feedbackSuccessImage != null) feedbackSuccessImage.gameObject.SetActive(true);
            if (feedbackFailImage != null) feedbackFailImage.gameObject.SetActive(false);

            isSolved = true;

            OpenSyncedDoors();

            StartCoroutine(ClosePanelAfterDelay(2.0f));
        }
        else
        {
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

    void OpenSyncedDoors()
    {
        if (pastDoor != null)
        {
            Debug.Log("과거 문 옾픈");
            pastDoor.OpenDoor();
        }

        if (!string.IsNullOrEmpty(timeObjectID) && GameManager.Instance != null)
        {
            GameManager.Instance.SyncDoorOpen(timeObjectID);
        }
         
        
    }


    IEnumerator ClosePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        this.gameObject.SetActive(false);  
    }
}