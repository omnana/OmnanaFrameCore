using System.Collections.Generic;
using System.IO;
using System.Linq;
using FrameCore.Runtime;
using FrameCore.Util;
using FrameCore.Utility;
using UnityEditor;

namespace FrameCore.Editor
{
    /// <summary>
    /// 该打包策略
    /// 1、一个文件夹一个包
    /// 2、一个scene一个ab，并且scene只能单独打ab
    /// 3、剩下的根据最小依赖树打包
    /// </summary>
    internal class AbBuilder
    {
        private readonly Dictionary<string, AbNode> _abNodeDic = new Dictionary<string, AbNode>();

        // 每个文件夹都会单独成包
        private static readonly HashSet<string> IndependentFolder = new HashSet<string>();
        public static readonly HashSet<string> SpecialFolder = new HashSet<string>();

        // 资源对应主文件夹名
        private static readonly Dictionary<string, string> AssetAbDic = new Dictionary<string, string>();

        private readonly HashSet<string> _folderRes = new HashSet<string>();
        private readonly HashSet<string> _sceneRes = new HashSet<string>();
        private readonly HashSet<string> _specialRes = new HashSet<string>();

        private static string _buildFolder;
        private static string _buildFolderPrefix;

        // // <ab, List<File.md5>>
        // private readonly Dictionary<string, List<string>> _abFileMd5NewDic = new Dictionary<string, List<string>>();
        // private Dictionary<string, List<string>> _abFileMd5OldDic;

        // public List<string> DeleteList => _abFileMd5OldDic?.Keys.ToList();

        /// <summary>
        /// 获取打包列表
        /// </summary>
        /// <returns></returns>
        public AssetBundleBuild[] Analyze(AssetConfig config)
        {
            _buildFolderPrefix = $"Assets/{config.name}/RawResources/";
            _buildFolder = AssetBundleHelper.GetBuildFolder(config.name);
            if (!Directory.Exists(_buildFolder))
            {
                FrameDebugger.LogError($"请在{config.name}文件夹下，创建 RawResources 资源文件集！！");
                return null;
            }

            Clear();
            if (config != null)
            {
                foreach (var folder in config.IndependentFolder)
                {
                    IndependentFolder.Add(folder.name);
                }

                foreach (var folder in config.StreamingAssetFolder)
                {
                    SpecialFolder.Add(folder.name);
                }
            }

            // _abFileMd5OldDic = GetAbFileDic();
            var builds = new List<AssetBundleBuild>();
            builds.AddRange(GetFolderBuilds());
            builds.AddRange(GetSceneBuilds());
            builds.AddRange(GetMinDepBuilds());
            builds.Add(SaveAssetConfig(_buildFolder));
            // SaveAbAssetHashConfig();
            return builds.ToArray();
        }

        // 将一个文件夹打成一个包
        private IEnumerable<AssetBundleBuild> GetFolderBuilds()
        {
            var result = new List<AssetBundleBuild>();
            var dirs = Directory.GetDirectories(_buildFolder, "*", SearchOption.AllDirectories);
            foreach (var root in dirs)
            {
                var folderName = Path.GetFileName(root);
                if (IndependentFolder.Contains(folderName) && !SpecialFolder.Contains(folderName))
                {
                    var dirFileDic = new Dictionary<string, List<string>>();
                    DirectoryUtil.TraverseFileOnFolder(root, info =>
                    {
                        if (!IsFileValid(info.FullName))
                            return;

                        var assetBundleName = GetAbNameFromDir(info.DirectoryName);
                        if (!dirFileDic.ContainsKey(assetBundleName))
                        {
                            dirFileDic.Add(assetBundleName, new List<string>());
                        }

                        var assetName = GetAssetName(info.FullName);
                        dirFileDic[assetBundleName].Add(assetName);
                        _folderRes.Add(assetName);
                        AssetAbDic.Add(GetAssetConfigName(assetName), assetBundleName);
                        // AddAbAssetMd5(assetBundleName, info.FullName);
                    });

                    foreach (var kv in dirFileDic)
                    {
                        // string[] assetNames = null;
                        // if (IsAbChange(kv.Key))
                        // {
                        //     assetNames = kv.Value.ToArray();
                        // }

                        var build = new AssetBundleBuild()
                        {
                            assetBundleName = kv.Key,
                            assetNames = kv.Value.ToArray()
                        };
                        result.Add(build);
                    }
                }
            }

            return result;
        }

