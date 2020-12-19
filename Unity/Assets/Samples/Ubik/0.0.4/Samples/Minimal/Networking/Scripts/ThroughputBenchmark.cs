using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Ubik.Messaging;
using UnityEngine;

namespace Ubik.Samples
{
    public class ThroughputBenchmark : MonoBehaviour, INetworkComponent, INetworkObject
    {
        private NetworkContext context;

        public int guid;

        public int messagesPerFrame = 10;
        public int maxMessageSize = 100000;

        public int totalMessages = int.MaxValue;

        public int sent = 0;
        public int received = 0;

        public bool run = false;
        public bool corrupt = false;

        private SHA256 sha526;
        private const int hashLength = 32;

        public NetworkId Id { get; } = new NetworkId(1);

        private void Awake()
        {
            guid = UnityEngine.Random.Range(0, 1000000);
            sha526 = SHA256.Create();
        }

        private void Start()
        {
            context = NetworkScene.Register(this);
        }

        public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
        {
            var hash = sha526.ComputeHash(message.bytes, message.start, message.length - hashLength);
            var compare = new ReadOnlySpan<byte>(message.bytes, message.start + message.length - hashLength, hashLength).SequenceEqual(hash);
            if(!compare)
            {
                Debug.LogError("Message Corruption Detected");
            }
            received++;
        }

        // Update is called once per frame
        void Update()
        {
            if (run)
            {
                for (int i = 0; i < messagesPerFrame; i++)
                {
                    if(sent >= totalMessages)
                    {
                        return;
                    }

                    int length = UnityEngine.Random.Range(hashLength + 2, maxMessageSize);
                    var message = ReferenceCountedSceneGraphMessage.Rent(length);

                    var datalength = length - hashLength;

                    for (int j = 0; j < datalength; j++)
                    {
                        message.bytes[message.start + j] = (byte)UnityEngine.Random.Range(0, 255);
                    }

                    Array.Copy(sha526.ComputeHash(message.bytes, message.start, datalength), 0, message.bytes, message.start + datalength, hashLength);

                    if (corrupt)
                    {
                        for (int j = 0; j < datalength; j++)
                        {
                            message.bytes[message.start + j] = (byte)UnityEngine.Random.Range(0, 255);
                        }
                    }

                    context.Send(message);

                    sent++;
                }
            }
        }
    }
}