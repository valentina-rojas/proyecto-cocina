using UnityEngine;
using UnityEngine.UI;

public class LavadoManos : MonoBehaviour
{
    public static LavadoManos Instance;

    [Header("Referencias")]
    public SoapController jabonDraggable;
    public Slider barra;

    [Header("UI Lavado")]
    public GameObject objetoBarra;

    [Header("Animaciones")]
    public ImageFrameAnimation animacionManos;
    public ImageFrameAnimation animacionEspuma;

    [Header("Configuración")]
    public float tiempoNecesario = 3f;
    public float velocidadMinima = 100f;

    [Header("Resultado Lavado")]
    public Image imagenManos;
    public Sprite manosLimpias;

    private float progreso;
    private bool completado;
    private Vector3 ultimaPosicionJabon;


    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        Reiniciar();
    }


    private void Update()
    {
        if (completado || jabonDraggable == null)
            return;


        bool estaLavando =
            AreaLavado.JugadorEstaEncima &&
            jabonDraggable.EstaSiendoArrastrado;


        if (estaLavando)
        {
            float distancia = Vector3.Distance(
                jabonDraggable.transform.position,
                ultimaPosicionJabon
            );

            float velocidad = distancia / Time.deltaTime;


            if (velocidad >= velocidadMinima)
            {
                progreso += Time.deltaTime;

                MostrarElementosLavado();


                if (animacionManos != null)
                    animacionManos.Play();


                if (animacionEspuma != null)
                    animacionEspuma.Play();
            }
            else
            {
                progreso = 0f;

                DetenerAnimaciones();
                OcultarElementosLavado();
            }
        }
        else
        {
            progreso = 0f;

            DetenerAnimaciones();
            OcultarElementosLavado();
        }


        progreso = Mathf.Clamp(
            progreso,
            0f,
            tiempoNecesario
        );


        if (barra != null)
            barra.value = progreso / tiempoNecesario;


        ultimaPosicionJabon = jabonDraggable.transform.position;


        if (progreso >= tiempoNecesario)
        {
            CompletarLavado();
        }
    }



    private void MostrarElementosLavado()
    {
        if (objetoBarra != null)
            objetoBarra.SetActive(true);


        if (animacionEspuma != null)
            animacionEspuma.gameObject.SetActive(true);
    }



    private void OcultarElementosLavado()
    {
        if (objetoBarra != null)
            objetoBarra.SetActive(false);


        if (animacionEspuma != null)
        {
            animacionEspuma.Stop();
            animacionEspuma.gameObject.SetActive(false);
        }
    }



    private void DetenerAnimaciones()
    {
        if (animacionManos != null)
            animacionManos.Stop();


        if (animacionEspuma != null)
            animacionEspuma.Stop();
    }



    private void CompletarLavado()
    {
        completado = true;


        DetenerAnimaciones();
        OcultarElementosLavado();


        Debug.Log("¡Lavado completado con éxito!");


        if (GameManager.Instance != null)
        {
            GameManager.Instance.LavadoManosCompleto();
        }
        else
        {
            Debug.LogWarning("GameManager.Instance es NULL.");
        }
    }



    public void Reiniciar()
    {
        progreso = 0f;
        completado = false;


        if (barra != null)
            barra.value = 0f;


        if (jabonDraggable != null)
            ultimaPosicionJabon = jabonDraggable.transform.position;


        DetenerAnimaciones();
        OcultarElementosLavado();


        Debug.Log("Lavado reiniciado.");
    }
}