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

    private int indiceActual = 0;


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
        if (
            imagenHamburguesa != null &&
            estadosHamburguesa != null &&
            estadosHamburguesa.Length > 0
        )
        {
            imagenHamburguesa.sprite =
                estadosHamburguesa[0];
        }
    }


    public bool IntentarAgregarIngrediente(
        IngredienteEmplatado ingrediente
    )
    {
        if (indiceActual >= ordenCorrecto.Length)
            return false;


        if (ingrediente.tipo != ordenCorrecto[indiceActual])
        {
            Debug.Log(
                "Ingrediente incorrecto."
            );

            return false;
        }


        indiceActual++;


        if (
            imagenHamburguesa != null &&
            indiceActual < estadosHamburguesa.Length
        )
        {
            imagenHamburguesa.sprite =
                estadosHamburguesa[indiceActual];
        }


        // Avisar al draggable que fue colocado correctamente.
        DraggableIngredienteEmplatado draggable =
            ingrediente.GetComponent<DraggableIngredienteEmplatado>();

        if (draggable != null)
        {
            draggable.ColocadoCorrectamente();
        }


        Debug.Log(
            "Ingrediente correcto."
        );


        // =====================================================
        // HAMBURGUESA COMPLETA
        // =====================================================

        if (indiceActual >= ordenCorrecto.Length)
        {
            Debug.Log(
                "¡Hamburguesa completa!"
            );

            if (EmplatadoManager.Instance != null)
            {
                EmplatadoManager.Instance.EmplatadoCompleto();
            }
            else
            {
                Debug.LogError(
                    "PlateManager: No existe EmplatadoManager."
                );
            }
        }


        return true;
    }
}