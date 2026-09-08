using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static void CargarEscena(string nombreEscena)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscena);
    }
}