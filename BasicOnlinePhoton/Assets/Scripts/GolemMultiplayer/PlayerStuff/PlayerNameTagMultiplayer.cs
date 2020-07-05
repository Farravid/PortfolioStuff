using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.UI;

/// <summary>
/// Este script controla el nombre del jugador en el juego online y en la sala de estar.
/// Debe ir atacheado al prefab del jugador que queramos ponerle el nombre.
/// En nuestro caso, va atacheado tanto al jugador de la escena online como al de la sala de estar
/// </summary>
/// <author>David Martinez Garcia</author>

public class PlayerNameTagMultiplayer : MonoBehaviourPun
{
    [SerializeField] private Text nameTagText; 

    private void Start()
    {
        //Si el nametag es el mio lo destacamos de color rojo
        if (photonView.IsMine) {
            SetName(Color.red);
        }
        //Los nombres de los demas jugadores seran negros
        else
        {
            SetName(Color.black);
        }
    }

    private void LateUpdate()
    {
        //Tenemos que hacer eque el tag siempr mire hacia la camara
        //De esta manera el tag del jugador nos seguira
        nameTagText.gameObject.transform.LookAt(nameTagText.gameObject.transform.position + Camera.main.transform.rotation * Vector3.forward,Camera.main.transform.rotation * Vector3.up);

    }

    /// <summary>
    /// Establcece el color del tag del player
    /// </summary>
    /// <param name="color"></param>
    /// <author>David Martinez Garcia</author>
    private void SetName(Color color) {
        nameTagText.text = photonView.Owner.NickName;
        nameTagText.color = color;
    }
}
