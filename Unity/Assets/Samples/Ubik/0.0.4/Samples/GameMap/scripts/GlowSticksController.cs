using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Ubik.Samples;

public class GlowSticksController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameController GameCentor;
    public GameObject Mapblocks;
    
   
    //
    private bool have_run = false;
    private GameObject[] glowsticks;
    private GameObject[] mapblocks;

    // judge overlap
    public float ybase = (float)2.5;
    public float overlap_gap_distrance = (float)0.15;
    void Start()
    {
        glowsticks = new GameObject[4];
        glowsticks[0] = this.transform.Find("glowstick0").gameObject;
        glowsticks[1] = this.transform.Find("glowstick1").gameObject;
        glowsticks[2] = this.transform.Find("glowstick2").gameObject;
        glowsticks[3] = this.transform.Find("glowstick3").gameObject;
        //for (int i = 0; i < 4; i++)
        //{
            
        //    print("glowsticks[i]: " + glowsticks[i].transform.localPosition);
        //    print("--" + glowsticks[i].transform.GetChild(0).localPosition);
        //    print("--" + glowsticks[i].transform.GetChild(1).localPosition);
        //    print("--" + glowsticks[i].transform.GetChild(2).position);
        //    print("--" + glowsticks[i].transform.GetChild(3).position);
        //}
        for (int i = 0; i < glowsticks.Length; i++)
        {
            glowsticks[i].SetActive(false);
        }


        int mapblocks_num = 0;
        foreach (Transform child in Mapblocks.transform)
        {
            mapblocks_num++;
        }
        //print("mapblocks_list length:" + mapblocks_num);
        mapblocks = new GameObject[mapblocks_num];
        for (int i = 0; i < mapblocks_num; i++)
        {
            mapblocks[i] = Mapblocks.transform.Find("block"+i.ToString()).gameObject;
        }

        

    }

    // Update is called once per frame
    void Update()
    {
        // avtivate
        if (GameCentor.start_play && !have_run)
        {
            for (int i = 0; i < GameCentor.player_num; i++)
            {
                glowsticks[i].SetActive(true);

                var tmp = mapblocks[0].transform.position;
                tmp.y = ybase + (float)(overlap_gap_distrance * i);
                glowsticks[i].transform.position = tmp;
            }
            have_run = true;
        }

        // for which player
        if (GameCentor.start_play)
        {
            for (int i = 0; i < GameCentor.player_num; i++)
            {
                int map_index = GameCentor.map_index[i] % mapblocks.Length;

                // whether this glow stick is overlap with another and how many sticks overlap
                int overlap_num = 0;
                for (int j = 0; j <= i - 1; j++)
                {
                    int tmp_map_index = GameCentor.map_index[j];
                    if (map_index == tmp_map_index)
                    {
                        overlap_num++;
                    }
                }
                //print("overlap: " + overlap_num);

                //print("map_dinex:  "+map_index);
                // change position
                var tmp = mapblocks[map_index].transform.position;
                //print("tmp" + tmp);
                tmp.y = ybase + (overlap_gap_distrance * overlap_num);
                //print("tmp.y" + tmp);
                glowsticks[i].transform.position = tmp;
                //print("glowsticks" + glowsticks[i].transform.position + "mapblocks: " + mapblocks[map_index].transform.position);
                //glowsticks[i].transform.position = Vector3();


                //print("-----: " + (double)glowsticks[i].transform.localPosition.z + "-------: " + (double)mapblocks[map_index].transform.localPosition.z);


            }
            
        }
    
        
    }
}
