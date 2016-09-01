using UnityEngine;
using System.Collections;

public class ScreenFix : MonoBehaviour {

    void Awake()
    {
        int width = (int)(640 * 0.6f);
        int height = (int)(1136 * 0.6f);
        Screen.SetResolution(width, height, false);
    }
}
