﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void quit()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }
}
