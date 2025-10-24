using System.Collections;
using UnityEngine;

public class Maneta : MonoBehaviour
{
    [System.Serializable]
    public class CursorSprite
    {
        public Sprite sprite;
        public Vector2 hotspot = Vector2.zero; // Punto de click del cursor
        [Range(0.5f, 4f)]
        public float escala = 1f; // Escala del cursor (1 = tamaño original)

        [HideInInspector]
        public Texture2D texture;
    }

    [Header("Sprites del Cursor")]
    public CursorSprite cursorNormal;
    public CursorSprite cursorHover;      // Cuando está sobre un objeto interactuable
    public CursorSprite cursorClick;      // Cuando hace click
    public CursorSprite cursorCorrect;       // Cuando arrastra algo
    public CursorSprite cursorWrong;     // Para acciones de ataque

    private CursorSprite currentCursor;
    private static Maneta instance;

    public static Maneta Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Maneta>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Convertir todos los sprites a texturas al iniciar
        ConvertSpritesToTextures();
    }

    void Start()
    {
        SetCursorNormal();
    }

    void Update()
    {
        // Ejemplo: cambiar cursor al hacer click
        if (Input.GetMouseButtonDown(0))
        {
            SetCursorClick();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SetCursorNormal();
        }
    }

    private void ConvertSpritesToTextures()
    {
        ConvertSpriteToTexture(cursorNormal);
        ConvertSpriteToTexture(cursorHover);
        ConvertSpriteToTexture(cursorClick);
        ConvertSpriteToTexture(cursorCorrect);
        ConvertSpriteToTexture(cursorWrong);
    }

    private void ConvertSpriteToTexture(CursorSprite cursorSprite)
    {
        if (cursorSprite == null || cursorSprite.sprite == null) return;

        Sprite sprite = cursorSprite.sprite;
        Texture2D tex = sprite.texture;

        // Verificar si la textura es legible
        if (!tex.isReadable)
        {
            Debug.LogWarning($"La textura del sprite '{sprite.name}' no es legible. " +
                           "Asegúrate de activar 'Read/Write Enabled' en la configuración del sprite.");
            return;
        }

        // Obtener los píxeles del sprite
        int width = (int)sprite.rect.width;
        int height = (int)sprite.rect.height;

        Color[] pixels = tex.GetPixels(
            (int)sprite.textureRect.x,
            (int)sprite.textureRect.y,
            width,
            height
        );

        // Calcular nuevo tamaño según la escala
        int newWidth = Mathf.RoundToInt(width * cursorSprite.escala);
        int newHeight = Mathf.RoundToInt(height * cursorSprite.escala);

        // Crear textura con el tamaño escalado
        Texture2D newTex = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
        newTex.filterMode = FilterMode.Point; // Para mantener pixeles nítidos

        // Escalar los píxeles
        Color[] scaledPixels = new Color[newWidth * newHeight];
        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                int origX = Mathf.FloorToInt(x / cursorSprite.escala);
                int origY = Mathf.FloorToInt(y / cursorSprite.escala);

                if (origX < width && origY < height)
                {
                    scaledPixels[y * newWidth + x] = pixels[origY * width + origX];
                }
            }
        }

        newTex.SetPixels(scaledPixels);
        newTex.Apply();

        cursorSprite.texture = newTex;

        // Ajustar el hotspot según la escala
        cursorSprite.hotspot *= cursorSprite.escala;
    }

    // Métodos públicos para cambiar el cursor
    public void SetCursorNormal()
    {
        SetCursor(cursorNormal);
    }

    public void SetCursorHover()
    {
        SetCursor(cursorHover);
    }

    public void SetCursorClick()
    {
        SetCursor(cursorClick);
    }

    public void SetCursorCorrect()
    {
        SetCursor(cursorCorrect);
    }

    public void SetCursorWrong()
    {
        SetCursor(cursorWrong);
    }

    private void SetCursor(CursorSprite cursorSprite)
    {
        if (cursorSprite == null || cursorSprite.texture == null)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            return;
        }

        currentCursor = cursorSprite;
        Cursor.SetCursor(cursorSprite.texture, cursorSprite.hotspot, CursorMode.Auto);
    }

    // Método para obtener el cursor actual
    public CursorSprite GetCurrentCursor()
    {
        return currentCursor;
    }

    // Ocultar/Mostrar cursor
    public void HideCursor()
    {
        Cursor.visible = false;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
    }

    // Bloquear/Desbloquear cursor
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public IEnumerator ActivarCorrectTemporal()
    {
        Debug.Log("ANTES - Sprite actual:");
        SetCursorCorrect();
        Debug.Log("DESPUÉS SetCursorCorrect - Sprite:");
        yield return new WaitForSeconds(1f);
        Debug.Log("ANTES de volver a normal");
        SetCursorNormal();
        Debug.Log("DESPUÉS SetCursorNormal - Sprite");
    }
    public IEnumerator ActivarWrongTemporal()
    {
        SetCursorWrong();
        yield return new WaitForSeconds(1f);
        SetCursorNormal();

        Debug.Log("MAAAAAAAL");
    }
}