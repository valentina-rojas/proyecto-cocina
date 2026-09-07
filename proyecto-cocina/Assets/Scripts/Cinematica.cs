using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CinematicaManager : MonoBehaviour
{
    [Header("Cinemática")]
    [SerializeField] private Image imagenCinematica;
    [SerializeField] private Sprite[] imagenes;

    [Header("Texto")]
    [SerializeField] private TextMeshProUGUI textoCinematica;
    [SerializeField] private float velocidadTexto = 0.04f;
    [SerializeField] private float tiempoDespuesDelTexto = 2f;

    [Header("Botón")]
    [SerializeField] private Button botonSaltar;

    // Un texto para cada imagen
    private string[] textos =
    {
        "La ciudad está en caos...",
        "Nuestra escuela es ahora el último refugio...",
        "Un helicóptero deja varias cajas en la azotea de la escuela...",
        "Las cajas contienen varios ingredientes y un manual de instrucciones...",
        "Tu misión es preparar alimentos seguros para todos...",
        "Tu trabajo será cocinar y nosotros te ayudaremos mientras revisamos el manual."
    };

    private Coroutine cinematicaCoroutine;

    private void Start()
    {
        botonSaltar.onClick.AddListener(CargarGameplay);

        cinematicaCoroutine = StartCoroutine(ReproducirCinematica());
    }

    IEnumerator ReproducirCinematica()
    {
        int cantidad = Mathf.Min(imagenes.Length, textos.Length);

        for (int i = 0; i < cantidad; i++)
        {
            // Mostrar imagen
            imagenCinematica.sprite = imagenes[i];

            // Limpiar texto anterior
            textoCinematica.text = "";

            // Escribir el texto letra por letra
            yield return StartCoroutine(TipearTexto(textos[i]));

            // Mantener la imagen durante 2 segundos
            // después de terminar de escribir el texto
            yield return new WaitForSeconds(tiempoDespuesDelTexto);
        }

        // Cuando termina la última imagen, ir al Gameplay
        CargarGameplay();
    }

    IEnumerator TipearTexto(string texto)
    {
        foreach (char letra in texto)
        {
            textoCinematica.text += letra;

            yield return new WaitForSeconds(velocidadTexto);
        }
    }

    public void CargarGameplay()
    {
        if (cinematicaCoroutine != null)
        {
            StopCoroutine(cinematicaCoroutine);
            cinematicaCoroutine = null;
        }

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