using UnityEngine;

public class MezcladoManager : MonoBehaviour
{
    public static MezcladoManager Instance { get; private set; }

    [Header("Referencias de Spawn")]
    [SerializeField] private Transform contenedorSpawn;

    [Header("Secuencia de Ingredientes (Prefabs)")]
    [SerializeField] private IngredienteMezclable[] prefabsIngredientes;

    [Header("Animación de Palpitar")]
    [SerializeField] private float velocidadPalpitar = 4f;
    [SerializeField] private float escalaMinima = 0.98f;
    [SerializeField] private float escalaMaxima = 1.02f;

    private int indiceActual = 0;
    private IngredienteMezclable ingredienteInstanciado;

    // Control de palpitación interna
    private Transform objetivoPalpitar;
    private Vector3 escalaOriginal = Vector3.one;
    private float tiempoAnimacion;
    private bool estaPalpitando;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        CameraManager.Instance?.MostrarCamaraMezcladoIngredientes();
    }

    private void OnDisable()
    {
        ResetearEscala();
    }

    private void LateUpdate()
    {
        if (!estaPalpitando || objetivoPalpitar == null || !objetivoPalpitar.gameObject.activeInHierarchy) return;

        tiempoAnimacion += Time.unscaledDeltaTime * velocidadPalpitar;
        float factor = (Mathf.Sin(tiempoAnimacion) + 1f) * 0.5f;
        float multiplicadorEscala = Mathf.Lerp(escalaMinima, escalaMaxima, factor);

        objetivoPalpitar.localScale = escalaOriginal * multiplicadorEscala;
    }

    /// <summary>
    /// Llamado desde GameManager cuando termina el Cortado
    /// </summary>
    public void IniciarMezclado()
    {
        gameObject.SetActive(true);
        CameraManager.Instance?.MostrarCamaraMezcladoIngredientes(); 

        indiceActual = 0;
        SpawnearIngredienteActual();
    }

    public void SpawnearIngredienteActual()
    {
        ResetearEscala();

        if (prefabsIngredientes == null || indiceActual >= prefabsIngredientes.Length)
        {
            CompletarActividad();
            return;
        }

        IngredienteMezclable prefab = prefabsIngredientes[indiceActual];
        if (prefab == null || contenedorSpawn == null)
        {
            Debug.LogWarning("MezcladoManager: Falta asignar el prefab o el contenedorSpawn.");
            return;
        }

        ingredienteInstanciado = Instantiate(prefab, contenedorSpawn);
        ingredienteInstanciado.transform.localScale = Vector3.one;
        ingredienteInstanciado.transform.localRotation = Quaternion.identity;

        RectTransform rt = ingredienteInstanciado.GetComponent<RectTransform>();
        if (rt != null) rt.anchoredPosition = Vector2.zero;

        // Iniciar el efecto de palpitar en el ingrediente recién creado
        IniciarPalpitar(ingredienteInstanciado.transform);
    }

    public void OnMezclaCompletada()
    {
        ResetearEscala();
        indiceActual++;

        if (prefabsIngredientes != null && indiceActual < prefabsIngredientes.Length)
        {
            SpawnearIngredienteActual();
        }
        else
        {
            CompletarActividad();
        }
    }

    private void CompletarActividad()
    {
        ResetearEscala();
        UIManager.Instance?.PrepararBotonContinuar(OnClicSiguiente);
    }

    private void OnClicSiguiente()
    {
        UIManager.Instance?.OcultarBotonContinuar();
        GameManager.Instance?.ContinuarDespuesDelMezclado();
    }

    // =========================================================
    // CONTROL DE PALPITACIÓN
    // =========================================================

    private void IniciarPalpitar(Transform objetivo)
    {
        if (objetivo == null) return;

        objetivoPalpitar = objetivo;
        // Respalda la escala base real configurada
        escalaOriginal = objetivo.localScale.sqrMagnitude > 0.001f ? objetivo.localScale : Vector3.one;
        tiempoAnimacion = 0f;
        estaPalpitando = true;
    }

    public void DetenerPalpitarIngrediente()
    {
        estaPalpitando = false;
        if (objetivoPalpitar != null)
        {
            objetivoPalpitar.localScale = escalaOriginal;
        }
    }

    public void ReanudarPalpitarIngrediente()
    {
        if (objetivoPalpitar != null)
        {
            estaPalpitando = true;
        }
    }

    private void ResetearEscala()
    {
        estaPalpitando = false;
        if (objetivoPalpitar != null)
        {
            objetivoPalpitar.localScale = escalaOriginal;
            objetivoPalpitar = null;
        }
        tiempoAnimacion = 0f;
    }
}