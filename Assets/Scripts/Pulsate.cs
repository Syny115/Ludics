using UnityEngine;

public class Pulsate : MonoBehaviour
{
    [Header("Configuraci�n del efecto")]
    public float speed = 2f;       // Velocidad de expansi�n/contracci�n
    public float scaleAmount = 0.2f; // Cu�nto se expande (20% m�s)
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale; // Guarda el tama�o original
    }

    void Update()
    {
        // Calcula el factor de expansi�n usando una onda senoidal
        float scaleFactor = 1 + Mathf.Sin(Time.time * speed) * scaleAmount;

        // Aplica el nuevo tama�o
        transform.localScale = initialScale * scaleFactor;
    }
}
