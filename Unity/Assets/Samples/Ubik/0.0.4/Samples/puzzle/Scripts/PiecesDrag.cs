using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Ubik.Messaging;

namespace Ubik.Samples
{
    public class PiecesDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, INetworkComponent
    {
        Image maskImage;
        [SerializeField]
        Vector3 offset = Vector3.zero;
        public float canvasScale = 0.003f;
        RectTransform rectTransform;
        public RectTransform container;

        //public GameController gameController;

        [SerializeField]
        PuzzleManager Instance;

        // ---- about the Network -----
        bool owner = false;
        bool pre_not_owner = true;
        private NetworkContext content;
        private struct Message
        {
            //public RectTransform trans;
            public Vector3 trans;

            public Message(Vector3 transform)
            {
                this.trans = transform;
            }
        }

        void Start()
        {
            Instance = this.GetComponentInParent<PuzzleManager>();
            rectTransform = this.GetComponent<RectTransform>();
            content = NetworkScene.Register(this);
            // initial grid position in Puzzle Manager
            Instance.UpdatePosition(transform.position, rectTransform.gameObject.name);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.enterEventCamera, out Vector3 globalMousePos))
            {
                offset = rectTransform.position - globalMousePos;
                // if start drag, set owner true
                owner = true;
                pre_not_owner = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePos))
            {
                rectTransform.position = globalMousePos + offset;
            }
        }

        // TZY version
        public void OnEndDrag(PointerEventData eventData)
        {

            GameObject thisObj = this.gameObject;
            GameObject target = Instance.SetPiece(thisObj, rectTransform);

            // if user put it near the anchor, use 'magnetic force' to drag the piece to the 100% accurate position
            if (target)
            {
                // automatically set this piece to the anchor position
                rectTransform.position = target.transform.position;
            }
            // when ending drag, I will not set owner as false immediately. Because sending message function is inside
            // of the update funtion. If I do so, the last correctPiece states will not be sended. So rely on assistant
            // variable pre_not_owner, we can send the last state and set owner as false in update function.
            pre_not_owner = true;
        }


        void Update()
        {
            // when update, check if the puzzle is finished
            bool isFinish = Instance.IsFinish();

            // Updata position matrix and Update Correction Matrix in PuzzleManager Instance
            Instance.UpdatePosition(rectTransform.position, name);
            Instance.UpdateGridCorrection();

            // sending msg
            if (owner)
            {
                content.SendJson(new Message(rectTransform.position));
            }

            if (pre_not_owner)
            { owner = false; pre_not_owner = false; }
        }


        public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
        {
            if (owner == false)
            {
                var msg = message.FromJson<Message>();
                rectTransform.position = msg.trans;
            }
            // self connection (2 rooms in one scence scenario), used in debug network code by one pc
        }
    }
}
