using UnityEngine;

public class SeleccionRecetaManager : MonoBehaviour
{
    public static SeleccionRecetaManager Instance;

    private InventorySlot[] slots;
    private bool seleccionActiva = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        slots = FindObjectsByType<InventorySlot>(FindObjectsSortMode.None);
    }

    private void OnEnable()
    {
        DraggableItem.OnAnyItemEndDrag += ChequearReceta;
    }

    private void OnDisable()
    {
        DraggableItem.OnAnyItemEndDrag -= ChequearReceta;
    }

    public void IniciarSeleccion()
    {
        seleccionActiva = true;

        Debug.Log("Comenzó la selección de ingredientes.");
        ChequearReceta();
    }

    private void ChequearReceta()
    {
        if (!seleccionActiva)
            return;

        string recetaActual = DayManager.Instance.recetaActual.Trim().ToLower();

        int necesarios = 0;
        int correctos = 0;

        foreach (InventorySlot slot in slots)
        {
            IngredienteData[] ingredientes =
                slot.GetComponentsInChildren<IngredienteData>();

            foreach (IngredienteData ingrediente in ingredientes)
            {
                if (ingrediente.nombreReceta.Trim().ToLower() == recetaActual)
                {
                    necesarios++;

                    if (slot.tipoDeEstanteAceptado == "mesa")
                    {
                        correctos++;
                    }
                }
            }
        }

        Debug.Log($"Receta: {recetaActual}");
        Debug.Log($"Ingredientes correctos en mesa: {correctos}/{necesarios}");

        if (necesarios > 0 && correctos == necesarios)
        {
            seleccionActiva = false;

            Debug.Log("Todos los ingredientes de la receta están en la mesa.");

            GameManager.Instance.ActivarLavado();
        }
    }
}