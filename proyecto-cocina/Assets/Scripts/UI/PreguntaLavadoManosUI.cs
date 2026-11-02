using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PreguntaLavadoManosUI : MonoBehaviour
{
    public static PreguntaLavadoManosUI Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private Button botonSi;
    [SerializeField] private Button botonNo;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        panel.SetActive(false);
    }

    public void Mostrar(UnityAction alAceptar, UnityAction alRechazar)
    {
        botonSi.onClick.RemoveAllListeners();
        botonNo.onClick.RemoveAllListeners();

        botonSi.onClick.AddListener(() => Responder(alAceptar));
        botonNo.onClick.AddListener(() => Responder(alRechazar));

        panel.SetActive(true);
    }

    private void Responder(UnityAction accion)
    {
        panel.SetActive(false);
        accion?.Invoke();
    }
}