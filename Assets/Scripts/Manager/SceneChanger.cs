using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    

    // M�todo p�blico para cambiar de escena
    public void ChangeScene(string sceneName)
    {
        
            SceneManager.LoadScene(sceneName); // Cargar la escena especificada
       
    }
}