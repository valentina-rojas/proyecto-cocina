using UnityEngine;

public class MezcladoManager : MonoBehaviour
{
    public static MezcladoManager Instance { get; private set; }


    [Header("Referencias de Spawn")]
    [SerializeField] private Transform contenedorSpawn;

    [Header("Secuencia de Ingredientes (Prefabs)")]
    [SerializeField] private IngredienteMezclable[] prefabsIngredientes;

    private int indiceActual = 0;
    private IngredienteMezclable ingredienteInstanciado;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    
    /// Llamado desde GameManager cuando termina el Cortado
    public void IniciarMezclado()
    {
      
        CameraManager.Instance?.MostrarCamaraMezcladoIngredientes(); 

        indiceActual = 0;
        SpawnearIngredienteActual();
    }

    public void SpawnearIngredienteActual()
    {
        if (indiceActual >= prefabsIngredientes.Length)
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
    }

    public void OnMezclaCompletada()
    {
        indiceActual++;

        if (indiceActual < prefabsIngredientes.Length)
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

        UIManager.Instance?.PrepararBotonContinuar(OnClicSiguiente);
    }

    private void OnClicSiguiente()
    {
        UIManager.Instance?.OcultarBotonContinuar();

        // Avanzar el flujo en GameManager hacia Cocción
        GameManager.Instance?.ContinuarDespuesDelMezclado();
    }
}