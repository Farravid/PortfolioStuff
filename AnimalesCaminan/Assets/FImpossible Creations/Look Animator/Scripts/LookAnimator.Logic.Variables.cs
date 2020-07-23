using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FLook
{
    /// <summary>
    /// FC: In this partial class we store most of the variables
    /// </summary>
    public partial class FLookAnimator
    {
        /// <summary> How many back bones should be used - helper for editor auto-get of head parent bones </summary>
        public int BackBonesCount = 0;
        public int _preBackBonesCount = 0;
        /// <summary> List of main bone transform used by Look Animator </summary>
        public List<LookBone> LookBones;

        #region Regular Calculations Variables

        // ?
        //internal bool HardStopLook = false;

        /// <summary> Variable to define what's going on with look calculations </summary>
        public EFHeadLookState LookState { get; protected set; }

        /// <summary> Just saved last position of look start position, can be helpful in extending component for custom purposes </summary>
        public Vector3 LastLookStartPosition { get; private set; }

        /// <summary> When target to follow is null then head will stop moving instead of going back to look in forward direction </summary>
        [Tooltip("When target to follow is null then head will stop moving instead of going back to look in forward direction")]
        public bool NoTargetHeadStops = false;

        /// <summary> Variable used in calculating look animation, remembered only to use for avoid one small animation bug </summary>
        //private Quaternion targetLookRotationForBackBones;

        /// <summary> Target look rotation but reserved for LeadBone, there can be "Additional Modules" rotations for nodding etc. </summary>
        private Quaternion targetLookRotation;

        /// <summary> targetLookRotation without minLookAngle - can be used for eyes </summary>
        public Quaternion targetRotationMin { get; private set; }
        public float MinHeadLookAngleValue { get; protected set; }


        private Animator animator;
        protected bool initialized = false;

        #endregion

        /// <summary> Weight of animating bones in local - it can go to 0 when max ranges are exceeded </summary>
        private float finalMotionWeight = 1;
        private float animatedMotionWeight = 1;
        private float _velo_animatedMotionWeight = 1;

        /// <summary> Making rotation animation speed look more smooth when look target is changed </summary>
        private float changeTargetSmootherWeight = 0f;
        private float changeTargetSmootherBones = 0f;

        /// <summary> Variable helping getting right rotations for eye bones if by default they have unconverted rotations to unity axes </summary>
        private Vector3 preLookDir;


        public void InitializeBaseVariables()
        {
            _LOG_NoRefs();

            LookState = EFHeadLookState.Null;

            SetAutoWeightsDefault();
            ComputeBonesRotationsFixVariables();

            InitBirdMode();
            ResetBones();
            ResetAnimateBones();
            //RememberKeyframeRotations();

            // ?
            //preLookRotation = CalculateLimitationAndStuff(Quaternion.LookRotation(transform.forward).eulerAngles, false);
            smoothLookPosition = GetForwardPosition();
            lookFreezeFocusPoint = BaseTransform.InverseTransformPoint(smoothLookPosition);
            refreshReferencePose = true;
            RefreshStartLookPoint = true;
            rootStaticRotation = BaseTransform.rotation;
            //initLeadBoneRotation = LeadBone.localRotation;
            _preBackBonesCount = BackBonesCount;
            lastBaseRotation = BaseTransform.rotation;

            // Reset corrections, sometimes quaternion is NaN there we fix it
            for (int i = 0; i < LookBones.Count; i++)
            {
                if (LookBones[i].correctionOffset == Vector3.zero) LookBones[i].correctionOffset = Vector3.zero;
                LookBones[i].lastKeyframeRotation = LookBones[i].Transform.localRotation;
                LookBones[i].RefreshBoneDirections(BaseTransform);
            }

            //? //if (LeadBone.transform.parent) initHeadParentRot = LeadBone.transform.parent.rotation * Quaternion.Inverse(transform.rotation); else initHeadParentRot = LeadBone.transform.rotation * Quaternion.Inverse(transform.rotation);

            if (UseEyes) InitEyesModule();

            initialized = true;
        }

    }
}