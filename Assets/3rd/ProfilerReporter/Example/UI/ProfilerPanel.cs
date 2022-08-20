using UnityEngine;

public class ProfilerPanel : MonoBehaviour
{
    private static ProfilerPanel _panel;
    
    public static ProfilerPanel Open()
    {
        if (_panel == null)
        {
            var prefab = Resources.Load<GameObject>("ProfilerPanel");
            var go = Instantiate(prefab, GameObject.Find("ProfilerReporter").transform, false);
            _panel = go.GetComponent<ProfilerPanel>();
        }
        
        _panel.gameObject.SetActive(true);
        return _panel;
    }
    
    public ProfileWarningItem MemoryItem;
    public ProfileWarningItem FPSItem;
    
    void Awake()
    {
        MemoryItem.gameObject.SetActive(false);
        FPSItem.gameObject.SetActive(false);
        ProfilerReporter.OnHighMemoryWarningDelegate = OnHighMemoryWarning;
        ProfilerReporter.OnLowFrameRateWarningDelegate = OnLowFrameRateWarning;
    }

    private void OnHighMemoryWarning()
    {
        MemoryItem.Open("内存占用高达90%，请检查你的神仙代码！");
    }

    private void OnLowFrameRateWarning()
    {
        FPSItem.Open("FPS长期低于30，请检查啥情况！");
    }
}
