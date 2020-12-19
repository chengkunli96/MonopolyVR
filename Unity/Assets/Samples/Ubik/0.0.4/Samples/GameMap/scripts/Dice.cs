using System.Collections;
using System.Collections.Generic;
using Ubik.Messaging;
using Ubik.XR;
using UnityEngine;

namespace Ubik.Samples
{
    public class Dice : MonoBehaviour, IGraspable, INetworkObject, INetworkComponent
    {
        private Hand follow;
        private NetworkContext context;
        private Rigidbody rb;

        public string owner_id = null;
        public int result = 0;
        public bool owner = false;
        public bool local_owner = false;
        public bool send_message = false;
        public Vector3 vel = new Vector3(0, 0, 0);
        public GameObject avatar_list;

        public struct DiceMessage
        {
            public string owner_id;
            public int result;

            public DiceMessage(string owner_id, int result)
            {
                this.owner_id = owner_id;
                this.result = result;
            }
        }

        public float throwStrength = 1f;

        public NetworkId Id { get; } = new NetworkId(10);

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Grasp(Hand controller)
        {
            follow = controller;
            Ubik.Avatars.Avatar[] avatars = avatar_list.GetComponentsInChildren<Ubik.Avatars.Avatar>();
            owner_id = avatars[0].name;
            //float min = 100000;
            //foreach (Ubik.Avatars.Avatar avatar in avatars)
            //{
            //    GameObject child = GameObject.Find(avatar.name + "/" + "Floating_BodyA" + "/" + "Floating_RightHand_A");
            //    float dist = Vector3.Distance(child.transform.position, controller.transform.position);
            //    if (dist < min)
            //    {
            //        min = dist;
            //        owner_id = avatar.name;
            //    }
            //}
            string cliped_id = null;
            int idx = owner_id.IndexOf("#");
            for (int i=idx+1; i<owner_id.Length; i++)
            {
                cliped_id += owner_id[i];
            }
            owner_id = cliped_id;
            owner = true;
            local_owner = true;
            send_message = false;
            rb.AddTorque(transform.up * 10);
            rb.AddTorque(transform.right * 10);
            rb.AddTorque(transform.forward * 10);
        }

        public void Release(Hand controller)
        {
            follow = null;
            local_owner = false;
            rb.AddForce(-rb.velocity, ForceMode.VelocityChange);
            rb.AddForce(controller.velocity * throwStrength, ForceMode.VelocityChange);
        }

        // Start is called before the first frame update
        void Start()
        {
            context = NetworkScene.Register(this);
            owner_id = null;
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
            if (!follow && owner_id != null && rb.velocity == vel && rb.angularVelocity == vel)
            {
                Vector3 y = new Vector3(0, 1, 0);
                if (Vector3.Dot(rb.transform.forward, y) >= 0.9)
                {
                    result = 3;
                }
                if (Vector3.Dot(rb.transform.right, y) >= 0.9)
                {
                    result = 5;
                }
                if (Vector3.Dot(rb.transform.up, y) >= 0.9)
                {
                    result = 1;
                }
                if (Vector3.Dot(rb.transform.forward, y) <= -0.9)
                {
                    result = 4;
                }
                if (Vector3.Dot(rb.transform.right, y) <= - 0.9)
                {
                    result = 2;
                }
                if (Vector3.Dot(rb.transform.up, y) <= -0.9)
                {
                    result = 6;
                }
                owner = false;
            }
        }

        public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
        {
            // the other end has taken control of this object
            rb.isKinematic = true;
            owner = false;
            local_owner = false;
            var state = message.FromJson<TransformMessage>();
            transform.localPosition = state.position;
            transform.localRotation = state.rotation;
        }
    }
}