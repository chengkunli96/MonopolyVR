using System;
using System.Collections;
using System.Collections.Generic;
using Ubik.Rooms;
using UnityEngine;
using UnityEngine.UI;


using TMPro;
using Ubik.Samples;

public class ShowMapBlocksInfo : MonoBehaviour
{
    public string InfoMessage = "";
    public RawImage PreviewImage;

    public GameObject Mapblocks;
    private GameObject[] mapblocks;
    // Start is called before the first frame update
    void Start()
    {
        int mapblocks_num = 0;
        foreach (Transform child in Mapblocks.transform)
        {
            mapblocks_num++;
        }
    
        mapblocks = new GameObject[mapblocks_num];
        for (int i = 0; i < mapblocks_num; i++)
        {
            mapblocks[i] = Mapblocks.transform.Find("block" + i.ToString()).gameObject;
        }
    }

    // Update is called once per frame
    private PlayerPanel PlayerInfo;
    private GameController GameCentor;
    void Update()
    {
        PlayerInfo = this.transform.parent.parent.parent.parent.gameObject.GetComponent<PlayerPanel>();
        GameCentor = PlayerInfo.GameCentor;
        
        if (PlayerInfo.have_run)
        {
            for (int i = 0; i < mapblocks.Length; i++)
            {
                float x = PlayerInfo.real_position.x;
                float z = PlayerInfo.real_position.z;
                float _x = mapblocks[i].transform.position.x;
                float _z = mapblocks[i].transform.position.z;
                string blockmessage = mapblocks[i].GetComponent<BlockInfo>().InfoMessage ?? null;
                //print("---- " + (mapblocks[i].GetComponent<BlockInfo>().InfoMessage==null));
                //print("+++" + (blockmessage==null) + (blockmessage==""));
                if (x < _x + 5 && x > _x - 5 && z < _z + 5 && z > _z - 5)
                {
                    if (blockmessage == null || blockmessage=="")
                    {
                        InfoMessage = "This block has none massage";
                        PreviewImage.texture = null;
                    }
                    else
                    {
                        InfoMessage = blockmessage;
                        PreviewImage.texture = mapblocks[i].GetComponent<BlockInfo>().image;
                    }
                    break;
                }
                else
                {
                    InfoMessage = "there is not a block";
                    PreviewImage.texture = null;
                }

            }
            
        }
        else 
        {
            InfoMessage = "Have not started a game";
        }

        this.transform.Find("TextInfo").GetComponent<TMP_Text>().text = InfoMessage;
    }
}
