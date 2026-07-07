using UnityEngine;
using UnityEngine.UI;

public class LavadoManos : MonoBehaviour
{
    public static LavadoManos instance;

    [Header("Referencias")]
    public DraggableItem jabonDraggable; // Referencia al script del jabón
    public Slider barra;

    [Header("Configuración")]
    public float tiempoNecesario = 3f;
    public float velocidadMinima = 2f;    // Ajusta según qué tan rápido tengan que moverlo
    public float velocidadEnfriamiento = 0.5f; // Qué tan rápido baja la barra si se detienen

    private float progreso;
    private bool completado;
    private Vector3 ultimaPosicionJabon;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Reiniciar();
    }

    private void Update()
    {
        if (completado || jabonDraggable == null) return;

        // Comprobamos si el jugador está arrastrando el jabón Y está sobre las manos
       
        bool estaLavando = AreaLavado.JugadorEstaEncima && UnityEngine.InputSystem.Pointer.current != null && UnityEngine.InputSystem.Pointer.current.press.isPressed;

        if (estaLavando)
        {
            // Medimos cuánto se movió el jabón en este frame
            float distanciaMovida = Vector3.Distance(jabonDraggable.transform.position, ultimaPosicionJabon);
            
            // Convertimos a una velocidad independiente del framerate
            float velocidadActual = distanciaMovida / Time.deltaTime;

            if (velocidadActual >= velocidadMinima)
            {
                // Sube el progreso
                progreso += Time.deltaTime;
            }
            else
            {
                // Si está encima pero quieto, el progreso disminuye lento
                progreso -= Time.deltaTime * velocidadEnfriamiento;
            }
        }
        else
        {
            // Si sacó el jabón o soltó el click, la barra disminuye de a poco
            progreso -= Time.deltaTime * velocidadEnfriamiento;
        }

        // Aseguramos que el progreso no sea menor a 0 ni mayor al tiempo necesario
        progreso = Mathf.Clamp(progreso, 0f, tiempoNecesario);

        // Actualizamos la UI de la barra (0 a 1)
        barra.value = progreso / tiempoNecesario;

        // Guardamos la posición actual para el siguiente frame
        ultimaPosicionJabon = jabonDraggable.transform.position;

        // Condición de victoria
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
        barra.value = 0f;
        if (jabonDraggable != null)
        {
            ultimaPosicionJabon = jabonDraggable.transform.position;
        }
    }
}