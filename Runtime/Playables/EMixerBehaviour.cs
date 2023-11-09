using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;

#nullable enable

namespace Celezt.Timeline
{
    public class EMixerBehaviour : PlayableBehaviour
    {
        public ETrackAsset Track => _track;

        public bool IsProcessing => _firstBehaviour != null || _secondBehaviour != null;

        public double Time => _time;
        public double PreviousTime => _previousTime;
        public bool IsPlayingForward => _isPlayingForward;

        public IEnumerable<EPlayableBehaviour> CurrentBehaviours
        {
            get
            {
                if (_firstBehaviour != null)
                    yield return _firstBehaviour;
                if (_secondBehaviour != null) 
                    yield return _secondBehaviour;
            }
        }
        public IEnumerable<EPlayableAsset> CurrentAssets => CurrentBehaviours.Select(x => x.Asset);

        private float _firstWeight;
        private float _secondWeight;
        private Playable _firstPlayable;
        private Playable _secondPlayable;
        private EPlayableBehaviour? _firstBehaviour;
        private EPlayableBehaviour? _secondBehaviour;

        internal ETrackAsset _track = null!;

        private double _previousTime;
        private double _time;
        private bool _isPlayingForward;

        /// <summary>
        /// When a clip is first entered regardless of direction.
        /// </summary>
        protected virtual void OnEnterClip(Playable playable, EPlayableBehaviour behaviour, FrameData info, float weight, object playerData) { }
        /// <summary>
        /// When processing a clip. Does not call at the same frame as enter clip.
        /// </summary>
        protected virtual void OnProcessClip(Playable playable, EPlayableBehaviour behaviour, FrameData info, float weight, object playerData) { }
        /// <summary>
        /// After a clip has been exited.
        /// </summary>
        protected virtual void OnExitedClip(Playable playable, EPlayableBehaviour behaviour, FrameData info, float weight, object playerData) { }

        public sealed override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            ProcessClips(playable, info, playerData);
        }

        private void ProcessClips(Playable playable, FrameData info, object playerData)
        {
            _time = playable.GetTime();
            _isPlayingForward = _time >= _previousTime;

            float currentFirstWeight = 0;
            float currentSecondWeight = 0;
            Playable currentFirstPlayable = default;
            Playable currentSecondPlayable = default;
            EPlayableBehaviour? currentFirstBehaviour = null;
            EPlayableBehaviour? currentSecondBehaviour = null;

            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                float weight = playable.GetInputWeight(i);
                if (weight > 0)
                {
                    var currentPlayable = playable.GetInput(i);
                   
                    if (!currentPlayable.GetPlayableType().IsSubclassOf(typeof(EPlayableBehaviour)))
                        continue;

                    var currentBehaviour = ((ScriptPlayable<EPlayableBehaviour>)currentPlayable).GetBehaviour();

                    if (currentFirstBehaviour == null)
                    {
                        currentFirstPlayable = currentPlayable;
                        currentFirstBehaviour = currentBehaviour;
                    }
                    else if (currentSecondBehaviour == null)
                    {
                        currentSecondPlayable = currentPlayable;
                        currentSecondBehaviour = currentBehaviour;
                    }

                    if (currentBehaviour == currentFirstBehaviour)
                        currentFirstWeight = weight;
                    else if (currentBehaviour == currentSecondBehaviour)
                        currentSecondWeight = weight;
                }
            }

            if (_firstBehaviour != null &&
                _firstBehaviour != currentFirstBehaviour &&
                _firstBehaviour != currentSecondBehaviour)
            {
                _firstBehaviour.OnExited(_firstPlayable, info, _firstWeight, playerData);
                OnExitedClip(_firstPlayable, _firstBehaviour, info, _firstWeight, playerData);
            }

            if (_secondBehaviour != null &&
                _secondBehaviour != currentFirstBehaviour &&
                _secondBehaviour != currentSecondBehaviour)
            {
                _secondBehaviour.OnExited(_secondPlayable, info, _secondWeight, playerData);
                OnExitedClip(_secondPlayable, _secondBehaviour, info, _secondWeight, playerData);
            }

            if (currentFirstBehaviour != null)
            {
                if (currentFirstBehaviour != _firstBehaviour &&
                    currentFirstBehaviour != _secondBehaviour)
                {
                    currentFirstBehaviour.OnEnter(currentFirstPlayable, info, currentFirstWeight, playerData);
                    OnEnterClip(currentFirstPlayable, currentFirstBehaviour, info, currentFirstWeight, playerData);
                }
                else
                {
                    currentFirstBehaviour.OnProcess(currentFirstPlayable, info, currentFirstWeight, playerData);
                    OnProcessClip(currentFirstPlayable, currentFirstBehaviour, info, currentFirstWeight, playerData);
                }
            }

            if (currentSecondBehaviour != null)
            {
                if (currentSecondBehaviour != _firstBehaviour &&
                    currentSecondBehaviour != _secondBehaviour)
                {
                    currentSecondBehaviour.OnEnter(currentSecondPlayable, info, currentSecondWeight, playerData);
                    OnEnterClip(currentSecondPlayable, currentSecondBehaviour, info, currentSecondWeight, playerData);
                }
                else
                {
                    currentSecondBehaviour.OnProcess(currentSecondPlayable, info, currentSecondWeight, playerData);
                    OnProcessClip(currentSecondPlayable, currentSecondBehaviour, info, currentSecondWeight, playerData);
                }
            }

            _firstWeight = currentFirstWeight;
            _secondWeight = currentSecondWeight;
            _firstPlayable = currentFirstPlayable;
            _secondPlayable = currentSecondPlayable;
            _firstBehaviour = currentFirstBehaviour;
            _secondBehaviour = currentSecondBehaviour;

            _previousTime = _time;
        }
    }
}
