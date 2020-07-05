using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Cinemachine;

/// <summary>
/// Este script establece la disposicion de los elementos de la sala de espera.
/// Es decir, prepara la sala de espera
/// </summary>

public class SalaEsperaSetup : MonoBehaviourPunCallbacks
{
    [Tooltip("Prefab usado para representar al playerParado")]
    public GameObject playerPrefabQuieto;

    [Tooltip("Lista de transforms que sera donde spawnen nuestros personajes")]
    public List<Transform> spawnSalaTransforms;

    //Numero de jugadores
    private int numeroJugadores;

    //Instancia de cada player que hemos creado
    private GameObject instancePlayerParado;

    #region Callbacks Methods

    private void Start()
    {
        InstanciarJugadorParado();
    }


    /// <summary>
    /// Metodo que se activa cuando un jugador deja la sala.
    /// En nuestro caso lo que hacemos es mover a los jugadores de manera que si alguien se sale, todos los jugadores pasaran a lo posicion anterior.
    /// </summary>
    /// <param name="otherPlayer">El jugador que deja la sala</param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //1 4 6 7

        //1 4   7 
        

        int contadorPosicionPlayer = 1;
        Player[] playerList = PhotonNetwork.PlayerList;
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            for (int i = 0; i < playerList.Length; i++)
            {
                if (otherPlayer.ActorNumber < playerList[i].ActorNumber)
                {
                    //Mover jugador
                    if (instancePlayerParado.GetPhotonView().IsMine)
                    {
                        instancePlayerParado.transform.position = ElegirSpawnSalaEspera(contadorPosicionPlayer).position;
                        contadorPosicionPlayer++;
                    }
                }
                else
                {
                    contadorPosicionPlayer++;
                }
            }
        }else if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            instancePlayerParado.transform.position = ElegirSpawnSalaEspera(1).position;
        }
            
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.ActorNumber);
        
    }


    #endregion


    #region Metodos privados

    /// <summary>
    /// Instancia un jugador parado por cada jugador que entra en la partida
    /// </summary>
    private void InstanciarJugadorParado()
    {
        numeroJugadores = PhotonNetwork.CurrentRoom.PlayerCount;
        Transform transformElegido = ElegirSpawnSalaEspera(numeroJugadores);
        instancePlayerParado = (GameObject)PhotonNetwork.Instantiate(this.playerPrefabQuieto.name, transformElegido.position, transformElegido.rotation, 0);
    }

    /// <summary>
    /// Selecciona el spawn correspondiente a cada jugador.
    /// </summary>
    /// <param name="numJugadores">Numero de jugadores actuales</param>
    /// <returns></returns>
    private Transform ElegirSpawnSalaEspera(int numJugadores)
    {

        return spawnSalaTransforms[numJugadores - 1];

    }

    #endregion
}
