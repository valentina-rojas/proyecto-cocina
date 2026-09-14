using UnityEngine;

public class InicioDiaUI : MonoBehaviour
{
    public void MostrarPanel()
    {
        if (PopupContenido.Instance != null)
        {
            // PopupContenido se encarga de buscar la receta y mostrarla
            PopupContenido.Instance.MostrarInstruccionesReceta(CerrarPanel);
        }
        else
        {
            CerrarPanel();
        }
    }

    private void CerrarPanel()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EmpezarSeleccionReceta();
        }
    }
}