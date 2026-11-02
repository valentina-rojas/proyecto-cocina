using UnityEngine;

public class ScoreData : MonoBehaviour
{
    public bool IngredientesMalOrdenados { get; private set; }
    public bool CarneCruda { get; private set; }
    public bool CarneQuemada { get; private set; }
    public bool ManosSinLavarPostMezclado { get; private set; }
    public bool ManosSinLavarPostCortado { get; private set; }

    // Contador acumulativo de alimentos contaminados
    public int ErroresCortadoContaminado { get; private set; } = 0;

    // Sigue existiendo para cualquier script o popup que consulte el bool
    public bool ContaminacionCruzadaCortado => ErroresCortadoContaminado > 0;

    public void RegistrarIngredientesMalOrdenados() => IngredientesMalOrdenados = true;
    public void RegistrarCarneCruda() => CarneCruda = true;
    public void RegistrarCarneQuemada() => CarneQuemada = true;
    public void RegistrarManosSinLavarPostMezclado() => ManosSinLavarPostMezclado = true;
    public void RegistrarManosSinLavarPostCortado() => ManosSinLavarPostCortado = true;
    

    // Incrementa +1 por cada alimento contaminado
    public void RegistrarContaminacionCruzadaCortado()
    {
        ErroresCortadoContaminado++;
    }

    public bool EsVictoria()
    {
        return !IngredientesMalOrdenados &&
            !CarneCruda &&
            !CarneQuemada &&
            !ManosSinLavarPostMezclado &&
            !ManosSinLavarPostCortado &&
            ErroresCortadoContaminado == 0;
    }

    public void Resetear()
    {
        IngredientesMalOrdenados = false;
        CarneCruda = false;
        CarneQuemada = false;
        ManosSinLavarPostMezclado = false;
        ManosSinLavarPostCortado = false;
        ErroresCortadoContaminado = 0;
    }
}