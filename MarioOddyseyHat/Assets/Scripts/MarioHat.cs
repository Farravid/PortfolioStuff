using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioHat : MonoBehaviour
{
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colisiono con el player");
            player.GetComponent<Animator>().SetTrigger("voltereta");
        }
    }
}
