using UnityEngine;

public class FloatingButton : MonoBehaviour
{
    [Header("Configuración de Flotación")]
    [SerializeField] private float amplitud = 10f; // Distancia del movimiento arriba/abajo (en píxeles para UI)
    [SerializeField] private float velocidad = 1f; // Velocidad de la flotación
    [SerializeField] private float desfaseInicial = 0f; // Para que múltiples botones no floten sincronizados

    [Header("Configuración UI")]
    [SerializeField] private bool esElementoUI = true; // Si es un elemento UI (Canvas)

    private RectTransform rectTransform;
    private Transform regularTransform;
    private Vector2 posicionInicialUI;
    private Vector3 posicionInicial3D;
    private float tiempo;
    private bool usaRectTransform;

    void Start()
    {
        // Detectar si es un elemento UI o un objeto regular
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null && esElementoUI)
        {
            // Es un elemento UI
            usaRectTransform = true;
            posicionInicialUI = rectTransform.anchoredPosition;
            Debug.Log($"[FloatingButton] '{name}' configurado para UI - Posición inicial: {posicionInicialUI}");
        }
        else
        {
            // Es un objeto regular 3D/2D
            usaRectTransform = false;
            regularTransform = transform;
            posicionInicial3D = regularTransform.localPosition;
            Debug.Log($"[FloatingButton] '{name}' configurado para objeto regular - Posición inicial: {posicionInicial3D}");
        }

        tiempo = desfaseInicial;
    }

    void Update()
    {
        tiempo += Time.deltaTime * velocidad;

        // Calcula el desplazamiento vertical usando una función seno para movimiento suave
        float offsetY = Mathf.Sin(tiempo) * amplitud;

        // Aplica el movimiento según el tipo de objeto
        if (usaRectTransform)
        {
            // Para elementos UI - usa anchoredPosition (posición relativa al padre)
            rectTransform.anchoredPosition = posicionInicialUI + new Vector2(0, offsetY);
        }
        else
        {
            // Para objetos regulares - usa localPosition
            regularTransform.localPosition = posicionInicial3D + new Vector3(0, offsetY, 0);
        }
    }

    // Método para cambiar la amplitud en runtime
    public void CambiarAmplitud(float nuevaAmplitud)
    {
        amplitud = nuevaAmplitud;
    }

    // Método para cambiar la velocidad en runtime
    public void CambiarVelocidad(float nuevaVelocidad)
    {
        velocidad = nuevaVelocidad;
    }

    // Método para reiniciar la posición
    public void ReiniciarPosicion()
    {
        tiempo = desfaseInicial;

        if (usaRectTransform)
        {
            rectTransform.anchoredPosition = posicionInicialUI;
        }
        else
        {
            regularTransform.localPosition = posicionInicial3D;
        }
    }

    // Método para pausar/reanudar la flotación
    public void SetPausado(bool pausado)
    {
        enabled = !pausado;
    }
}