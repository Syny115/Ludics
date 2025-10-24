using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

[Serializable]
public class PreguntaFormas
{
    public string textoPregunta;
    public Sprite imagenCentral;
    public Sprite imagenBoton1;
    public Sprite imagenBoton2;
    public Sprite imagenBoton3;
    public int respuestaCorrecta; // 1 para boton1, 2 para boton2, 3 para boton3
}

public class MG_Shapes : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI textoPreguntaUI;
    [SerializeField] private Image imagenCentral;
    [SerializeField] private Button boton1;
    [SerializeField] private Button boton2;
    [SerializeField] private Button boton3;
    [SerializeField] private Image imagenBoton1;
    [SerializeField] private Image imagenBoton2;
    [SerializeField] private Image imagenBoton3;

    [Header("Preguntas")]
    [SerializeField] private PreguntaFormas[] preguntas;

    [Header("Configuración de Animación")]
    [SerializeField] private float tiempoEsperaAntesCambio = 1f;
    [SerializeField] private float intensidadShake = 15f;
    [SerializeField] private float duracionShake = 0.5f;

    public Maneta maneta;
    public SoundManager soundManager;
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
        boton1.onClick.AddListener(() => VerificarRespuesta(1));
        boton2.onClick.AddListener(() => VerificarRespuesta(2));
        boton3.onClick.AddListener(() => VerificarRespuesta(3));
    }

    private void MostrarPreguntaActual()
    {
        if (indicePreguntaActual >= preguntas.Length)
        {
            Debug.Log("¡Has completado todas las preguntas!");
            return;
        }

        PreguntaFormas preguntaActual = preguntas[indicePreguntaActual];

        // Actualizar el texto
        textoPreguntaUI.text = preguntaActual.textoPregunta;

        // Actualizar la imagen central
        if (preguntaActual.imagenCentral != null)
        {
            imagenCentral.sprite = preguntaActual.imagenCentral;
        }

        // Actualizar las imágenes de los botones
        if (preguntaActual.imagenBoton1 != null)
        {
            imagenBoton1.sprite = preguntaActual.imagenBoton1;
        }

        if (preguntaActual.imagenBoton2 != null)
        {
            imagenBoton2.sprite = preguntaActual.imagenBoton2;
        }

        if (preguntaActual.imagenBoton3 != null)
        {
            imagenBoton3.sprite = preguntaActual.imagenBoton3;
        }
    }

    private void VerificarRespuesta(int botonPresionado)
    {
        if (esperandoCambio) return; // Evitar múltiples clics mientras esperamos

        PreguntaFormas preguntaActual = preguntas[indicePreguntaActual];

        // Verificar si la respuesta es correcta
        bool esCorrecto = (botonPresionado == preguntaActual.respuestaCorrecta);

        if (esCorrecto)
        {
            StartCoroutine(maneta.ActivarCorrectTemporal());
            Debug.Log("¡Respuesta correcta!");
            soundManager.PlayCorrectAnswer();

            StartCoroutine(EsperarYCambiarPregunta());
        }
        else
        {
            StartCoroutine(maneta.ActivarWrongTemporal());
            Debug.Log("Respuesta incorrecta");
            soundManager.PlayWrongAnswer();

            // Aplicar animación de shake al botón incorrecto
            Button botonIncorrecto = botonPresionado == 1 ? boton1 :
                                     botonPresionado == 2 ? boton2 : boton3;
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

        // Restaurar posición original
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

    // Función pública para pasar a la siguiente pregunta
    public void SiguientePregunta()
    {
        indicePreguntaActual++;

        // Reiniciar al inicio si se completaron todas las preguntas
        if (indicePreguntaActual >= preguntas.Length)
        {
            indicePreguntaActual = 0;
        }

        MostrarPreguntaActual();
    }

    // Función opcional para reiniciar el juego
    public void ReiniciarJuego()
    {
        indicePreguntaActual = 0;
        MostrarPreguntaActual();
    }

    // Función opcional para ir a una pregunta específica
    public void IrAPregunta(int indice)
    {
        if (indice >= 0 && indice < preguntas.Length)
        {
            indicePreguntaActual = indice;
            MostrarPreguntaActual();
        }
    }
}