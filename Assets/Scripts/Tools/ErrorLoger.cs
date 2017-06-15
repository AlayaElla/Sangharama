using UnityEngine;
using System.Collections;

public class ErrorLoger : MonoBehaviour {

    internal void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    internal void OnDisable()
    {
        Application.logMessageReceived += null;
    }

    private string m_logs;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logString">错误信息</param>
    /// <param name="stackTrace">跟踪堆栈</param>
    /// <param name="type">错误类型</param>
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        m_logs += logString + "\n";
    }

    public bool Log;
    private Vector2 m_scroll;
    string buttontext = "OPEN";
    internal void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 50, 0, 50, 50), buttontext))
        {
            Log = !Log;
            m_logs = null;
            if (Log)
                buttontext = "CLOSE";
            else
                buttontext = "OPEN";
        }


        if (!Log)
            return;

        GUIStyle bb = new GUIStyle();
        bb.fontSize = 20;
        bb.normal.textColor = new Color(255, 255, 255);

        m_scroll = GUILayout.BeginScrollView(m_scroll, bb);
        GUILayout.Label(m_logs, bb);
        GUILayout.EndScrollView();
    }
}
