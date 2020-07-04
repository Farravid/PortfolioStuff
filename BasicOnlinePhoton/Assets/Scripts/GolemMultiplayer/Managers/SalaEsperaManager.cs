﻿using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SalaEsperaManager : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Este script debe estar atacheado a un objeto de la sala que va a utilizarse como manager de la sala de espera de partida rapida
    /// Ademas el objeto al que este atacheado tiene que tener un photon view
    /// </summary>

    [Header("Navegacion entre escenas")]
    [SerializeField] private int multiplayerSceneIndex;
    [SerializeField] private int menuPrincipalSceneIndex;

    //numero de jugadores y jugadores totales que puede haber en la sala
    private int playerCount;
    private int roomSize;

    //minimo de jugadores para empezar la cuenta atras
    [SerializeField] private int minPlayersToStart;

    //Variables de texto para manejar el display de la cuenta atras general y de la cuenta atras del jugador
    [SerializeField]
    private Text cuentaAtrasText;

    //varibales booleanas para controlar el estado del juego en la sala de espera
    private bool isReadyToCountDown;
    private bool isReadyToStart;
    private bool isStartingGame;

    //variables para el countdown
    private float timerToStartGame;
    private float notFullGameTimer;
    private float fullGameTimer;

    //variables para saber cuanto dura cada cuenta atras, ademas tambien las utilizaremos para resetear la cuenta atras
    [SerializeField]
    private float maxWaitTime;
    [SerializeField]
    private float maxFullRoomWaitTIme;




    void Start()
    {
        //Inicializamos variables
        fullGameTimer = maxFullRoomWaitTIme;
        notFullGameTimer = maxWaitTime;
        timerToStartGame = maxWaitTime;

        ContadorJugadores();
    }

    private void ContadorJugadores()
    {
        playerCount = PhotonNetwork.PlayerList.Length;
        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;

        if (playerCount == roomSize)
        {
            isReadyToStart = true;
        }
        else if (playerCount >= minPlayersToStart)
        {
            isReadyToCountDown = true;
        }
        else
        {
            isReadyToCountDown = false;
            isReadyToStart = false;
        }
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Cada vez que alguien entra en la sala, llamamos a nuestro contador de jugadores para que establezca los valores
        ContadorJugadores();
        //Ademas para sincronizar la cuenta atras, si somos el host mandaremos el timer nuestro que es el valido, a todos los demas jugadores para que se sincronice
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("RPC_SendTimer", RpcTarget.Others, timerToStartGame);
    }

    [PunRPC]
    private void RPC_SendTimer(float timeActual)
    {
        //Vale osea va actualizando lo de todas las personajes que entran si o si en los dos primeros pq siempre sera igual o menor que el tiempo de espera, y en caso de que sea menor que el de la sala llena tambien actualizara ese

        //RPC para sincronizar la cuenta atras a todos aquellos que hayan entrado despues de que la cuenta atras haya empezado a contar
        timerToStartGame = timeActual;
        notFullGameTimer = timeActual;
        if(timeActual < fullGameTimer)
        {
            fullGameTimer = timeActual;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Como tambien queremos actualizar el tiempo en funcion de si un jugador deja la sala, supongamos que son menos de los necesarios para empezar, calculamos otra vez los jugadores
        ContadorJugadores();
    }



    // Update is called once per frame
    void Update()
    {
        EsperarJugadores();
    }

    private void EsperarJugadores()
    {
        //En este metodo comprobamos que la sala este o no lista para empezar a contar hacia atras, o este llena y tengamos que empezar
        if(playerCount <= 1)
            ResetearCuentaAtras();
        //Si algunos de nuestros dos booleans estan activos, es decir el minimo de la sala esta cubierto o esta llena la sala
        if (isReadyToStart)
        {
            fullGameTimer -= Time.deltaTime;
            timerToStartGame = fullGameTimer;
            cuentaAtrasText.text = "La partida empezara en: ";
        }else if (isReadyToCountDown)
        {
            notFullGameTimer -= Time.deltaTime;
            timerToStartGame = notFullGameTimer;
            cuentaAtrasText.text = "La partida empezara en: ";
        }

        //Formateando el string para que salga como segundo
        if (isReadyToStart || isReadyToCountDown)
        {
            string textSecondsCountDown = string.Format("{0:00}", timerToStartGame);
            cuentaAtrasText.text = cuentaAtrasText.text + textSecondsCountDown;
        }

        //Si la cuenta llega a 0 cargaremos el nivel online
        if(timerToStartGame <= 0f)
        {
            if (isStartingGame)
                return;
            StartGame();
        }

    }

    private void ResetearCuentaAtras()
    {
        //Reseteamos la cuenta atras, basicamente es asignar los valores del principio
        cuentaAtrasText.text = "Esperando jugadores...";
        fullGameTimer = maxFullRoomWaitTIme;
        notFullGameTimer = maxWaitTime;
        timerToStartGame = maxWaitTime;
    }

    private void StartGame()
    {
        isStartingGame = true;
        //Si no somos el host no hacemos nada
        if (!PhotonNetwork.IsMasterClient)
            return;
        PhotonNetwork.LoadLevel(multiplayerSceneIndex);
    }

    public void AbandonarSalaClicked()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(menuPrincipalSceneIndex);
    }


}