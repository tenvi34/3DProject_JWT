using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace Polytope
{
    public class PT_Door_Control : MonoBehaviour
    {
        [SerializeField] private Text text;
        [SerializeField] private string playerTag = "Player";
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

#if ENABLE_LEGACY_INPUT_MANAGER

        private void Update()
        {
            if (isPlayerNextToDoor)
            {
                bool isDoorOpen = animator.GetBool("Open");
                text.text = isDoorOpen ? "Press E to close the door" : "Press E to open the door";

                if (Input.GetKeyDown(KeyCode.E))
                {
                    animator.SetBool("Open", !isDoorOpen);    
                }
            }
        }
#endif

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals(playerTag))
            {
#if ENABLE_LEGACY_INPUT_MANAGER
                isPlayerNextToDoor = true;
#else
                animator.SetBool("Open", true);
#endif
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag.Equals(playerTag))
            {
#if ENABLE_LEGACY_INPUT_MANAGER
                isPlayerNextToDoor = false;
                text.text = string.Empty;
#else
                animator.SetBool("Open", false);
#endif
            }
        }

        private bool isPlayerNextToDoor { get; set; } = false;
    }
}
