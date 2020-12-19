using System.Collections;
using System.Collections.Generic;
using Ubik.Samples;
using UnityEngine;
using UnityEngine.UI;

public class SelectSceneControl : MonoBehaviour
{
    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInParent<Button>().onClick.AddListener(Switch);
    }

    // Update is called once per frame
    void Switch()
    {
        RoomSceneManager.ChangeScene(this, sceneName);
    }
}
