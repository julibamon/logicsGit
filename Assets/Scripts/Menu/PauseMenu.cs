using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
public GameObject menu;

public GameObject iconoDobleSalto;
public GameObject iconoReceta1;
public GameObject iconoReceta2;
public GameObject iconoReceta3;

public GameObject iconoCaldero;

public GameObject iconoKeys;



//estos metodos se referencian  desde el update del scenecontroller, porque un update aqui empezaria desactivado asi que nunca se ejecutaria
public void Pause()
{
    Time.timeScale = 0f;
    menu.SetActive(true);
    //musicSlider.SetValueWithoutNotify(AudioSettingsManager.Instance.musicVolume);

    //sfxSlider.SetValueWithoutNotify(AudioSettingsManager.Instance.sfxVolume);

        //iconos de habilidades objetos:

        if (GameController.Instance.currentSD.worldData.itemsListW.Contains("RecetaTomate"))
        {
            iconoReceta1.SetActive(true);
        }
        if (GameController.Instance.currentSD.worldData.itemsListW.Contains("RecetaSal"))
        {
            iconoReceta2.SetActive(true);
        }
        if (GameController.Instance.currentSD.worldData.itemsListW.Contains("RecetaAceite"))
        {
            iconoReceta3.SetActive(true);
        }
        if (GameController.Instance.currentSD.worldData.itemsListW.Contains("MariKey"))
        {
            iconoCaldero.SetActive(true);
        }
        if (GameController.Instance.currentSD.worldData.itemsListW.Contains("HouseKEY"))
        {
            iconoKeys.SetActive(true);
        }
        if (GameController.Instance.currentSD.playerData.skillsList.Contains("DoubleJump"))
        {
            iconoDobleSalto.SetActive(true);
        }
}
public void Resume() //funcionalidad para el botón reanudar
{
    Time.timeScale=1f; //retomamos la actividad del juego tras pausarla en el método Pause()
    menu.SetActive(false);

}

public void GoToMenu()
{
    Time.timeScale = 1f;
    //parar la musica al ir al menu principal
    if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetPreserveMusic(false);
            MusicManager.Instance.PlaySceneMusic(null);
        }
    SceneManager.LoadScene("Menu");
}

}