using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
    bool Fullscreen = true; 
    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true;
        //Application.targetFrameRate = 60;
        Screen.fullScreen = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Fullscreen = !Fullscreen;
            Screen.fullScreen = Fullscreen;
            if(Fullscreen)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else Cursor.lockState = CursorLockMode.None;
        }
    }
}
