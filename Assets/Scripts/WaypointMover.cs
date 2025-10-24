using UnityEngine;
using System.Collections.Generic;

public class WaypointMover : MonoBehaviour
{
    [Header("Configuración de Ruta")]
    [SerializeField] private List<RectTransform> waypoints = new List<RectTransform>(); // Puntos de ruta UI
    [SerializeField] private float velocidad = 200f; // Velocidad de movimiento (píxeles por segundo)

    [Header("Modo de Recorrido")]
    [SerializeField] private ModoRecorrido modo = ModoRecorrido.Loop;

    [Header("Opciones Avanzadas")]
    [SerializeField] private float distanciaMinima = 5f; // Distancia para considerar que llegó al waypoint
    [SerializeField] private float tiempoEspera = 0f; // Tiempo de espera en cada waypoint
    [SerializeField] private AnimationCurve curvaVelocidad = AnimationCurve.Linear(0, 1, 1, 1); // Curva de velocidad
    [SerializeField] private bool usarPosicionesRelativas = true; // IMPORTANTE: Usar anchoredPosition en vez de position

    private RectTransform rectTransform;
    private int indiceActual = 0;
    private int direccion = 1; // 1 para adelante, -1 para atrás
    private float tiempoEsperaActual = 0f;
    private bool esperando = false;
    private List<Vector2> posicionesWaypoints = new List<Vector2>(); // Guardamos las posiciones relativas

    public enum ModoRecorrido
    {
        Loop,           // Vuelve al inicio cuando llega al final
        PingPong,       // Va y viene entre el primer y último punto
        OnceThenStop    // Recorre una vez y se detiene
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError("UIWaypointMover: Este script debe estar en un objeto UI con RectTransform!");
            enabled = false;
            return;
        }

        if (waypoints.Count == 0)
        {
            Debug.LogWarning("UIWaypointMover: No hay waypoints asignados!");
            enabled = false;
            return;
        }

        // CRÍTICO: Guardar las posiciones relativas de los waypoints al inicio
        // Esto evita problemas cuando el padre se mueve
        GuardarPosicionesWaypoints();

        // Coloca el objeto en el primer waypoint
        if (posicionesWaypoints.Count > 0)
        {
            rectTransform.anchoredPosition = posicionesWaypoints[0];
            Debug.Log($"[UIWaypointMover] '{name}' iniciado en waypoint 0: {posicionesWaypoints[0]}");
        }
    }

    void GuardarPosicionesWaypoints()
    {
        posicionesWaypoints.Clear();

        foreach (RectTransform waypoint in waypoints)
        {
            if (waypoint != null)
            {
                // Guardamos la posición anchoredPosition que es relativa al padre
                posicionesWaypoints.Add(waypoint.anchoredPosition);
                Debug.Log($"[UIWaypointMover] Waypoint '{waypoint.name}' guardado en posición: {waypoint.anchoredPosition}");
            }
            else
            {
                Debug.LogWarning("[UIWaypointMover] Un waypoint es null!");
                posicionesWaypoints.Add(Vector2.zero);
            }
        }
    }

    void Update()
    {
        if (posicionesWaypoints.Count == 0) return;

        // Si está esperando en un waypoint
        if (esperando)
        {
            tiempoEsperaActual -= Time.deltaTime;
            if (tiempoEsperaActual <= 0)
            {
                esperando = false;
                AvanzarAlSiguienteWaypoint();
            }
            return;
        }

        // Obtener posición objetivo (usando las posiciones guardadas)
        Vector2 objetivo = posicionesWaypoints[indiceActual];
        Vector2 posicionActual = rectTransform.anchoredPosition;

        // Calcular distancia al objetivo
        float distancia = Vector2.Distance(posicionActual, objetivo);

        // Si llegó al waypoint
        if (distancia < distanciaMinima)
        {
            if (tiempoEspera > 0)
            {
                esperando = true;
                tiempoEsperaActual = tiempoEspera;
            }
            else
            {
                AvanzarAlSiguienteWaypoint();
            }
            return;
        }

        // Mover hacia el objetivo con curva de velocidad
        float velocidadAjustada = velocidad * curvaVelocidad.Evaluate(Mathf.Clamp01(distancia / 100f));

        Vector2 nuevaPosicion = Vector2.MoveTowards(
            posicionActual,
            objetivo,
            velocidadAjustada * Time.deltaTime
        );

        rectTransform.anchoredPosition = nuevaPosicion;
    }

    private void AvanzarAlSiguienteWaypoint()
    {
        int indiceAnterior = indiceActual;

        switch (modo)
        {
            case ModoRecorrido.Loop:
                indiceActual = (indiceActual + 1) % posicionesWaypoints.Count;
                break;

            case ModoRecorrido.PingPong:
                indiceActual += direccion;

                // Cambiar dirección si llega a los extremos
                if (indiceActual >= posicionesWaypoints.Count)
                {
                    indiceActual = posicionesWaypoints.Count - 2;
                    direccion = -1;
                }
                else if (indiceActual < 0)
                {
                    indiceActual = 1;
                    direccion = 1;
                }
                break;

            case ModoRecorrido.OnceThenStop:
                indiceActual++;
                if (indiceActual >= posicionesWaypoints.Count)
                {
                    indiceActual = posicionesWaypoints.Count - 1;
                    enabled = false; // Detiene el script
                    Debug.Log($"[UIWaypointMover] '{name}' ha completado el recorrido y se detuvo");
                }
                break;
        }

        Debug.Log($"[UIWaypointMover] '{name}' avanzando: waypoint {indiceAnterior} → {indiceActual}");
    }

    // Método público para reiniciar el recorrido
    public void Reiniciar()
    {
        indiceActual = 0;
        direccion = 1;
        esperando = false;
        enabled = true;

        if (posicionesWaypoints.Count > 0)
        {
            rectTransform.anchoredPosition = posicionesWaypoints[0];
            Debug.Log($"[UIWaypointMover] '{name}' reiniciado");
        }
    }

    // Método para recalcular posiciones de waypoints (útil si se mueven en runtime)
    public void ActualizarPosicionesWaypoints()
    {
        GuardarPosicionesWaypoints();
        Debug.Log($"[UIWaypointMover] '{name}' posiciones de waypoints actualizadas");
    }

    // Método para cambiar la velocidad en runtime
    public void CambiarVelocidad(float nuevaVelocidad)
    {
        velocidad = nuevaVelocidad;
    }

    // Método para cambiar el modo de recorrido
    public void CambiarModo(ModoRecorrido nuevoModo)
    {
        modo = nuevoModo;
    }

    // Método para añadir un waypoint en runtime
    public void AgregarWaypoint(RectTransform nuevoWaypoint)
    {
        if (nuevoWaypoint != null && !waypoints.Contains(nuevoWaypoint))
        {
            waypoints.Add(nuevoWaypoint);
            posicionesWaypoints.Add(nuevoWaypoint.anchoredPosition);
            Debug.Log($"[UIWaypointMover] Waypoint '{nuevoWaypoint.name}' añadido");
        }
    }

    // Método para limpiar waypoints
    public void LimpiarWaypoints()
    {
        waypoints.Clear();
        posicionesWaypoints.Clear();
        Debug.Log($"[UIWaypointMover] Waypoints limpiados");
    }

    // Método para pausar/reanudar el movimiento
    public void SetPausado(bool pausado)
    {
        enabled = !pausado;
    }

    // Obtener información del estado actual
    public int GetIndiceActual() => indiceActual;
    public int GetTotalWaypoints() => posicionesWaypoints.Count;
    public bool EstaEsperando() => esperando;
}