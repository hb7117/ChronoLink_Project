using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToLoadingScene : MonoBehaviour
{
 public void Transition()
    {
        SceneHistory.previousSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("LoadingScene");
    }
}
