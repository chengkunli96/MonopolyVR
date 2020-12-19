using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubik.Messaging;


namespace Ubik.Samples
{
    public class GameController : MonoBehaviour, INetworkObject, INetworkComponent
    {
        public int player_num = 10;
        public int curr_player_index = 0;

        public GameObject avatar_list;
        public Dice dice;
        public GameObject mapblocks_list;

        public string[] player_ids;
        public int[] player_order;
        public int[] map_index;

        // variables for dice
        public string player_id = null;
        public int dice_result = 0;
        public bool get_dice_result = false;
        public bool start_play = false;
        public bool get_avatar_list = false;
        public bool print_order = false;

        // variables for mapblocks
        public string map_player_id;
        public int map_player_index;
        public int map_number;
        public int map_dice_result;
        public int buy_map_idx;
        public string buy_info = "";

        //variable for click bottun
        public bool is_click = false;
        public bool is_start = false;

        //variable for money
        public bool is_gain;
        public bool is_loss;
        public int loop_num;
        public int player_money;

        //variable for puzzle
        public int current_piece_index;
        public float finishtime = 0.0f;
        public bool get_puzzle_time = false;

        public bool is_spawn = false;

        private NetworkContext context;

        public struct DiceMessage
        {
            public string owner_id;
            public int result;

            public DiceMessage(string owner_id, int result)
            {
                this.owner_id = owner_id;
                this.result = result;
            }
        }

        public struct MapBlockMessage
        {
            public string player_id;
            public int map_index;
            public int map_dice_result;
            public MapBlockMessage(string player_id, int map_index, int map_dice_result)//, int player_idx)
            {
                this.player_id = player_id;
                this.map_index = map_index;
                this.map_dice_result = map_dice_result;
            }
        }

        public struct ClickMessage
        {
            public bool is_click;
            public bool is_start;
            public ClickMessage(bool is_click, bool is_start)
            {
                this.is_click = is_click;
                this.is_start = is_start;
            }
        }

        public struct BuyMapBlockMessage
        {
            public string buy_info;
            public int buy_map_idx;
            public BuyMapBlockMessage(string buy_info, int buy_map_idx)
            {
                this.buy_info = buy_info;
                this.buy_map_idx = buy_map_idx;
            }
        }

        public struct SpawnMessage
        {
            public int idx;
            public SpawnMessage(int idx)
            {
                this.idx = idx;
            }
        }

        public NetworkId Id { get; } = new NetworkId(5);

        public void ClickStartButton()
        {
            is_click = true;
            is_start = true;
            context.SendJson(new ClickMessage(is_click, is_start));
        }

