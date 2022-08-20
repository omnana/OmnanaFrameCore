using System;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;


public delegate void OnLowFrameRateWarningDelegate();
public delegate void OnHighMemoryWarningDelegate();

public class ProfilerReporter : MonoBehaviour
{
    #region 静态
    
    public static OnLowFrameRateWarningDelegate OnLowFrameRateWarningDelegate;
    public static OnHighMemoryWarningDelegate OnHighMemoryWarningDelegate;
    private static ProfilerReporter _profilerReporter;

    public static void Create(bool enableUI)
    {
        if (_profilerReporter == null)
        {
            _profilerReporter = new GameObject("ProfilerReporter").AddComponent<ProfilerReporter>();
            DontDestroyOnLoad(_profilerReporter);
        }

        _enableReportUI = enableUI;
    }

    public static void Destroy()
    {
        GameObject.Destroy(_profilerReporter.gameObject);
    }

    static bool _enableReportUI;


    #endregion

    private StringBuilder _sb;
    private float _elapse;
    private int _frequency = 60;

    private float _allowMemoryLevel; // 内存占用指标
    private const float LowFrameRateLevel = 30; // 最低帧率

    private int _onLowFrameRateCnt;
    private const int LowFrameRateFrequency = 5 * 60;

    private void Awake()
    {
        _sb = new StringBuilder();
        _elapse = int.MaxValue;
        _allowMemoryLevel = 0.90f * Profiler.GetTotalReservedMemoryLong();
    }

    private void Update()
    {
        _elapse ++;
        if (_elapse < _frequency)
            return;

        _elapse = 0f;

        float fps = 1f / Time.smoothDeltaTime;
        long totalReserved = Profiler.GetTotalReservedMemoryLong();
        long totalUnused = Profiler.GetTotalUnusedReservedMemoryLong();
        long allocate = Profiler.GetTotalAllocatedMemoryLong();
        long monoHeap = Profiler.GetMonoHeapSizeLong();
        long monoUsed = Profiler.GetMonoUsedSizeLong();

        _sb.Clear();
        _sb.AppendLine($"FPS: {Math.Round(fps, 2)}");
        _sb.AppendLine($"EffectiveFPS: {OnDemandRendering.effectiveRenderFrameRate}");
        _sb.AppendLine($"FrameInterval: {OnDemandRendering.renderFrameInterval}");
        _sb.AppendLine($"总内存: {Math.Round(totalReserved / 1024D / 1024D, 2)} MB");
        _sb.AppendLine($"空闲内存: {Math.Round(totalUnused / 1024D / 1024D, 2)} MB");
        _sb.AppendLine($"已占用内存: {Math.Round(allocate / 1024D / 1024D, 2)} MB");
        _sb.AppendLine($"Mono堆总内存: {Math.Round(monoHeap / 1024D / 1024D, 2)} MB");
        _sb.AppendLine($"Mono堆已占用内存: {Math.Round(monoUsed / 1024D / 1024D, 2)} MB");

        if (allocate > _allowMemoryLevel) // 内存占用高达90%
        {
            OnLowFrameRateWarningDelegate?.Invoke();
        }

        if (fps < LowFrameRateLevel)
        {
            _onLowFrameRateCnt++;
        }
        else
        {
            _onLowFrameRateCnt = 0;
        }

        if (_onLowFrameRateCnt > LowFrameRateFrequency)
        {
            OnHighMemoryWarningDelegate?.Invoke();
        }
    }

    private void OnGUI()
    {
        if(!_enableReportUI)
            return;
        
        GUIStyle guiStyle = new GUIStyle
        {
            fontSize = 50,
            normal =
            {
                textColor = Color.red,
            }
        };
        GUILayout.TextField(_sb.ToString(), guiStyle);
    }
}
