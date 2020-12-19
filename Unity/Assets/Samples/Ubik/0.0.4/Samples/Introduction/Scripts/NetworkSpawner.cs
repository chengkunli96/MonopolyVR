using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ubik.Messaging;
using Ubik.Rooms;

namespace Ubik.Samples
{
    public interface ISpawnable
    {
        void OnSpawned(bool local);
    }


    public class NetworkSpawner : MonoBehaviour, INetworkObject, INetworkComponent
    {
        public NetworkId Id { get; } = new NetworkId(3);//todo: get rid of this magic number

        public RoomClient roomClient;
        public PrefabCatalogue catalogue;

        private NetworkContext context;
        private Dictionary<int, GameObject> spawned;

        [Serializable]
        public struct Message // public to avoid warning 0649
        {
            public int catalogueIndex;
            public int networkId;
        }

        private void Awake()
        {
            spawned = new Dictionary<int, GameObject>();
        }

        void Start()
        {
            context = NetworkScene.Register(this);
            roomClient.OnRoom.AddListener(OnRoom);
        }

        private GameObject Instantiate(int i, int networkId, bool local)
        {
            var go = GameObject.Instantiate(catalogue.prefabs[i], transform);
            go.GetNetworkObjectInChildren().Id.Set(networkId);
            foreach (var item in go.GetComponentsInChildren<MonoBehaviour>())
            {
                if(item is ISpawnable)
                {
                    (item as ISpawnable).OnSpawned(local);
                }
            }
            spawned[networkId] = go;
            return go;
        }

        public GameObject Spawn(GameObject gameObject)
        {
            var i = ResolveIndex(gameObject);
            var networkId = NetworkScene.GenerateUniqueId();
            context.SendJson(new Message() { catalogueIndex = i, networkId = networkId });
            return Instantiate(i, networkId, true);
        }

        public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
        {
            var msg = message.FromJson<Message>();
            Instantiate(msg.catalogueIndex, msg.networkId, false);
        }

        public GameObject SpawnPersistent(GameObject gameObject)
        {
            var i = ResolveIndex(gameObject);
            var networkId = NetworkScene.GenerateUniqueId();
            var key = $"SpawnedObject{ networkId }";
            var spawned = Instantiate(i, networkId, true);
            roomClient.room[key] = JsonUtility.ToJson(new Message() { catalogueIndex = i, networkId = networkId });
            return spawned;
        }

        private void OnRoom()
        {
            foreach (var item in roomClient.room.Properties)
            {
                if(item.Key.StartsWith("SpawnedObject"))
                {
                    var msg = JsonUtility.FromJson<Message>(item.Value);
                    if(!spawned.ContainsKey(msg.networkId))
                    {
                        Instantiate(msg.catalogueIndex, msg.networkId, false);
                    }
                }
            }
        }

        private int ResolveIndex(GameObject gameObject)
        {
            var i = catalogue.IndexOf(gameObject);
            Debug.Assert(i >= 0, $"Could not find {gameObject.name} in Catalogue. Ensure that you've added your new prefab to the Catalogue on NetworkSpawner before trying to instantiate it.");
            return i;
        }

        public static GameObject Spawn(MonoBehaviour caller, GameObject prefab)
        {
            var spawner = FindNetworkSpawner(NetworkScene.FindNetworkScene(caller));
            return spawner.Spawn(prefab);
        }

        public static GameObject SpawnPersistent(MonoBehaviour caller, GameObject prefab)
        {
            var spawner = FindNetworkSpawner(NetworkScene.FindNetworkScene(caller));
            return spawner.SpawnPersistent(prefab);
        }

        private static NetworkSpawner FindNetworkSpawner(NetworkScene scene)
        {
            var spawner = scene.GetComponentInChildren<NetworkSpawner>();
            Debug.Assert(spawner != null, $"Cannot find NetworkSpawner Component for {scene}. Ensure a NetworkSpawner Component has been added.");
            return spawner;
        }
    }

}