using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject panelPopup;
    [SerializeField] private TextMeshProUGUI titulo;
    [SerializeField] private TextMeshProUGUI descripcion;
    [SerializeField] private Image imagenPopup;

    [Header("Barra de Contaminación")]
    [SerializeField] private Slider sliderContaminacion;
    [Tooltip("Arrastrá acá la Image que está dentro de 'Fill Area -> Fill' del Slider.")]
    [SerializeField] private Image fillSliderContaminacion;
    [SerializeField] private bool mostrarBarraSiempre = true;

    [Header("5 Colores de Errores")]
    [Tooltip("Element 0: 1er error | Element 4: 5to error (crítico)")]
    [SerializeField] private Color[] coloresErrores = new Color[5]
    {
        new Color(0.2f, 0.85f, 0.2f, 1f), // Element 0: Verde / 1er nivel
        new Color(0.8f, 0.85f, 0.1f, 1f), // Element 1: Amarillo verdoso
        new Color(1f, 0.75f, 0.1f, 1f),   // Element 2: Amarillo / Naranja
        new Color(1f, 0.45f, 0.1f, 1f),   // Element 3: Naranja intenso
        new Color(0.9f, 0.1f, 0.1f, 1f)    // Element 4: Rojo crítico
    };

    [Header("Botones")]
    [SerializeField] private Button botonContinuar;
    [SerializeField] private Button botonCerrar;

    [Header("Efecto de texto")]
    [SerializeField] private float velocidadTexto = 0.03f;

    private Coroutine coroutineTexto;
    private string textoCompleto;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Time.timeScale = 1f;
            if (panelPopup != null) panelPopup.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (sliderContaminacion != null && fillSliderContaminacion == null)
        {
            if (sliderContaminacion.fillRect != null)
            {
                fillSliderContaminacion = sliderContaminacion.fillRect.GetComponent<Image>();
            }
        }
    }

    public void MostrarPopup(string tituloTexto, string descripcionTexto, Sprite sprite = null, UnityAction accionContinuar = null, UnityAction accionOmitir = null)
    {
        textoCompleto = descripcionTexto;

        if (titulo != null)
            titulo.text = tituloTexto;

        if (imagenPopup != null)
        {
            imagenPopup.gameObject.SetActive(sprite != null);
            if (sprite != null) imagenPopup.sprite = sprite;
        }

        ActualizarBarraContaminacionVisual();

        if (panelPopup != null)
            panelPopup.SetActive(true);

        if (botonContinuar != null)
        {
            botonContinuar.onClick.RemoveAllListeners();
            botonContinuar.onClick.AddListener(() =>
            {
                CerrarVentana();
                accionContinuar?.Invoke();
            });

            botonContinuar.gameObject.SetActive(false);
            botonContinuar.interactable = false;
        }

        if (botonCerrar != null)
        {
            botonCerrar.onClick.RemoveAllListeners();
            botonCerrar.onClick.AddListener(() =>
            {
                CerrarVentana();
                UnityAction callbackFinal = accionOmitir ?? accionContinuar;
                callbackFinal?.Invoke();
            });

            botonCerrar.gameObject.SetActive(true);
        }

        if (coroutineTexto != null)
            StopCoroutine(coroutineTexto);

        coroutineTexto = StartCoroutine(TipearTexto());
        Time.timeScale = 0f;
    }

    // =========================================================
    // CONTROL DEL SLIDER Y COLORES (5 ESPACIOS)
    // =========================================================

    public void ActualizarNivelContaminacion(float valor, float valorMaximo = 5f)
    {
        if (sliderContaminacion == null) return;

        sliderContaminacion.minValue = 0f;
        sliderContaminacion.maxValue = valorMaximo > 0f ? valorMaximo : 5f;
        sliderContaminacion.value = valor;

        AplicarColorSegunErrores(Mathf.RoundToInt(valor));
    }

    private void ActualizarBarraContaminacionVisual()
    {
        if (sliderContaminacion == null) return;

        sliderContaminacion.gameObject.SetActive(mostrarBarraSiempre);
        sliderContaminacion.minValue = 0f;
        sliderContaminacion.maxValue = 5f;

        int errores = 0;
        if (GameManager.Instance != null && GameManager.Instance.Score != null)
        {
            if (GameManager.Instance.ingredientesMalOrdenados) errores++;
            if (GameManager.Instance.contaminacionCruzadaCortado) errores++;
            if (GameManager.Instance.carneCruda || GameManager.Instance.carneQuemada) errores++;
        }

        sliderContaminacion.value = errores;
        AplicarColorSegunErrores(errores);
    }

    private void AplicarColorSegunErrores(int cantidadErrores)
    {
        if (fillSliderContaminacion == null || coloresErrores == null || coloresErrores.Length == 0) return;

        // Si errores = 0 usa el índice 0; del 1 al 5 mapea a los índices 0 al 4
        int indiceColor = (cantidadErrores <= 1) ? 0 : Mathf.Clamp(cantidadErrores - 1, 0, coloresErrores.Length - 1);
        fillSliderContaminacion.color = coloresErrores[indiceColor];
    }

    // =========================================================
    // RUTINAS DE TEXTO Y CIERRE
    // =========================================================

    private IEnumerator TipearTexto()
    {
        if (descripcion == null) yield break;

        descripcion.text = "";
        foreach (char letra in textoCompleto)
        {
            descripcion.text += letra;
            yield return new WaitForSecondsRealtime(velocidadTexto);
        }

        if (botonContinuar != null)
        {
            botonContinuar.gameObject.SetActive(true);
            botonContinuar.interactable = true;
        }

        coroutineTexto = null;
    }

    private void CerrarVentana()
    {
        if (coroutineTexto != null)
        {
            StopCoroutine(coroutineTexto);
            coroutineTexto = null;
        }

        if (panelPopup != null)
            panelPopup.SetActive(false);

        Time.timeScale = 1f;
    }

    public void CerrarPopup() => CerrarVentana();

    private void OnDestroy() => Time.timeScale = 1f;
}