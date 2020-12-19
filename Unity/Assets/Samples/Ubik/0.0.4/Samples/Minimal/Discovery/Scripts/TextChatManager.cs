using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Ubik.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class TextChatManager : MonoBehaviour, INetworkComponent
{
    public Text conversationBox;
    public InputField sendBox;
    public Button sendButton;

    private NetworkContext context;

    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);

        sendButton.onClick.AddListener(SendMessage);
    }

    void SendMessage()
    {
        conversationBox.text += sendBox.text + "\n";
        context.Send(sendBox.text);
        sendBox.text = "";
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        conversationBox.text += message.ToString() + "\n";
    }
}
