using UnityEngine;
using Photon.Pun;
using Cinemachine;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private PhotonView photonView;
    private Animator animator;

    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 720.0f;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            CinemachineVirtualCamera virtualcamera = FindObjectOfType<CinemachineVirtualCamera>();
            if (virtualcamera != null)
            {
                virtualcamera.Follow = this.transform;
                virtualcamera.LookAt = this.transform;
            }
        }
       
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

        animator.SetFloat("moveSpeed", movement.magnitude);

        if (movement.magnitude > 0.1f)
        {
            transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}