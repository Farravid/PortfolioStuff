using System.Collections;
using UnityEngine;

namespace FIMSpace.FLook
{
    /// <summary>
    /// FC: In this partial class we implement helper courutines
    /// </summary>
    public partial class FLookAnimator
    {
        /// <summary>
        /// Support for 'animate physics' option inside unity's Animator
        /// </summary>
        private IEnumerator AnimatePhysicsClock()
        {
            animatePhysicsWorking = true;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                triggerAnimatePhysics = true;
            }
        }

        private bool animatePhysicsWorking = false;
        private bool triggerAnimatePhysics = false;


        /// <summary>
        /// Coroutine to turn whole look animation on or off
        /// </summary>
        private IEnumerator SwitchLookingTransition(float transitionTime, bool enableAnimation, System.Action callback = null)
        {
            float time = 0f;
            float startBlend = LookAnimatorAmount;

            while (time < transitionTime)
            {
                time += Time.deltaTime;
                float progress = time / transitionTime;

                if (enableAnimation)
                    LookAnimatorAmount = Mathf.Lerp(startBlend, 1f, progress);
                else
                    LookAnimatorAmount = Mathf.Lerp(startBlend, 0f, progress);

                yield return null;
            }

            if (callback != null) callback.Invoke();
        }



        /// <summary>
        /// Setting target Transform 
        /// </summary>
        private IEnumerator CResetMomentLookTransform(Transform transform, float time)
        {
            yield return new WaitForSeconds(time);
            MomentLookTransform = transform;
        }


        /// <summary>
        /// Refreshing reference 
        /// </summary>
        private IEnumerator CStartAfterTPose()
        {
            yield return null;
            yield return new WaitForSeconds(0.1f);
            InitializeBaseVariables();
            //Init();
        }


    }
}