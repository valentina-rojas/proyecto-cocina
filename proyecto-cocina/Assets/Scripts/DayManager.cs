using UnityEngine;
using UnityEngine.SceneManagement;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;

    [System.Serializable]
    public class Dia
    {
        public string nombreReceta;
        public Sprite imagenReceta;
    }

    [Header("Recetas de cada día")]
    public Dia[] dias;

    public string recetaActual { get; private set; }
    public Sprite imagenRecetaActual { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
{
    if (dias.Length > 0)
    {
        recetaActual = dias[0].nombreReceta;
        imagenRecetaActual = dias[0].imagenReceta;
    }
    else
    {
        recetaActual = "";
        imagenRecetaActual = null;

        Debug.LogWarning("No hay recetas configuradas en DayManager.");
    }
}
}