using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelScrollManager : MonoBehaviour
{
    [Header("Panel Settings")]
    [SerializeField] private RectTransform[] panels;
    [SerializeField] private RectTransform panelContainer;

    [Header("Animation Settings")]
    [SerializeField] private float transitionDuration = 0.4f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Navigation Buttons")]
    [SerializeField] private Button nextButton;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    private int currentPanelIndex = 0;
    private int nextPanelIndex = 0;
    private bool isTransitioning = false;
    private float panelHeight;

    // Posiciones virtuales para navegación infinita
    private const int POSITION_CURRENT = 0;
    private const int POSITION_NEXT = 1;

    void Start()
    {
        DebugLog("=== INICIANDO PANEL SCROLL MANAGER ===");

        if (panels.Length == 0)
        {
            Debug.LogError("No hay paneles asignados al array!");
            return;
        }

        DebugLog($"Total de paneles en array: {panels.Length}");

        // Obtener altura del panel basada en el canvas
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            panelHeight = canvas.GetComponent<RectTransform>().rect.height;
            DebugLog($"Altura del panel calculada: {panelHeight}");
        }
        else
        {
            Debug.LogError("No se encontró Canvas en los padres!");
        }

        // Verificar que panelContainer esté asignado
        if (panelContainer == null)
        {
            Debug.LogError("PanelContainer no está asignado!");
            return;
        }

        // Configurar posiciones iniciales
        SetupPanels();

        // Asignar evento al botón
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(GoToNextPanel);
            DebugLog("Botón Next configurado correctamente");
        }
        else
        {
            Debug.LogWarning("No hay botón Next asignado");
        }

        // Elegir el primer panel aleatorio y activarlo
        currentPanelIndex = Random.Range(0, panels.Length);
        panels[currentPanelIndex].anchoredPosition = new Vector2(0, 0);
        panels[currentPanelIndex].gameObject.SetActive(true);

        DebugLog($"Panel inicial: [{currentPanelIndex}] '{panels[currentPanelIndex].name}' - Posición: {panels[currentPanelIndex].anchoredPosition} - Activo: {panels[currentPanelIndex].gameObject.activeSelf}");

        // Preparar el siguiente panel
        PrepareNextPanel();

        DebugLog("=== INICIALIZACIÓN COMPLETADA ===\n");
    }

    void SetupPanels()
    {
        DebugLog("Configurando paneles...");

        // Desactivar todos los paneles inicialmente
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] == null)
            {
                Debug.LogError($"El panel en índice {i} es NULL!");
                continue;
            }

            panels[i].gameObject.SetActive(false);
            DebugLog($"  Panel [{i}] '{panels[i].name}' - Desactivado");
        }

        // Asegurar que el contenedor esté en la posición inicial
        panelContainer.anchoredPosition = Vector2.zero;
        DebugLog($"PanelContainer posición inicial: {panelContainer.anchoredPosition}");
    }

    void PrepareNextPanel()
    {
        // Si solo hay un panel, no hay alternativa
        if (panels.Length == 1)
        {
            nextPanelIndex = 0;
        }
        else
        {
            // Elegir siguiente panel aleatorio diferente al actual
            do
            {
                nextPanelIndex = Random.Range(0, panels.Length);
            }
            while (nextPanelIndex == currentPanelIndex);
        }

        // Posicionar el siguiente panel abajo (fuera de vista)
        panels[nextPanelIndex].anchoredPosition = new Vector2(0, -panelHeight);
        panels[nextPanelIndex].gameObject.SetActive(true);

        DebugLog($"Siguiente panel preparado: [{nextPanelIndex}] '{panels[nextPanelIndex].name}' - Posición: {panels[nextPanelIndex].anchoredPosition} - Activo: {panels[nextPanelIndex].gameObject.activeSelf}");
    }

    public void GoToNextPanel()
    {
        if (isTransitioning)
        {
            DebugLog("⚠️ Ya hay una transición en curso, ignorando...");
            return;
        }

        DebugLog($"\n--- INICIANDO TRANSICIÓN AL SIGUIENTE PANEL ---");
        DebugLog($"Panel actual: [{currentPanelIndex}] '{panels[currentPanelIndex].name}'");
        DebugLog($"Panel siguiente: [{nextPanelIndex}] '{panels[nextPanelIndex].name}'");

        StartCoroutine(TransitionToNextPanel());
    }

    private IEnumerator TransitionToNextPanel()
    {
        isTransitioning = true;

        // Deshabilitar el botón durante la transición
        if (nextButton != null)
            nextButton.interactable = false;

        DebugLog($"Estado ANTES de la transición:");
        DebugLog($"  - Panel actual [{currentPanelIndex}]: Pos={panels[currentPanelIndex].anchoredPosition}, Activo={panels[currentPanelIndex].gameObject.activeSelf}");
        DebugLog($"  - Panel siguiente [{nextPanelIndex}]: Pos={panels[nextPanelIndex].anchoredPosition}, Activo={panels[nextPanelIndex].gameObject.activeSelf}");
        DebugLog($"  - Container Pos: {panelContainer.anchoredPosition}");

        Vector2 startPos = panelContainer.anchoredPosition;
        Vector2 targetPos = new Vector2(0, panelHeight);

        DebugLog($"Animando de {startPos} a {targetPos} durante {transitionDuration}s");

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            float curveValue = transitionCurve.Evaluate(t);

            panelContainer.anchoredPosition = Vector2.Lerp(startPos, targetPos, curveValue);

            yield return null;
        }

        panelContainer.anchoredPosition = targetPos;
        DebugLog($"Animación completada - Container final: {panelContainer.anchoredPosition}");

        // Desactivar el panel anterior
        DebugLog($"Desactivando panel anterior [{currentPanelIndex}] '{panels[currentPanelIndex].name}'");
        panels[currentPanelIndex].gameObject.SetActive(false);

        // El panel siguiente ahora es el actual
        int oldPanelIndex = currentPanelIndex;
        currentPanelIndex = nextPanelIndex;

        DebugLog($"Cambiando currentPanelIndex: {oldPanelIndex} → {currentPanelIndex}");

        // Reposicionar el panel actual a la posición 0
        panels[currentPanelIndex].anchoredPosition = new Vector2(0, 0);
        DebugLog($"Panel actual reposicionado a: {panels[currentPanelIndex].anchoredPosition}");

        // Resetear la posición del contenedor
        panelContainer.anchoredPosition = Vector2.zero;
        DebugLog($"Container reseteado a: {panelContainer.anchoredPosition}");

        // Preparar el siguiente panel aleatorio
        PrepareNextPanel();

        isTransitioning = false;

        // Rehabilitar el botón
        if (nextButton != null)
            nextButton.interactable = true;

        DebugLog($"Estado DESPUÉS de la transición:");
        DebugLog($"  - Panel actual [{currentPanelIndex}]: Pos={panels[currentPanelIndex].anchoredPosition}, Activo={panels[currentPanelIndex].gameObject.activeSelf}");
        DebugLog($"  - Panel siguiente [{nextPanelIndex}]: Pos={panels[nextPanelIndex].anchoredPosition}, Activo={panels[nextPanelIndex].gameObject.activeSelf}");
        DebugLog($"--- TRANSICIÓN COMPLETADA ---\n");

        // Callback cuando cambia de panel
        OnPanelChanged(currentPanelIndex);
    }

    // Método virtual para override en clases heredadas
    protected virtual void OnPanelChanged(int newPanelIndex)
    {
        DebugLog($"✓ OnPanelChanged: Panel actual ahora es [{newPanelIndex}] '{panels[newPanelIndex].name}'");
    }

    // Método auxiliar para debug
    private void DebugLog(string message)
    {
        if (showDebugInfo)
        {
            Debug.Log($"[PanelScroll] {message}");
        }
    }

    // Métodos públicos útiles
    public int GetCurrentPanelIndex() => currentPanelIndex;
    public int GetTotalPanels() => panels.Length;
    public bool IsTransitioning() => isTransitioning;
    public GameObject GetCurrentPanel() => panels[currentPanelIndex].gameObject;

    // Método para verificar estado actual (útil para debugging)
    [ContextMenu("Mostrar Estado Actual")]
    public void ShowCurrentState()
    {
        Debug.Log("=== ESTADO ACTUAL DEL SISTEMA ===");
        Debug.Log($"Panel actual: [{currentPanelIndex}] '{panels[currentPanelIndex].name}'");
        Debug.Log($"Panel siguiente preparado: [{nextPanelIndex}] '{panels[nextPanelIndex].name}'");
        Debug.Log($"En transición: {isTransitioning}");
        Debug.Log($"Altura del panel: {panelHeight}");
        Debug.Log($"Posición del contenedor: {panelContainer.anchoredPosition}");
        Debug.Log("\nEstado de todos los paneles:");
        for (int i = 0; i < panels.Length; i++)
        {
            Debug.Log($"  [{i}] '{panels[i].name}' - Pos: {panels[i].anchoredPosition}, Activo: {panels[i].gameObject.activeSelf}");
        }
        Debug.Log("================================");
    }
}