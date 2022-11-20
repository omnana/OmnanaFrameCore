namespace FrameCore.Runtime
{
    public interface IPersistence
    {
        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool Write<T>(string key, T val);

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Read<T>(string key, T defaultValue);
        
        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key"></param>
        void Delete(string key);

        /// <summary>
        /// 全部清除
        /// </summary>
        void Clear();
    }
}
