using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._03_Controller
{
    [Serializable]
    public sealed class RefreshController : MonoBehaviour
    {
        [FormerlySerializedAs("NeedToRefresh")] public bool needToRefresh = false;
    }
}
