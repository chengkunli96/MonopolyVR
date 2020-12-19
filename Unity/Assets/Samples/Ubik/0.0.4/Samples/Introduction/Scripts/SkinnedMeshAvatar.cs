using System;
using System.Collections;
using System.Collections.Generic;
using Ubik.Dictionaries;
using Ubik.Avatars;
using UnityEngine.Events;
using UnityEngine;

namespace Ubik.Samples
{
    [RequireComponent(typeof(SkinnableAvatar))]
    public class SkinnedMeshAvatar : MonoBehaviour
    {
        public SkinnedMeshRenderer skinnedMeshRenderer;
        private SkinnableAvatar skinnable;

        private void Awake()
        {
            skinnable = GetComponent<SkinnableAvatar>();

            skinnable.OnSkinUpdated.AddListener(OnSkinUpdated);
        }

        private void OnDestroy ()
        {
            if (skinnable && skinnable != null)
            {
                skinnable.OnSkinUpdated.RemoveListener(OnSkinUpdated);
            }
        }

        private void OnSkinUpdated (Texture2D skin)
        {
            // This should clone the material just once, and re-use the clone
            // on subsequent calls. Whole avatar can still use the one material
            var material = skinnedMeshRenderer.material;
            material.mainTexture = skin;

            skinnedMeshRenderer.material = material;
        }
    }
}