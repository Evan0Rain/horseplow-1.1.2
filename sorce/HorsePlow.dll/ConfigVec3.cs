using System;
using Vintagestory.API.MathTools;

namespace HorsePlowMod
{
	// Token: 0x0200000C RID: 12
	public sealed class ConfigVec3
	{
		// Token: 0x06000090 RID: 144 RVA: 0x00006AD0 File Offset: 0x00004CD0
		internal Vec3f Resolve(Vec3f fallback)
		{
			return new Vec3f(this.X ?? fallback.X, this.Y ?? fallback.Y, this.Z ?? fallback.Z);
		}

		// Token: 0x04000084 RID: 132
		public float? X;

		// Token: 0x04000085 RID: 133
		public float? Y;

		// Token: 0x04000086 RID: 134
		public float? Z;
	}
}
