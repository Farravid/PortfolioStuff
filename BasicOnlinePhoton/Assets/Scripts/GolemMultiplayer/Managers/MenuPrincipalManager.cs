using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// Este script se encarga de manejar el menu principal online del juego, es decir,
/// el movimiento entre salas, conexiones y desconxiones de los jugadores a la hora de buscar partida etc...
/// Este script debe estar atacheado a un gameobject en la escena principal del menu
/// </summary>
/// <author> David Martinez Garcia </author>

public class MenuPrincipalManager : MonoBehaviourPunCallbacks
{
    #region Variables
    //Utilizamos para saber si el jugador esta conectado a los servidores de photon
    private bool isConnected = false;

    //Esto es por si lo sacas al mercado y vas metiendo actualizaciones, esto deberia ser updateado a otra version, ya que
    //la gente que no tenga el mismo gameversion no podra conectarse y jugar
    private const string GameVersion = "0.1";

    //Minimo de jugadores para que empiece la partida
    private const int MinPlayersStart = 1;
    //Maximo de jugadores en una partida
    private const int MaxPlayerStop = 4;

    [Tooltip("Index de la escena de la sala de espera que cargara una vez le demos a buscar partida rapida")]
    [SerializeField] private int indexEscenaSalaEspera;
    #endregion

    #region Init
    private void Awake() => PhotonNetwork.AutomaticallySyncScene = true;
    #endregion

    #region UI Methods

    /// <summary>
    /// Metodo que establece las caracteristicas principales de nuestro modo de juego partida rapida.
    /// Este metodo se activa una vez pulsas buscar partida rapida. Debe ir en el on Click de buscar partida
    /// </summary>
    /// /// <author> David Martinez Garcia </author>
    public void InicioPartidaRapida()
    {
        isConnected = true;

        if (PhotonNetwork.IsConnected)
        {
            //Esto no quiere decir que vayamos a entrar a una partida ya directamente, esto quiere decir que podemos entrar a un sala donde empiece una partida directamente
            // o podemos entrar a una sala de espera tipo risk of rain
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }


    /// <summary>
    /// Cancela la busqueda de partida rapida una vez esta buscando.
    /// Este metodo se activa cuando le damos al boton de cancelar partida rapida una vez estamos buscando.
    /// Debe ir en el onclick del boton cancelar del menu principal
    /// </summary>
    /// /// <author> David Martinez Garcia </author>
    public void CancelarBusquedaPartidaRapida()
    {
        PhotonNetwork.Disconnect();
    }



    #endregion


    #region Callbacks Methods

    /// <summary>
    /// Este callback se llama cuando el cliente se conecta a los servidores maestros de photon.
    /// </summary>
    /// /// <author> David Martinez Garcia </author>
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");

        if (isConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    /// <summary>
    /// Este callback se llama cuando el cliente se desconecta de los servidores de photon
    /// </summary>
    /// <param name="cause">Causa de la desconexion</param>
    /// /// <author> David Martinez Garcia </author>
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Desconectado debido a " + cause);
    }

    /// <summary>
    /// Se llama cuando un jugador logra entrar en una sala con exito.
    /// En nuesetro caso, si logra entrar con exito cargara la sala de espera de los jugadores
    /// </summary>
    /// /// <author> David Martinez Garcia </author>
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(indexEscenaSalaEspera);
    }

    /// <summary>
    /// Se llama cada vez que un jugador intenta acceder a una sala aleatoria y el resultado es un error.
    /// Si el jugador falla al entrar a alguna sala porque no existe ninguna sala. Este mismo jugador, creara una sala con las opciones predeterminadas
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    /// /// <author> David Martinez Garcia </author>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No clients are waiting for opponent, creating a new room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayerStop });
    }

    #endregion

}
