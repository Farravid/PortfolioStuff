using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;


/// <summary>
/// Este script maneja el comportamiento del chat del juego en todos sus niveles, tantos sala de espera como juego, este ultimo tiene la caracteristicas de ocultarlo o aparecerlo
/// la cual es manejada por el PlayerController
/// Este script debe ir attacheado al padre de todo el Setup UI del chat, en nuestro caso el objeto ChatManager
/// </summary>
/// <author> David Martinez Garcia </author>

public class ChatBehaviour : MonoBehaviourPunCallbacks, IChatClientListener
{
    #region Variables

    //Chatclient foton, maneja todo lo del chat
    public ChatClient chatClient;

    //Nombre del jugador local que utilizaremos para que aparezca en el chat
    private string username;

    //Nombre del canal del chat al cual se une el jugador. El nombre sera el id de la sala para que sea para todos los que no esten dentro de la sala distinto
    private string chatChannel;

    [Tooltip("Variable para el texto chat donde iran saliendo los mensjaes")]
    [SerializeField]private Text textoChat;

    [Tooltip("Variable donde escribe el usuario, en este caso es un input field")]
    [SerializeField]private InputField textoJugadorChat;

    #endregion

    #region Init

    void Start()
    {
        //La aplicacion correra en segundo plano
        Application.runInBackground = true;

        //Asignamos el nombre
        chatChannel = PhotonNetwork.CurrentRoom.Name;
        //Si no existen las opciones de chat en photon no hacemos nada y devolvemos un error
        if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat))
        {
            Debug.LogError("You need to set the chat app ID in the PhotonServerSettings file in order to continue.");
            return;
        }

        //Player local
        username = PhotonNetwork.LocalPlayer.NickName;

        this.chatClient = new ChatClient(this);
        this.chatClient.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());
    }

    void Update()
    {
        if (this.chatClient != null)
        {
            //Va actulizando y manejando el chat, importante llamarlo regularmente.
            this.chatClient.Service();
        }
        MandarMensaje();
    }

    #endregion

    #region Callbacks methods

    /// <summary>
    /// Este metodo se activa cuando abandonamos nosotros mismos una sala
    /// Cuando dejamos la sala en la que nos encontramos con el chat, nos desuscribimos del canal del chat
    /// </summary>
    /// <author> David Martinez Garcia </author>
    public override void OnLeftRoom()
    {
        //Nos desuscribimos del canal
        this.chatClient.Unsubscribe(new string[] { chatChannel });
    }

    /// <summary>
    /// Este metodo se invoca cada vez que un jugador abandona la sala.
    /// En este caso aparecera un mensaje informativo en el chat como que el jugador ha abandonado la sala
    /// </summary>
    /// <param name="otherPlayer">Jugador que abandona la sala</param>
    /// <author> David Martinez Garcia </author>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        this.chatClient.PublishMessage(chatChannel, username + " ha abandonado la sala");
    }

    #endregion

    #region Metodos Privados

    /// <summary>
    /// Se encarga de manejar si el jugador ha escrito un mensaje y ha presionado la tecla enter. En ese caso y si no es vacio
    /// el mensaje se enviara y el foco volvera al lugar de escrbiri.
    /// </summary>
    /// <author> David Martinez Garcia </author>
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

    #endregion


    #region Chat interface


    /// <summary>
    /// Cuando nos conectamos a los servidores de photon se activa este metodo.
    /// Nos suscribimos al nuevo canal y lo ponemos online.
    /// </summary>
    /// <author> David Martinez Garcia </author>
    public void OnConnected()
    {
        this.chatClient.Subscribe(new string[] { chatChannel },-1);
        this.chatClient.SetOnlineStatus(ChatUserStatus.Online);

    }

    /// <summary>
    /// Este metodo recibe todos los mensajes que se le envian al chat y los va metiendo en el chathandler.
    /// Aqui se almacenaran todos los mensajes
    /// </summary>
    /// <param name="channelName">Nombre del canal</param>
    /// <param name="senders">Quien lo envia</param>
    /// <param name="messages">Que mensaje envia</param>
    /// <author> David Martinez Garcia </author>
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            textoChat.text = textoChat.text + messages[i] + "\n";
        }
    }

    /// <summary>
    /// Se invoca cuando un jugador entra en la sala y se suscribe en algun canal.
    /// Se mostrara un mensaje informativo en el chat diciendo el jugador que ha entrado a la sala
    /// </summary>
    /// <param name="channels"></param>
    /// <param name="results"></param>
    public void OnSubscribed(string[] channels, bool[] results)
    {
        this.chatClient.PublishMessage(chatChannel, username + ": ha entrado en la sala");
    }




    //Los demas metodos no tienen ninguna implementacion adicional pero como implementamos una interfaz deben estar aqui
    public void OnDisconnected()
    { }
    public void OnPrivateMessage(string sender, object message, string channelName)
    { }
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

    #endregion

}
