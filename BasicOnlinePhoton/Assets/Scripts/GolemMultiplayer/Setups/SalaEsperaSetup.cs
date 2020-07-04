using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Cinemachine;

public class SalaEsperaSetup : MonoBehaviourPunCallbacks
{
    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefabQuieto;

    public List<Transform> spawnSalaTransforms;

    private int numeroJugadores;

    private GameObject instancePlayerParado;

    #region Callbacks Methods

    private void Start()
    {
        InstanciarJugadorParado();
    }

    private void InstanciarJugadorParado()
    {
        numeroJugadores = PhotonNetwork.CurrentRoom.PlayerCount;
        Transform transformElegido = ElegirSpawnSalaEspera(numeroJugadores);
        instancePlayerParado = (GameObject)PhotonNetwork.Instantiate(this.playerPrefabQuieto.name, transformElegido.position, transformElegido.rotation, 0);
    }

    private Transform ElegirSpawnSalaEspera(int numJugadores) {

       return spawnSalaTransforms[numJugadores - 1];

    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount > 0)
            instancePlayerParado.transform.position = ElegirSpawnSalaEspera(PhotonNetwork.CurrentRoom.PlayerCount).position;
    }


    #endregion


    #region Metodos privados

    #endregion
}
