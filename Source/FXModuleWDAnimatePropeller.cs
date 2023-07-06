using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static WaterDrinker.WDStatics;

namespace WaterDrinker
{
    public class FXModuleWDAnimatePropeller : PartModule
    {
        Transform lowThrottleTransform;
        Transform highThrottleTransform;

        [KSPField]
        public bool preferMultiMode = true;
        [KSPField]
        public int engineIndex = 0;
        [KSPField]
        public string engineName;
        [KSPField]
        public string lowThrottleTransformName;
        [KSPField]
        public string highThrottleTransformName;
        [KSPField]
        public float lowHighThrottleThreshold;
        [UI_Toggle(disabledText = LOC_WD_MIRROR_FLAG_ENABLED, enabledText = LOC_WD_MIRROR_FLAG_DISABLED)]
        [KSPField(isPersistant = true, guiActive = false, guiActiveEditor = true, guiName = LOC_WD_MIRROR_FLAG, groupName = WD_GROUP_CODE, groupDisplayName = LOC_WD_GROUP_NAME, groupStartCollapsed = true)]
        public bool isMirrored;
        [KSPField]
        public string animationName;
        [KSPField]
        public float baseAnimationSpeed = 0.01F;
        [KSPField]
        public float baseAnimationMult = 1.0F;
        [KSPField]
        public int animationLayer = 1;

        private bool isInFlightScene;
        private bool isInHighThrottle;
        private Animation animation;
        private IEngineStatus engineReference;

        private float mirrorDirectionMult;
        private float reverserDirectionMult;

        public override void OnStart(StartState state)
        {
            // Fetch the throttle transforms.

            lowThrottleTransform = base.part.FindModelTransform(lowThrottleTransformName);
            if(lowThrottleTransform == null)
            {
                Debug.Log($"[{GetType()}] Low throttle transform not found.");
                base.enabled = false;
                return;
            }

            highThrottleTransform = base.part.FindModelTransform(highThrottleTransformName);
            if (highThrottleTransform == null)
            {
                Debug.Log($"[{GetType()}] High throttle transform not found.");
                base.enabled = false;
                return;
            }

            // Fetch engine module on part.

            engineReference = base.part.Modules.FindEngineNearby(engineName, engineIndex, preferMultiMode);

            if(engineReference == null)
            {
                Debug.Log($"[{GetType()}] Engine reference not found.");
                base.enabled = false;
                return;
            }

            // Fetch the animation.

            animation = base.part.FindModelAnimator(animationName);

            if(animation == null)
            {
                Debug.Log($"[{GetType()}] Animation not found.");
                base.enabled = false;
                return;
            }

            animation[animationName].wrapMode = WrapMode.Loop;
            animation[animationName].weight = 1.0F;
            animation[animationName].speed = baseAnimationSpeed;
            animation[animationName].normalizedTime = 0f;
            animation[animationName].layer = animationLayer;

            animation.Play(animationName);

            // If flipped, reverse the animation.

            mirrorDirectionMult = (isMirrored) ? -1.0F : 1.0F;
        }

        public void Update()
        {
            isInFlightScene = HighLogic.LoadedSceneIsFlight;

            if (!isInFlightScene)
            {
                lowThrottleTransform.gameObject.SetActive(true);
                highThrottleTransform.gameObject.SetActive(false);
                animation[animationName].speed = 0.0F;
                return;
            }

            if(!engineReference.isOperational)
            {
                if(Mathf.Abs(animation[animationName].speed) >= 0.001F)
                {
                    animation[animationName].speed = Mathf.MoveTowards(animation[animationName].speed, 0.0F, Time.deltaTime);
                }
                else
                {
                    animation[animationName].speed = 0.0F;
                }

                lowThrottleTransform.gameObject.SetActive(true);
                highThrottleTransform.gameObject.SetActive(false);
                return;
            }

            mirrorDirectionMult = (isMirrored)
                ? -1.0F
                : 1.0F;

            // Set the speed of the propeller animation,
            // taking into account the settings, mirroring, and reversers.

            animation[animationName].speed = (baseAnimationSpeed + (engineReference.throttleSetting * baseAnimationMult))
                * mirrorDirectionMult; 

            isInHighThrottle = engineReference.throttleSetting >= lowHighThrottleThreshold;

            lowThrottleTransform.gameObject.SetActive(!isInHighThrottle);
            highThrottleTransform.gameObject.SetActive(isInHighThrottle); 
        }
    }
}
