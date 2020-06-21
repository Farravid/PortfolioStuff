using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioHat : MonoBehaviour
{
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colisiono con el player");
            player.GetComponent<Animator>().SetTrigger("voltereta");
        }
        //if (other.gameObject.CompareTag("Transformarse"))
        //{
            //Debug.Log("Collisiono con un elemento el cual me puedo transformar");
        //}
    }
}
