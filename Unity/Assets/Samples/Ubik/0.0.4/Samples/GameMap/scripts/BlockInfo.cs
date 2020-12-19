using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInfo : MonoBehaviour
{
    [TextArea(3, 20)]
    public string InfoMessage = "";
    public Texture2D image;
    // Start is called before the first frame update
    void Start()
    {
        //GameObject map = gameObject.GetComponentInParent<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
