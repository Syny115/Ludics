using UnityEngine;

public class Pulsate : MonoBehaviour
{
    [Header("Configuración del efecto")]
    public float speed = 2f;       // Velocidad de expansión/contracción
    public float scaleAmount = 0.2f; // Cuánto se expande (20% más)
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale; // Guarda el tamaño original
    }

    void Update()
    {
        // Calcula el factor de expansión usando una onda senoidal
        float scaleFactor = 1 + Mathf.Sin(Time.time * speed) * scaleAmount;

        // Aplica el nuevo tamaño
        transform.localScale = initialScale * scaleFactor;
    }
}
