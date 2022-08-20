using System;
using System.ComponentModel;

namespace FrameCore.Runtime
{
    public static class EnumUtil
    {
        public static string ToDes(this Enum enumeration)
        {
            var type = enumeration.GetType();
            var memInfo = type.GetMember(enumeration.ToString());
            if (memInfo.Length <= 0) 
                return enumeration.ToString();
            
            var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attrs.Length > 0 ? ((DescriptionAttribute) attrs[0]).Description : enumeration.ToString();
        }
    }
}
