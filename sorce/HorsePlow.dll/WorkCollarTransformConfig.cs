using System;
using Vintagestory.API.MathTools;

namespace HorsePlowMod
{
	// Token: 0x0200000B RID: 11
	public sealed class WorkCollarTransformConfig
	{
		// Token: 0x0600008D RID: 141 RVA: 0x000069B0 File Offset: 0x00004BB0
		internal ResolvedWorkCollarTransform Resolve(ResolvedWorkCollarTransform fallback)
		{
			ConfigVec3 translation = this.Translation;
			Vec3f translation2 = ((translation != null) ? translation.Resolve(fallback.Translation) : null) ?? fallback.Translation.Clone();
			ConfigVec3 rotationDegrees = this.RotationDegrees;
			Vec3f rotationDegrees2 = ((rotationDegrees != null) ? rotationDegrees.Resolve(fallback.RotationDegrees) : null) ?? fallback.RotationDegrees.Clone();
			ConfigVec3 scale = this.Scale;
			Vec3f vec3f = ((scale != null) ? scale.Resolve(fallback.Scale) : null) ?? fallback.Scale.Clone();
			vec3f.X = WorkCollarTransformConfig.SanitizeScale(vec3f.X, fallback.Scale.X);
			vec3f.Y = WorkCollarTransformConfig.SanitizeScale(vec3f.Y, fallback.Scale.Y);
			vec3f.Z = WorkCollarTransformConfig.SanitizeScale(vec3f.Z, fallback.Scale.Z);
			return new ResolvedWorkCollarTransform
			{
				Translation = translation2,
				RotationDegrees = rotationDegrees2,
				Scale = vec3f
			};
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00006AA8 File Offset: 0x00004CA8
		private static float SanitizeScale(float value, float fallback)
		{
			return (float.IsFinite(value) && value > 0.001f) ? value : fallback;
		}

		// Token: 0x04000081 RID: 129
		public ConfigVec3 Translation;

		// Token: 0x04000082 RID: 130
		public ConfigVec3 RotationDegrees;

		// Token: 0x04000083 RID: 131
		public ConfigVec3 Scale;
	}
}
