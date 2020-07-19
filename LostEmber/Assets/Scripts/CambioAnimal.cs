using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CambioAnimal : MonoBehaviour
{
    [SerializeField] private float distanciaCambio;
    [SerializeField] private CinemachineFreeLook camaraAnimal;

    [SerializeField] private Transform playerTransform; 

    private GameObject animalElegido;

    private void Start()
    {
        camaraAnimal.Priority = 1;
    }


    // Update is called once per frame
    void Update()
    {
        BuscarAnimal();
        VueltaHumano();
    }

    private void VueltaHumano()
    {
        //Lo que significa que el animal puede volver a estado 
        if(this.gameObject.tag!= "Player")
        {
            //Si somos un animal y pulsamos z volvemos al humano
            if (Input.GetKeyDown(KeyCode.Z))
            {

                MonoBehaviour[] componentesAnimalPropio = this.gameObject.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour c in componentesAnimalPropio)
                {
                    c.enabled = false;
                }

                this.gameObject.GetComponent<Animator>().enabled = true;
                this.gameObject.GetComponent<Collider>().enabled = true;
                this.gameObject.GetComponent<CharacterController>().enabled = true;


                playerTransform.gameObject.SetActive(true);
                playerTransform.position = new Vector3(0f, 3f, 0f);
            }
        }
    }

    private void BuscarAnimal()
    {
        float distanciaMenor = Mathf.Infinity;


        GameObject[] posiblesAnimales = GameObject.FindGameObjectsWithTag("Cambiable");
        if (posiblesAnimales != null)
        {
            foreach(GameObject animal in posiblesAnimales)
            {
                float dis = Vector3.Distance(this.transform.position, animal.transform.position);
                Debug.Log("Distancia: " + dis);
                //Debug.Log("Distancia entre yo y " + animal.gameObject.name + " es " + dis);
                //El nombre del animal tiene que ser diferente al nuestro para que la distancia no sea 0 
                if (dis <= distanciaCambio)
                {
                    if (animal.gameObject.name != this.gameObject.name)
                    {
                        Debug.Log("entro aqui que pasa");
                        if (dis <= distanciaMenor)
                        {
                            Debug.Log("Aqui entro y el animal pasa a no ser null pq no");

                            animalElegido = animal;
                            distanciaMenor = dis;
                        }
                    }
                }
            }



            //Si existe animal al que intercambiarse
            if (animalElegido != null)
            {
                //Deberemos mostrar un hud
                Debug.Log("Permitido el campo");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ///Todo esto pasarlo a un metodo lo que hay dentro de cuadno pulsamos E

                    CambiarAnimal();
                }
            }

            


        }
    }

    private void CambiarAnimal()
    {


        camaraAnimal.Priority = -1;
        //Aqui falta lo de ir cambiando de camaras de script y demas y tal pascualo
        //tambien deveremos hacer desaparecer el personaje



        //Si el objeto que controlamos es el jugador lo ponemos Active false, si es un animal deberemos activar otra vez el movimiento aleatorio
        if (this.gameObject.tag == "Player")
        {
            this.gameObject.SetActive(false);
            MonoBehaviour[] componentes = animalElegido.gameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour c in componentes)
            {
                c.enabled = true;
            }
        }
        else
        {
            //Si pasamos de un animal a otro debemos hacer varias cosas

            //Y del animal que estamos manejando debemos desactivar el controller y activar el path random automatico
            MonoBehaviour[] componentesAnimalPropio = this.gameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour c in componentesAnimalPropio)
            {
                c.enabled = false;
            }

            this.gameObject.GetComponent<Animator>().enabled = true;
            this.gameObject.GetComponent<Collider>().enabled = true;
            this.gameObject.GetComponent<CharacterController>().enabled = true;



            //Establecemos todos como activos del animal que vamos a pasar 
            MonoBehaviour[] componentesAnimalCambio = animalElegido.gameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour c in componentesAnimalCambio)
            {
                c.enabled = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, distanciaCambio);
    }

}
