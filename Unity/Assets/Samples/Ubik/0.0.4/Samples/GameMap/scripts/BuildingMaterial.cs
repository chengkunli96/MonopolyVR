using System.Collections;
using System.Collections.Generic;
using Ubik.Messaging;
using Ubik.XR;
using UnityEngine;

namespace Ubik.Samples
{
    public class BuildingMaterial : MonoBehaviour, IGraspable, INetworkObject, INetworkComponent
    {
        private Hand follow;
        private NetworkContext context;
        private Rigidbody rb;

        public bool owner = false;
        public GameObject centerComponent;
        private Vector2 center;

        public float throwStrength = 1f;
        public GameController gameController;
        public bool is_set = false;
        public NetworkId Id { get; } = new NetworkId();

        private float activate_distance_threshold = 1.2f;

        // Base position, it's the world coordinate where you want put your biulding on
        public Vector3 BasePosition = new Vector3(-15.0f, 0, 32.0f);
        private List<Vector3> wall_bias = new List<Vector3>();
        private List<Vector3> wall_angle = new List<Vector3>();

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            centerComponent = GameObject.Find("TransferDoor");
            center = new Vector2(centerComponent.transform.position.x, centerComponent.transform.position.z);
            // find local gamecontroller
            gameController = GameObject.Find("GameController").GetComponent<GameController>();

            // Initial the wall bias
            float half_wall_width = 0.5f;
            wall_bias.Add(new Vector3(half_wall_width, 0, 0));
            wall_bias.Add(new Vector3(0, 0, -half_wall_width));
            wall_bias.Add(new Vector3(-half_wall_width, 0, 0));
            wall_bias.Add(new Vector3(0, 0, -half_wall_width));

            wall_angle.Add(new Vector3(0, 0, 0));
            wall_angle.Add(new Vector3(0, 90, 0));
            wall_angle.Add(new Vector3(0, 180, 0));
            wall_angle.Add(new Vector3(0, -90, 0));
        }

        public void Grasp(Hand controller)
        {
            follow = controller;
            owner = true;
        }

        public void Release(Hand controller)
        {
            follow = null;
            is_set = false;
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
                // TZY
                transform.position = follow.transform.position;
                transform.rotation = follow.transform.rotation;
            }

            // if a player throw the matrial and material stoped
            if (follow == null && rb.velocity == new Vector3(0, 0, 0) && rb.angularVelocity == new Vector3(0, 0, 0))
            {
                //Debug.Log("The center is:" + center);
                //Debug.Log("I am here:" + this.gameObject.transform.localPosition);
                float dis = Mathf.Pow(center.x - transform.position.x, 2) + Mathf.Pow(center.y - transform.position.z, 2);
                dis = Mathf.Sqrt(dis);

                if (dis < activate_distance_threshold)
                {
                    if (!is_set)
                    {
                        int index = gameController.current_piece_index;
                        gameController.current_piece_index += 1;

                        int floor_index = index / 5;
                        int wall_index = index % 5;

                        Debug.Log("The index is:" + index);

                        if (wall_index <= 3)
                        {
                            transform.position = BasePosition + wall_bias[wall_index] + new Vector3(0, 2, 0) * floor_index;
                            Debug.Log("The position has been set to:" + transform.position);
                            transform.rotation = Quaternion.Euler(wall_angle[wall_index]);
                        }
                        GetComponent<Rigidbody>().useGravity = false;
                        GetComponent<BoxCollider>().enabled = false;
                        is_set = true;
                    }
                }
                //Debug.Log("The distance:" + dis);
            }

            if (owner)
            {
                context.SendJson(new TransformMessage(transform));
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            var name = collision.collider.name;
            Debug.Log("I am Collision with " + name);

        }

        public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
        {
            // the other end has taken control of this object
            // print("process message");
            rb.isKinematic = true;
            owner = false;
            var state = message.FromJson<TransformMessage>();
            //transform.localPosition = state.position;
            //transform.localRotation = state.rotation;
            // TZY
            transform.position = state.position;
            transform.rotation = state.rotation;
        }
    }
}