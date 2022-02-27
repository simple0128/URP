using System;


namespace UnityEngine.Rendering.Universal 
{
    [Serializable, VolumeComponentMenu("Post-processing/TemporalAntialiasing")]
    public class TemporalAntialiasing : VolumeComponent, IPostProcessComponent
    {

        [Tooltip("The diameter (in texels) inside which jitter samples are spread. Smaller values result in crisper but more aliased output, while larger values result in more stable, but blurrier, output.")]
        public ClampedFloatParameter jitterSpread = new ClampedFloatParameter(0.1f, 1f, 0.75f);

        /// <summary>
        /// Controls the amount of sharpening applied to the color buffer. High values may introduce
        /// dark-border artifacts.
        /// </summary>
        [Tooltip("Controls the amount of sharpening applied to the color buffer. High values may introduce dark-border artifacts.")]
        [Range(0f, 3f)]        
        public ClampedFloatParameter sharpness = new ClampedFloatParameter(0.0f, 3f, 0.25f);


        /// <summary>
        /// The blend coefficient for a stationary fragment. Controls the percentage of history
        /// sample blended into the final color.
        /// </summary>
        [Tooltip("The blend coefficient for a stationary fragment. Controls the percentage of history sample blended into the final color.")]
        [Range(0f, 0.99f)]        
        public ClampedFloatParameter stationaryBlending = new ClampedFloatParameter(0.0f, 0.99f, 0.95f);

        /// <summary>
        /// The blend coefficient for a fragment with significant motion. Controls the percentage of
        /// history sample blended into the final color.
        /// </summary>
        [Tooltip("The blend coefficient for a fragment with significant motion. Controls the percentage of history sample blended into the final color.")]
        [Range(0f, 0.99f)]        
        public ClampedFloatParameter motionBlending = new ClampedFloatParameter(0.0f, 0.99f, 0.85f);

        public bool IsActive() => jitterSpread.value > 0f;

        public bool IsTileCompatible() => false;
    }
}

