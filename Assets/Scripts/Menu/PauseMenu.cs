using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
public GameObject menu;

//estos metodos se referencian  desde el update del gamecontroller, porque un update aqui empezaria desactivado asi que nunca se ejecutaria
public void Pause()
{
    Time.timeScale = 0f;
    menu.SetActive(true);
}
public void Resume() //funcionalidad para el botón reanudar
{
    Time.timeScale=1f; //retomamos la actividad del juego tras pausarla en el método Pause()
    menu.SetActive(false);
}

public void GoToMenu()
{
    Time.timeScale = 1f;
    SceneManager.LoadScene("Menu");
}

}