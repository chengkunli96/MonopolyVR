using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Text;
using Ubik.WebRtc;

public class DataChannelTextChatManager : MonoBehaviour
{
    public Text conversationBox;
    public InputField sendBox;
    public Button sendButton;
    public WebRtcDataChannel channel;

    private void Reset()
    {
        channel = GetComponentInParent<WebRtcDataChannel>();
    }

    // Start is called before the first frame update
    void Start()
    {
        sendButton.onClick.AddListener(SendMessage);
    }

    void SendMessage()
    {
        conversationBox.text += sendBox.text + "\n";
        channel.Send(new Ubik.Networking.ReferenceCountedMessage(Encoding.UTF8.GetBytes(sendBox.text)));
        sendBox.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        var msg = channel.Receive();
        if(msg != null)
        {
            conversationBox.text += Encoding.UTF8.GetString(msg.bytes, msg.start, msg.length) + "\n";
            msg.Release();
        }
    }
}
