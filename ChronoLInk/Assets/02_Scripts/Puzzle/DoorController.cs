using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public float openSpeed = 1.0f;
    public float openDistance = 3.0f;
    private bool isOpen = false;
    private Vector3 originalPosition;
    private Vector3 targetPosition;

    void Awake()
    {
        originalPosition = transform.position;
        
        targetPosition = originalPosition + (Vector3.left * openDistance);
    }

    
    public void OpenDoor()
    {
        if (isOpen) return; 

        isOpen = true;
        
        StartCoroutine(MoveDoorCoroutine(targetPosition));
    }

     
    IEnumerator MoveDoorCoroutine(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < openSpeed)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, (elapsedTime / openSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;  
    }


}
