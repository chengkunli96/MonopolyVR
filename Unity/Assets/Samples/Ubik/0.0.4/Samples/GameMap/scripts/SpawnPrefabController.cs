using System.Collections;
using System.Collections.Generic;
using Ubik;
using UnityEngine;
using UnityEngine.UI;

namespace Ubik.Samples
{
    public class SpawnPrefabController : MonoBehaviour
    {
        // TZY
        public GameObject Prefab1;
        public GameObject Prefab2;
        public GameObject Prefab3;
        public GameObject Prefab4;
        public GameObject Prefab5;
        public GameObject Prefab6;
        public GameObject Prefab7;

        public List<GameObject> prefabs = new List<GameObject>();

        public GameController GameCenter;

        // Start is called before the first frame update
        void Start()
        {
            GetComponentInParent<Button>().onClick.AddListener(Spawn);

            // TZY
            prefabs.Add(Prefab1);
            prefabs.Add(Prefab2);
            prefabs.Add(Prefab3);
            prefabs.Add(Prefab4);
            prefabs.Add(Prefab5);
            prefabs.Add(Prefab6);
            prefabs.Add(Prefab7);
        }

        public void Spawn()
        {
            if (GameCenter.player_money >= 1000)
            {
                GameCenter.is_loss = true;
                GameCenter.player_money -= 1000;
                print("SpawnPerfabCOntroller, current index：" + GameCenter.current_piece_index);
                NetworkSpawner.Spawn(this, prefabs[GameCenter.current_piece_index]);

                // UPDATE PIECE INDEX
                GameCenter.current_piece_index += 1;
                GameCenter.is_spawn = true;
                if (GameCenter.current_piece_index >= 7)
                    GameCenter.current_piece_index = 6;
            }
        }

    }
}