        // 每个scene独立一个ab
        private IEnumerable<AssetBundleBuild> GetSceneBuilds()
        {
            var result = new List<AssetBundleBuild>();
            var sceneDir = $"{_buildFolderPrefix}Scenes";
            if (!DirectoryUtil.Exist(sceneDir))
                return result;

            FileUtility.SearchAllFile(sceneDir, ".unity", (fileName, fullName) =>
            {
                if (!IsFileValid(fullName))
                    return;

                var assetBundleName = GetAbNameFromDir(sceneDir);
                var assetName = GetAssetName(fullName);
                _sceneRes.Add(assetName);
                AssetAbDic.Add(GetAssetConfigName(assetName), assetBundleName);
                // AddAbAssetMd5(assetBundleName, fullName);
                // string[] assetNames = null;
                // if (IsAbChange(assetBundleName))
                // {
                //     assetNames = new[] {assetName};
                // }

                result.Add(new AssetBundleBuild
                {
                    assetBundleName = assetBundleName,
                    assetNames = new[] {assetName}
                });
            });

            return result;
        }

        // 其他根据最小依赖树打包
        private IEnumerable<AssetBundleBuild> GetMinDepBuilds()
        {
            var result = new List<AssetBundleBuild>();
            var dirs = Directory.GetDirectories(_buildFolder);
            var targetFiles = (from d in dirs
                let dirName = Path.GetFileNameWithoutExtension(d)
                where !IndependentFolder.Contains(dirName) && !SpecialFolder.Contains(dirName)
                from file in Directory.GetFiles(d, "*.*", SearchOption.AllDirectories)
                let abFile = GetAbNameFromFile(file)
                where !file.EndsWith(".meta") && !file.EndsWith(".unity") && !_folderRes.Contains(abFile) &&
                      !_sceneRes.Contains(abFile)
                select file).ToList();

            AnalyzeComplex(targetFiles);
            foreach (var n in _abNodeDic.Values)
            {
                if (n.IsCombine)
                    continue;

                var assetBundleName = GetAbName(n);
                var assetNames = n.Combinelist.ToArray();
                foreach (var assetName in assetNames)
                {
                    AssetAbDic.Add(GetAssetConfigName(assetName), assetBundleName);
                    // AddAbAssetMd5(assetBundleName, (Application.dataPath + "/" + assetName).Replace("assets/", ""));
                }

                // if (!IsAbChange(assetBundleName))
                // {
                //     assetNames = null;
                // }

                var build = new AssetBundleBuild {assetBundleName = assetBundleName, assetNames = assetNames,};

                result.Add(build);
            }

            return result.ToArray();
        }

        // 保存资源关系配置
        private AssetBundleBuild SaveAssetConfig(string configPath)
        {
            var filePath = $"{configPath}/assetConfig.bytes";
            var assetConfig = LitJson.JsonMapper.ToJson(AssetAbDic);
            var bytes = System.Text.Encoding.UTF8.GetBytes(assetConfig);
            FileUtility.WriteFile(filePath, bytes);
            return new AssetBundleBuild()
            {
                assetBundleName = "assetConfig",
                assetNames = new[] {GetAssetName(filePath)}
            };
        }

        // // 保存资源关系配置
        // private void SaveAbAssetHashConfig()
        // {
        //     var filePath = $"{Environment.CurrentDirectory}\\abAssetHashConfig.bytes";
        //     var assetConfig = LitJson.JsonMapper.ToJson(_abFileMd5NewDic);
        //     var bytes = System.Text.Encoding.UTF8.GetBytes(assetConfig);
        //     FileUtility.WriteFile(filePath, bytes);
        // }

        // // 获取ab跟资源的关系表
        // private Dictionary<string, List<string>> GetAbFileDic()
        // {
        //     var filePath = $"{Environment.CurrentDirectory}\\abAssetHashConfig.bytes";
        //     if (FileUtility.Exists(filePath))
        //     {
        //         var bytes = FileUtility.ReadFileToByte(filePath);
        //         var str = System.Text.Encoding.UTF8.GetString(bytes);
        //         return LitJson.JsonMapper.ToObject<Dictionary<string, List<string>>>(str);
        //     }
        //
        //     return null;
        // }

        // 建立依赖节点，生成一张有向无环图
        private void AnalyzeComplex(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                // 提取在unity资源Assets目录下路径
                if (!IsFileValid(file))
                    continue;

                var abNode = CreateNode(file);
                AnalyzeNode(abNode);
            }

            foreach (var n in _abNodeDic.Where(n => n.Value.BeDependences.Count == 0))
            {
                n.Value.IsRoot = true;
            }

