using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateManager : MonoBehaviour
{
    public static PlateManager Instance;

    [Header("Imagen de la hamburguesa")]
    public Image imagenHamburguesa;

    [Header("Sprites")]
    public Sprite[] estadosHamburguesa;

    [Header("Orden correcto")]
    public TipoIngrediente[] ordenCorrecto;

    [Header("Ingredientes disponibles en mesa")]
    [Tooltip("Arrastra aquí los GameObjects/Scripts de los ingredientes que están en la mesa para emplatar")]
    [SerializeField] private List<IngredienteEmplatado> ingredientesEnMesa = new List<IngredienteEmplatado>();

    [Header("Animación de Palpitar")]
    [SerializeField] private float velocidadPalpitar = 4f;
    [SerializeField] private float escalaMinima = 0.98f;
    [SerializeField] private float escalaMaxima = 1.02f;

    private int indiceActual = 0;

    // Control de palpitación interna
    private Transform ingredientePalpitando;
    private readonly Dictionary<Transform, Vector3> escalasOriginales = new Dictionary<Transform, Vector3>();
    private float tiempoAnimacion;
    private bool estaPalpitando;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (imagenHamburguesa != null &&
            estadosHamburguesa != null &&
            estadosHamburguesa.Length > 0)
        {
            imagenHamburguesa.sprite = estadosHamburguesa[0];
        }

        // Si la lista está vacía en el Inspector, busca automáticamente los ingredientes en la escena/panel
        if (ingredientesEnMesa.Count == 0)
        {
            ingredientesEnMesa.AddRange(FindObjectsOfType<IngredienteEmplatado>(true));
        }

        RegistrarEscalasIniciales();
        ActualizarIngredientePalpitando();
    }

    private void OnDisable()
    {
        ResetearEscala();
    }

    private void LateUpdate()
    {
        if (!estaPalpitando || ingredientePalpitando == null || !ingredientePalpitando.gameObject.activeInHierarchy)
            return;

        tiempoAnimacion += Time.unscaledDeltaTime * velocidadPalpitar;
        float factor = (Mathf.Sin(tiempoAnimacion) + 1f) * 0.5f;
        float multiplicadorEscala = Mathf.Lerp(escalaMinima, escalaMaxima, factor);

        if (escalasOriginales.TryGetValue(ingredientePalpitando, out Vector3 escalaBase))
        {
            ingredientePalpitando.localScale = escalaBase * multiplicadorEscala;
        }
    }

    private void RegistrarEscalasIniciales()
    {
        foreach (var ing in ingredientesEnMesa)
        {
            if (ing != null && !escalasOriginales.ContainsKey(ing.transform))
            {
                Vector3 escala = ing.transform.localScale;
                escalasOriginales[ing.transform] = escala.sqrMagnitude > 0.001f ? escala : Vector3.one;
            }
        }
    }

    // =========================================================
    // CONTROL DE GUÍA VISUAL / PALPITACIÓN
    // =========================================================

    public void ActualizarIngredientePalpitando()
    {
        ResetearEscala();

        if (ordenCorrecto == null || indiceActual >= ordenCorrecto.Length)
            return;

        TipoIngrediente tipoBuscado = ordenCorrecto[indiceActual];

        // Encuentra el ingrediente activo en mesa que corresponde a este paso
        for (int i = 0; i < ingredientesEnMesa.Count; i++)
        {
            IngredienteEmplatado ing = ingredientesEnMesa[i];
            if (ing != null && ing.gameObject.activeSelf && ing.tipo == tipoBuscado)
            {
                IniciarPalpitar(ing.transform);
                break;
            }
        }
    }

    private void IniciarPalpitar(Transform objetivo)
    {
        if (objetivo == null) return;

        ingredientePalpitando = objetivo;

        if (!escalasOriginales.ContainsKey(objetivo))
        {
            Vector3 escala = objetivo.localScale;
            escalasOriginales[objetivo] = escala.sqrMagnitude > 0.001f ? escala : Vector3.one;
        }

        tiempoAnimacion = 0f;
        estaPalpitando = true;
    }

    public void DetenerPalpitarActual()
    {
        estaPalpitando = false;
        if (ingredientePalpitando != null && escalasOriginales.TryGetValue(ingredientePalpitando, out Vector3 escalaBase))
        {
            ingredientePalpitando.localScale = escalaBase;
        }
    }

    public void ReanudarPalpitarActual()
    {
        if (ingredientePalpitando != null)
        {
            estaPalpitando = true;
        }
    }

    private void ResetearEscala()
    {
        estaPalpitando = false;
        if (ingredientePalpitando != null && escalasOriginales.TryGetValue(ingredientePalpitando, out Vector3 escalaBase))
        {
            ingredientePalpitando.localScale = escalaBase;
            ingredientePalpitando = null;
        }
        tiempoAnimacion = 0f;
    }

    // =========================================================
    // LÓGICA DE AGREGAR AL PLATO
    // =========================================================

    public bool IntentarAgregarIngrediente(IngredienteEmplatado ingrediente)
    {
        if (indiceActual >= ordenCorrecto.Length)
            return false;

        if (ingrediente.tipo != ordenCorrecto[indiceActual])
        {
            Debug.Log("Ingrediente incorrecto.");
            return false;
        }

        // Si fue el correcto, aseguramos resetear la escala del que acaba de entrar
        ResetearEscala();

        indiceActual++;

        if (imagenHamburguesa != null &&
            indiceActual < estadosHamburguesa.Length)
        {
            imagenHamburguesa.sprite = estadosHamburguesa[indiceActual];
        }

        // Avisar al draggable que fue colocado correctamente.
        DraggableIngredienteEmplatado draggable = ingrediente.GetComponent<DraggableIngredienteEmplatado>();
        if (draggable != null)
        {
            draggable.ColocadoCorrectamente();
        }

        Debug.Log("Ingrediente correcto.");

        // =====================================================
        // SIGUIENTE INGREDIENTE O FINALIZAR
        // =====================================================

        if (indiceActual < ordenCorrecto.Length)
        {
            // Pasa a palpitar el siguiente ingrediente de la lista
            ActualizarIngredientePalpitando();
        }
        else
        {
            Debug.Log("¡Hamburguesa completa!");

            if (EmplatadoManager.Instance != null)
            {
                EmplatadoManager.Instance.EmplatadoCompleto();
            }
            else
            {
                Debug.LogError("PlateManager: No existe EmplatadoManager.");
            }
        }

        return true;
    }
}