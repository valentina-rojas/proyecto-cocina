using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [Header("Cámaras")]
    public GameObject camaraAlmacenIngredientes;
    public GameObject camaraLavadoManos;
    public GameObject camaraCortadoIngredientes;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        MostrarCamaraAlmacenIngredientes();
    }

    private void DesactivarTodas()
    {
        camaraAlmacenIngredientes.SetActive(false);
        camaraLavadoManos.SetActive(false);
        camaraCortadoIngredientes.SetActive(false);
    }

    public void MostrarCamaraAlmacenIngredientes()
    {
        DesactivarTodas();
        camaraAlmacenIngredientes.SetActive(true);
    }

    public void MostrarCamaraLavadoManos()
    {
        DesactivarTodas();
        camaraLavadoManos.SetActive(true);
    }

    public void MostrarCamaraCortadoIngredientes()
    {
        DesactivarTodas();
        camaraCortadoIngredientes.SetActive(true);
    }
}