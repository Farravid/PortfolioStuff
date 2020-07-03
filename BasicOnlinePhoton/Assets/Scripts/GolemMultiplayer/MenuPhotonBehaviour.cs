﻿using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPhotonBehaviour : MonoBehaviourPunCallbacks
{
    private bool isConnected = false;

    //Esto es por si lo sacas al mercado y vas metiendo actualizaciones, esto deberia ser updateado a otra version, ya que
    //la gente que no tenga el mismo gameversion no podra conectarse y jugar
    private const string GameVersion = "0.1";

    //Minimo de jugadores para que empiece la partida
    private const int MinPlayersStart = 2;
    //Maximo de jugadores en una partida
    private const int MaxPlayerStop = 3;

    //Texto de info para buscar partida rapida
    public Text waitingText;

    //Queremos saber si el jugador se encuentra en una sala o no para actualizarle los jugadores que hay en la sala
    private bool isInRoom = false;


    //
    public Button botonCancelarPartidaRapida;

    private void Awake() => PhotonNetwork.AutomaticallySyncScene = true;

    private void Update()
    {


        EstablecerTextoPartidaRapida();

    }

    private void EstablecerTextoPartidaRapida()
    {
        if (isInRoom)
            waitingText.text = PhotonNetwork.CurrentRoom.PlayerCount + "/8 jugadores. Buscando mas oponenetes";
        else if (!isConnected)
            waitingText.text = "";
    }

    public void InicioPartidaRapida()
    {
        isConnected = true;

        //El texto que aparece en pantalla de carga
        waitingText.text = "Buscando partida...";

        //La ui tenemos que cambiarla

        if (PhotonNetwork.IsConnected)
        {
            //Esto no quiere decir que vayamos a entrar a una partida ya directamente, esto quiere decir que podemos entrar a un sala donde empiece una partida directamente
            // o podemos entrar a una sala de espera tipo risk of rain
            PhotonNetwork.JoinRandomRoom();
        }
        else {
            PhotonNetwork.GameVersion = GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void CancelarPartidaRapida()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        botonCancelarPartidaRapida.interactable = false;
    }

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
        //Ajustar ui
        Debug.Log("Desconectado debido a " + cause);
    }

    public override void OnJoinedRoom()
    {
        botonCancelarPartidaRapida.interactable = true;
        isInRoom = true;
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
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No clients are waiting for opponent, creating a new room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayerStop });
    }

    public override void OnLeftRoom()
    {
        isConnected = false;
        isInRoom = false;
    }



    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Establecemos el numero de jugadores que hay en la sala para que se sepa cuando va a empezar
        

        if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayerStop)
        {
            //No se podra acceder a la sala
            PhotonNetwork.CurrentRoom.IsOpen = false;

            PhotonNetwork.LoadLevel(1);
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount >= MinPlayersStart && PhotonNetwork.CurrentRoom.PlayerCount < MaxPlayerStop)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }





}
