
using System;
using _00_Asset.StompyRobot.SRDebugger.Scripts.Internal;
using _00_Asset.StompyRobot.SRDebugger.Scripts.Profiler;
using _00_Asset.StompyRobot.SRF.Scripts.Service;

namespace _00_Asset.StompyRobot.SRDebugger.Scripts.Services
{
#if UNITY_2018_1_OR_NEWER
    using UnityEngine.Rendering;

#endif

    public static class ProfilerServiceSelector
    {
        [ServiceSelector(typeof(IProfilerService))]
        public static Type GetProfilerServiceType()
        {
#if UNITY_2018_1_OR_NEWER
            if(GraphicsSettings.renderPipelineAsset != null)
            {
                return typeof(SRPProfilerService);
            }
#endif

            return typeof(ProfilerServiceImpl);
        }
    }

    public struct ProfilerFrame
    {
        public double FrameTime;
        public double OtherTime;
        public double RenderTime;
        public double UpdateTime;
    }

    public interface IProfilerService
    {
        float AverageFrameTime { get; }
        float LastFrameTime { get; }
        CircularBuffer<ProfilerFrame> FrameBuffer { get; }
    }
}
