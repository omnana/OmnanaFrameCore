using UnityEngine;

namespace FrameCore.Runtime
{
    public static class EntityExtension
    {
        public static void SetPos(this IEntity entity, Vector2 position)
        {
            entity.Object.transform.position = position;
        }
        
        public static void SetLocalPos(this IEntity entity, Vector2 position)
        {
            entity.Object.transform.localPosition = position;
        }

        public static Vector3 GetPos(this IEntity entity)
        {
            return entity.Object.transform.position;
        }

        public static Vector3 GetLocalPos(this IEntity entity)
        {
            return entity.Object.transform.localPosition;
        }

        public static void SetAngle(this IEntity entity, Vector3 angle)
        {
            entity.Object.transform.eulerAngles = angle;
        }

        public static void SetLocalAngle(this IEntity entity, Vector3 angle)
        {
            entity.Object.transform.localEulerAngles = angle;
        }

        public static void SetActive(this IEntity entity, bool isActive)
        {
            entity.Object.gameObject.SetActive(isActive);
        }

        public static void SetActive(this MonoBehaviour mono, bool isActive)
        {
            mono.gameObject.SetActive(isActive);
        }
    }
}
