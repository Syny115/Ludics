using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [Header("Paneles del Menú")]
    public GameObject mainMenu;
    public GameObject selectMenu;
    public GameObject achievementsMenu;
    public GameObject popUp;

    [Header("Desplegables")]
    public GameObject scienceButton;
    public GameObject languageButton;
    public GameObject artButton;

    public GameObject scienceCategories;
    public GameObject languageCategories;
    public GameObject artCategories;

    [Header("Compendio de Minijuegos")]
    public GameObject mainMenuCanvas;
    public GameObject scienceChallengeMinigames;
    public GameObject sciencePracticeMinigames;
    public GameObject languageChallengeMinigames;
    public GameObject languagePracticeMinigames;
    public GameObject artChallengeMinigames;
    public GameObject artPracticeMinigames;
    public GameObject popurriMinigames;

    [Header("HUD In-Game")]
    public GameObject pointsSlider;
    public GameObject popUpButton;
    //public GameObject headlines;

    private string categoriaActual;

    //Start
    void Start()
    {
        //Inicia el programa con el MainMenu
        MostrarPanel(mainMenu);
    }

    //Navegacion entre paneles
    public void OpenMainMenu()
    {
        MostrarPanel(mainMenu);

        pointsSlider.SetActive(false);
        popUpButton.SetActive(false);
        //headlines.SetActive(false);

        scienceChallengeMinigames.SetActive(false);
        sciencePracticeMinigames.SetActive(false);

        languageChallengeMinigames.SetActive(false);
        languagePracticeMinigames.SetActive(false);

        artChallengeMinigames.SetActive(false);
        artPracticeMinigames.SetActive(false);

        popurriMinigames.SetActive(false);

    }

    public void OpenSelectMenu()
    {
        MostrarPanel(selectMenu);

        scienceButton.SetActive(true);
        languageButton.SetActive(true);
        artButton.SetActive(true);

        scienceCategories.SetActive(false);
        languageCategories.SetActive(false);
        artCategories.SetActive(false);
    }

    public void OpenAchievementsMenu()
    {
        MostrarPanel(achievementsMenu);
    }

    public void OpenPopUp()
    {
        MostrarPanel(popUp);
    }

    public void CerrarPopUp()
    {
        popUp.SetActive(false);
    }

    //Elegir Categoria
    public void OpenScienceCategories()
    {
        artButton.SetActive(true);
        languageButton.SetActive(true);

        scienceButton.SetActive(false);
        MostrarBoton(scienceCategories);

        languageCategories.SetActive(false);
        artCategories.SetActive(false);
    }

    public void OpenLanguageCategories()
    {
        scienceButton.SetActive(true);
        artButton.SetActive(true);

        languageButton.SetActive(false);
        MostrarBoton(languageCategories);

        scienceCategories.SetActive(false);
        artCategories.SetActive(false);
    }

    public void OpenArtCategories()
    {
        scienceButton.SetActive(true);
        languageButton.SetActive(true);

        artButton.SetActive(false);
        MostrarBoton(artCategories);

        scienceCategories.SetActive(false);
        languageCategories.SetActive(false);
    }

    //Entrar en Compendio de MiniJuegos
    public void GoToSciencePractice()
    {
        ActivarCanvas(sciencePracticeMinigames);   
    }
    public void GoToLanguagePractice()
    {
        ActivarCanvas(languagePracticeMinigames);
    }
    public void GoToArtPractice()
    {
        ActivarCanvas(artPracticeMinigames);
    }

    public void GoToScienceChallenge()
    {
        ActivarCanvas(scienceChallengeMinigames);
    }
    public void GoToLanguageChallenge()
    {
        ActivarCanvas(languageChallengeMinigames);
    }
    public void GoToArtChallenge()
    {
        ActivarCanvas(artChallengeMinigames);
    }

    public void GoToPopurri()
    {
        ActivarCanvas(popurriMinigames);
    }

    //Audio (PENDIENTE)

    public void ToggleSFX()
    {
        // Implementa la lógica para activar/desactivar SFX
        bool sfxActivo = !PlayerPrefs.HasKey("SFX_Activo") || PlayerPrefs.GetInt("SFX_Activo") == 1;
        sfxActivo = !sfxActivo;
        PlayerPrefs.SetInt("SFX_Activo", sfxActivo ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("SFX: " + (sfxActivo ? "Activado" : "Desactivado"));
        // Aquí aplicarías el cambio al AudioManager
    }

    public void ToggleOST()
    {
        // Implementa la lógica para activar/desactivar OST
        bool ostActivo = !PlayerPrefs.HasKey("OST_Activo") || PlayerPrefs.GetInt("OST_Activo") == 1;
        ostActivo = !ostActivo;
        PlayerPrefs.SetInt("OST_Activo", ostActivo ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("OST: " + (ostActivo ? "Activado" : "Desactivado"));
        // Aquí aplicarías el cambio al AudioManager
    }

    //Mostrar panel
    private void MostrarPanel(GameObject panelAMostrar)
    {
        //Desactivar paneles
        mainMenu.SetActive(false);
        selectMenu.SetActive(false);
        achievementsMenu.SetActive(false);
        popUp.SetActive(false);

        //Activar panel
        panelAMostrar.SetActive(true);
    }

    //Mostrar boton categorias
    private void MostrarBoton(GameObject botonAMostrar)
    {
        //Activar botones
        botonAMostrar.SetActive(true);
    }

    //Mostrar compendio de minijuegos
    private void ActivarCanvas(GameObject canvasAMostrar)
    {
        selectMenu.SetActive(false);

        scienceChallengeMinigames.SetActive(false);
        sciencePracticeMinigames.SetActive(false);

        languageChallengeMinigames.SetActive(false);
        languagePracticeMinigames.SetActive(false);

        artChallengeMinigames.SetActive(false);
        artPracticeMinigames.SetActive(false);

        popurriMinigames.SetActive(false);

        pointsSlider.SetActive(true);
        popUpButton.SetActive(true);
        //headlines.SetActive(true);

        //Activar canvas
        canvasAMostrar.SetActive(true);
    }
        
}