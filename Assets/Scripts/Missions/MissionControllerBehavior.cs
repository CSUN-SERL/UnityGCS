using UnityEngine;
// using Assets.Scripts;

namespace Missions
{
    [RequireComponent(typeof(MissionTimer))]
    [RequireComponent(typeof(MissionMetaData))]
    [RequireComponent(typeof(MissionLifeCycleController))]
    public class MissionControllerBehavior : MonoBehaviour
    {
    }
}