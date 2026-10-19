using System;

namespace HorsePlowMod
{
	// Token: 0x02000012 RID: 18
	public sealed class HorsePlowUserConfig
	{
		// Token: 0x060000B7 RID: 183 RVA: 0x00002518 File Offset: 0x00000718
		public void Normalize()
		{
			this.ForwardSpeedMultiplier = HorsePlowUserConfig.SanitizeMultiplier(this.ForwardSpeedMultiplier);
			this.ReverseSpeedMultiplier = HorsePlowUserConfig.SanitizeMultiplier(this.ReverseSpeedMultiplier);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00007AAC File Offset: 0x00005CAC
		public static float SanitizeMultiplier(float value)
		{
			return (float.IsFinite(value) && value > 0f) ? value : 1f;
		}

		// Token: 0x040000A9 RID: 169
		public float ForwardSpeedMultiplier = 1f;

		// Token: 0x040000AA RID: 170
		public float ReverseSpeedMultiplier = 1f;
	}
}
