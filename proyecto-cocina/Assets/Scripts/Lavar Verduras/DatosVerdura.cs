using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DatosVerdura : MonoBehaviour
{
    [Header("Identificación")]
    public string nombreVerdura = "Tomate";

    [Header("Sprites en Mesada")]
    public Sprite spriteMesaSucia;         
    public Sprite spriteMesaLimpia;        

    [Header("Sprites en Manos")]
    public Sprite spriteManoSosteniendo;  
    public Sprite spriteManoBajoAgua;     
    public Sprite spriteManoLimpiaFinal;  

    [Header("Animación de Frotado")]
    public Sprite[] framesFrotado;        
    public float frameRate = 0.08f;        

    [Header("Dificultad")]
    public float distanciaFrotadoNecesaria = 1800f;

    public Button Boton { get; private set; }

    private void Awake()
    {
        Boton = GetComponent<Button>();

        // Asignar automáticamente el sprite sucio si tiene Image
        if (spriteMesaSucia != null && TryGetComponent<Image>(out var img))
        {
            img.sprite = spriteMesaSucia;
        }
    }
}