using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Cinemachine;

/// <summary>
/// Este script establece la disposicion de los elementos del juego.
/// Es decir, prepara el juego
/// </summary>
/// <author>David Martinez Garcia</author>

public class GameSetup : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Este script debe ir atacheado a un objeto en la escena del juego.
    /// Este objeto sera el que maneje el setup del juego, ej: GameSetupController como objeto
    /// </summary>
    /// <author>David Martinez Garcia</author>

    #region Variables

    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;

    #endregion

    #region Callbacks Methods

    private void Start()
    {
        InstanciarJugador();
    }



    private void Update()
    {
        DestruirCopiaPlayer();
    }

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// Carga el menu principal
    /// </summary>
    /// <author>David Martinez Garcia</author>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

    #region UI Methods

    /// <summary>
    /// Cuando el jugador pulsa el boton de salir de la sala.
    /// Abandona la sala
    /// </summary>
    /// <author>David Martinez Garcia</author>
    public void OnSalirSalaClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Metodos privados

    /// <summary>
    /// Si lo permite, se instancia el jugador en el juego, en una determinada posicion y con una determina rotacion.
    /// El permiso para instanciar el jugador esta controlado en el playerController
    /// </summary>
    /// <author>David Martinez Garcia</author>
    private void InstanciarJugador()
    {
        if (PlayerController.LocalPlayerInstance == null)
        {
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        }
    }

    /// <summary>
    /// Al principio del juego, hay posibilidad que se instancie algun objeto no deseado de la sala de espera.
    /// Este meotodo los elimina en caso de que haya alguno.
    /// </summary>
    /// <author>David Martinez Garcia</author>
    private void DestruirCopiaPlayer()
    {
        GameObject destuir = GameObject.FindWithTag("PlayerWaiting");
        if (destuir != null)
        {
            Destroy(destuir);
        }
    }

    #endregion
}
