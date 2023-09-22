using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Celezt.Timeline
{
    public abstract class ETrackAsset : TrackAsset
    {
        public EMixerBehaviour Mixer { get; internal set; }

        protected PlayableDirector _director;

        private HashSet<EPlayableAsset> _pendingOnCreate = new HashSet<EPlayableAsset>();

        protected virtual EMixerBehaviour CreateTrackMixer(PlayableGraph graph, PlayableDirector director, GameObject go, int inputCount) => new EMixerBehaviour();

        public sealed override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            if (_director == null)
                _director = graph.GetResolver() as PlayableDirector;

            EMixerBehaviour template = CreateTrackMixer(graph, _director, go, inputCount);
            template._track = this;

            foreach (TimelineClip clip in GetClips())
            {
                if (clip.asset is EPlayableAsset)
                {
                    EPlayableAsset asset = clip.asset as EPlayableAsset;
                    asset._clip = clip;
                    asset._director = _director;

                    EPlayableBehaviour behaviour = null;
                    if (asset.BehaviourReference == null)
                        behaviour = asset.Initialization(graph, go);
                    else
                        behaviour = asset.BehaviourReference;

                    behaviour._director = _director;
                    behaviour._asset = asset;
                    behaviour._clip = clip;

                    clip.displayName = asset.name;

                    behaviour.OnCreateTrackMixer(graph, go, clip);

                    if (_pendingOnCreate.Contains(asset))
                    {
                        behaviour.OnCreate();
                        _pendingOnCreate.Remove(asset);
                    }
                }
            }

            return ScriptPlayable<EMixerBehaviour>.Create(graph, template, inputCount);
        }

        protected sealed override void OnCreateClip(TimelineClip clip)
        {
            if (clip.asset is EPlayableAsset asset)    // Waiting on being created.
                _pendingOnCreate.Add(asset);
        }
    }
}
