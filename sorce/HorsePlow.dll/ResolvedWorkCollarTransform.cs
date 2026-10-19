using System;
using Vintagestory.API.MathTools;

namespace HorsePlowMod
{
	// Token: 0x0200000E RID: 14
	public sealed class ResolvedWorkCollarTransform
	{
		// Token: 0x06000094 RID: 148 RVA: 0x00006C40 File Offset: 0x00004E40
		public ResolvedWorkCollarTransform Clone()
		{
			return new ResolvedWorkCollarTransform
			{
				Translation = this.Translation.Clone(),
				RotationDegrees = this.RotationDegrees.Clone(),
				Scale = this.Scale.Clone()
			};
		}

		// Token: 0x0400008F RID: 143
		public Vec3f Translation;

		// Token: 0x04000090 RID: 144
		public Vec3f RotationDegrees;

		// Token: 0x04000091 RID: 145
		public Vec3f Scale;
	}
}
