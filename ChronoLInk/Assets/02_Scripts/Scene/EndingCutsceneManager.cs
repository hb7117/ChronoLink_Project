using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime; // DisconnectCause 사용을 위해 추가
using System.Collections;

public class EndingCutsceneManager : MonoBehaviourPunCallbacks
{
    public Image fadePanel;
    public Image cutsceneImage;
    public Text dialogText;

    [TextArea(3, 5)]
    public string[] dialogLines;

    public float typeSpeed = 0.05f;
    public float fadeDuration = 2.0f;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isFading = false;
    private string currentText = "";

    void Start()
    {
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            Color c = fadePanel.color;
            c.a = 1f;
            fadePanel.color = c;
        }

        if (dialogText != null) dialogText.text = "";

        StartCoroutine(StartSequence());
    }

    void Update()
    {
        if (isFading) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopCoroutine("TypewriterEffect");
                dialogText.text = currentText;
                isTyping = false;
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    IEnumerator StartSequence()
    {
        isFading = true;
        yield return StartCoroutine(Fade(1, 0));
        isFading = false;

        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (currentLineIndex < dialogLines.Length)
        {
            currentText = dialogLines[currentLineIndex];
            StartCoroutine("TypewriterEffect", currentText);
            currentLineIndex++;
        }
        else
        {
            StartCoroutine(EndSequence());
        }
    }

    IEnumerator TypewriterEffect(string line)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    IEnumerator EndSequence()
    {
        isFading = true;
        yield return StartCoroutine(Fade(0, 1));

        LeaveAndLoadLobby();
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        Color color = fadePanel.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadePanel.color = color;
    }

    // [수정된 부분] 방 나가기 과정을 생략하고 즉시 연결 끊기
    void LeaveAndLoadLobby()
    {
        Time.timeScale = 1f;
        Debug.Log("엔딩 종료: 즉시 연결을 끊고 로비로 이동합니다.");

        // LeaveRoom 대신 바로 Disconnect를 호출하여 방장 승계 딜레이를 없앱니다.
        PhotonNetwork.Disconnect();
    }

    // [수정된 부분] 연결이 끊어지면 실행되는 콜백 (OnLeftRoom 대신 사용)
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("연결 끊김 완료. 로비 씬 로드.");
        SceneManager.LoadScene("LobbyScene");
    }
}