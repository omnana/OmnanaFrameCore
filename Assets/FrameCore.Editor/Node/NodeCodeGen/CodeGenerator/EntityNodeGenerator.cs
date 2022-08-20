using FrameCore.Runtime;
using FrameCore.Util;
using FrameCore.Utility;
using UnityEngine;

namespace FrameCore.Editor
{
    [CodeGen(typeof(EntityObject))]
    public class EntityNodeGenerator : CodeGenerator
    {
        public EntityNodeGenerator(GameObject go) : base(go)
        {
            LoadObjectCollector<EntityObject>();
        }

        public override bool Legal { get; } = true;

        public override ObjectCollector[] GetObjectCollectors() => new[] {_objectCollector};

        public override void Generate()
        {
            Convert(_objectCollector);
        }
    
        private void Convert(ObjectCollector objectCollector)
        {
            var mainFolder = CodeWriteHelpUtil.GetPrefabMapFolder(Product, "Entity/", GO, true);
            var entityGenFolder = $"{mainFolder}/Gen";

            if (!DirectoryUtil.Exist(mainFolder))
                DirectoryUtil.CreateDirectory(mainFolder);
            if (!DirectoryUtil.Exist(entityGenFolder))
                DirectoryUtil.CreateDirectory(entityGenFolder);

            var entityFilePath = $"{mainFolder}/{Name}.cs";
            var entityContextFilePath = $"{mainFolder}/{Name}Context.cs";
            var genFilePath = $"{entityGenFolder}/{Name}.Gen.cs";
            if (!FileUtility.Exists(entityFilePath))
                WriteEntity(objectCollector, entityFilePath);
            if (!FileUtility.Exists(entityContextFilePath))
                WriteEntityContext(objectCollector, entityContextFilePath);

            WriteEntityGen(objectCollector, genFilePath);
            Debug.Log($"{Name}相关代码生成成功！");
        }

        private void WriteEntity(ObjectCollector objectCollector, string path)
        {
            using (CodeWriter writer = new CodeWriter(path))
            {
                WriteNamespace(writer, objectCollector);
                writer.WriteLine();
                writer.Bracket($"namespace {Product}");
                {
                    writer.Bracket($"public class {Name}Data : BaseEntityData");
                    writer.EndBracket();
                
                    writer.Bracket($"public partial class {Name} : BaseEntity<{Name}Context, {Name}Data>");
                    {
                        writer.Bracket("protected override void OnInit()");
                        writer.EndBracket();
                    }
                    writer.EndBracket();
                
                }
                writer.EndBracket();
            }
        }
    
        private void WriteEntityGen(ObjectCollector objectCollector, string path)
        {
            using (CodeWriter writer = new CodeWriter(path))
            {
                var prefabPath = CodeWriteHelpUtil.GetPrefabEntityAssetPath(GO);
                WriteNamespace(writer, objectCollector);
                writer.WriteLine();
                writer.Bracket($"namespace {Product}");
                {
                    writer.WriteLine("// 禁止手动修改");
                    writer.Bracket($"public static partial class EntityKeys");
                    writer.WriteLine($@"public static readonly EntityKey {Name.Replace("Entity", "")} = new EntityKey(""{prefabPath}"", () => new {Name}(), () => new {Name}Context());");
                    writer.EndBracket();
                
                    writer.WriteLine("");
                    writer.Bracket($"public partial class {Name}");
                    {
                        CodeWriteHelpUtil.WriteFields2(writer, objectCollector);
                        writer.WriteLine("");
                        writer.Bracket("public override void Gen()");
                        CodeWriteHelpUtil.WriteFields4(writer, objectCollector);
                        writer.EndBracket();
                    }
                    writer.EndBracket();
                }
                writer.EndBracket();
            }
        }
    
        private void WriteEntityContext(ObjectCollector objectCollector, string path)
        {
            using (CodeWriter writer = new CodeWriter(path))
            {
                WriteNamespace(writer, objectCollector);
                writer.WriteLine();
                writer.Bracket($"namespace {Product}");
                {
                    writer.Bracket($"public class {Name}Context : BaseEntityContext");
                    writer.EndBracket();
                }
                writer.EndBracket();
            }
        }
    }
}
