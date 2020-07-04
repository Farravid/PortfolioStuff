using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Cinemachine;

public class GameSetup : MonoBehaviourPunCallbacks
{
    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;

    #region Callbacks Methods

    private void Start()
    {
        if (PlayerController.LocalPlayerInstance == null)
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        }
        else
        {
            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }
    }

    private void Update()
    {
        DestruirCopiaPlayer();
    }

    private void DestruirCopiaPlayer()
    {
        GameObject destuir = GameObject.FindWithTag("PlayerWaiting");
        if (destuir != null)
        {
            Debug.Log("Destuyo");
            Destroy(destuir);
        }
    }



    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Este metodo comprueba cuando un jugador se conecta a la sala.
    /// En caso de este jugador ser el host de la sala se creara el juego, si no lo es, tan solo se unira
    /// </summary>
    /// <param name="other"></param>
    public override void OnPlayerEnteredRoom(Player other)
    {

    }

    /// <summary>
    /// Este metodo comprueba cuando un jugador deja la sala
    /// En caso de que el jugador que haya dejado la sala sea el host de la partida, la partida se volvera a cargar estableciendo un nuevo host
    /// </summary>
    /// <param name="other"></param>
    public override void OnPlayerLeftRoom(Player other)
    {
        //Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
        //if (PhotonNetwork.IsMasterClient)
        //{
          //  LoadNivelOnline();
        //}
    }


    #endregion

    #region UI Methods

    public void OnSalirSalaClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Metodos privados


    /// <summary>
    /// Si somos el host, cargamos el nivel correspondiente
    /// </summary>
    /*void LoadNivelOnline()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(1);
        else
            return;
    }*/

    #endregion
}
