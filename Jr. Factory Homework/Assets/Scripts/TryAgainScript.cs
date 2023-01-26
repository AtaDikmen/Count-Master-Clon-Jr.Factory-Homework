using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TryAgainScript : MonoBehaviour
{
    [SerializeField] private GameObject canvas;

    public void Canvas()
    {
        canvas.SetActive(true);
    }

    public void TryAgainButton()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
