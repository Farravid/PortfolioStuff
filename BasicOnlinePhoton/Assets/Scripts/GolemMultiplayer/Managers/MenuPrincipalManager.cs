using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPrincipalManager : MonoBehaviourPunCallbacks
{
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





    private void Awake() => PhotonNetwork.AutomaticallySyncScene = true;

    #region UI Methods

    public void InicioPartidaRapida()
    {
        isConnected = true;


        //La ui tenemos que cambiarla

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


    public void CancelarBusquedaPartidaRapida()
    {
        PhotonNetwork.Disconnect();
    }



    #endregion


    #region Callbacks Methods

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");

        if (isConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Desconectado debido a " + cause);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Client successfully joined a room");
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if(playerCount != MinPlayersStart)
        {
            //Ajustar ui
            Debug.Log("Client is waiting for an opponent");

        }
        else
        {
            Debug.Log("Listo para empezar");
        }
        PhotonNetwork.LoadLevel(indexEscenaSalaEspera);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No clients are waiting for opponent, creating a new room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayerStop });
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Debug.Log("Hola entro aqui");


        /*if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayerStop)
        {
            PhotonNetwork.LoadLevel(indexEscenaSalaEspera);
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount >= MinPlayersStart && PhotonNetwork.CurrentRoom.PlayerCount < MaxPlayerStop)
        {
            PhotonNetwork.LoadLevel(indexEscenaSalaEspera);
        }*/
        PhotonNetwork.LoadLevel(indexEscenaSalaEspera);
    }

    #endregion



}
