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

    [Tooltip("The prefab to use for representing the name of the player")]
    public GameObject playerTagPrefab;

    public static GameObject playerTagInstance;

    public List<Transform> spawnSalaTransforms;

    private int numeroJugadores;

    public List<GameObject> jugadores;

    private GameObject instancePlayerParado;

    #region Callbacks Methods

    private void Start()
    {
        //InstanciarNombreJugador();
        InstanciarJugadorParado();
    }

   //private void InstanciarNombreJugador()
    //{
      //  playerTagInstance = (GameObject)PhotonNetwork.Instantiate(this.playerTagPrefab.name, Vector3.zero, Quaternion.identity, 0);
    //}


    /*private void Update()
    {
        ContarJugadores();
    }

    private void ContarJugadores()
    {
        numeroJugadores = PhotonNetwork.CurrentRoom.PlayerCount;
        switch (numeroJugadores)
        {
            case 1:
                jugadores[0].SetActive(true);
                jugadores[1].SetActive(false);
                jugadores[2].SetActive(false);
                jugadores[3].SetActive(false);
                break;
            case 2:
                jugadores[0].SetActive(true);
                jugadores[1].SetActive(true);
                jugadores[2].SetActive(false);
                jugadores[3].SetActive(false);
                break;
            case 3:
                jugadores[0].SetActive(true);
                jugadores[1].SetActive(true);
                jugadores[2].SetActive(true);
                jugadores[3].SetActive(false);
                break;
            case 4:
                jugadores[0].SetActive(true);
                jugadores[1].SetActive(true);
                jugadores[2].SetActive(true);
                jugadores[3].SetActive(true);
                break;
        }
    }*/

    



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
