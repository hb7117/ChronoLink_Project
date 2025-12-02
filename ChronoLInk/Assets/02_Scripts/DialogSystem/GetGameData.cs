using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGameData : MonoBehaviour
{
    public Entity_GameDB entity_GameDB;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Entity_GameDB.Param param in entity_GameDB.sheets[0].list)
        {
            Debug.Log(param.index + " - " + param.objectid + " - " + param.text);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
