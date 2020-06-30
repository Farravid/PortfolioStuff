using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;

public class LauncherMultiplayer : MonoBehaviourPunCallbacks
{
    #region Variables

    /// <summary>
    /// Es el numero de version del cliente. Los usuarios estan separados unos de otro por la veresion del juego.
    /// </summary>
    string gameVersion = "1";

    //UI variables e inputs Fields
    public Button crearSalaButton;
    public Button unirseSalaButton;
    public Button cancelarUnirseSalaButton;

    public InputField nombreSalaField;
    public InputField numeroJugadoresField;
    

    #endregion

    #region CallBack Methods

    private void Awake()
    {
        //Esto nos asegura que PhotonNetwork.LoadLever() en el host y en los clientes va a ser la misma sala
        PhotonNetwork.AutomaticallySyncScene = true;

        //Nos conectamos al servidor mastaer de photon mediante las settings de photonserversetting
        PhotonNetwork.ConnectUsingSettings();
        //Establecemos la version del juego
        PhotonNetwork.GameVersion = gameVersion;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Nos conectamos a la master, estamos en el servidor: " + PhotonNetwork.CloudRegion);

        //Hacemos interactuables los elementos del menu
        crearSalaButton.interactable = true;
        unirseSalaButton.interactable = true;

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Nos desconectamos de la master");

        //Hacemos no interactuables los elemtnos de la ui
        //Activamos los elementos del menu
        crearSalaButton.interactable = false;
        unirseSalaButton.interactable = false;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Nos hemos metido en una sala");

        //Vamos a comprobar si cuando nos unimos a una sala solo somos una persona, eso signifcia que seresmos el host de la partida
        //por lo tanto deberemos cargar el nivel del online

        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            //Cargamos el nivel con photonnetwork para que se sincronice
            PhotonNetwork.LoadLevel(1);
        }

    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Fallo en unirse a una sala aleatoria, no existe ninguna sala disponible abierta");

        //Ajustamos el ui
        crearSalaButton.interactable = true;
        unirseSalaButton.gameObject.SetActive(true);
        cancelarUnirseSalaButton.gameObject.SetActive(false);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Fallo al crear una sala, el nombre de la sala que has escogido esta ya en uso");

    }


    #endregion

    #region Public Methods
    #endregion

    #region UI Onclicked methods

    public void OnCrearSalaClicked()
    {
        int numMaxPlayers = 5;

        if (!String.IsNullOrEmpty(numeroJugadoresField.text))
        {
            numMaxPlayers = Convert.ToInt32(numeroJugadoresField.text);
            if (numMaxPlayers == 0)
                numMaxPlayers = 5;
        }
        RoomOptions roomOPs = new RoomOptions { MaxPlayers = (byte)numMaxPlayers };


        if (!String.IsNullOrEmpty(nombreSalaField.text))
            PhotonNetwork.CreateRoom(nombreSalaField.text, roomOPs);
        else
            Debug.Log("El nombre de la sala no puede estar vacio");
    }

    public void OnUnirseSalaClicked()
    {
        PhotonNetwork.JoinRandomRoom();

        //Ajustamos los botones y la ui
        crearSalaButton.interactable = false;
        unirseSalaButton.gameObject.SetActive(false);
        cancelarUnirseSalaButton.gameObject.SetActive(true);
    }

    public void OnCancelarUnirseSalaClicked()
    {
        PhotonNetwork.LeaveRoom();

        //Ajustamos el ui
        crearSalaButton.interactable = true;
        unirseSalaButton.gameObject.SetActive(true);
        cancelarUnirseSalaButton.gameObject.SetActive(false);
    }

    #endregion
}
