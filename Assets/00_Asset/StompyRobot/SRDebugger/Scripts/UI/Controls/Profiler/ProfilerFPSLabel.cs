using _00_Asset.StompyRobot.SRDebugger.Scripts.Services;
using _00_Asset.StompyRobot.SRF.Scripts.Components;
using _00_Asset.StompyRobot.SRF.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace _00_Asset.StompyRobot.SRDebugger.Scripts.UI.Controls.Profiler
{
    public class ProfilerFPSLabel : SRMonoBehaviourEx
    {
        private float _nextUpdate;

        protected override void Update()
        {
            base.Update();

            if (Time.realtimeSinceStartup > _nextUpdate)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            _text.text = "FPS: {0:0.00}".Fmt(1f/_profilerService.AverageFrameTime);

            _nextUpdate = Time.realtimeSinceStartup + UpdateFrequency;
        }
#pragma warning disable 649

        [Import] private IProfilerService _profilerService;

        public float UpdateFrequency = 1f;

        [RequiredField] [SerializeField] private Text _text;

#pragma warning restore 649
    }
}
