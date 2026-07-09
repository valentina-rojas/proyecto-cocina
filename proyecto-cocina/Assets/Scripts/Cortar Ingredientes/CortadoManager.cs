using UnityEngine;

public class CortadoManager : MonoBehaviour
{
    public static CortadoManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void IngredienteCortado()
    {
        Debug.Log("Ingrediente cortado completamente.");

        GameManager.Instance.CortadoCompleto();
    }
}