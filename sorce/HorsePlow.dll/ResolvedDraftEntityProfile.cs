using System;
using Vintagestory.API.MathTools;

namespace HorsePlowMod
{
	// Token: 0x0200000D RID: 13
	public sealed class ResolvedDraftEntityProfile
	{
		// Token: 0x06000092 RID: 146 RVA: 0x00006B44 File Offset: 0x00004D44
		internal static ResolvedDraftEntityProfile CreateEmergencyFallback()
		{
			return new ResolvedDraftEntityProfile
			{
				Name = "emergency-fallback",
				ForwardBlocksPerSecond = 0.96f,
				ReverseBlocksPerSecond = 0.42f,
				LeftPrimaryRopeAnchor = new Vec3f(0.53f, 1.4f, 0.28f),
				LeftSecondaryRopeAnchor = new Vec3f(0.53f, 1.4f, -0.35f),
				RightPrimaryRopeAnchor = new Vec3f(0.53f, 1.4f, -0.35f),
				RightSecondaryRopeAnchor = new Vec3f(0.53f, 1.4f, 0.28f),
				WorkCollarTransform = new ResolvedWorkCollarTransform
				{
					Translation = new Vec3f(0f, 0f, 0f),
					RotationDegrees = new Vec3f(0f, 0f, 0f),
					Scale = new Vec3f(1f, 1f, 1f)
				}
			};
		}

		// Token: 0x04000087 RID: 135
		public string Name;

		// Token: 0x04000088 RID: 136
		public float ForwardBlocksPerSecond;

		// Token: 0x04000089 RID: 137
		public float ReverseBlocksPerSecond;

		// Token: 0x0400008A RID: 138
		public Vec3f LeftPrimaryRopeAnchor;

		// Token: 0x0400008B RID: 139
		public Vec3f LeftSecondaryRopeAnchor;

		// Token: 0x0400008C RID: 140
		public Vec3f RightPrimaryRopeAnchor;

		// Token: 0x0400008D RID: 141
		public Vec3f RightSecondaryRopeAnchor;

		// Token: 0x0400008E RID: 142
		public ResolvedWorkCollarTransform WorkCollarTransform;
	}
}
