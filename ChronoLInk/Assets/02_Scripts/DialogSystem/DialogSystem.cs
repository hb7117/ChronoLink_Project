using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Photon.Pun; // Photon 기능 사용

public class DialogSystem : MonoBehaviour
{
    [Header("패널 설정 (3개)")]
    public GameObject pastPanel;     
    public GameObject futurePanel;   
    public GameObject globalPanel;   

    public Image BackGround;

    [SerializeField] private SpeakerUI[] speakers;
    [SerializeField] private DialogData[] dialogs;

    [SerializeField] private bool DialogInit = true;
    [SerializeField] private bool dialogsDB = false;

    public int currentDialogIndex = -1;
    public int currentSpeakerIndex = 0;
    public float typingSpeed = 0.1f;
    public bool isTypingEffect = false;

    private int targetEndIndex = -100;
    public Entity_Dialogue entity_Dialogue;

    private void Awake()
    {
        CloseAllPanels();
        SetAllClose();
        if (dialogsDB)
        {
            LoadDialogsFromDB();
        }
    }

    private void CloseAllPanels()
    {
        if (pastPanel != null) pastPanel.SetActive(false);
        if (futurePanel != null) futurePanel.SetActive(false);
        if (globalPanel != null) globalPanel.SetActive(false);
    }

    private void OpenCorrectPanel(bool isGlobal)
    {
        CloseAllPanels();  

        if (isGlobal)
        {
            if (globalPanel != null) globalPanel.SetActive(true);
        }
        else
        {
            object characterValue;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("character", out characterValue))
            {
                string myRole = (string)characterValue;  

                if (myRole == "Past")
                {
                    if (pastPanel != null) pastPanel.SetActive(true);
                }
                else if (myRole == "Future")
                {
                    if (futurePanel != null) futurePanel.SetActive(true);
                }
            }
            else
            {
                if (pastPanel != null) pastPanel.SetActive(true);
            }
        }
    }

 
    private void LoadDialogsFromDB()
    {
        Array.Clear(dialogs, 0, dialogs.Length);
        if (entity_Dialogue.sheets.Count > 0)
        {
            Array.Resize(ref dialogs, entity_Dialogue.sheets[0].list.Count);
            int ArrayCursor = 0;
            foreach (Entity_Dialogue.Param param in entity_Dialogue.sheets[0].list)
            {
                dialogs[ArrayCursor].index = param.index;
                dialogs[ArrayCursor].speakerUIindex = param.speakerUIindex;
                dialogs[ArrayCursor].name = param.name;
                dialogs[ArrayCursor].dialogue = param.dialogue;
                dialogs[ArrayCursor].characterPath = param.characterPath;
                dialogs[ArrayCursor].backGroundPath = param.BackGroundPath;
                dialogs[ArrayCursor].tweenType = param.tweenType;
                dialogs[ArrayCursor].nextindex = param.nextindex;
                dialogs[ArrayCursor].nextScene = param.nextScene;
                ArrayCursor += 1;
            }
        }
    }

    private void SetActiveObjects(SpeakerUI speaker, bool visible)
    {
        speaker.imageDialog.gameObject.SetActive(visible);
        speaker.textName.gameObject.SetActive(visible);
        speaker.textDialogue.gameObject.SetActive(visible);
        speaker.objectArrow.SetActive(false);

        if (speaker.imgCharacter != null)
        {
            speaker.imgCharacter.gameObject.SetActive(visible);
            Color color = speaker.imgCharacter.color;
            color.a = visible ? 1 : 0.2f;
            speaker.imgCharacter.color = color;
        }
    }

    private void SetAllClose()
    {
        for (int i = 0; i < speakers.Length; i++) SetActiveObjects(speakers[i], false);
    }

    private void EndDialogSystem()
    {
        SetAllClose();
        CloseAllPanels();  
    }

    public void StartDialog(int startIndex, int endIndex, bool isGlobal)
    {
         
        OpenCorrectPanel(isGlobal);

        SetAllClose();
        targetEndIndex = endIndex;
        SetNextDialog(startIndex);
    }

    private void SetNextDialog(int currentIndex)
    {
        SetAllClose();
        currentDialogIndex = currentIndex;
        currentSpeakerIndex = dialogs[currentDialogIndex].speakerUIindex;

        if (currentSpeakerIndex >= 0 && currentSpeakerIndex < speakers.Length)
        {
            SetActiveObjects(speakers[currentSpeakerIndex], true);
            speakers[currentSpeakerIndex].textName.text = dialogs[currentDialogIndex].name;
            StopCoroutine("OnTypingText");
            StartCoroutine("OnTypingText");
        }
    }

    private IEnumerator OnTypingText()
    {
        int index = 0;
        isTypingEffect = true;

        if (dialogs[currentDialogIndex].characterPath != "None")
            speakers[currentSpeakerIndex].imgCharacter.sprite = Resources.Load<Sprite>(dialogs[currentDialogIndex].characterPath);

        if (dialogs[currentDialogIndex].backGroundPath != "None" && BackGround != null)
            BackGround.sprite = Resources.Load<Sprite>(dialogs[currentDialogIndex].backGroundPath);

        string fullText = dialogs[currentDialogIndex].dialogue;
        while (index < fullText.Length + 1)
        {
            speakers[currentSpeakerIndex].textDialogue.text = fullText.Substring(0, index);
            index++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;
        speakers[currentSpeakerIndex].objectArrow.SetActive(true);
    }

    public bool UpdateDialog()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTypingEffect == true)
            {
                isTypingEffect = false;
                StopCoroutine("OnTypingText");
                speakers[currentSpeakerIndex].textDialogue.text = dialogs[currentDialogIndex].dialogue;
                speakers[currentSpeakerIndex].objectArrow.SetActive(true);
                return false;
            }

            if (currentDialogIndex == targetEndIndex)
            {
                EndDialogSystem();
                return true;
            }

            if (dialogs[currentDialogIndex].speakerUIindex == -1)
            {
                if (dialogs[currentDialogIndex].nextindex != -100)
                    SetNextDialog(dialogs[currentDialogIndex].nextindex);
                else
                {
                    EndDialogSystem();
                    return true;
                }
            }
            else if (dialogs[currentDialogIndex].nextScene.CompareTo("None") != 0)
            {
                SceneManager.LoadScene(dialogs[currentDialogIndex].nextScene);
            }
            else if (dialogs[currentDialogIndex].nextindex != -100)
            {
                SetNextDialog(dialogs[currentDialogIndex].nextindex);
            }
            else
            {
                EndDialogSystem();
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        if (currentDialogIndex != -1)
        {
            if (UpdateDialog())
            {
                currentDialogIndex = -1;
            }
        }
    }

    [System.Serializable] public struct SpeakerUI { public Image imgCharacter; public Image imageDialog; public Text textName; public Text textDialogue; public GameObject objectArrow; }
    [System.Serializable] public struct DialogData { public int index; public int speakerUIindex; public string name; public string dialogue; public string characterPath; public string backGroundPath; public int tweenType; public int nextindex; public string nextScene; }
}