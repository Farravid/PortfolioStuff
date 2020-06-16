using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script que se attachea a los objetos que no estan rotos para posteriormente si el weapon colisiona con estse cambiarlo por su prefab que si este roto de
/// esta manera conseguimos el efecto de romper cosas que vemos en la escena
/// </summary>

public class Destruccion : MonoBehaviour
{
    //Todos los objetos y todas las partes del objeto tiene que tener rigidbody, ademas lo del roto sera un mesh collider con la malla del objeto y ser convexo

    //Mismo objetjo pero hecho de pedazos
    public GameObject brokenObject;
    private void OnCollisionEnter(Collision collision)
    {
        //El tag que lleve el arma
        if (collision.gameObject.CompareTag("Axe"))
        {
            Instantiate(brokenObject, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
