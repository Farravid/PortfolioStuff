using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Este script se encarga de manejar el menu principal online del juego, es decir,
/// el movimiento entre salas, conexiones y desconxiones de los jugadores a la hora de buscar partida etc...
/// Este script debe estar atacheado a un gameobject en la escena principal del menu
/// </summary>
/// <author> David Martinez Garcia </author>

public class MenuPrincipalManager : MonoBehaviourPunCallbacks
{
    #region Variables
    //Utilizamos para saber si el jugador esta conectado a los servidores de photon y quiere buscar una partida rapida
    private bool isConnectedPartidaRapida = false;

    //Esto es por si lo sacas al mercado y vas metiendo actualizaciones, esto deberia ser updateado a otra version, ya que
    //la gente que no tenga el mismo gameversion no podra conectarse y jugar
    private const string GameVersion = "0.1";

    //Minimo de jugadores para que empiece la partida
    private const int MinPlayersStart = 1;
    //Maximo de jugadores en una partida
    private const int MaxPlayerStop = 4;

    [Tooltip("Index de la escena de la sala de espera que cargara una vez le demos a buscar partida rapida")]
    [SerializeField] private int indexEscenaSalaEspera;

    [Tooltip("Index de la escena de la sala privada que cargara una vez le demos a crear partida rapida")]
    [SerializeField] private int indexEscenaSalaPrivada;


    [Tooltip("Boton para unirse a una sala privada con la id de esa sala")]
    [SerializeField] private Button botonUnirseSalaPrivada;
    [Tooltip("Input field de la ID de la sala a la que el jugador quiere unirse")]
    [SerializeField] private InputField inputFieldIDRoom;




    //Variables para detectar que boton a pulsado y que acciones debemos de realizar
    private bool isSalaPrivada = false;
    private bool isUnirseSalaPrivada = false;


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
        isConnectedPartidaRapida = true;

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
    

    public void CrearSalaPrivada()
    {
        isSalaPrivada = true;

        if (PhotonNetwork.IsConnected)
        {
            CrearSala(false);
        }
        else
        {
            PhotonNetwork.GameVersion = GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void UnirseSalaPrivada()
    {
        isUnirseSalaPrivada = true;

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRoom(inputFieldIDRoom.text);
        }
        else
        {
            PhotonNetwork.GameVersion = GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }

    }

    /// <summary>
    /// Cambia el estado del boton para permitir o no si unirse a una sala privada en relacion al campo del idroom
    /// </summary>
    /// <param name="idRoom"> Value del input field del id de la sala</param>
    public void PermitirUnirseSalaPrivada(string idRoom)
    {
        botonUnirseSalaPrivada.interactable = !string.IsNullOrEmpty(idRoom);
    }


    /// <summary>
    /// Cancela la busqueda de partida rapida una vez esta buscando.
    /// Este metodo se activa cuando le damos al boton de cancelar partida rapida una vez estamos buscando.
    /// Debe ir en el onclick del boton cancelar del menu principal
    /// </summary>
    /// /// <author> David Martinez Garcia </author>
    public void CancelarOnline()
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

        if (isConnectedPartidaRapida)
        {
            PhotonNetwork.JoinRandomRoom();
        }

        if (isSalaPrivada)
        {
            CrearSala(false);
        }

        if (isUnirseSalaPrivada)
        {
            PhotonNetwork.JoinRoom(inputFieldIDRoom.text);
        }
    }




    /// <summary>
    /// Se llama si el crear la sala ha dado lugar a un error. En este caso se volvera a llamar a crear sala para intentarlo de nuevo.
    /// El desencadenante de este problema suele ser que el id de una sala es igual a la de otra, por lo tanto repitiendo el proceso conseguiremos nuestra sala
    /// </summary>
    /// <author> David Martinez Garcia </author>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //Ajustar depende cual sea la sala que ha fallado, muy importante ajustar esto
        if (isSalaPrivada)
            CrearSala(false);
        else if(isConnectedPartidaRapida)
            CrearSala(true);
    }

    /// <summary>
    /// Este callback se llama cuando el cliente se desconecta de los servidores de photon
    /// </summary>
    /// <param name="cause">Causa de la desconexion</param>
    /// /// <author> David Martinez Garcia </author>
    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnectedPartidaRapida = false;
        isSalaPrivada = false;
        isUnirseSalaPrivada = false;
        Debug.Log("Desconectado debido a " + cause);
    }

    /// <summary>
    /// Se llama cuando un jugador logra entrar en una sala con exito.
    /// En nuesetro caso, si logra entrar con exito cargara la sala de espera de los jugadores
    /// </summary>
    /// /// <author> David Martinez Garcia </author>
    public override void OnJoinedRoom()
    {
        if(isConnectedPartidaRapida)
            PhotonNetwork.LoadLevel(indexEscenaSalaEspera);
        else if(isSalaPrivada || isUnirseSalaPrivada)
            PhotonNetwork.LoadLevel(indexEscenaSalaPrivada);

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
        if (!isUnirseSalaPrivada)
            CrearSala(true);
    }


    #endregion

    #region Private Methods

    /// <summary>
    /// Crea una sala de photon. Determina si sera visible en la lista de salas o no con el parametro
    /// </summary>
    /// <param name="visible">Determina si la sala sera visible o no</param>
    private void CrearSala(bool visible)
    {
        int randomIdRoom = Random.Range(0, 10000);
        PhotonNetwork.CreateRoom(randomIdRoom.ToString(), new RoomOptions { MaxPlayers = MaxPlayerStop, IsVisible = visible });
    }

    #endregion

}
