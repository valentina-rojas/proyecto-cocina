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

        Debug.Log(
            $"Área: {AreaLavado.JugadorEstaEncima} | " +
            $"Arrastrando: {jabonDraggable.EstaSiendoArrastrado} | " +
            $"Está lavando: {estaLavando}"
        );

        if (estaLavando)
        {
            float distancia = Vector3.Distance(
                jabonDraggable.transform.position,
                ultimaPosicionJabon);

            float velocidad = distancia / Time.deltaTime;

            Debug.Log($"Distancia: {distancia} | Velocidad: {velocidad}");

            if (velocidad >= velocidadMinima)
            {
                progreso += Time.deltaTime;
                Debug.Log($"✔ Sumando progreso. Progreso actual: {progreso}");
            }
            else
            {
                Debug.Log("✖ Jabón demasiado quieto. Reiniciando progreso.");
                progreso = 0f;
            }
        }
        else
        {
            Debug.Log("✖ No está lavando (no está sobre el área o no se está arrastrando).");
            progreso = 0f;
        }

        progreso = Mathf.Clamp(progreso, 0f, tiempoNecesario);

        if (barra != null)
        {
            barra.value = progreso / tiempoNecesario;
            Debug.Log($"Barra: {barra.value}");
        }
        else
        {
            Debug.LogError("La referencia a la barra es NULL.");
        }

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

        Debug.Log("Lavado reiniciado.");
    }
}