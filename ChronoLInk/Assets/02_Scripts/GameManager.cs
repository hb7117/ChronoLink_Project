using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Rendering;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
public class GameManager : MonoBehaviour
{
    [Header("플레이어 캐릭터 프리팹")]
    [SerializeField] private GameObject pastCharacter;
    [SerializeField] private GameObject futureCharacter;

    private Vector3 pastSpawnPosition = new Vector3(10f, 2f, 0f);
    private Vector3 futureSpawnPosition = new Vector3(-10f, 2f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        string character = (string)PhotonNetwork.LocalPlayer.CustomProperties["character"];

        GameObject prefabToSpawn = null;
        Vector3 spawnPosition = Vector3.zero;

        if(character == "Past")
        {
            prefabToSpawn = pastCharacter;
            spawnPosition = pastSpawnPosition;
            Debug.LogFormat("'{0}' 역할을 선택 , 스폰위치 : {1}", character, spawnPosition);

        }
        else if(character == "Future")
        {
            prefabToSpawn = futureCharacter;
            spawnPosition = futureSpawnPosition;    
            Debug.LogFormat("'{0}' 역할을 선택, 스폰 위치: {1}", character, spawnPosition);
        }

        if (prefabToSpawn != null)
        {
            PhotonNetwork.Instantiate(prefabToSpawn.name, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("캐릭터 선택 정보가 없거나 잘못되었습니다");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
