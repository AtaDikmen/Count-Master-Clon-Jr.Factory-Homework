using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GateScript : MonoBehaviour
{
    public TextMeshPro GateNo;
    public int randomNum;
    public bool multiply;

    void Start()
    {
        if (multiply)
        {
            randomNum = Random.Range(1, 3);
            GateNo.text = "X" + randomNum;
        }
        else
        {
            randomNum = Random.Range(10, 100);

            if (randomNum % 2 != 0)
            {
                randomNum += 1;
            }

            GateNo.text = randomNum.ToString();
        }
    }

    void Update()
    {
        
    }
}
