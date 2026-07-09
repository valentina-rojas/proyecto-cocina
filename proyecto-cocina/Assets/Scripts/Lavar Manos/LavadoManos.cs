using UnityEngine;
using UnityEngine.UI;

public class LavadoManos : MonoBehaviour
{
    public static LavadoManos Instance;

    [Header("Referencias")]
    public SoapController jabonDraggable;
    public Slider barra;

    [Header("Configuración")]
    public float tiempoNecesario = 3f;
    public float velocidadMinima = 100f;

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
            float distancia =
                Vector3.Distance(
                    jabonDraggable.transform.position,
                    ultimaPosicionJabon);

            float velocidad = distancia / Time.deltaTime;

            if (velocidad >= velocidadMinima)
            {
                progreso += Time.deltaTime;
            }
            else
            {
                // Si el jabón se queda quieto, reinicia la barra.
                progreso = 0f;
            }
        }
        else
        {
            // Si salió del área o soltó el jabón, reinicia.
            progreso = 0f;
        }

        progreso = Mathf.Clamp(progreso, 0f, tiempoNecesario);

        barra.value = progreso / tiempoNecesario;

        ultimaPosicionJabon = jabonDraggable.transform.position;

        if (progreso >= tiempoNecesario)
        {
            CompletarLavado();
        }
    }

    private void CompletarLavado()
    {
        completado = true;

        Debug.Log("¡Lavado completado con éxito!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LavadoManosCompleto();
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
    }
}