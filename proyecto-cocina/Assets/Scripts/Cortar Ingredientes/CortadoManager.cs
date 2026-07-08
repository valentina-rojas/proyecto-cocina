using UnityEngine;

public class CortadoManager : MonoBehaviour
{
    public static CortadoManager Instance;

    public int cortesNecesarios = 4;

    private int cortesRealizados;

    private void Awake()
    {
        Instance = this;
    }

    public void RegistrarCorte()
    {
        cortesRealizados++;

        Debug.Log(cortesRealizados + "/" + cortesNecesarios);

        if (cortesRealizados >= cortesNecesarios)
        {
            Debug.Log("Tomate cortado.");

            GameManager.Instance.CortadoCompleto();
        }
    }
}