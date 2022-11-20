
using System.Diagnostics;
using System.Reflection;
using Debug = UnityEngine.Debug;

public static class FrameDebugger
{
    public static readonly string logID = "------------------------------------FrameDebugger------------------------------------";
    public static void Log(object message, LogColorType color = LogColorType.Pink)
    {
#if UNITY_EDITOR
        Debug.Log(FormatMessage(message, color));
#elif DEBUG_LOG
            Debug.Log(message);
#endif
    }

    public static void LogError(object message, LogColorType color = LogColorType.Red)
    {
#if UNITY_EDITOR
        Debug.LogError(FormatMessage(message, color));
#elif DEBUG_LOG
            Debug.LogError(message);
#endif
    }

    public static void LogWarning(object message, LogColorType color = LogColorType.Yellow)
    {
#if UNITY_EDITOR
        Debug.LogWarning(FormatMessage(message, color));
#elif DEBUG_LOG
            Debug.LogWarning(message);
#endif
    }

    private static string FormatMessage(object message, LogColorType color)
    {
        string strMsg = message.ToString();
        StackFrame sf = new StackTrace(true).GetFrame(2);
        MethodBase mb = sf.GetMethod();
        int line = +sf.GetFileLineNumber();

        if (strMsg == null)
            strMsg = "Message is null";
        else if (strMsg.Length <= 0)
            strMsg = "Message is empty";

        strMsg += "</color>\n" + mb.DeclaringType.FullName + "." + sf.GetMethod().Name + "|Line:" + line + "\n" + logID + "\n" + sf.GetFileName() + "\n" + line;

        FieldInfo fieldInfo = color.GetType().GetField(color.ToString());
        LogColorAttribute ANet = fieldInfo.GetCustomAttribute<LogColorAttribute>();

        return "<color=" + ANet.color + ">" + strMsg;
    }
}
