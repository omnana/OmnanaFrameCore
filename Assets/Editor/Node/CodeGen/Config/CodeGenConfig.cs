using System.Collections.Generic;
using UnityEngine;

namespace FrameCore.Editor
{
    [CreateAssetMenu(fileName = "CodeGenConfig", menuName = "Create CodeGenConfig", order = 0)]
    public class CodeGenConfig : ScriptableObject
    {
        [SerializeField] public List<UnityEditorInternal.AssemblyDefinitionAsset> CodeGenRefAssemblies =
            new List<UnityEditorInternal.AssemblyDefinitionAsset>();
    }
}