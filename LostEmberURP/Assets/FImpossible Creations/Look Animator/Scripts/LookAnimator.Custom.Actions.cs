using UnityEngine;

namespace FIMSpace.FLook
{
    public partial class FLookAnimator
    {

        /// <summary>
        /// Tell controller to stop or start looking without changing target follow object.
        /// </summary>
        /// <param name="enableLooking"> if look == null looking will be toggled </param>
        /// <param name="transitionTime"> Transition time in seconds </param>
        public void SwitchLooking(bool? enableLooking = null, float transitionTime = 0.2f, System.Action callback = null)
        {
            bool enableAnimation = true;

            if (enableLooking == null)
            {
                if (LookAnimatorAmount > 0.5f) enableAnimation = false;
            }
            else if (enableLooking == false) enableAnimation = false;

            // Applying current pose to lerped variables to avoid one-frame stutter
            //if (enableAnimation)
            //{
            //    localAnimationWeight = 0.0f;
            //    SmoothChangeTarget(0.3f, 1f);

            //    newBonesRotations = new Quaternion[lerpRotations.Length + 1];
            //    for (int i = 0; i < lerpRotations.Length; i++)
            //    {
            //        lerpRotations[i] = GetParentBone(i).rotation;
            //        newBonesRotations[i] = lerpRotations[i];
            //    }

            //    headLerpRot = LeadBone.rotation;

            //    for (int i = 0; i < lerpRotationsUltra.Length; i++)
            //    {
            //        lerpRotationsUltra[i] = GetParentBone(i).rotation;
            //    }

            //    headLerpRot = LeadBone.rotation;

            //    newBonesRotations[newBonesRotations.Length - 1] = headLerpRot;
            //}

            StopAllCoroutines();
            StartCoroutine(SwitchLookingTransition(transitionTime, enableAnimation, callback));
        }


        /// <summary>
        /// Setting new target to follow by head bones
        /// </summary>
        public void SetLookTarget(Transform transform)
        {
            ObjectToFollow = transform;
            MomentLookTransform = null;
        }



    }
}