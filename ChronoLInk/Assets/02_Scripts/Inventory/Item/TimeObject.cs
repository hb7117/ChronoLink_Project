using UnityEngine;
using Photon.Pun;
using System.Collections; // 코루틴 사용

public class TimeObject : MonoBehaviourPun
{
    public string timeObjectID = "Unique_ID_01";
    public bool isPastObject = true;

     
    void OnEnable()
    {
         
        if (!isPastObject)
        {
            
            StartCoroutine(RegisterWithGameManager());
        }
    }

    IEnumerator RegisterWithGameManager()
    {
        float timer = 0f;
        while (GameManager.Instance == null && timer < 1.0f)
        {
            yield return null; 
            timer += Time.deltaTime;
        }

         
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterTimeObject(timeObjectID, this);
        }
        
    }

   
}