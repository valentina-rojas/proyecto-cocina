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
    [SerializeField] private bool mostrarBarraSiempre = true; // Si es false, se puede prender solo en feedbacks

    [Header("Botones")]
    [SerializeField] private Button botonContinuar;
    [SerializeField] private Button botonCerrar;

    [Header("Efecto de texto")]
    [SerializeField] private float velocidadTexto = 0.03f;

    private UnityAction accionAlCerrar;
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
        }
    }

    private void Start()
    {
        if (botonContinuar != null)
        {
            botonContinuar.onClick.RemoveAllListeners();
            botonContinuar.onClick.AddListener(CerrarPopup);
            botonContinuar.gameObject.SetActive(false);
        }

        if (botonCerrar != null)
        {
            botonCerrar.onClick.RemoveAllListeners();
            botonCerrar.onClick.AddListener(CerrarPopup);
            botonCerrar.gameObject.SetActive(true);
        }
    }

    // =========================================================
    // MOSTRAR POPUP
    // =========================================================

    public void MostrarPopup(string tituloTexto, string descripcionTexto, Sprite sprite = null, UnityAction accion = null)
    {
        accionAlCerrar = accion;
        textoCompleto = descripcionTexto;

        if (titulo != null)
            titulo.text = tituloTexto;

        if (imagenPopup != null)
        {
            imagenPopup.gameObject.SetActive(sprite != null);
            if (sprite != null) imagenPopup.sprite = sprite;
        }

        // Actualiza el nivel de contaminación actual al abrir cualquier popup
        ActualizarBarraContaminacionVisual();

        if (panelPopup != null)
            panelPopup.SetActive(true);

        if (botonContinuar != null)
        {
            botonContinuar.gameObject.SetActive(false);
            botonContinuar.interactable = false;
        }

        if (coroutineTexto != null)
            StopCoroutine(coroutineTexto);

        coroutineTexto = StartCoroutine(TipearTexto());
        Time.timeScale = 0f;
    }

    // =========================================================
    // CONTROL DEL SLIDER
    // =========================================================

    public void ActualizarNivelContaminacion(float valor, float valorMaximo = -1f)
    {
        if (sliderContaminacion == null) return;

        if (valorMaximo > 0f)
            sliderContaminacion.maxValue = valorMaximo;

        sliderContaminacion.value = valor;
    }

    private void ActualizarBarraContaminacionVisual()
    {
        if (sliderContaminacion == null) return;

        sliderContaminacion.gameObject.SetActive(mostrarBarraSiempre);

        // Si existe GameManager y ScoreData, sincroniza automáticamente el valor
        if (GameManager.Instance != null && GameManager.Instance.Score != null)
        {
            // Reemplazá 'TotalErrores' por la variable o método de tu ScoreData
            int errores = 0;
            if (GameManager.Instance.ingredientesMalOrdenados) errores++;
            if (GameManager.Instance.contaminacionCruzadaCortado) errores++;
            if (GameManager.Instance.carneCruda || GameManager.Instance.carneQuemada) errores++;

            sliderContaminacion.value = errores;
        }
    }

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

    public void CerrarPopup()
    {
        if (coroutineTexto != null)
        {
            StopCoroutine(coroutineTexto);
            coroutineTexto = null;
        }

        if (panelPopup != null)
            panelPopup.SetActive(false);

        Time.timeScale = 1f;

        UnityAction accion = accionAlCerrar;
        accionAlCerrar = null;
        accion?.Invoke();
    }

    private void OnDestroy() => Time.timeScale = 1f;
}