using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

[Serializable]
sealed public class RefreshController : MonoBehaviour
{
    [FormerlySerializedAs("NeedToRefresh")] public bool needToRefresh = false;
}
