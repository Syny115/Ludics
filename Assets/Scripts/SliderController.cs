using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [Header("Slider Reference")]
    [SerializeField] private Slider targetSlider;

    [Header("Increment Settings")]
    [SerializeField] private float incrementAmount = 0.1f;
    [Tooltip("Cantidad que se suma al value del slider")]

    [Header("Decrement Settings")]
    [SerializeField] private float decrementAmount = 0.1f;
    [Tooltip("Cantidad que se resta al value del slider")]

    [Header("Auto Decrement")]
    [SerializeField] private bool autoDecrement = false;
    [Tooltip("Si está activado, el slider se reduce automáticamente cada segundo")]
    [SerializeField] private float autoDecrementAmount = 0.1f;
    [Tooltip("Cantidad que se resta automáticamente cada segundo")]
    [SerializeField] private GameObject pauseObject = null;
    [Tooltip("Si este objeto está activo, el auto-decremento se pausa")]

    [Header("Optional: Clamp Values")]
    [SerializeField] private bool useCustomLimits = false;
    [SerializeField] private float customMinValue = 0f;
    [SerializeField] private float customMaxValue = 1f;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    private float autoDecrementTimer = 0f;

    void Start()
    {
        if (targetSlider == null)
        {
            Debug.LogError("No hay Slider asignado en el SliderController!");
            return;
        }

        // Aplicar límites personalizados si están activados
        if (useCustomLimits)
        {
            targetSlider.minValue = customMinValue;
            targetSlider.maxValue = customMaxValue;
            DebugLog($"Límites del slider configurados: Min={customMinValue}, Max={customMaxValue}");
        }

        DebugLog($"SliderController iniciado - Value actual: {targetSlider.value}");
    }

    void Update()
    {
        if (!autoDecrement || targetSlider == null)
            return;

        // Si el objeto de pausa está activo, no aplicar auto-decremento
        if (pauseObject != null && pauseObject.activeInHierarchy)
        {
            DebugLog("Auto-decremento pausado: objeto de pausa activo");
            return;
        }

        autoDecrementTimer += Time.deltaTime;

        if (autoDecrementTimer >= 1f)
        {
            autoDecrementTimer = 0f;
            ApplyAutoDecrement();
        }
    }

    /// <summary>
    /// Aplica el decremento automático al slider
    /// </summary>
    private void ApplyAutoDecrement()
    {
        float oldValue = targetSlider.value;
        float newValue = targetSlider.value - autoDecrementAmount;

        // Limitar al mínimo del slider
        newValue = Mathf.Max(newValue, targetSlider.minValue);

        targetSlider.value = newValue;

        DebugLog($"Auto-decremento: {oldValue:F2} → {newValue:F2} (-{autoDecrementAmount:F2})");
    }

    /// <summary>
    /// Incrementa el value del slider
    /// </summary>
    public void IncreaseSliderValue()
    {
        if (targetSlider == null)
        {
            Debug.LogWarning("No se puede aumentar: Slider no asignado");
            return;
        }

        float oldValue = targetSlider.value;
        float newValue = targetSlider.value + incrementAmount;

        // Limitar al máximo del slider
        newValue = Mathf.Min(newValue, targetSlider.maxValue);

        targetSlider.value = newValue;

        DebugLog($"Aumentar: {oldValue:F2} → {newValue:F2} (+{incrementAmount:F2})");
    }

    /// <summary>
    /// Decrementa el value del slider
    /// </summary>
    public void DecreaseSliderValue()
    {
        if (targetSlider == null)
        {
            Debug.LogWarning("No se puede disminuir: Slider no asignado");
            return;
        }

        float oldValue = targetSlider.value;
        float newValue = targetSlider.value - decrementAmount;

        // Limitar al mínimo del slider
        newValue = Mathf.Max(newValue, targetSlider.minValue);

        targetSlider.value = newValue;

        DebugLog($"Disminuir: {oldValue:F2} → {newValue:F2} (-{decrementAmount:F2})");
    }

    /// <summary>
    /// Establece el value del slider directamente
    /// </summary>
    public void SetSliderValue(float value)
    {
        if (targetSlider == null)
        {
            Debug.LogWarning("No se puede establecer valor: Slider no asignado");
            return;
        }

        float oldValue = targetSlider.value;
        targetSlider.value = value;

        DebugLog($"Valor establecido: {oldValue:F2} → {targetSlider.value:F2}");
    }

    /// <summary>
    /// Obtiene el value actual del slider
    /// </summary>
    public float GetSliderValue()
    {
        if (targetSlider == null)
            return 0f;

        return targetSlider.value;
    }

    /// <summary>
    /// Reinicia el slider a su valor mínimo
    /// </summary>
    public void ResetSliderToMin()
    {
        if (targetSlider == null)
            return;

        targetSlider.value = targetSlider.minValue;
        DebugLog($"Slider reiniciado a mínimo: {targetSlider.minValue}");
    }

    /// <summary>
    /// Establece el slider a su valor máximo
    /// </summary>
    public void SetSliderToMax()
    {
        if (targetSlider == null)
            return;

        targetSlider.value = targetSlider.maxValue;
        DebugLog($"Slider establecido a máximo: {targetSlider.maxValue}");
    }

    // Método auxiliar para debug
    private void DebugLog(string message)
    {
        if (showDebugInfo)
        {
            Debug.Log($"[SliderController] {message}");
        }
    }

    // Métodos para probar desde el inspector (botones de contexto)
    [ContextMenu("Aumentar Valor")]
    private void TestIncrease()
    {
        IncreaseSliderValue();
    }

    [ContextMenu("Disminuir Valor")]
    private void TestDecrease()
    {
        DecreaseSliderValue();
    }

    [ContextMenu("Mostrar Valor Actual")]
    private void ShowCurrentValue()
    {
        if (targetSlider != null)
        {
            Debug.Log($"Valor actual del slider: {targetSlider.value} (Min: {targetSlider.minValue}, Max: {targetSlider.maxValue})");
        }
    }
}