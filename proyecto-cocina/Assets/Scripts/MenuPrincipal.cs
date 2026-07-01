using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField] private Button botonJugar;

    void Start()
    {
        botonJugar.onClick.AddListener(IrACinematica);
    }

    public void IrACinematica()
    {
        SceneManager.LoadScene("Cinematica");
    }
}