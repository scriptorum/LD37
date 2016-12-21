using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A lightweight 2D animator.
 */
namespace Spewnity
{
    public class Anim : MonoBehaviour
    {
        public List<Sprite> frames;
        public List<AnimSequence> sequences;
        public string currentAnim;
        public int frame = 0;
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