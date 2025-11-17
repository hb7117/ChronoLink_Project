using UnityEngine;
using Photon.Pun;
using System.Linq;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class PlayerTimeSyncer : MonoBehaviourPunCallbacks
{
    public float syncRadius = 3f;
    public KeyCode syncKey = KeyCode.R;
    public float syncCooldown = 3.0f;
    public LayerMask timeObjectLayer;

    public AudioClip glitchSound;
    public float effectDuration = 0.3f;

    private float currentCooldown = 0f;
    private Image glitchEffectImage;
    private AudioSource audioSource;

    void Start()
    {
         
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // 씬에 있는 Glitch UI 찾기
        GameObject glitchUIObj = GameObject.FindGameObjectWithTag("GlitchEffectUI");
        if (glitchUIObj != null)
        {
            glitchEffectImage = glitchUIObj.GetComponent<Image>();
            if (glitchEffectImage != null)
            {
                glitchEffectImage.color = Color.clear;
            }
            else
            {
                Debug.LogWarning("GlitchEffectUI 태그 오브젝트에 Image 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("GlitchEffectUI 태그를 가진 UI 오브젝트를 씬에서 찾을 수 없습니다.");
        }

         
        if (!photonView.IsMine)
        {
             
            enabled = false;
            return;
        }
    }

    void Update()
    {
         

        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }

        if (Input.GetKeyDown(syncKey) && currentCooldown <= 0)
        {
            if (PerformTimeAlteration())
            {
                currentCooldown = syncCooldown;

                 
                this.photonView.RPC("RPC_PlayGlitchEffect", RpcTarget.All);
            }
        }
    }

    private bool PerformTimeAlteration()
    {
        // 이 함수는 기존과 동일
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, syncRadius, timeObjectLayer);
        if (hitColliders.Length == 0) return false;

        Collider closestCollider = hitColliders
            .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
            .FirstOrDefault();

        if (closestCollider == null) return false;

        TimeObject objectToSync = closestCollider.GetComponent<TimeObject>();

        if (objectToSync != null && objectToSync.isPastObject)
        {
            GameManager.Instance.SyncTimeObject(
                objectToSync.timeObjectID,
                objectToSync.transform.localPosition
            );
            return true;
        }
        else if (objectToSync == null)
        {
            Debug.LogWarning($"Closest collider '{closestCollider.gameObject.name}' does not have TimeObject component. Check object setup.");
            return false;
        }
        else if (!objectToSync.isPastObject)
        {
            Debug.LogWarning($"Closest TimeObject '{closestCollider.gameObject.name}' is not a Past object (it's a Future object). Cannot sync from here.");
            return false;
        }

        return false;
    }

     
    [PunRPC]
    void RPC_PlayGlitchEffect()
    {
         
        StartCoroutine(PlayGlitchEffect());
    }


    IEnumerator PlayGlitchEffect()
    {
        
        if (audioSource != null && glitchSound != null)
        {
            audioSource.PlayOneShot(glitchSound);
        }

        if (glitchEffectImage != null)
        {
            float timer = 0;
            while (timer < effectDuration)
            {
                float randomAlpha = Random.Range(0.2f, 0.5f);
                glitchEffectImage.color = new Color(1, 1, 1, randomAlpha);

                yield return new WaitForSeconds(0.05f);
                timer += 0.05f;
            }
            glitchEffectImage.color = Color.clear;
        }
    }
}