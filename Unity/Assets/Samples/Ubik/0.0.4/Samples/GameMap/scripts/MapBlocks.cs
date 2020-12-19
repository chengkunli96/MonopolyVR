using System.Collections;
using System.Collections.Generic;
using Ubik.Messaging;
using Ubik.XR;
using UnityEngine;

namespace Ubik.Samples
{
    public class MapBlocks : MonoBehaviour//，INetworkObject, INetworkComponent
    {
        public bool is_buy = false;
        // Start is called before the first frame update
        void Start()
        {
            GameObject house = transform.Find("House").gameObject;
            if (house != null)
                house.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
