using UnityEngine;
using UnityEngine.UI;

public class Carne : MonoBehaviour
{
    [Header("Referencias")]
    public Hornalla hornalla;
    public Slider barraCoccion;
    public Image fillBarra;
    public SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    public Sprite carneCruda;
    public Sprite carneMedia;
    public Sprite carneCocida;
    public Sprite carneQuemada;

    [Header("Configuración")]
    public float tiempoAmarillo = 4f;
    public float tiempoVerde = 7f;
    public float tiempoRojo = 10f;

    private float tiempoCoccion;

    public enum Estado
    {
        Cruda,
        Cocida,
        Quemada
    }

    public Estado estado;

    private void Start()
    {
        estado = Estado.Cruda;

        barraCoccion.minValue = 0;
        barraCoccion.maxValue = tiempoRojo;
        barraCoccion.value = 0;

        barraCoccion.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!hornalla.estaEncendida)
            return;

        tiempoCoccion += Time.deltaTime;

        barraCoccion.value = tiempoCoccion;

        if (tiempoCoccion < tiempoAmarillo)
        {
            estado = Estado.Cruda;

            fillBarra.color = Color.yellow;
            spriteRenderer.sprite = carneCruda;
        }
        else if (tiempoCoccion < tiempoVerde)
        {
            estado = Estado.Cocida;

            fillBarra.color = Color.green;
            spriteRenderer.sprite = carneCocida;
        }
        else
        {
            estado = Estado.Quemada;

            fillBarra.color = Color.red;
            spriteRenderer.sprite = carneQuemada;
        }
    }

    public void VerificarCoccion()
    {
        switch (estado)
        {
            case Estado.Cruda:
                Debug.Log("❌ La carne quedó cruda.");
                break;

            case Estado.Cocida:
                Debug.Log("✅ Cocción correcta.");
                break;

            case Estado.Quemada:
                Debug.Log("❌ La carne se quemó.");
                break;
        }
    }


    public void MostrarBarra()
    {
        barraCoccion.gameObject.SetActive(true);
    }

    public void OcultarBarra()
    {
        barraCoccion.gameObject.SetActive(false);
    }
}