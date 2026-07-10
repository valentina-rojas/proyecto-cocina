using System.Collections.Generic;
using UnityEngine;
using System.Collections; // Necesario para las Corrutinas (IEnumerator)

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
        // En lugar de chequear directo, llamamos a la corrutina para evitar bugs de conteo
        DraggableItem.OnAnyItemEndDrag += AlTerminarArrastre;
    }

    private void OnDisable()
    {
        DraggableItem.OnAnyItemEndDrag -= AlTerminarArrastre;
    }

    private void AlTerminarArrastre()
    {
        if (seleccionActiva)
        {
            StartCoroutine(ChequearRecetaAlFinalDelFrame());
        }
    }

    private IEnumerator ChequearRecetaAlFinalDelFrame()
    {
        yield return new WaitForEndOfFrame();
        ChequearReceta();
    }

    public void IniciarSeleccion()
    {
        seleccionActiva = true;
        Debug.Log("Comenzó la selección de ingredientes.");

        List<IngredienteData> ingredientesNecesarios = new List<IngredienteData>();
        string recetaActual = DayManager.Instance.recetaActual.Trim().ToLower();

        // Busca todos los ingredientes de la receta actual en la escena
        IngredienteData[] ingredientesEscena = FindObjectsByType<IngredienteData>(FindObjectsSortMode.None);

        foreach (IngredienteData ingrediente in ingredientesEscena)
        {
            if (ingrediente.nombreReceta.Trim().ToLower() == recetaActual)
            {
                ingredientesNecesarios.Add(ingrediente);
            }
        }

        // Mostrar lista en pantalla
        if (RecetaUIManager.Instance != null)
        {
            RecetaUIManager.Instance.MostrarReceta(ingredientesNecesarios);
        }

        ChequearReceta();
    }

    private void ChequearReceta()
    {
        if (!seleccionActiva)
            return;

        string recetaActual = DayManager.Instance.recetaActual.Trim().ToLower();

        int necesarios = 0;
        int correctosEnMesa = 0;
        int incorrectosEnMesa = 0; // NUEVO: Contador para los intrusos

        foreach (InventorySlot slot in slots)
        {
            IngredienteData[] ingredientes = slot.GetComponentsInChildren<IngredienteData>();

            foreach (IngredienteData ingrediente in ingredientes)
            {
                string recetaIngrediente = ingrediente.nombreReceta.Trim().ToLower();

                // 1. Si el ingrediente pertenece a la receta del día
                if (recetaIngrediente == recetaActual)
                {
                    necesarios++;

                    if (slot.tipoDeEstanteAceptado == "mesa")
                    {
                        correctosEnMesa++;
                    }
                }
                // 2. Si NO pertenece a la receta, pero está en la mesa
                else if (slot.tipoDeEstanteAceptado == "mesa")
                {
                    incorrectosEnMesa++;
                }
            }
        }

        Debug.Log($"Receta: {recetaActual}");
        Debug.Log($"Correctos en mesa: {correctosEnMesa}/{necesarios} | Incorrectos en mesa: {incorrectosEnMesa}");

        // Actualiza tachado de ingredientes
        if (RecetaUIManager.Instance != null)
        {
            RecetaUIManager.Instance.ActualizarLista();
        }

        // CONDICIÓN ESTRICTA: Tienen que estar todos los necesarios Y CERO incorrectos
        if (necesarios > 0 && correctosEnMesa == necesarios && incorrectosEnMesa == 0)
        {
           // seleccionActiva = false;

            Debug.Log("¡Perfecto! En la mesa están exclusivamente los ingredientes de la receta.");
            GameManager.Instance.ActivarLavado();
        }
    }
}