        public void ClickBuyHouse()
        {
            if (player_money >= 2000)
            {
                int player_idx = (curr_player_index - 1) % player_num;
                int map_idx = map_index[player_idx] % map_number;
                GameObject mapblock = mapblocks_list.transform.GetChild(map_idx).gameObject;
                if (!mapblock.GetComponent<MapBlocks>().is_buy)
                {
                    mapblock.transform.FindChild("House").gameObject.SetActive(true);
                    mapblock.GetComponent<MapBlocks>().is_buy = true;
                    mapblock.GetComponent<BlockInfo>().InfoMessage = mapblock.GetComponent<BlockInfo>().InfoMessage + "\n" + "\n" +
                                                                "player " + player_ids[player_idx] + " has bought this place";
                    player_money -= 2000;
                    is_loss = true;
                    context.SendJson(new BuyMapBlockMessage(mapblock.GetComponent<BlockInfo>().InfoMessage, map_idx));
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            context = NetworkScene.Register(this);
            map_number = mapblocks_list.transform.childCount;
        }

        public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
        {
            // receive dice message
            var spawn_state = message.FromJson<SpawnMessage>();
            if (spawn_state.idx > 0)
            {
                current_piece_index = spawn_state.idx;
            }
            else
            {
                var dice_state = message.FromJson<DiceMessage>();

                player_id = dice_state.owner_id;
                dice_result = dice_state.result;

                // receive map index message
                var map_state = message.FromJson<MapBlockMessage>();
                map_player_id = map_state.player_id;
                map_player_index = map_state.map_index;
                map_dice_result = map_state.map_dice_result;
                get_dice_result = true;

                var click_state = message.FromJson<ClickMessage>();
                is_click = click_state.is_click;
                is_start = click_state.is_start;

                var buy_state = message.FromJson<BuyMapBlockMessage>();
                buy_info = buy_state.buy_info;
                buy_map_idx = buy_state.buy_map_idx;
            }
        }

        // Update is called once per frame
        void Update()
        {
            // first get all the players' id
            if (is_click && !get_avatar_list)
            {
                Ubik.Avatars.Avatar[] avatars = avatar_list.GetComponentsInChildren<Ubik.Avatars.Avatar>();
                foreach (Ubik.Avatars.Avatar avatar in avatars)
                {
                    print(avatar);
                }
                player_num = avatars.Length;
                print(player_num);
                player_ids = new string[player_num];
                player_order = new int[player_num];
                map_index = new int[player_num];
                for (int i = 0; i < player_num; i++)
                {
                    player_ids[i] = null;
                    player_order[i] = 0;
                    map_index[i] = 0;
                }
                is_gain = true;
                player_money = 2000;
                current_piece_index = 0;
                get_avatar_list = true;
                is_click = false;
                get_dice_result = false;
            }

            // after getting ids, each player throws a dice to determine the order
            if (!start_play && get_avatar_list)
            {
                if (dice.owner_id != null && dice.result != 0)
                {
                    bool is_id_exist = false;
                    for (int i = 0; i < player_num; i++)
                    {
                        if (dice.owner_id == player_ids[i])
                            is_id_exist = true;
                    }
                    //print(dice.owner_id);
                    if (!is_id_exist)
                    {
                        //print("receive local avatar result");
                        //print(dice.owner_id + " " + dice.result);
                        player_ids[curr_player_index] = dice.owner_id;
                        player_order[curr_player_index] = dice.result;
                        context.SendJson(new DiceMessage(dice.owner_id, dice.result));
                        curr_player_index += 1;
                        dice.owner_id = null;
                        dice.result = 0;
                    }
                    else
                    {
                        dice.owner_id = null;
                        dice.result = 0;
                        //print("next avatar");
                    }
                }

                if (get_dice_result)
                {
                    bool is_id_exist = false;
                    for (int i = 0; i < player_num; i++)
                    {
                        if (player_id == player_ids[i])
                            is_id_exist = true;
                    }
                    if (!is_id_exist)
                    {
                        //print("receive remote avatar result");
                        //print(player_id + " " + dice_result);
                        player_ids[curr_player_index] = player_id;
                        player_order[curr_player_index] = dice_result;
                        curr_player_index += 1;
                        get_dice_result = false;
                    }
                    else
                    {
                        get_dice_result = false;
                        print("next avatar");
                    }
                }

                if (curr_player_index == player_num)
                {
                    start_play = true;
                    // sort the player order
                    for (int i = 0; i < player_num - 1; i++)
                    {
                        for (int j = 0; j < player_num - i - 1; j++)
                        {
                            if (player_order[j] < player_order[j + 1])
                            {
                                int b = player_order[j];
                                player_order[j] = player_order[j + 1];
                                player_order[j + 1] = b;
                                string c = player_ids[j];
                                player_ids[j] = player_ids[j + 1];
                                player_ids[j + 1] = c;
                            }
                        }
                    }
                    string tmp1 = null;
                    string tmp2 = null;
                    for (int i = 0; i < player_num; i++)
                    {
                        tmp1 += player_ids[i] + " ";
                        tmp2 += player_order[i] + " ";
                    }
                    print("All the player has thrown the dice once, the order is as follows");
                    print(tmp1);
                    print(tmp2);
                }
            }

            // each player has thrown the dice once and the order is determined
            if (start_play)
            {
                if (is_spawn)
                {
                    context.SendJson(new SpawnMessage(current_piece_index));
                    //print(current_piece_index);
                    is_spawn = false;
                }
                if (finishtime > 0.0f && !get_puzzle_time)
                {
                    print("Finish puzzle game, well done!");
                    is_gain = true;
                    player_money += 500;
                    finishtime = 0.0f;
                    get_puzzle_time = true;
                }
                if (buy_info != "" && buy_info != null)
                {
                    print(buy_info);
                    mapblocks_list.transform.GetChild(buy_map_idx).gameObject.GetComponent<MapBlocks>().is_buy = true;
                    mapblocks_list.transform.GetChild(buy_map_idx).gameObject.GetComponent<BlockInfo>().InfoMessage = buy_info;
                    mapblocks_list.transform.GetChild(buy_map_idx).gameObject.transform.FindChild("House").gameObject.SetActive(true);
                    buy_info = "";
                    get_dice_result = false;
                }

                // if receive local avatar information
                if (dice.owner_id != null && dice.result != 0)
                {

                    if (dice.owner_id == player_ids[curr_player_index % player_num])
                    {
                        print("Avatar " + dice.owner_id + " is playing");
                        int idx = curr_player_index % player_num;
                        int tmp = loop_num;
                        map_index[idx] += dice.result;
                        print("Please go to mapblock " + map_index[idx]);
                        loop_num = map_index[idx] / map_number;
                        if (loop_num > tmp)
                        {
                            is_gain = true;
                            player_money += 500;
                        }
                        if (mapblocks_list.transform.GetChild(map_index[idx] % map_number).gameObject.GetComponent<MapBlocks>().is_buy)
                        {
                            player_money += 500;
                            is_gain = true;
                        }
                        // send local message
                        context.SendJson(new DiceMessage(dice.owner_id, dice.result));
                        context.SendJson(new MapBlockMessage(dice.owner_id, map_index[idx], dice.result));//, idx));
                        dice.owner_id = null;
                        dice.result = 0;
                        curr_player_index += 1;
                        get_dice_result = false;
                        print_order = false;
                    }
                    else
                    {
                        print("wrong player");
                        dice.owner_id = null;
                        dice.result = 0;
                        get_dice_result = false;
                        print_order = false;
                    }
                }
                // if receive remote avatar information
                if (get_dice_result)
                {
                    if (map_player_id == player_ids[curr_player_index % player_num])
                    {
                        print("Avatar " + map_player_id + " is playing");
                        int idx = curr_player_index % player_num;
                        map_index[idx] += map_dice_result;
                        print("Please go to mapblock " + map_index[idx]);
                        if (mapblocks_list.transform.GetChild(map_index[idx] % map_number).gameObject.GetComponent<MapBlocks>().is_buy)
                        {
                            player_money += 500;
                            is_gain = true;
                        }
                        curr_player_index += 1;
                        get_dice_result = false;
                        print_order = false;
                    }
                    else
                    {
                        print("wrong player");
                        get_dice_result = false;
                        print_order = false;
                    }
                }
            }


        }
    }
}