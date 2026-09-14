using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class SlotIngredienteCortar
{
    public Button boton;
    public Image imagenIcono;
    public GameObject marcaCompletado;
}

public class CortadoManager : MonoBehaviour
{
    public static CortadoManager Instance { get; private set; }

    [Header("Paneles de la Etapa")]
    [SerializeField] private GameObject panelSeleccionIngredientes;
    [SerializeField] private GameObject panelSeleccionTabla;
    [SerializeField] private GameObject panelSeleccionCuchillo;
    [SerializeField] private GameObject panelMesaDeCorte;

    [Header("Botones de Tablas")]
    [SerializeField] private Button botonTablaCarnes;
    [SerializeField] private Button botonTablaVegetales;
    [SerializeField] private Button botonTablaSecos;

    [Header("Sprites de las Tablas en Escena")]
    [SerializeField] private Sprite spriteTablaCarnes;
    [SerializeField] private Sprite spriteTablaVegetales;
    [SerializeField] private Sprite spriteTablaSecos;

    [Header("Botones de Cuchillos")]
    [SerializeField] private Button botonCuchilloCarnes;
    [SerializeField] private Button botonCuchilloVegetales;
    [SerializeField] private Button botonCuchilloSecos;
    [SerializeField] private Button botonCuchilloAderezos;
    [SerializeField] private Button botonCuchilloSucio;

    [Header("Sprites de los Cuchillos en Escena")]
    [SerializeField] private Sprite spriteCuchilloCarnes;
    [SerializeField] private Sprite spriteCuchilloVegetales;
    [SerializeField] private Sprite spriteCuchilloSecos;
    [SerializeField] private Sprite spriteCuchilloAderezos;
    [SerializeField] private Sprite spriteCuchilloSucio;

    [Header("Slots de la UI (Máximo 3)")]
    [SerializeField] private SlotIngredienteCortar[] slotsIngredientes = new SlotIngredienteCortar[3];

    [Header("Ingredientes a Cortar en este Nivel/Escena")]
    [Tooltip("Arrastra aquí directamente los ingredientes que se cortan")]
    [SerializeField] private List<IngredienteData> ingredientesEtapa = new List<IngredienteData>();

    [Header("Mesa y Cuchillo")]
    [SerializeField] private Image imagenTabla;
    [SerializeField] private KnifeController knifeController;
    [SerializeField] private Transform contenedorIngrediente;

    [Header("UI General")]
    [SerializeField] private Button botonContinuar;

    // Variables para seguimiento de elecciones y errores
    private IngredienteData ingredienteSeleccionado;
    private CuttableIngredient ingredienteEnMesa;
    private TipoAlimento tipoTablaSeleccionada;
    private TipoAlimento tipoCuchilloSeleccionado;
    private int ingredientesCortadosCount = 0;

