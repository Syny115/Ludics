using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

[Serializable]
public class PreguntaGenerica
{
    public string textoPregunta;
    public Sprite imagenBoton1;
    public Sprite imagenBoton2;
    public bool boton1EsCorrecto; // true si el bot�n 1 es correcto, false si es el bot�n 2
}

public class MG_Preguntas : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI textoPreguntaUI;
    [SerializeField] private Button boton1;
    [SerializeField] private Button boton2;
    [SerializeField] private Image imagenBoton1;
    [SerializeField] private Image imagenBoton2;

    [Header("Preguntas")]
    [SerializeField] private PreguntaGenerica[] preguntas;

    [Header("Configuraci�n de Animaci�n")]
    [SerializeField] private float tiempoEsperaAntesCambio = 1f;
    [SerializeField] private float intensidadShake = 15f;
    [SerializeField] private float duracionShake = 0.5f;

    public SliderController sliderController;
    public PanelScrollManager panelScrollManager;
    public int numPreguntas;

    private int indicePreguntaActual = 0;
    private bool esperandoCambio = false;

    void Start()
    {
        if (preguntas.Length > 0)
        {
            MostrarPreguntaActual();
        }
        else
        {
            Debug.LogError("No hay preguntas en el array!");
        }

        // Asignar listeners a los botones
        boton1.onClick.AddListener(() => VerificarRespuesta(true));
        boton2.onClick.AddListener(() => VerificarRespuesta(false));
    }

    private void MostrarPreguntaActual()
    {
        if (indicePreguntaActual >= preguntas.Length)
        {
            Debug.Log("�Has completado todas las preguntas!");
            return;
        }

        PreguntaGenerica preguntaActual = preguntas[indicePreguntaActual];

        // Actualizar el texto
        textoPreguntaUI.text = preguntaActual.textoPregunta;

        // Actualizar las im�genes de los botones
        if (preguntaActual.imagenBoton1 != null)
        {
            imagenBoton1.sprite = preguntaActual.imagenBoton1;
        }

        if (preguntaActual.imagenBoton2 != null)
        {
            imagenBoton2.sprite = preguntaActual.imagenBoton2;
        }
    }

    private void VerificarRespuesta(bool esBoton1)
    {
        if (esperandoCambio) return; // Evitar m�ltiples clics mientras esperamos

        if (indicePreguntaActual == numPreguntas - 1)
        {
            indicePreguntaActual = 0;
        }

        PreguntaGenerica preguntaActual = preguntas[indicePreguntaActual];

        // Verificar si la respuesta es correcta
        bool esCorrecto = (esBoton1 && preguntaActual.boton1EsCorrecto) ||
                         (!esBoton1 && !preguntaActual.boton1EsCorrecto);

        if (esCorrecto)
        {
            Debug.Log("�Respuesta correcta!");
            StartCoroutine(EsperarYCambiarPregunta());
        }
        else
        {
            Debug.Log("Respuesta incorrecta");
            // Aplicar animaci�n de shake al bot�n incorrecto
            Button botonIncorrecto = esBoton1 ? boton1 : boton2;
            StartCoroutine(ShakeBoton(botonIncorrecto.transform));
            StartCoroutine(EsperarYCambiarPregunta());
        }
    }

    private IEnumerator ShakeBoton(Transform botonTransform)
    {
        Vector3 posicionOriginal = botonTransform.localPosition;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracionShake)
        {
            float offsetX = Mathf.Sin(tiempoTranscurrido * 30f) * intensidadShake;
            botonTransform.localPosition = posicionOriginal + new Vector3(offsetX, 0, 0);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        // Restaurar posici�n original
        botonTransform.localPosition = posicionOriginal;
    }

    private IEnumerator EsperarYCambiarPregunta()
    {
        esperandoCambio = true;

        // Esperar el tiempo configurado
        yield return new WaitForSeconds(tiempoEsperaAntesCambio);

        SiguientePregunta();
        panelScrollManager.GoToNextPanel();
        sliderController.IncreaseSliderValue();

        esperandoCambio = false;
    }

    // Funci�n p�blica para pasar a la siguiente pregunta
    public void SiguientePregunta()
    {
        indicePreguntaActual++;
        MostrarPreguntaActual();
    }

    // Funci�n opcional para reiniciar el juego
    public void ReiniciarJuego()
    {
        indicePreguntaActual = 0;
        MostrarPreguntaActual();
    }

    // Funci�n opcional para ir a una pregunta espec�fica
    public void IrAPregunta(int indice)
    {
        if (indice >= 0 && indice < preguntas.Length)
        {
            indicePreguntaActual = indice;
            MostrarPreguntaActual();
        }
    }
}