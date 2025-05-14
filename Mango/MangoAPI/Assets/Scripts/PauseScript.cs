using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    private bool isPause;
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {

            if(!isPause)
            {
                Time.timeScale = 0f;
                isPause = true;
            }
            else
            {
                Time.timeScale = 1f;
                isPause = false;
            }
        }
    }
}
