// using Spine.Unity;
// using UnityEngine;
// using System;
//
// namespace FrameCore.Asset.UI.UISpine
// {
//     /// <summary>
//     /// UISpine �򵥷�װ
//     /// </summary>
//     [RequireComponent(typeof(SkeletonGraphic))]
//     public class UISpine : MonoBehaviour, IUISpine
//     {
//         public float Ratio { get => _ratio; set { _ratio = value; } }
//         public string AnimationName
//         {
//             get
//             {
//                 if (_skeletonGraphic == null)
//                 {
//                     _skeletonGraphic = GetComponent<SkeletonGraphic>();
//                 }
//                 return _skeletonGraphic?.startingAnimation;
//             }
//         }
//
//         public bool EnableFrameMode { get => _enableFrameMode; set { _enableFrameMode = value; } }
//
//         public SkeletonGraphic SkeletonGraphic
//         {
//             get
//             {
//                 if (_skeletonGraphic == null)
//                 {
//                     _skeletonGraphic = GetComponent<SkeletonGraphic>();
//                 }
//
//                 return _skeletonGraphic;
//             }
//         }
//
//         [HideInInspector] [SerializeField] private float _ratio;
//
//         [HideInInspector] [SerializeField] private bool _enableFrameMode;
//
//         private SkeletonGraphic _skeletonGraphic;
//
//         public void SetSpine(string assetName)
//         {
//             _skeletonGraphic.skeletonDataAsset = SpineUtility.LoadSpine(assetName);
//             _skeletonGraphic.Initialize(true);
//         }
//
//         public void SetSpineAsync(string assetName, Action callback)
//         {
//             SpineUtility.LoadSpineAsync(assetName, (asset) =>
//             {
//                 _skeletonGraphic.skeletonDataAsset = asset;
//                 _skeletonGraphic.Initialize(true);
//                 callback?.Invoke();
//             });
//         }
//
//         public void SetSkin(string skinName)
//         {
//             _skeletonGraphic.Skeleton.SetSkin(skinName);
//         }
//
//         public void SetAttachment(string slotName, string attachmentName)
//         {
//             _skeletonGraphic.Skeleton.SetAttachment(slotName, attachmentName);
//         }
//
//         public void Play(string animationName, bool loop)
//         {
//             if (_skeletonGraphic == null || _skeletonGraphic.AnimationState == null)
//                 return;
//
//             _skeletonGraphic.AnimationState.ClearTrack(0);
//             _skeletonGraphic.AnimationState.SetAnimation(0, animationName, loop);
//         }
//
//         public void Pause()
//         {
//             if (_skeletonGraphic == null || _skeletonGraphic.AnimationState == null)
//                 return;
//
//             _skeletonGraphic.freeze = true;
//         }
//
//         public void Resume()
//         {
//             if (_skeletonGraphic == null || _skeletonGraphic.AnimationState == null)
//                 return;
//
//             _skeletonGraphic.freeze = false;
//         }
//
//         public void Stop()
//         {
//             if (_skeletonGraphic == null || _skeletonGraphic.AnimationState == null)
//                 return;
//
//             _skeletonGraphic.AnimationState.ClearTracks();
//         }
//
//         public void SetFrame(string animationName, float ratio)
//         {
//             if (string.IsNullOrEmpty(animationName) || _skeletonGraphic == null || _skeletonGraphic.AnimationState == null)
//                 return;
//
//             EnableFrameMode = true;
//             _skeletonGraphic.timeScale = 0;
//             _skeletonGraphic.AnimationState.ClearTrack(0);
//             var trackEntry = _skeletonGraphic.AnimationState.SetAnimation(0, animationName, false);
//             trackEntry.TrackTime = ratio * trackEntry.AnimationEnd;
//         }
//
//         protected void Awake()
//         {
//             Load();
//         }
//
//         private void Load()
//         {
//             _skeletonGraphic = GetComponent<SkeletonGraphic>();
//             if (!_skeletonGraphic.SkeletonDataAsset)
//             {
//                 _skeletonGraphic.Initialize(true);
//             }
//             if (EnableFrameMode)
//             {
//                 _skeletonGraphic.startingLoop = false;
//                 _skeletonGraphic.timeScale = 0;
//                 _skeletonGraphic.OnMeshAndMaterialsUpdated += OnMeshAndMaterialsUpdated;
//             }
//         }
//
//         private void OnMeshAndMaterialsUpdated(SkeletonGraphic sg)
//         {
//             SetFrame(AnimationName, Ratio);
//             _skeletonGraphic.OnMeshAndMaterialsUpdated -= OnMeshAndMaterialsUpdated;
//         }
//
//         public void Dispose()
//         {
//             _skeletonGraphic.Clear();
//             _skeletonGraphic.skeletonDataAsset = null;
//         }
//     }
// }
