using UnityEngine;

namespace FrameCore.Runtime
{
    public class ObjInfo : MonoBehaviour
    {
        public int InstanceId = -1;
        public string AssetName = string.Empty;

        public void Init()
        {
            if (string.IsNullOrEmpty(AssetName))
                return;

            // 非空，说明通过克隆实例化，添加引用计数
            InstanceId = gameObject.GetInstanceID();
            IocContainer.Resolve<IPrefabModule>().AddAssetRef(AssetName, gameObject);
        }

        private void OnDestroy()
        {
            // 被动销毁，保证引用计数正确
            IocContainer.Resolve<IPrefabModule>().Destroy(gameObject);
        }
    }
}