            // 剪枝
            CropAbNodeMap();

            // 删除多余节点
            DeleteRedundantNode();
        }

        private AbNode CreateNode(string path)
        {
            var name = GetAbNameFromFile(path);
            if (string.IsNullOrEmpty(name))
                return null;

            if (_abNodeDic.ContainsKey(name))
                return _abNodeDic[name];

            var abNode = new AbNode()
            {
                Name = name,
                Path = path,
            };

            abNode.Combinelist.Add(GetAssetName(path));
            _abNodeDic.Add(name, abNode);
            return _abNodeDic[name];
        }

        /// <summary>
        /// 加载依赖项
        /// </summary>
        /// <param name="abNode"></param>
        private void AnalyzeNode(AbNode abNode)
        {
            if (abNode == null)
                return;

            // 提取依赖项
            var dependPaths = AssetDatabase.GetDependencies(abNode.Path);
            foreach (var tempDependPath in dependPaths)
            {
                var tempDependPathName = GetAbNameFromFile(tempDependPath);
                if (tempDependPathName.Equals(abNode.Name) ||
                    _folderRes.Contains(tempDependPathName)) // 依赖自己， 或者已经在独立包内， 或者在被忽略的文件夹
                    continue;

                if (!IsFileValid(tempDependPath)) // 判读文件合法性。具体看整合代码
                    continue;

                var abDependNode = CreateNode(tempDependPath);
                if (abDependNode == null)
                    continue;

                abNode.Dependences.Add(tempDependPath, abDependNode); // 添加依赖项
                abDependNode.BeDependences.Add(abNode.Path, abNode); // 添加被依赖项
                if (_abNodeDic.ContainsKey(tempDependPathName))
                    continue;

                _abNodeDic.Add(tempDependPathName, abDependNode);
                AnalyzeNode(abDependNode); // 递归分析，保证所有节点被创建
            }
        }

        /// <summary>
        /// 裁剪
        /// </summary>
        private void CropAbNodeMap()
        {
            foreach (var abNode in _abNodeDic.Values)
            {
                if (abNode.IsRoot)
                    continue;

                var cropList = new List<string>();

                // 被依赖项
                foreach (var beDepend in abNode.BeDependences.Values)
                {
                    var cropNodePath = string.Empty;

                    // 被依赖项的依赖项
                    foreach (var depend in beDepend.Dependences.Values.Where(depend =>
                        depend.Dependences.ContainsKey(abNode.Path)))
                    {
                        cropNodePath = abNode.Path;
                        cropList.Add(beDepend.Path);
                    }

                    if (!string.IsNullOrEmpty(cropNodePath))
                    {
                        beDepend.Dependences.Remove(cropNodePath);
                    }
                }

                foreach (var c in cropList)
                {
                    abNode.BeDependences.Remove(c);
                }
            }
        }

        /// <summary>
        /// 减去多余的资源包
        /// </summary>
        private void DeleteRedundantNode()
        {
            foreach (var abNode in _abNodeDic.Values)
            {
                MergeWithBeDependNode(abNode);
            }

            foreach (var abNode in _abNodeDic.Values)
            {
                MergeWithDependNode(abNode);
            }
        }

        /// <summary>
        /// 跟被依赖合并
        /// </summary>
        /// <param name="abNode"></param>
        private void MergeWithBeDependNode(AbNode abNode)
        {
            if (abNode.IsCombine || string.IsNullOrEmpty(abNode.Path) || abNode.Path.ToLower().EndsWith(".shader"))
                return;

            if (abNode.Dependences.Count == 0)
            {
                // 向上合并
                if (abNode.BeDependences.Count != 1)
                    return;

                var beDepend = abNode.BeDependences.Values.ToArray()[0];
                abNode.IsCombine = true;
                beDepend.Combinelist.AddRange(abNode.Combinelist);
                abNode.BeDependences.Remove(beDepend.Path);
                beDepend.Dependences.Remove(abNode.Path);
            }
            else
            {
                var depends = abNode.Dependences.Values.ToArray();
                foreach (var dp in depends)
                {
                    MergeWithBeDependNode(dp);
                }
            }
        }

