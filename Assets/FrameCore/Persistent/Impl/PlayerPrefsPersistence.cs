using LitJson;
using UnityEngine;

namespace FrameCore.Runtime
{
    public class PlayerPrefsPersistence : IPersistence
    {
        public bool Write<T>(string key, T val)
        {
            var json = JsonMapper.ToJson(val);
            PlayerPrefs.SetString(key, json);
            return true;
        }

        public T Read<T>(string key, T defaultValue)
        {
            var str = PlayerPrefs.GetString(key);
            return string.IsNullOrEmpty(str) ? defaultValue : JsonMapper.ToObject<T>(str);
        }

        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public void Clear()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
