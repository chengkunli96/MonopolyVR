using System.Collections;
using System.Collections.Generic;
using Ubik.Messaging;
using Ubik.XR;
using UnityEngine;


using TMPro;
using Ubik.Samples;

public class PlayerPanel : MonoBehaviour
{
    //
    public GameController GameCentor;
    public GameObject avatarManager;


    // player porperties
    public string player_id;
    public int player_order;
    public int money;
    public int mapindex;
    public Vector3 real_position;
   
    public string color0 = "Yellow";
    public string color1 = "Green";
    public string color2 = "Orange";
    public string color3 = "Blue";
    
    


    // avatar manager
    Ubik.Avatars.Avatar avatar;

    public bool have_run = false;
    private string player_name;
    public string[] colors_list;


    // Start is called before the first frame update
    void Start()
    {
        // initialize player porperties
        mapindex = 0;
        player_name = null;
        real_position = new Vector3(0,0,0);
        avatar = null;

        // initialize colors list
        colors_list = new string[4];
        colors_list[0] = color0;
        colors_list[1] = color1;
        colors_list[2] = color2;
        colors_list[3] = color3;

        //gameObject.GetComponent<Renderer>().enabled = false;
        this.transform.localPosition = new Vector3(0, 0, 0);

    }

    // Update is called once per frame
    private bool is_get_playerid = false;
    void FixedUpdate()
    {
        if (!is_get_playerid)
        {
            Ubik.Avatars.Avatar[] avatars = avatarManager.GetComponentsInChildren<Ubik.Avatars.Avatar>();
            avatar = avatars[0];

            player_name = avatar.name;
            string cliped_id = null;
            int idx = player_name.IndexOf("#");
            for (int i = idx + 1; i < player_name.Length; i++)
            {
                cliped_id += player_name[i];
            }
            player_id = cliped_id;

            is_get_playerid = true;
        }
        // get player's name
        if (GameCentor.start_play && !have_run)
        {
            
            for (int i = 0; i < GameCentor.player_num; i++)
            {
                if (player_id == GameCentor.player_ids[i])
                {
                    player_order = i;
                }
            }

            have_run = true;
            //gameObject.GetComponent<Renderer>().enabled = true;
        }
        
        
        if (GameCentor.start_play && have_run)
        {
            // other player porperties
            mapindex = GameCentor.map_index[player_order];

            // get position
            Transform playerhead  = avatar.transform.Find("Floating_BodyA/Floating_Head");
            //Transform playerhead = this.transform.parent;
            real_position = playerhead.position;

            // get money
            if (GameCentor.is_gain || GameCentor.is_loss)
            {
                money = GameCentor.player_money;
                GameCentor.is_gain = false;
                GameCentor.is_loss = false;
            }

        }

    }

    
}
