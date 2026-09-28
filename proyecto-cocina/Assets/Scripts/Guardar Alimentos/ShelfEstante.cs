using UnityEngine;

public class ShelfEstante : MonoBehaviour
{
    [Header("Configuración de Superficie")]
    [Tooltip("Marcar si este mueble/superficie es la mesa inicial")]
    public bool esMesa = false;

    [Tooltip("Categoría que acepta este estante (se ignora si es mesa)")]
    public TipoAlimento tipoAceptado;
}