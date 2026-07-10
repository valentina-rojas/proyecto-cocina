using UnityEngine;

public enum TipoIngrediente
{
    Carne,
    Queso,
    Tomate,
    PanSuperior
}

public class IngredienteEmplatado : MonoBehaviour
{
    public TipoIngrediente tipo;
}