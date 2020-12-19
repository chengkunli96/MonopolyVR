using System;
using System.Collections;
using System.Collections.Generic;
using Ubik.Dictionaries;
using UnityEngine;

namespace Ubik.Samples
{
    public class KenneyAvatar : Avatars.Avatar
    {
        public Texture2D skin;
        private Texture2D currentSkin;
        private Material material;
        private SerializableDictionary properties;

        private void Awake()
        {
            properties = new SerializableDictionary();
        }

        public void ChangeSkin(Texture2D texture)
        {
            DoChangeSkin(texture);

            properties["kenney-skin-source"] = "base64";
            properties["kenney-skin-data"] = Convert.ToBase64String(texture.EncodeToPNG());

            Updated();
        }

        private void DoChangeSkin (Texture2D texture)
        {
            if(material == null)
            {
                material = GetComponentInChildren<SkinnedMeshRenderer>().material; // make a copy of the material
            }

            material.mainTexture = texture;

            skin = texture;
            currentSkin = texture;
        }

        private void Update()
        {
            if(skin != currentSkin)
            {
                currentSkin = skin;
                ChangeSkin(skin);
            }
        }

        // public override void GetProperties(SerializableDictionary properties)
        // {
        //     properties["kenney-skin-source"] = this.properties["kenney-skin-source"];
        //     properties["kenney-skin-data"] = this.properties["kenney-skin-data"];
        // }

        // public override void SetProperties(SerializableDictionary properties)
        // {
        //     if(!IsModified(properties))
        //     {
        //         return;
        //     }

        //     this.properties["kenney-skin-source"] = properties["kenney-skin-source"];
        //     this.properties["kenney-skin-data"] = properties["kenney-skin-data"];

        //     if(properties["kenney-skin-data"] != null && properties["kenney-skin-source"] == "base64")
        //     {
        //         Texture2D texture = new Texture2D(1, 1);
        //         texture.LoadImage(Convert.FromBase64String(properties["kenney-skin-data"]));
        //         DoChangeSkin(texture);
        //     }
        // }

        private bool IsModified (SerializableDictionary remoteProperties)
        {
            return
                this.properties["kenney-skin-source"] != remoteProperties["kenney-skin-source"] ||
                this.properties["kenney-skin-data"] != remoteProperties["kenney-skin-data"];
        }
    }
}