        /// <summary>
        /// 所有依赖项合并
        /// </summary>
        /// <param name="abNode"></param>
        private void MergeWithDependNode(AbNode abNode)
        {
            if (abNode.IsCombine)
                return;

            var depends = abNode.Dependences.Values.ToArray();
            for (var i = 0; i < depends.Length; i++)
            {
                if (depends[i].IsCombine)
                    continue;

                for (var j = i + 1; j < depends.Length; j++)
                {
                    if (depends[j].IsCombine)
                        continue;

                    if (IsBeDependsEqual(depends[i], depends[j]))
                    {
                        depends[i].Combinelist.AddRange(depends[j].Combinelist);
                        foreach (var beDepend in depends[j].BeDependences.Values)
                        {
                            beDepend.Dependences.Remove(depends[j].Path);
                        }

                        depends[j].IsCombine = true;
                        depends[j].BeDependences.Clear();
                        abNode.Dependences.Remove(depends[j].Path);
                    }
                }
            }
        }

        private bool IsBeDependsEqual(AbNode a, AbNode b)
        {
            if (a.BeDependences.Count != b.BeDependences.Count || a.BeDependences.Count == 0)
                return false;

            return a.BeDependences.Values.All(beDepend => b.BeDependences.ContainsKey(beDepend.Path));
        }

        private void Clear()
        {
            IndependentFolder.Clear();
            AssetAbDic.Clear();
            SpecialFolder.Clear();

            _folderRes.Clear();
            _abNodeDic.Clear();
            _sceneRes.Clear();
            _specialRes.Clear();
            // _abFileMd5NewDic.Clear();
        }

        #region util

        private bool IsFileValid(string filePath)
        {
            if (filePath.EndsWith(".meta")) return false;
            if (filePath.EndsWith(".cs")) return false;
            if (filePath.EndsWith(".dll")) return false;
            return true;
        }

        // 从abNode拿ab名字
        private string GetAbName(AbNode abNode)
        {
            var abName = abNode.Name.Replace(".ab", "");
            if (abNode.IsRoot)
            {
                // root节点以所在路径为加载路径
                return abName + ".ab";
            }
            
            return "Depends/" + abName + ".ab";
            // return GetHashName(abNode.Name).ToString();
        }

        // 拿文件的ab名
        private string GetAbNameFromFile(string filePath)
        {
            filePath = filePath.Replace("\\", "/");
            var start = filePath.IndexOf("Assets/");
            var end = filePath.LastIndexOf(".");
            var unityPath = filePath.Substring(start, end - start);
            var abName = unityPath.Replace(_buildFolderPrefix, "").ToLower();
            // return GetHashName(abName).ToString();
            return abName + ".ab";
        }

        // 拿文件夹的ab名
        private string GetAbNameFromDir(string dir)
        {
            dir = dir.Replace("\\", "/");
            var abName = dir.Substring(dir.IndexOf("RawResources/") + "RawResources/".Length).ToLower();
            // return GetHashName(abName).ToString();
            return abName + ".ab";
        }

        // 拿资源路径
        private string GetAssetName(string filePath)
        {
            filePath = filePath.Replace("\\", "/");
            return filePath.Substring(filePath.IndexOf("Assets")).ToLower();
        }

        private string GetAssetConfigName(string assetName)
        {
            return assetName.Substring(assetName.IndexOf("rawresources/") + "rawresources/".Length).ToLower();
        }

        uint GetHashName(string abName)
        {
            if (string.IsNullOrEmpty(abName))
                return 0;

            char[] bitarray = abName.ToCharArray();
            int count = bitarray.Length;

            uint hash = 0;
            while (count-- > 0)
            {
                hash = hash * 10 + bitarray[count];
            }

            return hash;
        }

        // private void AddAbAssetMd5(string abName, string filePath)
        // {
        //     if (!_abFileMd5NewDic.ContainsKey(abName))
        //     {
        //         _abFileMd5NewDic.Add(abName, new List<string>());
        //     }
        //
        //     _abFileMd5NewDic[abName].Add(Md5Helper.GetMd5HashFromFile(filePath, DownloadDefine.DownloadLen));
        // }

        // private bool IsAbChange(string abName)
        // {
        //     if (_abFileMd5OldDic == null || !_abFileMd5OldDic.ContainsKey(abName) || !_abFileMd5NewDic.ContainsKey(abName))
        //         return true;
        //
        //     var existHs = new HashSet<string>();
        //     var olds = _abFileMd5OldDic[abName];
        //     foreach (var file in olds)
        //     {
        //         existHs.Add(file);
        //     }
        //
        //     var news = _abFileMd5NewDic[abName];
        //     foreach (var file in news)
        //     {
        //         if (!existHs.Contains(file))
        //         {
        //             _abFileMd5OldDic.Remove(abName);
        //             return true;
        //         }
        //     }
        //
        //     _abFileMd5OldDic.Remove(abName);
        //     return false;
        // }

        #endregion
    }
}
