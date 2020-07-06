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
    [SerializeField] private int tiempoPrePartida;

    [Tooltip("Texto que utilizamos para la cuenta atras cuando el jugador le de a empezar")]
    [SerializeField] private Text infoSalaText;

    [Tooltip("ID de la sala para que otros jugadores puedan unirse a ella")]
    [SerializeField] private Text idSalaText;

    #endregion

    private void Start() => idSalaText.text = "ID sala: " + PhotonNetwork.CurrentRoom.Name;

    private void Update() => PermitirComienzoPartida();


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


    public void ComenzarPartidaPrivada()
    {

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
