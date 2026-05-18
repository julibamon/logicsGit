using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
//ENTIDAD CREADA PARA PODER MANEJAR METODOS UPDATES DE OBJETOS QUE INICIALMENTE ESTÁN DESACTIVADOS (como el menu de pausa)

public PauseMenu pauseMenu;


    void Update()
    {
         if(Input.GetKeyDown(KeyCode.Escape)) //si estamos cerrando el menu de pausa
        {
            if (pauseMenu.menu.activeSelf)
            {
                pauseMenu.Resume();
            }
            else //si estamos abriendo el menu de pausa
            {
                pauseMenu.Pause();
            }
        }
    }


}