using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// Este script se encarga de manejar el comportamiento de la sala privada. Se encarga de las conexiones y desconexiones de los jugadores
/// ademas, se encarga de establecer y sincronizar la cuenta atras de los jugadores para poder empezar.
/// Este script debe estar atacheado a un objeto de la sala privada, que va a utilizarse como manager de la sala de espera de la sala privada.
/// Ademas el objeto al que este atacheado tiene que tener un photon view
/// </summary>
/// <author> David Martinez Garcia </author>

public class SalaPrivadaManager : MonoBehaviourPunCallbacks
{

    #region Variables

    [Header("Navegacion entre escenas")]
    [Tooltip("Index de la escena del menu principal")]
    [SerializeField] private int multiplayerSceneIndex;
    [Tooltip("Index de la escena del juego")]
    [SerializeField] private int menuPrincipalSceneIndex;


    [Tooltip("Boton de comenzar la partida que solo sera visible para el host de la sala")]
    [SerializeField] private Button botonEmpezarPartida;

    [Tooltip("Tiempo despues de que el host de la sala le de a empezar partida antes de empezar")]
    [SerializeField] private float tiempoPrePartida;
    //Utilizamos esta varible que sera igual a tiempoPrePartida en el start en caso de querer resetar el tiempo
    private float tiempoReset;
    //Tiempo que en realidad queda para empezar la partida
    private float tiempoRestante;

    [Tooltip("Texto que utilizamos para la cuenta atras cuando el jugador le de a empezar")]
    [SerializeField] private Text infoSalaText;

    [Tooltip("ID de la sala para que otros jugadores puedan unirse a ella")]
    [SerializeField] private Text idSalaText;

    //Utilizamos para saber si la se puede hacer la cuenta atras para dar paso a la partida, esta cuenta no se podra parar a menos que la sala se cierre, es decir todos se salgan
    private bool isComenzarPulsado = false;

    //Utilizamos esta variable para cargar una y solo una vez el mapa multijugador
    private bool isGameStarted = false;

    #endregion

    #region Init

    private void Start() {
        idSalaText.text = "ID sala: " + PhotonNetwork.CurrentRoom.Name;
        tiempoRestante = tiempoPrePartida;
        tiempoReset = tiempoPrePartida;
    }

    private void Update() {
        PermitirComienzoPartida();
        CuentaAtrasNoParable();
    }

    #endregion

    #region UI Methods

    /// <summary>
    /// Este metodo se activa cuando se pulsa el boton de comenzar partida.
    /// Activa el boolean para dar paso a la cuenta atras
    /// </summary>
    /// <author> David Martinez Garcia </author>
    public void ComenzarPartidaPrivada() => isComenzarPulsado = true;

    /// <summary>
    /// Se activa cuando el jugador clicka para abandonar la sala en la que se encuentra.
    /// Abandona la sala en la que esta.
    /// </summary>
    /// <author> David Martinez Garcia </author>
    public void AbandonarSalaClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Callback Methods

    /// <summary>
    /// Se activa cuando un jugador entra en la sala.
    /// Actualiza el contador de cuenta atras a todos los jugadores
    /// </summary>
    /// <param name="newPlayer">Jugador que entra en la sala</param>
    /// <author> David Martinez Garcia </author>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Ademas para sincronizar la cuenta atras, si somos el host mandaremos el timer nuestro que es el valido, a todos los demas jugadores para que se sincronice
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("RPC_SendTimer", RpcTarget.Others, tiempoRestante);
    }


    /// <summary>
    /// Se activa cuando un jugador deja la sala
    /// Si el total de jugadores es menos que 2, es decir , no se puede empezar la partida, se resetea todo como del principio
    /// </summary>
    /// <param name="otherPlayer">Jugador que abandona la sala</param>
    /// <author> David Martinez Garcia </author>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Reseteamos tiemers y text
        if (PhotonNetwork.PlayerList.Length < 2)
        {
            tiempoPrePartida = tiempoReset;
            tiempoRestante = tiempoReset;
            isComenzarPulsado = false;
            infoSalaText.text = "";
        }
    }


    /// <summary>
    /// Se activa cuando el jugador deja la sala.
    /// Carga la escena del menu principal.
    /// </summary>
    /// <author> David Martinez Garcia </author>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(menuPrincipalSceneIndex);
    }

    #region Sincronizacion online
    /// <summary>
    /// Actualiza el Text de info de la sala a todos los jugadores
    /// </summary>
    /// <param name="textoInfo">Texto actual de la info de la sala</param>
    /// <author> David Martinez Garcia </author>
    [PunRPC]
    private void RPC_SendText(string textoInfo)
    {
        //RPC para sincronizar la cuenta atras a todos aquellos que hayan entrado despues de que la cuenta atras haya empezado a contar
        infoSalaText.text = textoInfo;
    }




    /// <summary>
    /// Actualiza el tiempo de la cuenta atras, que posteriormente sera mandada a todos los jugadores para la sincronizacion
    /// </summary>
    /// <param name="timeActual"></param>
    /// <author> David Martinez Garcia </author>
    [PunRPC]
    private void RPC_SendTimer(float timeActual)
    {
        //RPC para sincronizar la cuenta atras a todos aquellos que hayan entrado despues de que la cuenta atras haya empezado a contar
        tiempoRestante = timeActual;
    }
    #endregion

    #endregion

    #region Private Methods

    /// <summary>
    /// Controla el boton de empezar partida para el host de la sala.
    /// De tal forma, el host de la partida solo pondra empezar la partida cuando haya minimo dos jugadores
    /// </summary>
    /// <author> David Martinez Garcia </author>
    private void PermitirComienzoPartida()
    {
       if(PhotonNetwork.PlayerList.Length > 1)
        {
            if (PhotonNetwork.IsMasterClient)
                botonEmpezarPartida.interactable = true;
        }
        else
        {
            botonEmpezarPartida.interactable = false;
        }
    }




    /// <summary>
    /// Controla la cuenta atras, el Text de info de la sala, y el cargar la escena del juego online una vez el contador llega a 0.
    /// Solo se ejecutara el codigo de este metodo si se ha pulsado el boton de comenzar partida
    /// </summary>
    /// <author> David Martinez Garcia </author>
    private void CuentaAtrasNoParable()
    {
        if (isComenzarPulsado)
        {
            //Vamos bajando el contador
            tiempoPrePartida -= Time.deltaTime;
            tiempoRestante = tiempoPrePartida;

            //Guardamos los segundos en un string y los asignamos al Text de la informacion de la sala.
            string textSecondsCountDown = string.Format("{0:00}", tiempoRestante);
            infoSalaText.text = "La partida empezara en: " + textSecondsCountDown;

            //Si somos el host, enviamos la informacion de este texto a los demas jugadores para que tambien se les actualice
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC("RPC_SendText", RpcTarget.Others, infoSalaText.text);


            //Si el tiempo llega a 0 comenzara la partida
            if(tiempoRestante <= 0f)
            {
                if (isGameStarted)
                    return;

                isGameStarted = true;
                //Si no somos el host no hacemos nada
                if (!PhotonNetwork.IsMasterClient)
                    return;
                PhotonNetwork.LoadLevel(multiplayerSceneIndex);
            }
        }
    }

    #endregion

}
