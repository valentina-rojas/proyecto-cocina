using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SlicePoint : MonoBehaviour
{
    public CuttableIngredient ingrediente;

    private bool cortado;

    private BoxCollider2D box;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (cortado)
            return;

        if (other.GetComponent<KnifeController>() == null)
            return;

        float y = other.bounds.center.y;

        float arriba = box.bounds.max.y;
        float abajo = box.bounds.min.y;

        // El cuchillo debe terminar cerca del borde inferior
        if (y <= abajo + 0.05f)
        {
            cortado = true;

            ingrediente.RealizarCorte();
            CortadoManager.Instance.RegistrarCorte();
        }
    }
}