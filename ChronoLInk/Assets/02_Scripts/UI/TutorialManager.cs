using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialCanvas;
    public GameObject backButton;
    public List<GameObject> tutorialPages;

    private int currentIndex = 0;

    void Start()
    {
        OpenTutorial();
    }

    public void OnClickOpenTutorial()
    {
        if (!tutorialCanvas.activeSelf)
        {
            OpenTutorial();
        }
    }

    private void OpenTutorial()
    {
        currentIndex = 0;
        UpdateUI();
    }

    public void OnClickNext()
    {
        if (currentIndex < tutorialPages.Count - 1)
        {
            currentIndex++;
            UpdateUI();
        }
        else
        {
            EndTutorial();
        }
    }

    public void OnClickBack()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (tutorialCanvas != null) tutorialCanvas.SetActive(true);

        for (int i = 0; i < tutorialPages.Count; i++)
        {
            if (i == currentIndex)
            {
                tutorialPages[i].SetActive(true);
            }
            else
            {
                tutorialPages[i].SetActive(false);
            }
        }

        if (backButton != null)
        {
            backButton.SetActive(currentIndex > 0);
        }
    }

    public void EndTutorial()
    {
        if (tutorialCanvas != null) tutorialCanvas.SetActive(false);
    }
}