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
    string buttontext = "CLOSE";
    internal void OnGUI()
    {
        if (GUI.Button(new Rect(0, Screen.height - 50, 50, 50), buttontext))
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
        m_scroll = GUILayout.BeginScrollView(m_scroll);
        GUILayout.Label(m_logs);
        GUILayout.EndScrollView();
    }
}
