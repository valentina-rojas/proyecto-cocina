using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [Header("UI")]
    public GameObject panelPopup;
    public TextMeshProUGUI titulo;
    public TextMeshProUGUI descripcion;

    [Header("Botones")]
    public Button botonContinuar;
    public Button botonCerrar;

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

            if (panelPopup != null)
                panelPopup.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //================================================
        // BOTÓN CONTINUAR
        //================================================

        if (botonContinuar != null)
        {
            botonContinuar.onClick.RemoveAllListeners();
            botonContinuar.onClick.AddListener(CerrarPopup);

            // Oculto hasta que termine el texto
            botonContinuar.gameObject.SetActive(false);
        }

        //================================================
        // BOTÓN CERRAR / OMITIR
        //================================================

        if (botonCerrar != null)
        {
            botonCerrar.onClick.RemoveAllListeners();
            botonCerrar.onClick.AddListener(OmitirPopup);

            // Visible desde el comienzo
            botonCerrar.gameObject.SetActive(true);
        }
    }

    //====================================================
    // MOSTRAR POPUP
    //====================================================

    public void MostrarPopup(
        string tituloTexto,
        string descripcionTexto,
        UnityAction accion = null)
    {
        accionAlCerrar = accion;

        textoCompleto = descripcionTexto;

        if (titulo != null)
            titulo.text = tituloTexto;

        if (panelPopup != null)
            panelPopup.SetActive(true);

        //================================================
        // CONFIGURAR BOTÓN CONTINUAR
        //================================================

        if (botonContinuar != null)
        {
            botonContinuar.gameObject.SetActive(false);
            botonContinuar.interactable = false;
        }

        //================================================
        // CONFIGURAR BOTÓN CERRAR
        //================================================

        if (botonCerrar != null)
        {
            botonCerrar.gameObject.SetActive(true);
            botonCerrar.interactable = true;
        }

        //================================================
        // DETENER COROUTINE ANTERIOR
        //================================================

        if (coroutineTexto != null)
        {
            StopCoroutine(coroutineTexto);
        }

        coroutineTexto = StartCoroutine(TipearTexto());

        // Pausar juego
        Time.timeScale = 0f;
    }

    //====================================================
    // TIPEAR TEXTO
    //====================================================

    private IEnumerator TipearTexto()
    {
        if (descripcion == null)
            yield break;

        descripcion.text = "";

        foreach (char letra in textoCompleto)
        {
            descripcion.text += letra;

            yield return new WaitForSecondsRealtime(
                velocidadTexto
            );
        }

        // Terminó el texto
        MostrarBotonContinuar();

        coroutineTexto = null;
    }

    //====================================================
    // MOSTRAR BOTÓN CONTINUAR
    //====================================================

    private void MostrarBotonContinuar()
    {
        if (botonContinuar != null)
        {
            botonContinuar.gameObject.SetActive(true);
            botonContinuar.interactable = true;
        }
    }

    //====================================================
    // CONTINUAR
    //====================================================

    public void CerrarPopup()
    {
        CerrarYContinuar();
    }

    //====================================================
    // OMITIR / CERRAR
    //====================================================

    public void OmitirPopup()
    {
        Debug.Log("Popup omitido.");

        CerrarYContinuar();
    }

    //====================================================
    // CERRAR Y EJECUTAR SIGUIENTE ACCIÓN
    //====================================================

    private void CerrarYContinuar()
    {
        // Detener tipeo
        if (coroutineTexto != null)
        {
            StopCoroutine(coroutineTexto);
            coroutineTexto = null;
        }

        // Cerrar panel
        if (panelPopup != null)
            panelPopup.SetActive(false);

        // Restaurar juego
        Time.timeScale = 1f;

        // Guardar acción antes de limpiarla
        UnityAction accion = accionAlCerrar;

        accionAlCerrar = null;

        // Ejecutar siguiente paso
        accion?.Invoke();
    }

    //====================================================
    // CERRAR SIN ACCIÓN
    //====================================================

    public void CerrarPopupSinAccion()
    {
        if (coroutineTexto != null)
        {
            StopCoroutine(coroutineTexto);
            coroutineTexto = null;
        }

        if (panelPopup != null)
            panelPopup.SetActive(false);

        Time.timeScale = 1f;

        accionAlCerrar = null;
    }

    //====================================================
    // SEGURIDAD
    //====================================================

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}