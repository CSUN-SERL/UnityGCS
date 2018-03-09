﻿//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_OSX

using System.Collections.Generic;
using System.Linq;
using Tobii.Framework.Internal;
using UnityEngine;

namespace Assets.Tobii.Framework.Internal
{
    /// <summary>
    ///     Keeps track of the current eye-gaze focus, whether there is a game
    ///     object with an IGazeFocusable component that is focused, or not.
    /// </summary>
    public class GazeFocus : IGazeFocus, IRegisterGazeFocusable,
        IGazeFocusInternal
    {
        private static readonly Dictionary<int, IGazeFocusable> FocusableObjects
            = new Dictionary<int, IGazeFocusable>();

        private static IScorer _multiScorer;

        private readonly GameObject _identifier =
            new GameObject("GazeFocus_UniqueId");

        private Camera _camera;
        private FocusedObject _focusedObject = FocusedObject.Invalid;

        private global::Tobii.Framework.GazePoint _lastHandledGazePoint =
            global::Tobii.Framework.GazePoint.Invalid;

        //---------------------------------------------------------------------
        // Public static properties and methods
        //---------------------------------------------------------------------

        public static bool IsInitialized { get; private set; }

        //---------------------------------------------------------------------
        // Internal static properties
        //---------------------------------------------------------------------

        internal static IScorer Scorer { get; set; }

        /// <summary>
        ///     Layers to detect gaze focus on.
        /// </summary>
        internal static LayerMask LayerMask { get; private set; }

        /// <summary>
        ///     Maximum distance to detect gaze focus within.
        /// </summary>
        internal static float MaximumDistance { get; private set; }

        //---------------------------------------------------------------------
        // Private methods and properties
        //---------------------------------------------------------------------

        private static IDataProvider<global::Tobii.Framework.GazePoint> GazePointDataProvider
        {
            get { return TobiiHost.GetInstance().GetGazePointDataProvider(); }
        }

        //---------------------------------------------------------------------
        // Implementing IGazeFocus
        //---------------------------------------------------------------------

        public Camera Camera
        {
            get { return _camera != null ? _camera : Camera.main; }
            set
            {
                _camera = value;
                if (Scorer != null) Scorer.Reset();
            }
        }

        /// <summary>
        ///     Gets the <see cref="GameObject" /> with gaze focus. Only game objects
        ///     with a <see cref="GazeAware" /> component can be focused using gaze.
        ///     Returns null if no object is focused.
        /// </summary>
        public FocusedObject FocusedObject
        {
            get { return _focusedObject; }

            private set
            {
                if (!_focusedObject.Equals(value))
                {
                    if (_focusedObject.IsValid)
                    {
                        var lostFocusComponent =
                            FocusableObjects[_focusedObject.Key];
                        lostFocusComponent.UpdateGazeFocus(false);
                    }

                    _focusedObject = value;

                    if (_focusedObject.IsValid)
                    {
                        var receivedFocusComponent =
                            FocusableObjects[_focusedObject.Key];
                        receivedFocusComponent.UpdateGazeFocus(true);
                    }
                }
            }
        }

        /// <summary>
        ///     Updates the gaze focus according to the latest gaze data.
        /// </summary>
        public void UpdateGazeFocus()
        {
            if (!IsInitialized) return;

            IEnumerable<global::Tobii.Framework.GazePoint> lastGazePoints;
            if (!TryGetLastGazePoints(out lastGazePoints))
            {
                FocusedObject = Scorer.GetFocusedObject();
                return;
            }

            FocusedObject = Scorer.GetFocusedObject(lastGazePoints, Camera);
        }

        //---------------------------------------------------------------------
        // Implementing IGazeFocusInternal
        //---------------------------------------------------------------------

        /// <summary>
        ///     Registers the supplied <see cref="IGazeFocusable" /> component so that
        ///     the <see cref="GameObject" /> it belongs to can be focused using eye-gaze.
        /// </summary>
        /// <param name="gazeFocusableComponent"></param>
        public void RegisterFocusableComponent(
            IGazeFocusable gazeFocusableComponent)
        {
            if (FocusableObjects.Count == 0) Initialize();

            var instanceId = gazeFocusableComponent.gameObject.GetInstanceID();
            if (!FocusableObjects.ContainsKey(instanceId))
                FocusableObjects.Add(instanceId, gazeFocusableComponent);
        }

        /// <summary>
        ///     Unregisters the supplied <see cref="IGazeFocusable" /> component so
        ///     that the <see cref="GameObject" /> it belongs to no longer can be
        ///     focused using eye-gaze.
        /// </summary>
        /// <param name="gazeFocusableComponent"></param>
        public void UnregisterFocusableComponent(
            IGazeFocusable gazeFocusableComponent)
        {
            var instanceId = gazeFocusableComponent.gameObject.GetInstanceID();
            if (_focusedObject.IsValid &&
                _focusedObject.GameObject.GetInstanceID() == instanceId)
                FocusedObject = FocusedObject.Invalid;

            FocusableObjects.Remove(instanceId);

            if (Scorer != null)
                Scorer.RemoveObject(gazeFocusableComponent.gameObject);
            if (_multiScorer != null)
                _multiScorer.RemoveObject(gazeFocusableComponent.gameObject);
        }

        /// <summary>
        ///     Notifies that the gaze focus settings have changed and need to be reloaded.
        /// </summary>
        public static void SettingsUpdated()
        {
            if (IsInitialized) ReloadSettings();
        }

        /// <summary>
        ///     Checks if a component is registered as a focusable object.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static bool IsFocusableObject(GameObject gameObject)
        {
            var instanceId = gameObject.GetInstanceID();
            return FocusableObjects.ContainsKey(instanceId);
        }

        private void Initialize()
        {
            if (IsInitialized) return;

            ReloadSettings();
            var gazePointDataProvider =
                TobiiHost.GetInstance().GetGazePointDataProvider();
            gazePointDataProvider.Start(_identifier.GetInstanceID());

            if (Scorer == null) Scorer = new SingleRaycastHistoricHitScore();
            if (_multiScorer == null)
                _multiScorer = new MultiRaycastHistoricHitScore();

            IsInitialized = true;
        }

        private static void ReloadSettings()
        {
            var gazeFocusSettings = GazeFocusSettings.Get();
            LayerMask = gazeFocusSettings.LayerMask;
            MaximumDistance = gazeFocusSettings.MaximumDistance;
            if (Scorer != null)
                Scorer.Reconfigure(MaximumDistance, LayerMask.value);
            if (_multiScorer != null)
                _multiScorer.Reconfigure(MaximumDistance, LayerMask.value);
        }

        private bool TryGetLastGazePoints(
            out IEnumerable<global::Tobii.Framework.GazePoint> gazePoints)
        {
            if (!IsInitialized)
            {
                gazePoints = new List<global::Tobii.Framework.GazePoint>();
                return false;
            }

            gazePoints = GazePointDataProvider
                .GetDataPointsSince(_lastHandledGazePoint).ToList();
            UpdateLastHandledGazePoint(gazePoints);

            return gazePoints.Any();
        }

        private void UpdateLastHandledGazePoint(
            IEnumerable<global::Tobii.Framework.GazePoint> gazePoints)
        {
            foreach (var gazePoint in gazePoints)
                if (gazePoint.Timestamp >
                    _lastHandledGazePoint.Timestamp + float.Epsilon)
                    _lastHandledGazePoint = gazePoint;
        }

        private bool IsDifferent(GameObject first, GameObject second)
        {
            if (null == first && null == second) return false;

            if (null == first || null == second) return true;

            return first.GetInstanceID() != second.GetInstanceID();
        }
    }
}

#else
using System.Collections.Generic;
using UnityEngine;

namespace Tobii.Gaming.Internal
{
    public class GazeFocus : GazeFocusStub
    {
        // all implementation is in the stub
    }
}
#endif