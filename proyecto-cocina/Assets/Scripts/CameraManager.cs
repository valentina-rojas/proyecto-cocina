using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [Header("Cámaras")]
    public GameObject camaraAlmacenIngredientes;
    public GameObject camaraLavadoManos;
    public GameObject camaraCortadoIngredientes;
    public GameObject camaraCoccionIngredientes;
    public GameObject camaraCamaraEmplatado;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        MostrarCamaraCortadoIngredientes();
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

    public void MostrarCamaraCoccionIngredientes()
    {
        DesactivarTodas();
        camaraCoccionIngredientes.SetActive(true);
    }

    public void MostrarCamaraEmplatado()
    {
        DesactivarTodas();
        camaraCamaraEmplatado.SetActive(true);
    }


}