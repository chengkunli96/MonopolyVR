using System;
using System.Collections;
using System.Collections.Generic;
using Ubik.Messaging;
using UnityEngine;

namespace Ubik.Avatars
{

    [RequireComponent(typeof(Avatar))]
    public class SimpleSingleTransformAvatarComponent : MonoBehaviour, INetworkComponent
    {
        private NetworkContext context;

        public int Id => componentId;
        public int componentId;

        [Serializable]
        private struct Message
        {
            public Vector3 position;
        }

        private Avatar avatar;

        public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
        {
            var m = JsonUtility.FromJson<Message>(message.ToString());
            transform.localPosition = m.position;
        }

        private void Awake()
        {
            avatar = GetComponent<Avatar>();
        }

        // Start is called before the first frame update
        void Start()
        {
            context = NetworkScene.Register(this);
        }

        // Update is called once per frame
        void Update()
        {
            if (avatar.local)
            {
                context.Send(JsonUtility.ToJson(new Message()
                {
                    position = transform.localPosition
                }));
            }
        }
    }
}