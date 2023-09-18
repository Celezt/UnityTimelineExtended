using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Celezt.Timeline
{
    public abstract class TrackAssetExtended : TrackAsset
    {
        public MixerBehaviourExtended Mixer { get; internal set; }

        protected PlayableDirector _director;

        private HashSet<PlayableAssetExtended> _pendingOnCreate = new HashSet<PlayableAssetExtended>();

        protected virtual MixerBehaviourExtended CreateTrackMixer(PlayableGraph graph, PlayableDirector director, GameObject go, int inputCount) => new MixerBehaviourExtended();

        public sealed override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            if (_director == null)
                _director = graph.GetResolver() as PlayableDirector;

            MixerBehaviourExtended template = CreateTrackMixer(graph, _director, go, inputCount);
            template.Track = this;

            foreach (TimelineClip clip in GetClips())
            {
                if (clip.asset is PlayableAssetExtended)
                {
                    PlayableAssetExtended asset = clip.asset as PlayableAssetExtended;
                    asset.Clip = clip;
                    asset.Director = _director;
                    asset.IsReady = true;

                    PlayableBehaviourExtended behaviour = null;
                    if (asset.BehaviourReference == null)
                        behaviour = asset.Initialization(graph, go);
                    else
                        behaviour = asset.BehaviourReference;

                    behaviour.Director = _director;
                    behaviour.Asset = asset;
                    behaviour.Clip = clip;

                    clip.displayName = asset.name;

                    behaviour.OnCreateTrackMixer(graph, go, clip);

                    if (_pendingOnCreate.Contains(asset))
                    {
                        behaviour.OnCreateClip();
                        _pendingOnCreate.Remove(asset);
                    }
                }
            }

            return ScriptPlayable<MixerBehaviourExtended>.Create(graph, template, inputCount);
        }

        protected sealed override void OnCreateClip(TimelineClip clip)
        {
            if (clip.asset is PlayableAssetExtended asset)    // Waiting on being created.
                _pendingOnCreate.Add(asset);
        }
    }
}
