using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class UIPalpitacion : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float velocidad = 4f;
    [SerializeField] private float escalaMinima = 0.95f;
    [SerializeField] private float escalaMaxima = 1.08f;
    [SerializeField] private bool iniciarAlActivar = false;

    private Coroutine corrutinaPalpitar;
    private Vector3 escalaOriginal = Vector3.one;
    private bool escalaGuardada = false;

    private void Awake()
    {
        AsegurarEscalaBase();
    }

    private void OnEnable()
    {
        AsegurarEscalaBase();
        if (iniciarAlActivar) Iniciar();
    }

    private void OnDisable()
    {
        Detener();
    }

    private void AsegurarEscalaBase()
    {
        // Si no la guardamos o si se guardó como (0,0,0) por problemas de canvas al spawnear:
        if (!escalaGuardada || transform.localScale == Vector3.zero)
        {
            if (transform.localScale != Vector3.zero)
            {
                escalaOriginal = transform.localScale;
                escalaGuardada = true;
            }
            else
            {
                escalaOriginal = Vector3.one;
                escalaGuardada = true;
            }
        }
    }

    public void Iniciar()
    {
        AsegurarEscalaBase();

        if (corrutinaPalpitar != null)
        {
            StopCoroutine(corrutinaPalpitar);
        }

        // Si el objeto no está activo en jerarquía, no se puede iniciar la corrutina
        if (gameObject.activeInHierarchy)
        {
            corrutinaPalpitar = StartCoroutine(RutinaPalpitar());
        }
    }

    public void Detener()
    {
        if (corrutinaPalpitar != null)
        {
            StopCoroutine(corrutinaPalpitar);
            corrutinaPalpitar = null;
        }

        if (escalaGuardada)
        {
            transform.localScale = escalaOriginal;
        }
    }

    private IEnumerator RutinaPalpitar()
    {
        // Esperamos un frame para que Canvas/Layout Group asiente la escala y posición inicial real
        yield return null;

        // Si se capturó antes de que el Layout estuviera listo, actualizamos la base
        if (transform.localScale != Vector3.zero && Mathf.Approximately(escalaOriginal.x, 1f) && transform.localScale != Vector3.one)
        {
            escalaOriginal = transform.localScale;
        }

        float tiempo = 0f;

        while (true)
        {
            tiempo += Time.unscaledDeltaTime * velocidad;
            float factor = (Mathf.Sin(tiempo) + 1f) * 0.5f;
            float escalaActual = Mathf.Lerp(escalaMinima, escalaMaxima, factor);

            transform.localScale = escalaOriginal * escalaActual;
            yield return null;
        }
    }
}