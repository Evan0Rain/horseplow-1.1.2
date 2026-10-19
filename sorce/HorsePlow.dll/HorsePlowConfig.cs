using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace HorsePlowMod
{
	// Token: 0x02000008 RID: 8
	public sealed class HorsePlowConfig
	{
		// Token: 0x06000082 RID: 130 RVA: 0x00006398 File Offset: 0x00004598
		public void Normalize()
		{
			if (this.HitchWhitelist == null)
			{
				this.HitchWhitelist = Array.Empty<string>();
			}
			if (this.EntityProfiles == null)
			{
				this.EntityProfiles = Array.Empty<DraftEntityProfile>();
			}
			string teamSpeedMode = this.TeamSpeedMode;
			string text = ((teamSpeedMode != null) ? teamSpeedMode.Trim() : null) ?? "";
			bool flag = text.Equals("Fastest", StringComparison.OrdinalIgnoreCase);
			if (flag)
			{
				this.TeamSpeedMode = "Fastest";
			}
			else
			{
				bool flag2 = text.Equals("Average", StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					this.TeamSpeedMode = "Average";
				}
				else
				{
					this.TeamSpeedMode = "Slowest";
				}
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00006434 File Offset: 0x00004634
		public bool IsEntityWhitelisted(Entity entity)
		{
			bool flag = ((entity != null) ? entity.Code : null) == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				string fullCode = entity.Code.ToShortString();
				string pathOnly = entity.Code.Path ?? "";
				foreach (string rawPattern in this.HitchWhitelist ?? Array.Empty<string>())
				{
					bool flag2 = HorsePlowConfig.MatchesPattern(rawPattern, fullCode, pathOnly);
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000064C4 File Offset: 0x000046C4
		internal static bool MatchesPattern(string rawPattern, string fullCode, string pathOnly)
		{
			bool flag = string.IsNullOrWhiteSpace(rawPattern);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				string text = rawPattern.Trim();
				string text2 = text.Contains(':') ? fullCode : pathOnly;
				bool flag2 = text2 == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					int num = 0;
					int i = 0;
					int num2 = -1;
					int num3 = 0;
					while (i < text2.Length)
					{
						bool flag3 = num < text.Length && (text[num] == '?' || char.ToUpperInvariant(text[num]) == char.ToUpperInvariant(text2[i]));
						if (flag3)
						{
							num++;
							i++;
						}
						else
						{
							bool flag4 = num < text.Length && text[num] == '*';
							if (flag4)
							{
								num2 = num++;
								num3 = i;
							}
							else
							{
								bool flag5 = num2 >= 0;
								if (!flag5)
								{
									return false;
								}
								num = num2 + 1;
								num3 = (i = num3 + 1);
							}
						}
					}
					while (num < text.Length && text[num] == '*')
					{
						num++;
					}
					result = (num == text.Length);
				}
			}
			return result;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000065F4 File Offset: 0x000047F4
		public ResolvedDraftEntityProfile ResolveProfile(Entity entity)
		{
			ResolvedDraftEntityProfile resolvedDraftEntityProfile = ResolvedDraftEntityProfile.CreateEmergencyFallback();
			DraftEntityProfile defaultProfile = this.DefaultProfile;
			ResolvedDraftEntityProfile resolvedDraftEntityProfile2 = ((defaultProfile != null) ? defaultProfile.ResolveAgainst(resolvedDraftEntityProfile, "default") : null) ?? resolvedDraftEntityProfile;
			string text;
			if (entity == null)
			{
				text = null;
			}
			else
			{
				AssetLocation code = entity.Code;
				text = ((code != null) ? code.ToShortString() : null);
			}
			string text2 = text ?? "";
			string text3;
			if (entity == null)
			{
				text3 = null;
			}
			else
			{
				AssetLocation code2 = entity.Code;
				text3 = ((code2 != null) ? code2.Path : null);
			}
			string pathOnly = text3 ?? "";
			foreach (DraftEntityProfile draftEntityProfile in this.EntityProfiles ?? Array.Empty<DraftEntityProfile>())
			{
				bool flag = draftEntityProfile == null || !draftEntityProfile.Enabled;
				if (!flag)
				{
					bool flag2 = !draftEntityProfile.Matches(text2, pathOnly);
					if (!flag2)
					{
						return draftEntityProfile.ResolveAgainst(resolvedDraftEntityProfile2, string.IsNullOrWhiteSpace(draftEntityProfile.Name) ? text2 : draftEntityProfile.Name);
					}
				}
			}
			return resolvedDraftEntityProfile2;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000066F0 File Offset: 0x000048F0
		public float CombineTeamSpeed(float leftSpeed, bool hasLeft, float rightSpeed, bool hasRight, float defaultSpeed)
		{
			bool flag = !hasLeft && !hasRight;
			float result;
			if (flag)
			{
				result = HorsePlowConfig.SanitizeSpeed(defaultSpeed, defaultSpeed);
			}
			else
			{
				bool flag2 = hasLeft && !hasRight;
				if (flag2)
				{
					result = HorsePlowConfig.SanitizeSpeed(leftSpeed, defaultSpeed);
				}
				else
				{
					bool flag3 = !hasLeft && hasRight;
					if (flag3)
					{
						result = HorsePlowConfig.SanitizeSpeed(rightSpeed, defaultSpeed);
					}
					else
					{
						leftSpeed = HorsePlowConfig.SanitizeSpeed(leftSpeed, defaultSpeed);
						rightSpeed = HorsePlowConfig.SanitizeSpeed(rightSpeed, defaultSpeed);
						bool flag4 = string.Equals(this.TeamSpeedMode, "Fastest", StringComparison.OrdinalIgnoreCase);
						if (flag4)
						{
							result = Math.Max(leftSpeed, rightSpeed);
						}
						else
						{
							bool flag5 = string.Equals(this.TeamSpeedMode, "Average", StringComparison.OrdinalIgnoreCase);
							if (flag5)
							{
								result = (leftSpeed + rightSpeed) * 0.5f;
							}
							else
							{
								result = Math.Min(leftSpeed, rightSpeed);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000067B4 File Offset: 0x000049B4
		public static float SanitizeSpeed(float value, float fallback)
		{
			return (float.IsFinite(value) && value > 0f) ? value : fallback;
		}

		// Token: 0x04000072 RID: 114
		public string TeamSpeedMode;

		// Token: 0x04000073 RID: 115
		public string[] HitchWhitelist;

		// Token: 0x04000074 RID: 116
		public DraftEntityProfile DefaultProfile;

		// Token: 0x04000075 RID: 117
		public DraftEntityProfile[] EntityProfiles;
	}
}
