using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SalaPrivadaManager : MonoBehaviourPunCallbacks
{

    #region Variables

    [Header("Navegacion entre escenas")]
    [SerializeField] private int multiplayerSceneIndex;
    [SerializeField] private int menuPrincipalSceneIndex;


    [Tooltip("Boton de comenzar la partida que solo sera visible para el host de la sala")]
    [SerializeField] private Button botonEmpezarPartida;

    [Tooltip("Tiempo despues de que el host de la sala le de a empezar partida antes de empezar")]
    [SerializeField] private float tiempoPrePartida;
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

    private void Start() {
        idSalaText.text = "ID sala: " + PhotonNetwork.CurrentRoom.Name;
        tiempoRestante = tiempoPrePartida;
    }

    private void Update() {
        PermitirComienzoPartida();
        CuentaAtrasNoParable();
    }


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


    public void ComenzarPartidaPrivada() => isComenzarPulsado = true;

    private void CuentaAtrasNoParable()
    {
        if (isComenzarPulsado)
        {
            tiempoPrePartida -= Time.deltaTime;
            tiempoRestante = tiempoPrePartida;

            string textSecondsCountDown = string.Format("{0:00}", tiempoRestante);
            infoSalaText.text = "La partida empezara en: " + textSecondsCountDown;

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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Ademas para sincronizar la cuenta atras, si somos el host mandaremos el timer nuestro que es el valido, a todos los demas jugadores para que se sincronice
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("RPC_SendTimer", RpcTarget.Others, tiempoRestante);
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


    /// <summary>
    /// Se activa cuando el jugador clicka para abandonar la sala en la que se encuentra.
    /// Abandona la sala en la que esta.
    /// </summary>
    /// <author> David Martinez Garcia </author>
    public void AbandonarSalaClicked()
    {
        PhotonNetwork.LeaveRoom();
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
}
