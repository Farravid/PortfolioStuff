using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputMultiplayer : MonoBehaviour
{
    [SerializeField] private InputField namePlayerInput = null;
    [SerializeField] private Button unirsePartidaRapidaButton = null;

    private const string PlayerPrefsNameKey = "PlayerName";

    private void Start()
    {
        EstablecerNombreJugador();
    }

    private void EstablecerNombreJugador()
    {
        //Si nunca hemos establecido el nombre a nuestro player, no va a hacer nada y el nombre srea el predefinido
        if(!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

        //Si no se returnea si que existse por lo tanto
        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

        namePlayerInput.text = defaultName;
        PermitirInicioInputField(defaultName);
    }

    //Este metodo va en el onchange del input field del nombre en modo dinamico
    public void PermitirInicioInputField(string defaultName)
    {
        //Con esto estamos forzando al jugador a poner al menos un nombre para poder empezar a jugar
        unirsePartidaRapidaButton.interactable = !string.IsNullOrEmpty(defaultName);
    }

    //Este metodo va cuando se le de al boton de inciar una partida
    public void SavePlayerNameWhenStartClicked()
    {
        string playerName = namePlayerInput.text;

        PhotonNetwork.NickName = playerName;
        PlayerPrefs.SetString(PlayerPrefsNameKey, playerName);
    }
}
