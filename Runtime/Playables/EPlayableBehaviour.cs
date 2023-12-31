using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Celezt.Timeline
{
    public class EPlayableBehaviour : PlayableBehaviour
    {
        /// <summary>
        /// If currently inside the clip.
        /// </summary>
        public bool IsActive
        {
            get
            {
                double time = Director.time;
                return time <= Clip.start && time > Clip.end;
            }
        }

        /// <summary>
        /// How much time that is left in unit interval [0-1]. 0 if before and 1 if after.
        /// </summary>
        public float Interval
        {
            get
            {
                double startTime = Clip.start;
                double endTime = Clip.end;
                double currentTime = Director.time;

                return Mathf.Clamp01((float)((currentTime - startTime) / (endTime - startTime)));
            }
        }

        public TimelineClip Clip => _clip;
        public EPlayableAsset Asset => _asset;
        public PlayableDirector Director => _director;

        internal TimelineClip _clip;
        internal EPlayableAsset _asset;
        internal PlayableDirector _director;

        /// <summary>
        /// When the clip is created.
        /// </summary>
        public virtual void OnCreate() { }

        public virtual void OnCreateTrackMixer(PlayableGraph graph, GameObject go, TimelineClip clip) { }

        /// <summary>
        /// When first entering the clip regardless of direction.
        /// </summary>
        public virtual void OnEnter(Playable playable, FrameData info, float weight, object playerData) { }
        /// <summary>
        /// When processing the clip. Does not call at the same frame as enter clip.
        /// </summary>
        public virtual void OnProcess(Playable playable, FrameData info, float weight, object playerData) { }
        /// <summary>
        /// After the clip has been exited.
        /// </summary>
        public virtual void OnExited(Playable playable, FrameData info, float weight, object playerData) { }

    }
}
