using UnityEngine;
using Photon.Pun;
using System.Linq;

[RequireComponent(typeof(PhotonView))]
public class PlayerTimeSyncer : MonoBehaviourPunCallbacks  
{
    public float syncRadius = 3f;
    public KeyCode syncKey = KeyCode.R;
    public float syncCooldown = 3.0f;
    public LayerMask timeObjectLayer;  

    private float currentCooldown = 0f;


    void Start()
    {
        // 이 컴포넌트는 과거 플레이어(내 캐릭터)만 활성화
        if (!photonView.IsMine)
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }

        if (Input.GetKeyDown(syncKey) && currentCooldown <= 0)
        {

            PerformTimeAlteration();
            Debug.Log("R키 눌림");
        }
    }

    private void PerformTimeAlteration()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, syncRadius, timeObjectLayer);
        Debug.Log($"PerformTimeAlteration: Found {hitColliders.Length} colliders in radius on correct layer.");

        if (hitColliders.Length == 0) return;

        Collider closestCollider = hitColliders
            .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
            .FirstOrDefault();

        if (closestCollider == null) return;

        TimeObject objectToSync = closestCollider.GetComponent<TimeObject>();

        if (objectToSync != null && objectToSync.isPastObject)
        {
            GameManager.Instance.SyncTimeObject(
                objectToSync.timeObjectID,
                objectToSync.transform.localPosition
            );
            currentCooldown = syncCooldown;
        }
        else if (objectToSync == null) ;
        else if (!objectToSync.isPastObject) ;
    }
}