using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ubik.Samples
{
    [CreateAssetMenu(menuName = "Prefab Catalogue")]
    public class PrefabCatalogue : ScriptableObject
    {
        public List<GameObject> prefabs;

        public int IndexOf(GameObject gameObject)
        {
            return prefabs.IndexOf(gameObject);
        }
    }
}