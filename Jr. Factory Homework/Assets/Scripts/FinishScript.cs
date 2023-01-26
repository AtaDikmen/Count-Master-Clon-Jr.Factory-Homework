using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishScript : MonoBehaviour
{
    void Start()
    {
        
    }

    public IEnumerator EndGame()
    {
        yield return new WaitForSecondsRealtime(2.5f);

        SceneManager.LoadScene(2);
    }

    public void ReStart()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
