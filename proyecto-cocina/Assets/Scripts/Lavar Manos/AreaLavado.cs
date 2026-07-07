using UnityEngine;
using UnityEngine.EventSystems;

public class AreaLavado : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Propiedad estática para que el manager la lea fácilmente sin buscar referencias
    public static bool JugadorEstaEncima { get; private set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        JugadorEstaEncima = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        JugadorEstaEncima = false;
    }

    private void OnDisable()
    {
        // Limpieza por si se apaga la pantalla en medio del juego
        JugadorEstaEncima = false;
    }
}