using FrameCore.Runtime;
using UnityEngine;

namespace GameLogic
{
    public partial class Entrance
    {
        private static void Init()
        {
            Application.targetFrameRate = 60;
            
            IocContainer.Resolve<IUIModule>().Open(UIPanelKeys.UIGameStartPanel);
            IocContainer.Resolve<IMapModule>().RunStage(MapStageKeys.MainStage);
        }
        
        private static void Update(float deltaTime)
        {
        }
        
        private static void LateUpdate(float deltaTime)
        {
        }
        
        private static void FixedUpdate(float deltaTime)
        {
        }
        
        private static void Destroy()
        {
            Debug.Log("Game Destroy");
        }
    }
}
