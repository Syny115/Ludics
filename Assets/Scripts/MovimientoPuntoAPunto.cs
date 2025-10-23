using UnityEngine;
using System.Collections;

public class MovimientoPuntoAPunto : MonoBehaviour
{
    [Header("Configuración de Puntos")]
    [SerializeField] private Transform[] puntos; // Array de puntos de destino
    [SerializeField] private bool bucleInfinito = true; // Si debe volver al inicio
    [SerializeField] private bool iniciarAutomaticamente = true;

    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private AnimationCurve curvaMovimiento = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Opciones Adicionales")]
    [SerializeField] private float tiempoEsperaEnPunto = 0f; // Tiempo de pausa en cada punto
    [SerializeField] private bool mirarHaciaDireccion = false; // Rotar hacia la dirección del movimiento

    private int indicePuntoActual = 0;
    private bool estaMoviendo = false;

    void Start()
    {
        if (puntos.Length == 0)
        {
            Debug.LogError("No hay puntos asignados al array de puntos!");
            return;
        }

        // Posicionar en el primer punto
        transform.position = puntos[0].position;

        if (iniciarAutomaticamente)
        {
            IniciarMovimiento();
        }
    }

    public void IniciarMovimiento()
    {
        if (!estaMoviendo && puntos.Length > 1)
        {
            StartCoroutine(MoverEntrePuntos());
        }
    }

    public void DetenerMovimiento()
    {
        estaMoviendo = false;
        StopAllCoroutines();
    }

    public void ReiniciarMovimiento()
    {
        DetenerMovimiento();
        indicePuntoActual = 0;
        transform.position = puntos[0].position;
        IniciarMovimiento();
    }

    private IEnumerator MoverEntrePuntos()
    {
        estaMoviendo = true;

        while (estaMoviendo)
        {
            // Calcular el siguiente punto
            int siguienteIndice = (indicePuntoActual + 1) % puntos.Length;

            // Si no es bucle y llegamos al final, detener
            if (!bucleInfinito && siguienteIndice == 0)
            {
                estaMoviendo = false;
                yield break;
            }

            Vector3 puntoInicio = puntos[indicePuntoActual].position;
            Vector3 puntoDestino = puntos[siguienteIndice].position;
            float distancia = Vector3.Distance(puntoInicio, puntoDestino);
            float duracion = distancia / velocidad;

            // Mover hacia el siguiente punto
            yield return StartCoroutine(MoverHaciaPunto(puntoInicio, puntoDestino, duracion));

            // Actualizar índice
            indicePuntoActual = siguienteIndice;

            // Esperar en el punto si está configurado
            if (tiempoEsperaEnPunto > 0)
            {
                yield return new WaitForSeconds(tiempoEsperaEnPunto);
            }
        }
    }

    private IEnumerator MoverHaciaPunto(Vector3 inicio, Vector3 destino, float duracion)
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float progreso = tiempoTranscurrido / duracion;

            // Aplicar la curva de animación
            float progresoConCurva = curvaMovimiento.Evaluate(progreso);

            // Interpolar la posición
            transform.position = Vector3.Lerp(inicio, destino, progresoConCurva);

            // Rotar hacia la dirección si está activado
            if (mirarHaciaDireccion)
            {
                Vector3 direccion = destino - inicio;
                if (direccion != Vector3.zero)
                {
                    Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 5f);
                }
            }

            yield return null;
        }

        // Asegurar que llegue exactamente al destino
        transform.position = destino;
    }

    // Método para añadir un punto programáticamente
    public void AñadirPunto(Transform nuevoPunto)
    {
        Transform[] nuevoArray = new Transform[puntos.Length + 1];
        puntos.CopyTo(nuevoArray, 0);
        nuevoArray[puntos.Length] = nuevoPunto;
        puntos = nuevoArray;
    }

    // Método para cambiar la velocidad en runtime
    public void CambiarVelocidad(float nuevaVelocidad)
    {
        velocidad = nuevaVelocidad;
    }

    // Visualizar los puntos y el recorrido en el editor
    void OnDrawGizmos()
    {
        if (puntos == null || puntos.Length == 0)
            return;

        // Dibujar los puntos
        Gizmos.color = Color.green;
        for (int i = 0; i < puntos.Length; i++)
        {
            if (puntos[i] != null)
            {
                Gizmos.DrawSphere(puntos[i].position, 0.3f);
            }
        }

        // Dibujar las líneas entre puntos
        Gizmos.color = Color.yellow;
        for (int i = 0; i < puntos.Length - 1; i++)
        {
            if (puntos[i] != null && puntos[i + 1] != null)
            {
                Gizmos.DrawLine(puntos[i].position, puntos[i + 1].position);
            }
        }

        // Si es bucle, conectar el último con el primero
        if (bucleInfinito && puntos.Length > 1 && puntos[0] != null && puntos[puntos.Length - 1] != null)
        {
            Gizmos.DrawLine(puntos[puntos.Length - 1].position, puntos[0].position);
        }
    }
}