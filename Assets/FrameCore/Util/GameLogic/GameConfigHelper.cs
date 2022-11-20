using UnityEngine;

namespace FrameCore.Runtime
{
    public static class GameConfigHelper
    {
        private static string _product;

        public static string GetProduct()
        {
            if (string.IsNullOrEmpty(_product))
            {
                var config = IdealResource.Load<GameLogicConfig>("Config\\GameLogicConfig");
                if (config == null)
                {
                    Debug.LogWarning("请在Resources/Config/文件夹下右键创建GameLogicConfig.asset文件，并指定程序集名称");
                    return _product;
                }

                _product = config.Product;
            }

            return _product;
        }
    }
}
