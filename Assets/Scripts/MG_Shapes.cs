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
    [SerializeField] private Image imageManetaGood;
    [SerializeField] private Image imageManetaEvil;

    [Header("Preguntas")]
    [SerializeField] private PreguntaFormas[] preguntas;

    [Header("Configuración de Animación")]
    [SerializeField] private float tiempoEsperaAntesCambio = 1f;
    [SerializeField] private float intensidadShake = 15f;
    [SerializeField] private float duracionShake = 0.5f;
    [SerializeField] private float duracionAnimacionManeta = 0.8f;
    [SerializeField] private float escalaMaximaManeta = 1.2f;

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

        // Ocultar las manetas al inicio
        OcultarManetas();
    }

    private void OcultarManetas()
    {
        if (imageManetaGood != null)
        {
            imageManetaGood.gameObject.SetActive(false);
        }
        if (imageManetaEvil != null)
        {
            imageManetaEvil.gameObject.SetActive(false);
        }
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
            sliderController.IncreaseSliderValue();

            // Mostrar animación de maneta correcta
            StartCoroutine(MostrarManetaCorrecta());

            StartCoroutine(EsperarYCambiarPregunta());
        }
        else
        {
            StartCoroutine(maneta.ActivarWrongTemporal());
            Debug.Log("Respuesta incorrecta");
            soundManager.PlayWrongAnswer();
            sliderController.DecreaseSliderValue();

            // Mostrar animación de maneta incorrecta
            StartCoroutine(MostrarManetaIncorrecta());

            // Aplicar animación de shake al botón incorrecto
            Button botonIncorrecto = botonPresionado == 1 ? boton1 :
                                     botonPresionado == 2 ? boton2 : boton3;
            StartCoroutine(ShakeBoton(botonIncorrecto.transform));
            StartCoroutine(EsperarYCambiarPregunta());
        }
    }

    private IEnumerator MostrarManetaCorrecta()
    {
        if (imageManetaGood == null) yield break;

        // Activar la maneta buena
        imageManetaGood.gameObject.SetActive(true);
        imageManetaGood.transform.localScale = Vector3.zero;

        float tiempoTranscurrido = 0f;
        float mitadDuracion = duracionAnimacionManeta / 2f;

        // Animación: escalar de 0 a escalaMaxima y luego a 1
        while (tiempoTranscurrido < duracionAnimacionManeta)
        {
            tiempoTranscurrido += Time.deltaTime;
            float progreso = tiempoTranscurrido / duracionAnimacionManeta;

            // Efecto de rebote: crece más de lo normal y luego se ajusta
            float escala;
            if (progreso < 0.5f)
            {
                // Primera mitad: crecer hasta escalaMaxima
                escala = Mathf.Lerp(0f, escalaMaximaManeta, progreso * 2f);
            }
            else
            {
                // Segunda mitad: ajustar a escala normal (1)
                escala = Mathf.Lerp(escalaMaximaManeta, 1f, (progreso - 0.5f) * 2f);
            }

            imageManetaGood.transform.localScale = Vector3.one * escala;

            yield return null;
        }

        imageManetaGood.transform.localScale = Vector3.one;

        // Esperar un momento antes de ocultar
        yield return new WaitForSeconds(0.3f);

        imageManetaGood.gameObject.SetActive(false);
    }

    private IEnumerator MostrarManetaIncorrecta()
    {
        if (imageManetaEvil == null) yield break;

        // Activar la maneta mala
        imageManetaEvil.gameObject.SetActive(true);
        imageManetaEvil.transform.localScale = Vector3.one;

        Vector3 posicionOriginal = imageManetaEvil.transform.localPosition;
        float tiempoTranscurrido = 0f;

        // Animación: shake más intenso para indicar error
        while (tiempoTranscurrido < duracionAnimacionManeta)
        {
            tiempoTranscurrido += Time.deltaTime;

            // Shake horizontal y vertical
            float offsetX = Mathf.Sin(tiempoTranscurrido * 40f) * intensidadShake * 1.5f;
            float offsetY = Mathf.Cos(tiempoTranscurrido * 40f) * intensidadShake * 0.5f;

            imageManetaEvil.transform.localPosition = posicionOriginal + new Vector3(offsetX, offsetY, 0);

            yield return null;
        }

        // Restaurar posición original
        imageManetaEvil.transform.localPosition = posicionOriginal;

        // Esperar un momento antes de ocultar
        yield return new WaitForSeconds(0.2f);

        imageManetaEvil.gameObject.SetActive(false);
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