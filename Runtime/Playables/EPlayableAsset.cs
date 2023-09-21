using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Celezt.Timeline
{
    public abstract class EPlayableAsset : PlayableAsset, ITimelineClipAsset
    {
        public virtual ClipCaps clipCaps => ClipCaps.None;

        public TimelineClip Clip => _clip;
        public PlayableDirector Director => _director;
        public EPlayableBehaviour BehaviourReference => _template;

        internal TimelineClip _clip;
        internal PlayableDirector _director;

        [SerializeReference, HideInInspector]
        private EPlayableBehaviour _template;
        
        protected abstract EPlayableBehaviour CreateBehaviour(PlayableGraph graph, GameObject owner);
         
        public sealed override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<EPlayableBehaviour>.Create(graph, _template);
        }

        internal EPlayableBehaviour Initialization(PlayableGraph graph, GameObject owner)
        {
            name = GetType().Name.Replace("Asset", "");
            _template = CreateBehaviour(graph, owner);
            return _template;
        }
    }
}
