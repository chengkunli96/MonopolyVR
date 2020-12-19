using System.Collections;
using System.Collections.Generic;
using Ubik.Messaging;
using Ubik.XR;
using UnityEngine;

namespace Ubik.Samples
{
    public class graspableObject : MonoBehaviour, IGraspable, INetworkObject, INetworkComponent
    {
        private Hand follow;
        private NetworkContext context;
        private Rigidbody rb;

        public bool owner = false;

        public float throwStrength = 1f;

        public NetworkId Id { get; } = new NetworkId();
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Grasp(Hand controller)
        {
            follow = controller;
            owner = true;
        }

        public void Release(Hand controller)
        {
            follow = null;
            rb.AddForce(-rb.velocity, ForceMode.VelocityChange);
            rb.AddForce(controller.velocity * throwStrength, ForceMode.VelocityChange);
        }

        // Start is called before the first frame update
        void Start()
        {
            context = NetworkScene.Register(this);
        }

        private void FixedUpdate()
        {
            if (follow != null)
            {
                owner = true;
                rb.isKinematic = false;
            }

            if (follow != null)
            {
                rb.AddForce(-rb.velocity, ForceMode.VelocityChange);
                rb.AddForce((follow.transform.position - rb.position) / Time.deltaTime, ForceMode.VelocityChange);
            }

            if (owner)
            {
                context.SendJson(new TransformMessage(transform));
            }
        }

        public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
        {
            // the other end has taken control of this object
            // print("process message");
            rb.isKinematic = true;
            owner = false;
            var state = message.FromJson<TransformMessage>();
            transform.localPosition = state.position;
            transform.localRotation = state.rotation;
        }
    }
}