using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ubik.Rooms;
using UnityEngine;
using UnityEngine.UI;

namespace Ubik.Samples
{
    public class RoomsMenuControl : MonoBehaviour
    {
        public Text Name;
        public Text SceneName;
        public RawImage ScenePreview;
        public Button SelfButton;
        public Button JoinButton;

        [HideInInspector]
        public RoomArgs room;

        private string existing;

        private void Awake()
        {
            JoinButton.onClick.AddListener(() => GetComponentInParent<RoomsMenuController>().Join(this));
            SelfButton.onClick.AddListener(() => GetComponentInParent<RoomsMenuController>().Select(this));
        }

        public void Bind(RoomArgs args, RoomClient client)
        {
            Name.text = args.name;
            SceneName.text = args.properties["scene-name"];

            var image = args.properties["scene-image"];
            if (image != null && image != existing)
            {
                client.GetBlob(args.guid, image, (base64image) =>
                {
                    if (base64image.Length > 0)
                    {
                        var texture = new Texture2D(1, 1);
                        texture.LoadImage(Convert.FromBase64String(base64image));
                        existing = image;
                        ScenePreview.texture = texture;
                    }
                });
            }

            room = args;
        }
    }
}