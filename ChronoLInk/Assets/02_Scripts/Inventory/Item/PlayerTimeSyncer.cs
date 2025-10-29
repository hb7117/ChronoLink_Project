using UnityEngine;
using Photon.Pun;
using System.Linq;

[RequireComponent(typeof(PhotonView))]
public class PlayerTimeSyncer : MonoBehaviourPunCallbacks // PunCallbacks 상속
{
    public float syncRadius = 3f;
    public KeyCode syncKey = KeyCode.R;
    public float syncCooldown = 3.0f;
    public LayerMask timeObjectLayer; // Inspector에서 "Item" 레이어 설정

    private float currentCooldown = 0f;

    // photonView는 MonoBehaviourPunCallbacks에서 제공

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
        // ★★★ 로그 1: 감지된 콜라이더 개수 확인 ★★★
        Debug.Log($"PerformTimeAlteration: Found {hitColliders.Length} colliders in radius on correct layer.");

        if (hitColliders.Length == 0) return;

        Collider closestCollider = hitColliders
            .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
            .FirstOrDefault();

        if (closestCollider == null) return;

        TimeObject objectToSync = closestCollider.GetComponent<TimeObject>();

        if (objectToSync != null && objectToSync.isPastObject)
        {
            // ★★★ 로그 2: GameManager 함수 호출 직전 확인 ★★★
            Debug.Log($"Attempting to call GameManager.SyncTimeObject for ID '{objectToSync.timeObjectID}' with localPos {objectToSync.transform.localPosition}");
            GameManager.Instance.SyncTimeObject(
                objectToSync.timeObjectID,
                objectToSync.transform.localPosition
            );
            currentCooldown = syncCooldown;
        }
        else if (objectToSync == null) { Debug.LogWarning("Closest collider does not have TimeObject component."); }
        else if (!objectToSync.isPastObject) { Debug.LogWarning("Closest TimeObject is not a Past object."); }
    }
}