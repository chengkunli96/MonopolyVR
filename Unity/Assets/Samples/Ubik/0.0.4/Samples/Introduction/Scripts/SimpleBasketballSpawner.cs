using System.Collections;
using System.Collections.Generic;
using Ubik.Messaging;
using UnityEngine;

namespace Ubik.Samples
{

    public class SimpleBasketballSpawner : MonoBehaviour, INetworkComponent, INetworkObject
    {
        public GameObject Prefab;
        private NetworkContext context;

        public NetworkId Id { get; } = new NetworkId(0); // for this sample we just set the Id as 1. This is not safe for general use. Assign the Id dynamically.

        void Start()
        {
            context = NetworkScene.Register(this);
        }

        private struct Message
        {
            public int newNetworkObjectId;
        }

        public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
        {
            var spawnmessage = message.FromJson<Message>();
            var ball = Instantiate(Prefab, transform).GetComponent<SimpleBasketball>();
            ball.Id.Set(spawnmessage.newNetworkObjectId);
            ball.owner = false;
        }

        public void SpawnBasketball()
        {
            var ball = Instantiate(Prefab, transform).GetComponent<SimpleBasketball>();
            ball.Id.Set(NetworkScene.GenerateUniqueId());
            ball.owner = true;
            context.SendJson(new Message() { newNetworkObjectId = ball.Id });
        }
    }
}