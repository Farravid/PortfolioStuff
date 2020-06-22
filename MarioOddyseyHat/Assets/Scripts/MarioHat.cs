using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioHat : MonoBehaviour
{
    public GameObject player;
    public MarioController marioController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.GetComponent<Animator>().SetTrigger("voltereta");
        }
        if (other.gameObject.CompareTag("Transformarse"))
        {
            Debug.Log("Colisiono con alguien que se puede transformar");
            marioController.setTransicion(true,other.gameObject);
            other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            other.gameObject.GetComponent<TargetController>().enabled = true;

        }
    }
}
