using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.UI;

public class PlayerNameTagMultiplayer : MonoBehaviourPun
{
    [SerializeField] private Text nameTagText; 

    private void Start()
    {
        if (photonView.IsMine) {
            SetName(Color.red);
        }
        else
        {
            SetName(Color.black);
        }
    }

    private void LateUpdate()
    {
        //Tenemos que hacer eque el tag siempr mire hacia la camara
        nameTagText.gameObject.transform.LookAt(nameTagText.gameObject.transform.position + Camera.main.transform.rotation * Vector3.forward,Camera.main.transform.rotation * Vector3.up);

    }

    private void SetName(Color color) {
        nameTagText.text = photonView.Owner.NickName;
        nameTagText.color = color;
    }
}
