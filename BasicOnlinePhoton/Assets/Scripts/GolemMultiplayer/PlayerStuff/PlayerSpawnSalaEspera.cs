using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

/// <summary>
/// Este script maneja el spawn y la posicion del player a lo largo de la sala de espera.
/// Este script tiene que estar atachedo al prefab del player de la sala de espera, en este caso PlayerWaiting
/// </summary>
/// <author>David Martinez Garcia</author>

public class PlayerSpawnSalaEspera : MonoBehaviourPunCallbacks
{


    /// <summary>
    /// Debe haber el mismo numero de trasnfroms que de jugadoresMax para que todo funcione correctamente.
    /// Ademas los spawns se van nombrando en relacion a numeros ej: 0,1,2
    /// </summary>
    /// <author>David Martinez Garcia</author> 

    #region Variables

    [Tooltip("Numero maximo de jugadores que hay, que se correspondera cada uno con un spawn transform")]
    [SerializeField]
    private int jugadoresMax;

    //Lista de transforms de la sala de espera
    public List<Transform> spawnSalaTransforms;

    #endregion

    #region Callback Methods
    private void Start()
    {
        EstablecerTransforms();
    }

    /// <summary>
    /// Se activa cuando un jugador deja la sala.
    /// Se reordena la posicion de los jugadores en la sala de espera.
    /// </summary>
    /// <param name="otherPlayer">El jugador que deja la sala</param>
    /// <author>David Martinez Garcia</author>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int contadorPosicionPlayer = 1;

        Player[] playerList = PhotonNetwork.PlayerList;

        if (PhotonNetwork.CurrentRoom.PlayerCount > 0)
        {
            for (int i = 0; i < playerList.Length; i++)
            {
                if (otherPlayer.ActorNumber < PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    //Mover jugador
                    if (photonView.IsMine)
                    {
                        this.transform.position = ElegirSpawnSalaEspera(contadorPosicionPlayer).position;
                        contadorPosicionPlayer++;
                    }
                }
                else
                {
                    contadorPosicionPlayer++;
                }
            }
        }
    }

    #endregion

    #region Metodos Privados
    /// <summary>
    /// Recoge todos los transforms spawn de la escena, en relacion a los maximos jugadores que hay
    /// </summary>
    /// <author>David Martinez Garcia</author>
    private void EstablecerTransforms()
    {
        for(int i = 0; i < jugadoresMax; i++)
        {
            spawnSalaTransforms.Add(GameObject.Find(i.ToString()).transform);
        }
    }

    /// <summary>
    /// Selecciona el spawn correspondiente a cada jugador.
    /// </summary>
    /// <param name="numJugadores">Numero de jugadores actuales</param>
    /// <returns>El transform seleccionado al cual se va a mover el player</returns>
    /// <author>David Martinez Garcia</author>
    private Transform ElegirSpawnSalaEspera(int numJugadores)
    {
        return spawnSalaTransforms[numJugadores - 1];
    }
    #endregion
}
