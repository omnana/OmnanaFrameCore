using FrameCore.Runtime;
using FrameCore.Util;
using FrameCore.Utility;
using UnityEngine;

namespace FrameCore.Editor
{
    [CodeGen(typeof(MapNodeObject))]
    public class MapNodeGenerator : CodeGenerator
    {
        public MapNodeGenerator(GameObject go) : base(go)
        {
            LoadObjectCollector<MapNodeObject>();
        }

        public override bool Legal { get; } = true;

        public override void Generate()
        {
            Convert(_objectCollector);
        }

        public override ObjectCollector[] GetObjectCollectors() => new[] {_objectCollector};

        private void Convert(ObjectCollector objectCollector)
        {
            var mainFolder = CodeWriteHelpUtil.GetPrefabMapFolder(Product,"Map/", GO);
            if (GetTarget<MapNodeObject>().NodeType == NodeType.MapStage)
            {
                mainFolder = mainFolder.Substring(0, mainFolder.LastIndexOf("/"));
            }

            var ctrlFolder = $"{mainFolder}/Controllers";
            var voFolder = $"{mainFolder}/VO";
            var voGenFolder = $"{mainFolder}/VO/Gen";

            if (!DirectoryUtil.Exist(ctrlFolder))
                DirectoryUtil.CreateDirectory(ctrlFolder);
            if (!DirectoryUtil.Exist(voFolder))
                DirectoryUtil.CreateDirectory(voFolder);
            if (!DirectoryUtil.Exist(voGenFolder))
                DirectoryUtil.CreateDirectory(voGenFolder);

            var ctrlFilePath = $"{ctrlFolder}/{Name}Controller.cs";
            var voFilePath = $"{voFolder}/{Name}VO.cs";
            var voGenFilePath = $"{voGenFolder}/{Name}VO.Gen.cs";
            var nodeBuilderFilePath = $"{mainFolder}/{Name}Builder.cs";

            if (!FileUtility.Exists(ctrlFilePath))
                WriteCtr(objectCollector, ctrlFilePath);

            if (!FileUtility.Exists(voFilePath))
                WriteVO(objectCollector, voFilePath);

            WriteVOGen(objectCollector, voGenFilePath);

            if (!FileUtility.Exists(nodeBuilderFilePath))
                WriteBuilder(objectCollector, nodeBuilderFilePath, CodeWriteHelpUtil.GetPrefabMapAssetPath(GO),
                    GetTarget<MapNodeObject>().NodeType == NodeType.MapStage);

            Debug.Log($"{Name}相关代码生成成功！");
        }

        private void WriteCtr(ObjectCollector objectCollector, string path)
        {
            using (CodeWriter writer = new CodeWriter(path))
            {
                WriteNamespace(writer, objectCollector);
                writer.WriteLine();
                writer.Bracket($"namespace {Product}");
                {
                    writer.Bracket($"public class {Name}Controller : MapNodeBaseController<{Name}VO>");
                    {
                        writer.Bracket("protected override void OnInit()");
                        writer.EndBracket();
                    }
                    writer.EndBracket();
                }
                writer.EndBracket();
            }
        }

        private void WriteVO(ObjectCollector objectCollector, string path)
        {
            using (CodeWriter writer = new CodeWriter(path))
            {
                WriteNamespace(writer, objectCollector);
                writer.WriteLine();
                writer.Bracket($"namespace {Product}");
                {
                    writer.Bracket($"public partial class {Name}VO : MapNodeBaseVO");
                    {
                        writer.Bracket("protected override void OnInit()");
                        writer.EndBracket();
                    }
                    writer.EndBracket();
                }
                writer.EndBracket();
            }
        }

        private void WriteVOGen(ObjectCollector objectCollector, string path)
        {
            using (CodeWriter writer = new CodeWriter(path))
            {
                WriteNamespace(writer, objectCollector);
                writer.WriteLine();
                writer.Bracket($"namespace {Product}");
                {
                    writer.WriteLine("// 禁止手动修改");
                    writer.Bracket($"public partial class {Name}VO");
                    {
                        CodeWriteHelpUtil.WriteFields2(writer, objectCollector);
                        writer.WriteLine("");
                        writer.Bracket("public override void SetObj(NodeObject obj)");
                        writer.WriteLine("base.SetObj(obj);");
                        CodeWriteHelpUtil.WriteFields3(writer, objectCollector);
                        writer.EndBracket();
                    }
                    writer.EndBracket();
                }
                writer.EndBracket();
            }
        }

        private void WriteBuilder(ObjectCollector objectCollector, string path, string assetPath, bool isStage = true)
        {
            using (CodeWriter writer = new CodeWriter(path))
            {
                WriteNamespace(writer, objectCollector);
                writer.WriteLine();

                writer.Bracket($"namespace {Product}");
                {
                    var keyStr = isStage ? "MapStageKeys" : "MapNodeKeys";
                    var keyStr1 = isStage ? "MapStageKey" : "MapNodeKey";
                    writer.Bracket($"public static partial class {keyStr}");
                    writer.WriteLine(
                        $@"public static readonly {keyStr1} {Name} = new {keyStr1}(""{assetPath}"",""{Name}"", () => new {Name}Builder());");
                    writer.EndBracket(); 
                    writer.WriteLine();
            
                    writer.Bracket($"public class {Name}Builder : NodeBuilder");
                    {                
                        writer.WriteLine("// add static node's key");
                        writer.Bracket($"protected override void InitStaticNodeKey(BaseNode node)");
                        writer.EndBracket();
                
                        writer.WriteLine("");
                
                        writer.WriteLine("// add static node's controllers");
                        writer.Bracket($"protected override void AddController(BaseNode node, NodeObject obj)");
                        writer.WriteLine($"node.AddController<{Name}Controller, {Name}VO>(obj);");
                        writer.EndBracket(); 
                    }
                    writer.EndBracket();
                }
                writer.EndBracket();
            }
        }
    }
}