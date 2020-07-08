using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class ChatBehaviour : MonoBehaviourPunCallbacks, IChatClientListener
{

    public ChatClient chatClient;
    private string username;


    
    private string chatChannel;


    //Variable para el texto chat donde iran saliendo los mensjaes
    [SerializeField]private Text textoChat;

    //Variable donde escribe el usuario, en este caso es un input field
    [SerializeField]private InputField textoJugadorChat;




    void Start()
    {
        Application.runInBackground = true;
        chatChannel = PhotonNetwork.CurrentRoom.Name;

        if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat))
        {
            Debug.LogError("You need to set the chat app ID in the PhotonServerSettings file in order to continue.");
            return;
        }

        username = PhotonNetwork.LocalPlayer.NickName;

        this.chatClient = new ChatClient(this);
        this.chatClient.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());
    }

    void Update()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Service(); // make sure to call this regularly! it limits effort internally, so calling often is ok!
        }
        MandarMensaje();
    }


    private void MandarMensaje()
    {
        if (!string.IsNullOrEmpty(textoJugadorChat.text))
        {
            if (!textoJugadorChat.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                this.chatClient.PublishMessage(chatChannel, username+": "+textoJugadorChat.text);
                textoJugadorChat.text = "";
                textoJugadorChat.ActivateInputField();
            }
        }
    }

    public void OnConnected()
    {
        Debug.Log("Nos suscribimos al canal");
        Debug.Log("Me suscribo al canal: " + chatChannel);
        this.chatClient.Subscribe(new string[] { chatChannel },-1);
        this.chatClient.SetOnlineStatus(ChatUserStatus.Online);

    }

    public void OnDisconnected()
    { }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            textoChat.text = textoChat.text + messages[i] + "\n";
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    { }

    public void OnSubscribed(string[] channels, bool[] results)
    {
       this.chatClient.PublishMessage(chatChannel, username+": ha entrado en la sala");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        this.chatClient.PublishMessage(chatChannel, username+" ha abandonado la sala");
    }

    public void OnUnsubscribed(string[] channels)
    { }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }



    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    { }

    public void OnChatStateChange(ChatState state)
    { }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }


    public override void OnLeftRoom()
    {
        Debug.Log("Nos desuscribimos al canal");
        this.chatClient.Unsubscribe(new string[] { chatChannel });
    }
}
