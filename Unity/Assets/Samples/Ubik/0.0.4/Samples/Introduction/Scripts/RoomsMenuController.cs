using System.Collections.Generic;
using Ubik.Rooms;
using UnityEngine;

namespace Ubik.Samples
{
    public class RoomsMenuController : MonoBehaviour
    {
        public RoomClient Roomclient;

        private float lastDiscoverTime;
        private List<RoomsMenuControl> controls;

        public Transform controlsContainer;
        public GameObject controlPrefab;
        public RoomsMenuInfoPanel infoPanel;

        private void Awake()
        {
            controls = new List<RoomsMenuControl>();

            foreach (Transform child in controlsContainer)
            {
                if (child.gameObject.GetComponent<RoomsMenuControl>())
                {
                    Destroy(child.gameObject);
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            Roomclient.OnRoomsAvailable.AddListener(OnRoomsAvailable);
            Roomclient.OnJoinedRoom.AddListener(OnJoinedRoom);
        }

        private void OnJoinedRoom()
        {

        }

        private void OnRoomsAvailable(List<RoomArgs> rooms)
        {
            while (controls.Count < rooms.Count)
            {
                controls.Add(GameObject.Instantiate(controlPrefab, controlsContainer).GetComponent<RoomsMenuControl>());
            }
            while (controls.Count > rooms.Count)
            {
                Destroy(controls[0].gameObject);
                controls.RemoveAt(0);
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                controls[i].Bind(rooms[i], Roomclient);
            }
        }

        public void Create()
        {
            Roomclient.JoinNew();
        }

        public void Join(RoomsMenuControl control)
        {
            Roomclient.Join(control.room.guid);
        }

        public void Select(RoomsMenuControl control)
        {
            infoPanel.Bind(control.room, Roomclient);
        }

        // Update is called once per frame
        void Update()
        {
            if(Mathf.Abs(lastDiscoverTime - Time.time) > 0.1f)
            {
                lastDiscoverTime = Time.time;
                Roomclient.DiscoverRooms();
            }
        }
    }
}