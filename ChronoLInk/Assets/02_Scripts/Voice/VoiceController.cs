using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using UnityEngine.UI;

[RequireComponent(typeof (PhotonView) , typeof(Recorder))]
public class VoiceController : MonoBehaviour
{
    private Recorder recorder;
    private PhotonView photonView;
    public GameObject VoiceOn;
    public GameObject VoiceOff;


    void Start()
    {
        photonView = GetComponent<PhotonView>();
        recorder = GetComponent<Recorder>();

        VoiceOn.SetActive(false);
        VoiceOff.SetActive(true);

        if (photonView.IsMine)
        {
            
            if (recorder != null)
            {
                recorder.TransmitEnabled = false;
            }
        }
    }

    void Update()
    {
        if (photonView.IsMine && recorder != null)
        {


            if (Input.GetKeyDown(KeyCode.V))
            {
                
                recorder.TransmitEnabled = true;
                VoiceOff.SetActive(false);
                VoiceOn.SetActive(true);
                
            }
            
            else if (Input.GetKeyUp(KeyCode.V))
            {
                recorder.TransmitEnabled = false;
                VoiceOn.SetActive(false);
                VoiceOff.SetActive(true);
                
            }
        }
    }
}
