using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Cinema : MonoBehaviour

{
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            CinemachineVirtualCamera cam = FindObjectOfType<CinemachineVirtualCamera>();

            if (cam != null)
            {
                cam.Follow = this.transform;
            }
            else Debug.LogError("카메라 못찾음");
            

            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
