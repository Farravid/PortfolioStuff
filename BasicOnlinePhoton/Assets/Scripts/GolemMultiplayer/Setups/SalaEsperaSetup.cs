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
/// <author>David Martinez Garcia</author>

public class SalaEsperaSetup : MonoBehaviourPunCallbacks
{

    /// <summary>
    /// Este script debe ir atacheado a un objeto en la escena de la sala de espera.
    /// Este objeto sera el que maneje el setup de la sala, ej: SalaEsperaSetup como objeto
    /// </summary>
    /// <author>David Martinez Garcia</author>

    #region Variables
    [Tooltip("Prefab usado para representar al playerParado")]
    public GameObject playerPrefabQuieto;

    [Tooltip("Lista de transforms que sera donde spawnen nuestros personajes")]
    public List<Transform> spawnSalaTransforms;

    //Numero de jugadores
    private int numeroJugadores;

    //Instancia de cada player que hemos creado
    private GameObject instancePlayerParado;
    #endregion

    #region Callbacks Methods

    private void Start()
    {
        InstanciarJugadorParado();
    }

    #endregion


    #region Metodos privados

    /// <summary>
    /// Instancia un jugador parado por cada jugador que entra en la partida
    /// </summary>
    /// <author>David Martinez Garcia</author>
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
    /// <returns>El transform seleccionado donde se teletransportara el personaje</returns>
    /// <author>David Martinez Garcia</author>
    private Transform ElegirSpawnSalaEspera(int numJugadores)
    {

        return spawnSalaTransforms[numJugadores - 1];
    }

    #endregion
}
