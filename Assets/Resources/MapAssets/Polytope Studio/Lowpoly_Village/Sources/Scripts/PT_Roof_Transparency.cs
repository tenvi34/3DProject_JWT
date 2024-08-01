using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytope
{
    public class PT_Roof_Transparency : MonoBehaviour
    {
        [System.Serializable]
        public class Group
        {
            [SerializeField] public GameObject parent;
            [SerializeField] public GameObject[] showGroups;
            [SerializeField] public GameObject[] hideGroups;
            public bool Inbound { get; set; } = false;
            public Collider Collider { get { return parent.GetComponent<Collider>(); } }

        }

        [SerializeField] private Group[] groups;
        [SerializeField] [Range(0f, 1f)] private float opaqueMin = .05f;
        [SerializeField] [Range(0f, 1f)] private float opaqueMax = 1f;
        [SerializeField] [Range(0f, 1f)] private float windowMin = .05f;
        [SerializeField] [Range(0f, 1f)] private float windowMax = .4f;
        [SerializeField] private float time = .5f;
        [SerializeField] private string playerTag = "Player";
        private const string shaderProperty = "_Transparency";
        private GameObject player = null;

        private void Update()
        {
            if(player != null)
            {
                foreach (Group group in groups)
                {
                    if (group.Collider.bounds.Contains(player.transform.position) && group.Inbound == false)
                    {
                        group.Inbound = true;
                        foreach (GameObject show in group.showGroups)
                        {
                            Show(show);
                        }

                        foreach (GameObject hide in group.hideGroups)
                        {
                            Hide(hide);
                        }
                    }

                    if(!group.Collider.bounds.Contains(player.transform.position) && group.Inbound == true)
                    {
                        group.Inbound = false;
                    }
                }
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals(playerTag))
            {
                player = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag.Equals(playerTag))
            {
                foreach(Group group in groups)
                {
                    Show(group.parent);
                    group.Inbound = false;
                }

                player = null;
            }
        }

        public void Show(GameObject group)
        {
            Renderer[] renderers = group.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                Collider col = renderer.gameObject.GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = true;
                }

                StartCoroutine(SetTransparency(renderer.materials[0], renderer.material.GetFloat(shaderProperty), opaqueMax, time));

                if(renderer.materials.Length > 1)
                {
                    StartCoroutine(SetTransparency(renderer.materials[1], renderer.material.GetFloat(shaderProperty), windowMax, time));
                }
            }
        }

        public void Hide(GameObject group)
        {
            Renderer[] renderers = group.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                Collider col = renderer.gameObject.GetComponent<Collider>();
                if(col != null)
                {
                    col.enabled = false;
                }

                StartCoroutine(SetTransparency(renderer.materials[0], renderer.material.GetFloat(shaderProperty), opaqueMin, time));

                if (renderer.materials.Length > 1)
                {
                    StartCoroutine(SetTransparency(renderer.materials[1], renderer.material.GetFloat(shaderProperty), windowMin, time));
                }
            }
        }

        private IEnumerator SetTransparency(Material mat, float initial, float target, float time)
        {
            float elapsed = 0;
            while (elapsed < time)
            {
                float transparencyValue = Mathf.Lerp(initial, target, elapsed / time);
                mat.SetFloat(shaderProperty, transparencyValue);
                elapsed += Time.deltaTime;
                yield return null;
            }

            mat.SetFloat(shaderProperty, target);
        }

        public bool Inbound { get; private set; }
    }
}