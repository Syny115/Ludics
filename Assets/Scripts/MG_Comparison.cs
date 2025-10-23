using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[Serializable]
public class Pregunta
{
    public string textoPregunta;
    public Sprite imagenBoton1;
    public Sprite imagenBoton2;
    public bool boton1EsCorrecto; // true si el bot�n 1 es correcto, false si es el bot�n 2
}

public class MG_Comparison : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI textoPreguntaUI;
    [SerializeField] private Button boton1;
    [SerializeField] private Button boton2;
    [SerializeField] private Image imagenBoton1;
    [SerializeField] private Image imagenBoton2;

    [Header("Preguntas")]
    [SerializeField] private Pregunta[] preguntas;

    public int numPreguntas;
    public SliderController sliderController;
    public PanelScrollManager panelScrollManager;

    private int indicePreguntaActual = 0;

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
            // Aqu� puedes a�adir l�gica para cuando se terminen las preguntas
            return;
        }

        Pregunta preguntaActual = preguntas[indicePreguntaActual];

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
        if(indicePreguntaActual == numPreguntas-1)
        {
            indicePreguntaActual = 0;
        }

        Pregunta preguntaActual = preguntas[indicePreguntaActual];

        // Verificar si la respuesta es correcta
        bool esCorrecto = (esBoton1 && preguntaActual.boton1EsCorrecto) ||
                         (!esBoton1 && !preguntaActual.boton1EsCorrecto);

        if (esCorrecto)
        {
            Debug.Log("�Respuesta correcta!");
            // Aqu� puedes a�adir feedback visual o sonido
            SiguientePregunta();
            panelScrollManager.GoToNextPanel();
            sliderController.IncreaseSliderValue();



        }
        else
        {
            Debug.Log("Respuesta incorrecta");
            // Aqu� puedes a�adir feedback de error
            SiguientePregunta();
            panelScrollManager.GoToNextPanel();
        }
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