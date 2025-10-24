using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Configuración")]
    public bool esInteractuable = true;
    public bool esArrastrable = false;
    public bool esEnemigo = false;

    private bool isDragging = false;
    private Vector3 offset;

    void OnMouseEnter()
    {
        if (!esInteractuable) return;

        // Cambiar cursor según el tipo de objeto
        if (esEnemigo)
        {
            Maneta.Instance.SetCursorWrong();
        }
        else if (esArrastrable)
        {
            Maneta.Instance.SetCursorHover();
        }
        else
        {
            Maneta.Instance.SetCursorHover();
        }
    }

    void OnMouseExit()
    {
        if (!isDragging)
        {
            Maneta.Instance.SetCursorNormal();
        }
    }

    void OnMouseDown()
    {
        if (esArrastrable)
        {
            isDragging = true;
            offset = transform.position - GetMouseWorldPosition();
            Maneta.Instance.SetCursorCorrect();
        }
        else
        {
            Maneta.Instance.SetCursorClick();
            OnInteract();
        }
    }

    void OnMouseDrag()
    {
        if (isDragging && esArrastrable)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            Maneta.Instance.SetCursorHover();
        }
        else
        {
            Maneta.Instance.SetCursorNormal();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void OnInteract()
    {
        // Aquí va la lógica de interacción
        Debug.Log($"Interactuando con: {gameObject.name}");

        if (esEnemigo)
        {
            Debug.Log("¡Atacando al enemigo!");
        }
    }
}