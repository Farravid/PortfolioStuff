using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WeaponThrow : MonoBehaviour
{
    #region Variables
    //Rigidbody del arma para las fisicas y demas
    public Rigidbody rbWeapon;
    //Fuerza con la que lanza
    public float throwForce = 150f;

    //Para simular la curva que hace al coger el arma necesitamos 3 puntos, 3 vectores
    //Uno sera la ultima posicion del hacha antes de retornarla, otra sera la posicion del player y la otra sera el punto de la curva por donde queremos que pase
    private Vector3 oldWeaponPos;

    //Este sera el 3 punto de la curva de bezier
    public Transform curvePoint;


    //Objeto padre del hacha al principio
    public Transform padreWeapon;

    //Vamos a recoger la posicion inicila y la rotcaion incial del hacha para posteriormente resetarlas para que siempre vuelva igual
    private Vector3 originalPos;
    private Vector3 originalRot;

    //Sabemos si el arma esta volviendo
    private bool isReturning;
    //Sabemos si el arma ha sido lanzada
    private bool isThrow;
    //Tiempo de la ecuacion de bezier va entre 0 y 1 0 siendo el principio de la curva y 1 el final de la curva en volver el arma la utilizamos para la curva de bezier ya que es una variable del metodo
    private float time = 0.0f;

    //Referncia al script del arma para controlar sus movimientos rotaciones y colisiones
    public WeaponScript scriptWeapong;

    //Script de la camara que utilizamos para hacer vibraciones cuando el hacha llega
    public CinemachineImpulseSource impulsoCamara;

    //Trail del hacha, lo utilizaremos para activarlo y desactivarlo
    public TrailRenderer trailWeapon;

    #endregion

    void Start()
    {
        //Asignamos los angulos
        originalPos = rbWeapon.transform.localPosition;
        originalRot = rbWeapon.transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (isReturning)
        {
            trailWeapon.emitting = true;
            if (time < 1.0f)
            {
                //Calculamos la curva de bezier para que su posicion sea la que describe la curva
                //Voy a dejar aqui random el link de la wikipedia que lo explica https://en.wikipedia.org/wiki/B%C3%A9zier_curve
                rbWeapon.transform.position = curvaBezier3points(time, oldWeaponPos, curvePoint.position, padreWeapon.position);
                //De esta manera el hacha rotara de una manera suave cuando venga
                //rbWeapon.rotation = Quaternion.Slerp(rbWeapon.transform.rotation, padreWeapon.rotation, 50 * Time.deltaTime);
                time += Time.deltaTime;

            }
            else
            {
                resetArma();
            }

        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && isThrow && !isReturning)
        {
            vueltaArma();
        }
    }

    public bool getThrow()
    {
        return isThrow;
    }
    public bool getReturning()
    {
        return isReturning;
    }

    /// <summary>
    /// El arma es lanzada, este metodo va enlazado a una animacion para que se lance el hacha a la vez que se realiza una animacion para que sea concorde
    /// Establecemos las caracteristicas propias del arma y ademas la rotamos a;adiendo una fuerza a su rigidbody en la direccion que este apuntnado la camara
    /// Ademas la rotamos para que vaya recta primero siempre
    /// </summary>
    public void throwArma()
    {
        rbWeapon.isKinematic = false;
        rbWeapon.gameObject.GetComponent<BoxCollider>().isTrigger = false;
        isReturning = false;
        isThrow = true;
        //El hacha no sera padre de ningun objeto para que no dependa de nadie y no interfiera en su rotacion o movimiento
        rbWeapon.transform.parent = null;
        //El arma se tira como le da la gana segun la posicion de la mano asique vamos a hacer que vaya recta de esta manera
        rbWeapon.transform.eulerAngles = new Vector3(0, -90 + transform.eulerAngles.y, 0);
        rbWeapon.transform.position += transform.right / 5;
        //Conseguimos que el hacha vaya hacia la direccion que hemos puesto, en este caso sera la direccion de la camara forward pero convertida en un punto global
        rbWeapon.AddForce(Camera.main.transform.forward * throwForce + transform.up * 2, ForceMode.Impulse);

        //Trail
        trailWeapon.emitting = true;
    }

    /// <summary>
    /// Es cuando el arma esta volviendo a la mano del personaje, aqui cojemos la ultima posicion que ha tomado ademas de establecer el time a 0 para poder realizar la curva de bezier de 0 a 1
    /// </summary>
    public void vueltaArma()
    {
        time = 0.0f;
        oldWeaponPos = rbWeapon.transform.position;
        isReturning = true;
        rbWeapon.velocity = Vector3.zero;
        rbWeapon.isKinematic = true;
        scriptWeapong.setEnMovimiento(true);
    }

    /// <summary>
    /// Este metodo resetea el arma una vez llega a nuestra mano, establece las propiedades de la weapon a las que tenia previamente a ser lanzada de esa manera conseguimos un movimiento realista
    /// Ademas a;ade un efecto de impulso a la camara muy util
    /// </summary>
    public void resetArma()
    {

        isThrow = false;
        isReturning = false;
        rbWeapon.transform.parent= padreWeapon.transform;
        rbWeapon.transform.localPosition = originalPos;
        rbWeapon.transform.localEulerAngles = originalRot;
        rbWeapon.transform.gameObject.GetComponent<BoxCollider>().isTrigger = true;
        //El efecto de coger el hacha genera un impulso en la camara que hace que se mueva un poco
        impulsoCamara.GenerateImpulse(Vector3.forward);

        //Trail
        trailWeapon.emitting = false;
    }


    public void setTrailEmitting(bool t)
    {
        trailWeapon.emitting = t;
    }

    /// <summary>
    /// Este metodo es el mas importante de toda la mecanica, lo que hace es la interpolacion entre tres puntos creando asi una curva de bezier con los puntos de control
    /// Necesitamos la pos0 que sera la posicion final del objeto, la pos1 que sera el segundo punto de control y el que dara la curvatura a nuestra curva y el ultimo punto es donde queremos que regrese el objeto
    /// El tiempo es un damping entre 0 y 1 que debe ser progresivo
    /// </summary>
    /// <param name="t">El tiempo es un damping entre 0 y 1 que debe ser progresivo</param>
    /// <param name="pos0">Posicion final del objeto</param>
    /// <param name="pos1">Posicion del punto de curvatura</param>
    /// <param name="pos2">Posicion de regreso del objeto</param>
    /// <returns></returns>
    Vector3 curvaBezier3points(float t, Vector3 pos0, Vector3 pos1, Vector3 pos2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = (uu * pos0) + (2 * u * t * pos1) + (tt * pos2);
        return p;
    }

}
