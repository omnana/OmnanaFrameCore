using FrameCore.Runtime;
using FrameCore.Util;
using FrameCore.Utility;
using UnityEngine;

namespace FrameCore.Editor
{
    [CodeGen(typeof(UINodeObject))]
    public class UINodeGenerator : CodeGenerator
    {
        public UINodeGenerator(GameObject go) : base(go)
        {
            LoadObjectCollector<UINodeObject>();
        }

        public override bool Legal { get; } = true;

        public override void Generate()
        {
            Convert();
        }

        public override ObjectCollector[] GetObjectCollectors() => new[] {_objectCollector};

        private void Convert()
        {
            var mainFolder = CodeWriteHelpUtil.GetPrefabUIFolder(Product, GO);
            if (GetTarget<UINodeObject>().NodeType == NodeType.UIPanel)
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
                WriteCtr(_objectCollector, ctrlFilePath);

            if (!FileUtility.Exists(voFilePath))
                WriteVO(_objectCollector, voFilePath);

            WriteVOGen(_objectCollector, voGenFilePath);

            if (!FileUtility.Exists(nodeBuilderFilePath))
                WriteBuilder(_objectCollector, nodeBuilderFilePath, CodeWriteHelpUtil.GetPrefabUIAssetPath(GO),
                    GetTarget<UINodeObject>().NodeType == NodeType.UIPanel);

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
                    writer.Bracket($"public class {Name}Controller : UINodeBaseController<{Name}VO>");
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
                    writer.Bracket($"public partial class {Name}VO : UINodeBaseVO");
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
                    var keyStr = isStage ? "UIPanelKeys" : "UINodeKeys";
                    var key2Str = isStage ? "UIPanelKey" : "UINodeKey";
                    writer.Bracket($"public static partial class {keyStr}");
                    writer.WriteLine(
                        $@"public static readonly {key2Str} {Name} = new {key2Str}(""{assetPath}"",""{Name}"", () => new {Name}Builder());");
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