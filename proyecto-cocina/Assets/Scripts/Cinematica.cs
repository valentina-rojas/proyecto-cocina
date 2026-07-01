using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CinematicaManager : MonoBehaviour
{
    [SerializeField] private Image imagenCinematica;
    [SerializeField] private Sprite[] imagenes;
    [SerializeField] private float tiempoPorImagen = 3f;
    [SerializeField] private Button botonSaltar;

    private void Start()
    {
        botonSaltar.onClick.AddListener(CargarGameplay);
        StartCoroutine(ReproducirCinematica());
    }

    IEnumerator ReproducirCinematica()
    {
        foreach (Sprite imagen in imagenes)
        {
            imagenCinematica.sprite = imagen;
            yield return new WaitForSeconds(tiempoPorImagen);
        }

        CargarGameplay();
    }

    public void CargarGameplay()
    {
        SceneManager.LoadScene("Gameplay");
    }

    private void OnDestroy()
    {
        if (botonSaltar != null)
        {
            botonSaltar.onClick.RemoveListener(CargarGameplay);
        }
    }
}