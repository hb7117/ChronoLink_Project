using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] private Slider progressbar;
    [SerializeField] private Text loadingText;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadingGameSceneAsync());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadingGameSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");

        asyncLoad.allowSceneActivation = false;
        loadingText.text = "Loading ..";

        while(asyncLoad.progress < 0.9f)
        {
            yield return new WaitForSeconds(2f);
            progressbar.value = asyncLoad.progress;
            yield return null;
        }
        loadingText.text = "아무 키나 눌러주세요";
        progressbar.value = 1f;

        while(!Input.anyKeyDown)
        {
            yield return null;

        }

        asyncLoad.allowSceneActivation = true;
    }
}
