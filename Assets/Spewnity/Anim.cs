using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

/**
 * A lightweight 2D animator.
 * Drag sprite frames to the frames array (you may have to lock the Anim's GameObject in the inspector).
 * Then specify animations with a unique name and definition. A definition consists of frame numbers 
 * (corresponding to the frames array) that are comma-separated. The definition may also include ranges
 * in the form of min-max (inclusive).
 */
namespace Spewnity
{
    [ExecuteInEditMode]
    public class Anim : MonoBehaviour
    {
        public bool livePreview;
        public string currentAnim;
        public List<Sprite> frames;
        public List<AnimSequence> sequences;
        private int frame = 0;
        private AnimSequence currentSeq;
        private float elapsed = 0;
        private Dictionary<string, AnimSequence> cache;
        private SpriteRenderer sr;

        public void Awake()
        {
            UpdateFrameArrays();

            cache = new Dictionary<string, AnimSequence>();
            foreach (AnimSequence seq in sequences)
                cache.Add(seq.name, seq);
            sr = GetComponent<SpriteRenderer>();
            if (sr == null)
                sr = gameObject.AddComponent<SpriteRenderer>();
        }

        public void Start()
        {
            Play(currentAnim);
        }

        public void OnValidate()
        {
            UpdateFrameArrays();
			// if(currentSeq.name != currentAnim)
			// 	Play(currentSeq.name);
        }

        public void Play(string name)
        {
            currentAnim = name;
            frame = 0;
            elapsed = 0;
            currentSeq = cache[currentAnim];
            if (currentSeq == null)
            {
                Debug.Log("Cannot find sequence " + name);
                return;
            }
            currentSeq.deltaTime = 1 / currentSeq.fps;
            UpdateView();
        }

        public void Update()
        {
            if (currentSeq == null)
                return;

            elapsed += Time.deltaTime;
            if (elapsed >= currentSeq.deltaTime)
            {
                elapsed -= currentSeq.deltaTime;
                if (++frame >= currentSeq.frameArray.Count)
                    frame = 0;
                UpdateView();
            }
        }

        private void UpdateView()
        {
            int cel = currentSeq.frameArray[frame];
            sr.sprite = frames[cel];
        }

        public void UpdateFrameArrays()
        {
            foreach (AnimSequence seq in sequences)
            {
                seq.frameArray = new List<int>();
                seq.desc = seq.desc.Replace(" ", "");
                foreach (string element in seq.desc.Split(','))
                {
                    if (element.Contains("-"))
                    {
                        string[] extents = element.Split('-');
                        int low = int.Parse(extents[0]);
                        int high = int.Parse(extents[1]);
                        for (int i = low; i <= high; i++)
                            seq.frameArray.Add(i);
                    }
                    else
                    {
                        int result = int.Parse(element);
                        seq.frameArray.Add(result);
                    }

                }
            }
        }
    }

	[CustomEditor(typeof(Anim))]
	public class AnimEditor: Editor
	{
        private SerializedProperty frames;
        private SerializedProperty sequences;

		public void OnEnable()
		{
            frames = serializedObject.FindProperty("frames");
            sequences = serializedObject.FindProperty("sequences");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			Anim anim = (Anim) target;
			anim.livePreview = EditorGUILayout.Toggle("Live Preview", anim.livePreview);
			if(anim.livePreview)
				EditorUtility.SetDirty(target);
            anim.currentAnim = EditorGUILayout.DelayedTextField(anim.currentAnim);
            EditorGUILayout.PropertyField(frames, true);
            EditorGUILayout.PropertyField(sequences, true);
			serializedObject.ApplyModifiedProperties();
		}
	}

    [System.Serializable]
    public class AnimSequence
    {
        public string name;

        [TooltipAttribute("Comma separated list of frames and ranges, e.g: 1-7,9,12-10")]
        public string desc;
        public float fps = 30;

        [HideInInspector]
        public List<int> frameArray; // frame string expanded to array
        [HideInInspector]
        public float deltaTime;
    }
}