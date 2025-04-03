using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    

    // Método público para cambiar de escena
    public void ChangeScene(string sceneName)
    {
        
            SceneManager.LoadScene(sceneName); // Cargar la escena especificada
       
    }
}