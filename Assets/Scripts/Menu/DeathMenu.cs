using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    public void Retry() //funcionalidad para el botón reintentar
    {
        Time.timeScale=1f; //retomamos la actividad del juego tras pausarla en el método Die()
        GameController.Instance.LoadGame(GameController.Instance.currentSlotId); //recargamos los datos de partida ACTUAL
        SceneManager.LoadScene(GameController.Instance.currentSD.playerData.currentNameScene); //recargamos la escena ya que no hemos pasado por el metodo SlotSelectorPlayOrLoad, que es donde teníamos esta linea de codigo
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

}