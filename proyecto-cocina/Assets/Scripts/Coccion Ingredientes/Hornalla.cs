using UnityEngine;
using UnityEngine.UI;

public class Hornalla : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject fuego;
    public Button botonCoccion;
    public Carne carne;

    public bool estaEncendida { get; private set; }

    private void Start()
    {
        fuego.SetActive(false);

        botonCoccion.onClick.AddListener(CambiarEstado);
    }

    private void OnDestroy()
    {
        botonCoccion.onClick.RemoveListener(CambiarEstado);
    }

    public void CambiarEstado()
    {
        estaEncendida = !estaEncendida;

        fuego.SetActive(estaEncendida);

        if (estaEncendida)
        {
            Debug.Log("🔥 Hornalla encendida");

            if (carne != null)
                carne.MostrarBarra();
        }
        else
        {
            Debug.Log("❌ Hornalla apagada");

            // COMUNICACIÓN CON EL GAMEMANAGER:
            // Al apagar la hornalla, permitimos avanzar al emplatado
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CoccionCompleta();
            }

            if (carne != null)
            {
                carne.OcultarBarra();
                carne.VerificarCoccion();
            }
        }
    }


    
}