    public int ErroresCometidos { get; private set; } = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        ConfigurarBotonesTablas();
        ConfigurarBotonesCuchillos();
    }

    private void Start()
    {
        SetBotonContinuar(false);
    }

    private void ConfigurarBotonesTablas()
    {
        VincularBotonTabla(botonTablaCarnes, spriteTablaCarnes, TipoAlimento.Carnes);
        VincularBotonTabla(botonTablaVegetales, spriteTablaVegetales, TipoAlimento.Vegetales);
        VincularBotonTabla(botonTablaSecos, spriteTablaSecos, TipoAlimento.Secos);
    }

    private void VincularBotonTabla(Button btn, Sprite spriteDestino, TipoAlimento tipo)
    {
        if (btn == null) return;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => SeleccionarTabla(spriteDestino, tipo));
    }

    private void ConfigurarBotonesCuchillos()
    {
        VincularBotonCuchillo(botonCuchilloCarnes, spriteCuchilloCarnes, TipoAlimento.Carnes);
        VincularBotonCuchillo(botonCuchilloVegetales, spriteCuchilloVegetales, TipoAlimento.Vegetales);
        VincularBotonCuchillo(botonCuchilloSecos, spriteCuchilloSecos, TipoAlimento.Secos);
        VincularBotonCuchillo(botonCuchilloAderezos, spriteCuchilloAderezos, TipoAlimento.Aderezos);
        VincularBotonCuchillo(botonCuchilloSucio, spriteCuchilloSucio, TipoAlimento.Sucio);
    }

    private void VincularBotonCuchillo(Button btn, Sprite spriteDestino, TipoAlimento tipo)
    {
        if (btn == null) return;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => SeleccionarCuchillo(spriteDestino, tipo));
    }

    public void IniciarCortado()
    {
        ingredientesCortadosCount = 0;
        ErroresCometidos = 0;
        SetBotonContinuar(false);

        if (ingredientesEtapa == null || ingredientesEtapa.Count == 0)
        {
            Debug.LogError("[CortadoManager] ¡No hay ingredientes asignados en el Inspector!");
            TerminarEtapaCortado();
            return;
        }

        for (int i = 0; i < ingredientesEtapa.Count; i++)
        {
            if (ingredientesEtapa[i] != null)
                ingredientesEtapa[i].yaCortado = false;
        }

        if (PopupContenido.Instance != null)
            PopupContenido.Instance.MostrarInstruccionesCortado(IniciarSeleccionIngredientes);
        else
            IniciarSeleccionIngredientes();
    }

    public void IniciarSeleccionIngredientes()
    {
        CameraManager.Instance?.MostrarCamaraCortadoIngredientes();

        if (panelSeleccionIngredientes != null) panelSeleccionIngredientes.SetActive(true);
        if (panelSeleccionTabla != null) panelSeleccionTabla.SetActive(false);
        if (panelSeleccionCuchillo != null) panelSeleccionCuchillo.SetActive(false);
        if (panelMesaDeCorte != null) panelMesaDeCorte.SetActive(false);

        SetBotonContinuar(false);
        ActualizarBotonesIngredientes();
    }

    private void ActualizarBotonesIngredientes()
    {
        for (int i = 0; i < slotsIngredientes.Length; i++)
        {
            int index = i;
            SlotIngredienteCortar slot = slotsIngredientes[i];

            if (slot == null || slot.boton == null) continue;

            if (i < ingredientesEtapa.Count && ingredientesEtapa[i] != null)
            {
                slot.boton.gameObject.SetActive(true);
                IngredienteData data = ingredientesEtapa[i];

                if (slot.imagenIcono != null && data.imagenIngrediente != null)
                {
                    slot.imagenIcono.sprite = data.imagenIngrediente.sprite;
                    slot.imagenIcono.enabled = true;
                }

                slot.boton.interactable = !data.yaCortado;
                if (slot.marcaCompletado != null)
                    slot.marcaCompletado.SetActive(data.yaCortado);

                slot.boton.onClick.RemoveAllListeners();
                slot.boton.onClick.AddListener(() => OnClickSeleccionarIngrediente(index));
            }
            else
            {
                slot.boton.gameObject.SetActive(false);
            }
        }
    }

    private void OnClickSeleccionarIngrediente(int index)
    {
        ingredienteSeleccionado = ingredientesEtapa[index];

        if (panelSeleccionIngredientes != null) 
            panelSeleccionIngredientes.SetActive(false);

        if (panelSeleccionTabla != null) 
            panelSeleccionTabla.SetActive(true);
    }

    public void SeleccionarTabla(Sprite spriteTabla, TipoAlimento tipo)
    {
        tipoTablaSeleccionada = tipo;

        if (imagenTabla != null && spriteTabla != null)
            imagenTabla.sprite = spriteTabla;

        if (panelSeleccionTabla != null) 
            panelSeleccionTabla.SetActive(false);

        if (panelSeleccionCuchillo != null) 
            panelSeleccionCuchillo.SetActive(true);
    }

    public void SeleccionarCuchillo(Sprite spriteCuchillo, TipoAlimento tipo)
    {
        tipoCuchilloSeleccionado = tipo;

        if (knifeController != null && spriteCuchillo != null && knifeController.image != null)
            knifeController.image.sprite = spriteCuchillo;

        if (panelSeleccionCuchillo != null) 
            panelSeleccionCuchillo.SetActive(false);

        PrepararMesaDeCorte();
    }

    private void PrepararMesaDeCorte()
    {
        if (panelMesaDeCorte != null) panelMesaDeCorte.SetActive(true);

        if (ingredienteEnMesa != null)
        {
            Destroy(ingredienteEnMesa.gameObject);
            ingredienteEnMesa = null;
        }

        if (ingredienteSeleccionado == null || ingredienteSeleccionado.prefabCortable == null)
        {
            Debug.LogError("[CortadoManager] Falta ingredienteSeleccionado o su prefab.");
            return;
        }

        if (contenedorIngrediente == null)
        {
            Debug.LogError("[CortadoManager] Falta asignar contenedorIngrediente.");
            return;
        }

        ingredienteEnMesa = Instantiate(ingredienteSeleccionado.prefabCortable, contenedorIngrediente, false);

        RectTransform rectTransform = ingredienteEnMesa.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localRotation = Quaternion.identity;
        }
        else
        {
            ingredienteEnMesa.transform.localPosition = Vector3.zero;
            ingredienteEnMesa.transform.localRotation = Quaternion.identity;
        }

        ingredienteEnMesa.InicializarIngrediente();

        if (knifeController != null)
        {
            knifeController.ReiniciarCuchillo();
            knifeController.gameObject.SetActive(true);
        }

        // Evalúa si las herramientas coinciden con el ingrediente
        ValidarSeleccionHerramientas();
    }

    private void ValidarSeleccionHerramientas()
    {
        if (ingredienteSeleccionado == null) return;

        bool huboError = false;

        // 1. Validar Tabla con la regla de 3 tablas
        if (!EsTablaValida(tipoTablaSeleccionada, ingredienteSeleccionado.tipo))
        {
            huboError = true;
            Debug.LogWarning($"[Error Contaminación] Tabla incorrecta: usaste tabla de {tipoTablaSeleccionada} con {ingredienteSeleccionado.nombreIngrediente} ({ingredienteSeleccionado.tipo}).");
        }
        else
        {
            Debug.Log($"[Higiene] Tabla correcta para {ingredienteSeleccionado.nombreIngrediente}.");
        }

        // 2. Validar Cuchillo
        if (!EsCuchilloValido(tipoCuchilloSeleccionado, ingredienteSeleccionado.tipo))
        {
            huboError = true;
            Debug.LogWarning($"[Error Contaminación] Cuchillo incorrecto: usaste cuchillo {tipoCuchilloSeleccionado} con {ingredienteSeleccionado.nombreIngrediente} ({ingredienteSeleccionado.tipo}).");
        }
        else
        {
            Debug.Log($"[Higiene] Cuchillo correcto para {ingredienteSeleccionado.nombreIngrediente}.");
        }

        if (huboError)
        {
            ErroresCometidos++;
            GameManager.Instance?.RegistrarContaminacionCruzadaCortado();
        }
    }

    private bool EsTablaValida(TipoAlimento tabla, TipoAlimento alimento)
    {
        switch (tabla)
        {
            case TipoAlimento.Carnes:
                return alimento == TipoAlimento.Carnes;

            case TipoAlimento.Vegetales:
                return alimento == TipoAlimento.Vegetales;

            case TipoAlimento.Secos:
                // La tercera tabla acepta secos, lácteos y aderezos
                return alimento == TipoAlimento.Secos || 
                       alimento == TipoAlimento.Lacteos || 
                       alimento == TipoAlimento.Aderezos;

            default:
                return false;
        }
    }

    private bool EsCuchilloValido(TipoAlimento cuchillo, TipoAlimento alimento)
    {
        // El cuchillo sucio siempre genera contaminación
        if (cuchillo == TipoAlimento.Sucio) return false;

        if (cuchillo == TipoAlimento.Carnes) return alimento == TipoAlimento.Carnes;
        if (cuchillo == TipoAlimento.Vegetales) return alimento == TipoAlimento.Vegetales;
        
        // Cuchillo de secos acepta secos y lácteos
        if (cuchillo == TipoAlimento.Secos) 
            return alimento == TipoAlimento.Secos || alimento == TipoAlimento.Lacteos;

        if (cuchillo == TipoAlimento.Aderezos) 
            return alimento == TipoAlimento.Aderezos;

        return cuchillo == alimento;
    }

    public void IngredienteCortado()
    {
        if (ingredienteSeleccionado != null)
            ingredienteSeleccionado.yaCortado = true;

        ingredientesCortadosCount++;

        if (knifeController != null)
            knifeController.gameObject.SetActive(false);

        SetBotonContinuar(true, OnContinuarTrasCorte);
    }

    private void OnContinuarTrasCorte()
    {
        SetBotonContinuar(false);

        if (ingredienteEnMesa != null)
            Destroy(ingredienteEnMesa.gameObject);

        if (ingredientesCortadosCount < ingredientesEtapa.Count)
        {
            IniciarSeleccionIngredientes();
        }
        else
        {
            FinalizarEtapaCortado();
        }
    }

    private void FinalizarEtapaCortado()
    {
        if (panelMesaDeCorte != null) panelMesaDeCorte.SetActive(false);

        TerminarEtapaCortado();
    }

    private void TerminarEtapaCortado()
    {
        SetBotonContinuar(false);
        GameManager.Instance?.ContinuarDespuesDelCortado();
    }

    private void SetBotonContinuar(bool visible, UnityAction accion = null)
    {
        if (botonContinuar == null) return;

        botonContinuar.onClick.RemoveAllListeners();
        if (visible && accion != null)
            botonContinuar.onClick.AddListener(accion);

        botonContinuar.interactable = visible;
        botonContinuar.gameObject.SetActive(visible);
    }
}