using System;
using _00_Asset.StompyRobot.SRF.Scripts.Components;
using UnityEngine.UI;

namespace _00_Asset.StompyRobot.SRDebugger.Scripts.UI.Controls.Data
{
    public class BoolControl : DataBoundControl
    {
        [RequiredField] public Text Title;

        [RequiredField] public Toggle Toggle;

        protected override void Start()
        {
            base.Start();
            Toggle.onValueChanged.AddListener(ToggleOnValueChanged);
        }

        private void ToggleOnValueChanged(bool isOn)
        {
            UpdateValue(isOn);
        }

        protected override void OnBind(string propertyName, Type t)
        {
            base.OnBind(propertyName, t);

            Title.text = propertyName;

            Toggle.interactable = !IsReadOnly;
        }

        protected override void OnValueUpdated(object newValue)
        {
            var value = (bool) newValue;
            Toggle.isOn = value;
        }

        public override bool CanBind(Type type, bool isReadOnly)
        {
            return type == typeof (bool);
        }
    }
}
