using System.Collections;
using System.Collections.Generic;
using Ubik.Avatars;
using UnityEngine;

namespace Ubik.Samples
{
    public class AvatarMenuController : MonoBehaviour
    {
        public GameObject[] avatars;
        public AvatarManager manager;

        public void ChangeAvatar(GameObject prefab)
        {
            if (Application.isPlaying)
            {
                manager.CreateLocalAvatar(prefab);
            }
        }

        private void Update()
        {

        }
    }
}