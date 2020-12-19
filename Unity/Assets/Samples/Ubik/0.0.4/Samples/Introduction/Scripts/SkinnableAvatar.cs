using System;
using System.Collections;
using System.Collections.Generic;
using Ubik.Dictionaries;
using Ubik.Avatars;
using UnityEngine.Events;
using UnityEngine;

namespace Ubik.Samples
{
    [RequireComponent(typeof(Avatars.Avatar))]
    public class SkinnableAvatar : MonoBehaviour
    {
        [Serializable]
        public class SkinEvent : UnityEvent<Texture2D> { }
        public SkinEvent OnSkinUpdated;

        public Texture2D skin;
        private Texture2D currentSkin;
        private Material material;
        private SerializableDictionary properties;
        private Avatars.Avatar avatar;

        private void Awake()
        {
            avatar = GetComponent<Avatars.Avatar>();
            properties = new SerializableDictionary();

            avatar.OnGetProperties.AddListener(GetProperties);
            avatar.OnSetProperties.AddListener(SetProperties);
        }

        private void OnDestroy ()
        {
            if (avatar && avatar != null)
            {
                avatar.OnGetProperties.RemoveListener(GetProperties);
                avatar.OnSetProperties.RemoveListener(SetProperties);
            }
        }

        public void ChangeSkin(Texture2D texture)
        {
            DoChangeSkin(texture);

            properties["skin-source"] = "base64";
            properties["skin-data"] = Convert.ToBase64String(texture.EncodeToPNG());

            avatar.Updated();
        }

        private void DoChangeSkin (Texture2D texture)
        {
            OnSkinUpdated.Invoke(texture);

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

        public void GetProperties(SerializableDictionary properties)
        {
            properties["skin-source"] = this.properties["skin-source"];
            properties["skin-data"] = this.properties["skin-data"];
        }

        public void SetProperties(SerializableDictionary properties)
        {
            if(!IsModified(properties))
            {
                return;
            }

            this.properties["skin-source"] = properties["skin-source"];
            this.properties["skin-data"] = properties["skin-data"];

            if(properties["skin-data"] != null && properties["skin-source"] == "base64")
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(Convert.FromBase64String(properties["skin-data"]));
                DoChangeSkin(texture);
            }
        }

        private bool IsModified (SerializableDictionary remoteProperties)
        {
            return
                this.properties["skin-source"] != remoteProperties["skin-source"] ||
                this.properties["skin-data"] != remoteProperties["skin-data"];
        }
    }
}