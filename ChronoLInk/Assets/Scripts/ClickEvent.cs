using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class ClickEvent : MonoBehaviourPunCallbacks
{
    

    [Header("规父甸扁 剁快扁")]
    public GameObject CreatPanel;

    [Header("规 格废 剁快扁")]
    public GameObject roomListContent;
    

    [Header("规 剁快扁 ")]
    public GameObject playerListContent;
   

    
    public void OnButtonClick(string buttonName)

    {

        if (buttonName == "Create")

        {
            roomListContent.SetActive(false);
            CreatPanel.SetActive(true);

        }

        else if (buttonName == "RoomCreate")

        {
            CreatPanel.SetActive(false);
            playerListContent.SetActive(true);





        }

    }


}