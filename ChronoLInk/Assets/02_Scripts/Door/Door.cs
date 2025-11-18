using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
     
    [SerializeField] private bool isLocked = false;
    [SerializeField] private string requiredKeyID;

     
    [SerializeField] private float openAngle = -90.0f;  
    [SerializeField] private float openSpeed = 2.0f;

    private bool isOpen = false;
    private bool isOpening = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

   
    public void TryOpenDoor(Inventory playerInventory)
    {
        if (isOpen || isOpening) return;

        if (isLocked)
        {
            
            if (playerInventory != null && playerInventory.HasItem(requiredKeyID))
            {
                
                playerInventory.RemoveItem(requiredKeyID);
                isLocked = false;
                StartCoroutine(OpenDoorCoroutine());
            }
             
        }
        else
        {
             
            
            StartCoroutine(OpenDoorCoroutine());
        }
    }

    
    private IEnumerator OpenDoorCoroutine()
    {
        isOpening = true;
        float time = 0;
        Quaternion startRotation = transform.rotation;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, openRotation, time);
            time += Time.deltaTime * openSpeed;
            yield return null;
        }

        transform.rotation = openRotation;
        isOpen = true;
        isOpening = false;
    }
}