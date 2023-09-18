using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Celezt.Timeline
{
    public class PlayableBehaviourExtended : PlayableBehaviour
    {
        /// <summary>
        /// If the clip is currently playing.
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                double time = Director.time;
                return time <= Clip.start && time > Clip.end;
            }
        }

        /// <summary>
        /// How much time is left in unit interval [0-1]. 0 if before and 1 if after.
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

        public TimelineClip Clip { get; internal set; }
        public PlayableAssetExtended Asset { get; internal set; }
        public PlayableDirector Director { get; internal set; }
        /// <summary>
        /// Current process state of the clip.
        /// </summary>
        public ProcessStates ProcessState { get; internal set; }

        public enum ProcessStates
        {
            None,
            Processing,
        }

        /// <summary>
        /// When the clip is created.
        /// </summary>
        public virtual void OnCreateClip() { }
        /// <summary>
        /// This function is called during the CreateTrackMixer phase of the PlayableGraph.
        /// </summary>
        /// <param name="graph">The Graph that owns the current PlayableBehaviour.</param>
        /// <param name="go">The GameObject that the graph is connected to.</param>
        /// <param name="clip">The TimelineClip that owns the current PlayableBehaviour.</param>
        public virtual void OnCreateTrackMixer(PlayableGraph graph, GameObject go, TimelineClip clip) { }
        /// <summary>
        /// This function is called during the ExitClip phase of the PlayableGraph.
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        /// <param name="userData">The user data of the ScriptPlayableOutput that initiated the process pass.</param>
        public virtual void ExitClip(Playable playable, FrameData info, object userData) { }
        /// <summary>
        /// This function is called during the EnterClip phase of the PlayableGraph.
        /// </summary>
        /// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
        /// <param name="info">A FrameData structure that contains information about the current frame context.</param>
        /// <param name="userData">The user data of the ScriptPlayableOutput that initiated the process pass.</param>
        public virtual void EnterClip(Playable playable, FrameData info, object userData) { }
    }
}
