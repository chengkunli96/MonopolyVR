using System;
using System.Collections;
using System.Collections.Generic;
using Ubik.Rooms;
using UnityEngine;
using UnityEngine.UI;

public class RoomsMenuInfoPanel : MonoBehaviour
{
    public RawImage PreviewImage;
    public Text Name;
    public Text Scene;
    public Text ScenePath;
    public Text Guid;

    private string existing;

    public void Bind(RoomArgs args, RoomClient client)
    {
        Name.text = args.name;
        Scene.text = args.properties["scene-name"];
        ScenePath.text = args.properties["scene-path"];
        Guid.text = args.guid;

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
                    PreviewImage.texture = texture;
                }
            });            
        }
    }
}
