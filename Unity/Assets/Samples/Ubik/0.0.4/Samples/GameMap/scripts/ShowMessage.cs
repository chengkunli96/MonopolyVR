using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Ubik.Samples;

public class ShowMessage : MonoBehaviour
{
    // Start is called before the first frame update
    private string StartMessage;
    private string PlayeridMessage;
    private string PlayerorderMessage;
    private string MoneyMessage;
    private string ProcessMessage;
    //private string MapindexMessage;
    void Start()
    {
        
    }

    // Update is called once per frame
    private bool one_time_message = false;
    private bool before_palying = false;
    private GameController GameCentor;
    private PlayerPanel PlayerInfo;
    void Update()
    {
        GameCentor = this.transform.parent.gameObject.GetComponent<PlayerPanel>().GameCentor;
        PlayerInfo = this.transform.parent.gameObject.GetComponent<PlayerPanel>();

        // show the massage befor start gaming
        if (!before_palying)
        {

            bool is_start = GameCentor.is_start;
            bool start_play = GameCentor.start_play;

            if (is_start && !start_play)
            {
                StartMessage = "Start Game :" + "\n" +
                               "Please throw dice to determine playing order.\n";
                string showMessage = StartMessage;

                // wheter you have throw
                for (int i = 0; i < GameCentor.player_num; i++)
                {
                    if (PlayerInfo.player_id == GameCentor.player_ids[i])
                    {
                        showMessage = StartMessage + "\n" + "You have thrown!";
                    }
                }
                this.transform.Find("Text").GetComponent<TMP_Text>().text = showMessage;
            }
            if (start_play)
            {
                before_palying = true;
            }

        }


        // one time message
        if (!one_time_message)
        {
            bool have_run = this.transform.parent.gameObject.GetComponent<PlayerPanel>().have_run;
            if (have_run) // judge whether game is starting and whether player panel is ready for necessary info
            {
                string player_id = this.transform.parent.gameObject.GetComponent<PlayerPanel>().player_id;
                PlayeridMessage = "Player ID: " + player_id;

                int player_order = this.transform.parent.gameObject.GetComponent<PlayerPanel>().player_order;
                string[] colors_list = this.transform.parent.gameObject.GetComponent<PlayerPanel>().colors_list;
                PlayerorderMessage = "My Order: " + player_order.ToString() + " (" + colors_list[player_order] + ")";

                one_time_message = true;
            }

        }


        if (one_time_message)
        {
            // get money
            int money = this.transform.parent.gameObject.GetComponent<PlayerPanel>().money;
            MoneyMessage = "Money: $" + money.ToString();
            ProcessMessage = "Right now, it's player" + (GameCentor.curr_player_index % GameCentor.player_num).ToString("0") + "'s turn";


            this.transform.Find("Text").GetComponent<TMP_Text>().text = PlayeridMessage + "\n" +
                                                                        PlayerorderMessage + "\n" +
                                                                        MoneyMessage + "\n" +
                                                                        "\n" + ProcessMessage;
        }

        // show in panel
        // this.transform.Find("Text").GetComponent<TMP_Text>().text = content;
    }
}
