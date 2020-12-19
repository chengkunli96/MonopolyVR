using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ubik.XR;
using UnityEngine;

namespace Ubik.Samples
{
    /// <summary>
    /// The Fireworks Box is a basic interactive object. This object uses the NetworkSpawner
    /// to create shared objects (fireworks).
    /// The Box can be grasped and moved around, but note that the Box itself is *not* network
    /// enabled, and each player has their own copy.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PuzzleSpanwer : MonoBehaviour, IUseable//, IGraspable
    {
        public GameObject FireworkPrefab;

        public bool already_spawn = false;

        private void Awake()
        {

        }


        public void UnUse(Hand controller)
        {
        }

        public void Use(Hand controller)
        {
            if (already_spawn)
            {
                return;
            }
            already_spawn = true;
            // it's strange, after this line, everything is unreachable
            GameObject panel = NetworkSpawner.SpawnPersistent(this, FireworkPrefab);

        }

        private void Update()
        {

        }
    }
}
