using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Este script maneja el input field del menu principal del jugador, para despues asignarle el nombre a la tag del nombre del player
/// Ademas, contiene una especie de DB que almacena el ultimo nombre que te has puesto de tal manera que cuando inicias el juego se te establce por defecto, con posibilidad de cambiarlo.
/// Este script debe ir atacheado al elemento Input Field del nombre del jugador del menu principal.
/// </summary>

public class PlayerInputMultiplayer : MonoBehaviour
{
    //Input field del nombre del jugador
    [SerializeField] private InputField namePlayerInput = null;
    //Boton de unirse a partida rapida
    [SerializeField] private Button unirsePartidaRapidaButton = null;
    [SerializeField] private Button crearSalaPrivadaButton = null;


    private const string PlayerPrefsNameKey = "PlayerName";

    private void Start()
    {
        EstablecerNombreJugador();
    }

    /// <summary>
    /// Si existia ya el nombre del jugador en algun registro se establece este
    /// </summary>
    /// <author>David Martinez Garcia</author>
    private void EstablecerNombreJugador()
    {
        //Si nunca hemos establecido el nombre a nuestro player, no va a hacer nada y el nombre srea el predefinido
        if(!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }

        //Si no se returnea si que existse por lo tanto
        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

        namePlayerInput.text = defaultName;
        PermitirInicioInputField(defaultName);
    }

    /// <summary>
    /// Este metodo se coloca en el onchange del campo InputField.
    /// Permite o no empezar el juego dependiendo si el nombre es null o no
    /// </summary>
    /// <param name="defaultName"></param>
    public void PermitirInicioInputField(string defaultName)
    {
        //Con esto estamos forzando al jugador a poner al menos un nombre para poder empezar a jugar
        unirsePartidaRapidaButton.interactable = !string.IsNullOrEmpty(defaultName);
        crearSalaPrivadaButton.interactable = !string.IsNullOrEmpty(defaultName);
    }

    /// <summary>
    /// Una vez se le da a inciar el juego, este metodo se encarga de guardar en la DB el nombre que ha elegido el jugador.
    /// Este metodo se inicia cuando se pulsa cualquier boton y accede a una sala.
    /// Este metodo debe estar en todos los onclick que den lugar a una nueva sala o a entrar en una sala
    /// </summary>
    /// <author>David Martinez Garcia</author>
    public void SavePlayerNameWhenStartClicked()
    {
        string playerName = namePlayerInput.text;

        PhotonNetwork.NickName = playerName;
        PlayerPrefs.SetString(PlayerPrefsNameKey, playerName);
    }
}
