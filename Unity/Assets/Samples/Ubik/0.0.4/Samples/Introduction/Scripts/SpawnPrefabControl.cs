using System.Collections;
using System.Collections.Generic;
using Ubik;
using UnityEngine;
using UnityEngine.UI;

namespace Ubik.Samples
{
    public class SpawnPrefabControl : MonoBehaviour
    {
        public GameObject Prefab;

        // Start is called before the first frame update
        void Start()
        {
            GetComponentInParent<Button>().onClick.AddListener(Spawn);
        }

        public void Spawn()
        {
            NetworkSpawner.Spawn(this, Prefab);
        }

    }
}