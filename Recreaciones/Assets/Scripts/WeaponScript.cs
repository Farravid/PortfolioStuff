using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public bool enMovimiento = true;
    public float rotateSpeed = -1800f;
    public WeaponThrow weaponThrowScript;



    void Update()
    {
        // Lo que hacemos es que cuando el arma esta en movimiento es decir o en la ida o en la vuelta la giramos para que de ese efecto de vuelta
        if (enMovimiento && weaponThrowScript.getThrow())
        {
            transform.eulerAngles += Vector3.forward * rotateSpeed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Capa de ground
        if (collision.gameObject.layer == 8)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            enMovimiento = false;
            weaponThrowScript.setTrailEmitting(false); 
        }
    }

    public void setEnMovimiento(bool b)
    {
        enMovimiento = b;
    }

    public bool getEnMovimiento()
    {
        return enMovimiento;
    }
}
