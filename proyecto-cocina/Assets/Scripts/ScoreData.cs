using UnityEngine;

public class ScoreData : MonoBehaviour
{
    public bool IngredientesMalOrdenados { get; private set; }
    public bool CarneCruda { get; private set; }
    public bool CarneQuemada { get; private set; }
    public bool ContaminacionCruzadaCortado { get; private set; }

    public void RegistrarIngredientesMalOrdenados() => IngredientesMalOrdenados = true;
    public void RegistrarCarneCruda() => CarneCruda = true;
    public void RegistrarCarneQuemada() => CarneQuemada = true;
    public void RegistrarContaminacionCruzadaCortado() => ContaminacionCruzadaCortado = true;

    public bool EsVictoria()
    {
        return !IngredientesMalOrdenados && 
               !CarneCruda && 
               !CarneQuemada && 
               !ContaminacionCruzadaCortado;
    }

    public void Resetear()
    {
        IngredientesMalOrdenados = false;
        CarneCruda = false;
        CarneQuemada = false;
        ContaminacionCruzadaCortado = false;
    }
}