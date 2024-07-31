using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polytope
{
    public class PT_Building_Animation : MonoBehaviour
    {
        [System.Serializable]
        private class SnapPoints
        {
            [SerializeField] public GameObject[] gameObjects;
            [SerializeField] public float startDelay = 0f;
            [SerializeField] public float duration = 1f;
        }

        [SerializeField] private float defaultOpacity = 1f;
        [SerializeField] private float glassOpacity = .24f;
        [SerializeField] private float timePerElement = .25f;
        [SerializeField] private float timePerGroup = 1f;
        [SerializeField] private float startupDelay = 2f;
        [SerializeField] private Transform[] groups;
        [SerializeField] private SnapPoints[] snapPoints;

        private const string shaderProperty = "_Transparency";
        private Renderer[] renderers;

        private void Awake()
        {
            if (renderers == null || renderers.Length == 0)
            {
                renderers = GetComponentsInChildren<Renderer>(true);
            }
        }

        private IEnumerator Play()
        {
            Hide();
            yield return new WaitForSeconds(startupDelay);
            
            if (groups.Length == 0)
            {
                StartCoroutine(AnimationA(defaultOpacity, glassOpacity, timePerElement));
            }
            else
            {
                StartCoroutine(AnimationB());
            }

            StartCoroutine(AnimationSnapPoints());
        }

        private IEnumerator AnimationA(float target0, float target1, float time)
        {
            foreach (Renderer renderer in renderers)
            {
                float elapsed = 0;

                while (elapsed < time)
                {
                    float div = elapsed / time;
                    float mat0 = Mathf.Lerp(0f, target0, div);
                    float mat1 = Mathf.Lerp(0f, target1, div);

                    renderer.materials[0].SetFloat(shaderProperty, mat0);
                    if(renderer.materials.Length > 1)
                    {
                        renderer.materials[1].SetFloat(shaderProperty, mat1);
                    }

                    elapsed += Time.deltaTime;
                    yield return null;
                }

                renderer.materials[0].SetFloat(shaderProperty, target0);
                if (renderer.materials.Length > 1)
                {
                    renderer.materials[1].SetFloat(shaderProperty, target1);
                }
            }
        }

        private IEnumerator AnimationB()
        {
            foreach (Transform transform in groups)
            {
                Renderer[] renderers = transform.GetComponentsInChildren<Renderer>(true);
                foreach(Renderer renderer in renderers)
                {
                    StartCoroutine(SetTransparency(renderer.materials[0], renderer.material.GetFloat(shaderProperty), defaultOpacity, timePerGroup));
                    if (renderer.materials.Length > 1)
                    {
                        StartCoroutine(SetTransparency(renderer.materials[1], renderer.material.GetFloat(shaderProperty), glassOpacity, timePerGroup));
                    }
                }

                yield return new WaitForSeconds(timePerGroup);
            }

        }

        private IEnumerator AnimationSnapPoints()
        {
            foreach(SnapPoints sp in snapPoints)
            {
                if(sp.startDelay > 0f)
                {
                    yield return new WaitForSeconds(sp.startDelay);
                }

                foreach(GameObject go in sp.gameObjects)
                {
                    go.SetActive(true);
                }

                yield return new WaitForSeconds(sp.duration);

                foreach (GameObject go in sp.gameObjects)
                {
                    go.SetActive(false);
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

        // Control

        public void Show()
        {
            if (renderers == null || renderers.Length == 0)
            {
                renderers = GetComponentsInChildren<Renderer>(true);
            }

            foreach (Renderer renderer in renderers)
            {
                renderer.materials[0].SetFloat(shaderProperty, defaultOpacity);

                if (renderer.materials.Length > 1)
                {
                    renderer.materials[1].SetFloat(shaderProperty, glassOpacity);
                }
            }

            foreach (SnapPoints sp in snapPoints)
            {
                foreach (GameObject go in sp.gameObjects)
                {
                    go.SetActive(true);
                }
            }
        }

        public void Hide()
        {
            if (renderers == null || renderers.Length == 0)
            {
                renderers = GetComponentsInChildren<Renderer>(true);
            }

            foreach (Renderer renderer in renderers)
            {
                renderer.materials[0].SetFloat(shaderProperty, 0f);

                if (renderer.materials.Length > 1)
                {
                    renderer.materials[1].SetFloat(shaderProperty, 0f);
                }
            }

            foreach (SnapPoints sp in snapPoints)
            {
                foreach (GameObject go in sp.gameObjects)
                {
                    go.SetActive(false);
                }
            }
        }

        public void StartAnimation()
        {
            StopAllCoroutines();
            StartCoroutine(Play());
        }
    }
}