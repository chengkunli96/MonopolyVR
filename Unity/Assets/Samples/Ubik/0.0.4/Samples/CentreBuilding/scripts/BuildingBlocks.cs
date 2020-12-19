using System.Collections;
using System.Collections.Generic;
using Ubik.Messaging;
using Ubik.XR;
using UnityEngine;

namespace Ubik.Samples
{
    public class BuildingBlocks : MonoBehaviour, IGraspable, INetworkObject, INetworkComponent
    {
        private Hand follow;
        private NetworkContext context;
        private Rigidbody rb;

        public bool owner = false;
        public GameObject centerComponent;
        private Vector3 centre;

        public float throwStrength = 1f;
        public GameController gameController;
        public NetworkId Id { get; } = new NetworkId();

        private float activate_distance_threshold = 1.2f;

        // Base position, it's the world coordinate where you want put your biulding on
        public Vector3 BasePosition = new Vector3(5.0f, 0, 5.0f);
        private List<Vector3> blockPositions = new List<Vector3>();
        private List<Vector3> blockAngles = new List<Vector3>();

        // Current building piece index, get from local GameController
        private int current_piece_index = 0;

        public struct finishMessage
        {
            public bool already_in_right_position;
            public finishMessage(bool already_in_right_position)
            {
                this.already_in_right_position = already_in_right_position;
            }
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            // find local gamecontroller
            gameController = GameObject.Find("GameController").GetComponent<GameController>();

            // Get now piece index
            current_piece_index = gameController.current_piece_index;

            // Get The position this piece should be 
            string center_object_name = "BlockPosition" + current_piece_index.ToString();
            centerComponent = GameObject.Find(center_object_name);
            centre = centerComponent.GetComponent<Transform>().position;

            // Awake the corresponding hint block
            centerComponent.GetComponent<MeshRenderer>().enabled = true;

            owner = false;

            // Initial the wall bias
            blockPositions.Add(new Vector3(-7.8716f, 1.93f, 25.81f));
            blockPositions.Add(new Vector3(-7.11f, 2.11f, 27.49f));
            blockPositions.Add(new Vector3(-7.789f, 1.72f, 29.55f));
            blockPositions.Add(new Vector3(-6.21f, 0.97f, 29.5f));
            blockPositions.Add(new Vector3(-4.98f, 0.14f, 27.42f));
            blockPositions.Add(new Vector3(-4.62f, 0.14f, 25.37f));
            blockPositions.Add(new Vector3(-4.46f, 0.23f, 29.6f));

            blockAngles.Add(new Vector3(0, 90, 0));
            blockAngles.Add(new Vector3(0, 90, 0));
            blockAngles.Add(new Vector3(0, 90, 0));
            blockAngles.Add(new Vector3(0, 90, 0));
            blockAngles.Add(new Vector3(-90, 0, 90));
            blockAngles.Add(new Vector3(-90, 0, 90));
            blockAngles.Add(new Vector3(0, 90, 0));
        }

        public void Grasp(Hand controller)
        {
            //current_piece_index = gameController.current_piece_index - 1;
            follow = controller;
            // when this is true, the object won't slowly drop down when grasping
            rb.isKinematic = true;
            owner = true;
        }

        public void Release(Hand controller)
        {
            follow = null;
            //rb.isKinematic = false;
        }

        // Start is called before the first frame update
        void Start()
        {
            context = NetworkScene.Register(this);
        }

        private void FixedUpdate()
        {
            //print("我当前正在看:" + "本地:" + current_piece_index + "Controller:" + gameController.current_piece_index);
            if (follow != null)
            {
                owner = true;
                // TZY
                if (current_piece_index > 2)
                    transform.position = follow.transform.position;
                else  // first 3 pieces are so high, so I add some y axis bias to make it easy to hold
                    transform.position = new Vector3(follow.transform.position.x, follow.transform.position.y + 1.5f, follow.transform.position.z);
                transform.rotation = follow.transform.rotation;
            }


            if (follow == null)  // TZY, only owner do this judgement. If not, if I drag the object to the nearby place, it will flash
            {
                if (owner == true && rb.velocity == new Vector3(0, 0, 0) && rb.angularVelocity == new Vector3(0, 0, 0))
                {
                    float dis = Mathf.Pow(centre.x - transform.position.x, 2) + Mathf.Pow(centre.z - transform.position.z, 2);
                    dis = Mathf.Sqrt(dis);

                    if (dis < activate_distance_threshold)
                    {
                        transform.position = blockPositions[current_piece_index];
                        transform.rotation = Quaternion.Euler(blockAngles[current_piece_index]);

                        GetComponent<Rigidbody>().useGravity = false;
                        GetComponent<BoxCollider>().enabled = false;

                        context.SendJson(new TransformMessage(transform));
                        owner = false;
                        // always sending, but only update when the receieving state is true
                        // context.SendJson(new finishMessage(is_finished));
                    }
                }
            }

            // Dymatic check if is finshed by comparing the distance is 0 or not
            if (follow == null)
            {
                rb.isKinematic = false;
                //print("位置对比:" + transform.position.x + '=' + blockPositions[current_piece_index].x + ' ' + transform.position.z + '=' + blockPositions[current_piece_index].z);
                if (Mathf.Abs(transform.position.x - blockPositions[current_piece_index].x) < 0.3 && (transform.position.z - blockPositions[current_piece_index].z) < 0.3 && transform.rotation == Quaternion.Euler(blockAngles[current_piece_index]))
                {
                    rb.isKinematic = true;
                    centerComponent.GetComponent<MeshRenderer>().enabled = false;
                    GetComponent<Rigidbody>().useGravity = false;
                    GetComponent<BoxCollider>().enabled = false;
                }
            }

            if (owner)
            {
                context.SendJson(new TransformMessage(transform));
            }

        }

        public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
        {
            owner = false;
            rb.isKinematic = true;
            var state = message.FromJson<TransformMessage>();
            transform.position = state.position;
            transform.rotation = state.rotation;
        }
    }
}
