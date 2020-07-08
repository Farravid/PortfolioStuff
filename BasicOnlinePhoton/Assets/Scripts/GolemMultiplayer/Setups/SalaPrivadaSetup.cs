using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

/// <summary>
/// Este script establece la disposicion de los elementos de la sala privada.
/// Es decir, prepara la sala privada.
/// Este script debe ir atacheado a un objeto en la escena de la sala privada
/// Este objeto sera el que maneje el setup de la sala, ej: SalaPrivadaSetup como objeto
/// </summary>
/// <author>David Martinez Garcia</author>


public class SalaPrivadaSetup : MonoBehaviourPunCallbacks
{
    #region Variables
    [Tooltip("Prefab usado para representar al playerParado")]
    public GameObject playerPrefabQuieto;

    [Tooltip("Lista de transforms que sera donde spawnen nuestros personajes")]
    public List<Transform> spawnSalaTransforms;

    //Numero de jugadores
    private int numeroJugadores;

    //Boton de comenzar partida
    [SerializeField]
    private Button comenzarPartidaButton;
    #endregion

    #region Callbacks Methods

    private void Start() => InstanciarJugadorParado();

    private void Update() => SetButtonComenzar();



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
        PhotonNetwork.Instantiate(this.playerPrefabQuieto.name, transformElegido.position, transformElegido.rotation, 0);
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

    /// <summary>
    /// Segun si somos el host o no de la partida, tendremos disponible el boton de empezar la partida
    /// </summary>
    /// <author>David Martinez Garcia</author>
    private void SetButtonComenzar()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            comenzarPartidaButton.gameObject.SetActive(true);
        }
    }

    #endregion
}
