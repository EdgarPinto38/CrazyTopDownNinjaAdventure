using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{

    public Image lifeBar;
    public float  actualLife;
    public float maxLife;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeBar.fillAmount = actualLife / maxLife;
    }
}
