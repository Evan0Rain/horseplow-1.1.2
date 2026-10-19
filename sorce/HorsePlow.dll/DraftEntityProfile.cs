using System;
using Vintagestory.API.MathTools;

namespace HorsePlowMod
{
	// Token: 0x02000009 RID: 9
	public sealed class DraftEntityProfile
	{
		// Token: 0x06000089 RID: 137 RVA: 0x000067DC File Offset: 0x000049DC
		public bool Matches(string fullCode, string pathOnly)
		{
			foreach (string rawPattern in this.Match ?? Array.Empty<string>())
			{
				bool flag = HorsePlowConfig.MatchesPattern(rawPattern, fullCode, pathOnly);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00006828 File Offset: 0x00004A28
		internal ResolvedDraftEntityProfile ResolveAgainst(ResolvedDraftEntityProfile fallback, string resolvedName)
		{
			float forwardBlocksPerSecond = HorsePlowConfig.SanitizeSpeed(this.ForwardBlocksPerSecond ?? fallback.ForwardBlocksPerSecond, fallback.ForwardBlocksPerSecond);
			float reverseBlocksPerSecond = HorsePlowConfig.SanitizeSpeed(this.ReverseBlocksPerSecond ?? fallback.ReverseBlocksPerSecond, fallback.ReverseBlocksPerSecond);
			RopeAnchorSetConfig ropeAnchors = this.RopeAnchors;
			WorkCollarTransformConfig workCollarTransform = this.WorkCollarTransform;
			ResolvedDraftEntityProfile resolvedDraftEntityProfile = new ResolvedDraftEntityProfile();
			resolvedDraftEntityProfile.Name = resolvedName;
			resolvedDraftEntityProfile.ForwardBlocksPerSecond = forwardBlocksPerSecond;
			resolvedDraftEntityProfile.ReverseBlocksPerSecond = reverseBlocksPerSecond;
			Vec3f vec3f;
			if (ropeAnchors == null)
			{
				vec3f = null;
			}
			else
			{
				ConfigVec3 leftPrimary = ropeAnchors.LeftPrimary;
				vec3f = ((leftPrimary != null) ? leftPrimary.Resolve(fallback.LeftPrimaryRopeAnchor) : null);
			}
			resolvedDraftEntityProfile.LeftPrimaryRopeAnchor = (vec3f ?? fallback.LeftPrimaryRopeAnchor.Clone());
			Vec3f vec3f2;
			if (ropeAnchors == null)
			{
				vec3f2 = null;
			}
			else
			{
				ConfigVec3 leftSecondary = ropeAnchors.LeftSecondary;
				vec3f2 = ((leftSecondary != null) ? leftSecondary.Resolve(fallback.LeftSecondaryRopeAnchor) : null);
			}
			resolvedDraftEntityProfile.LeftSecondaryRopeAnchor = (vec3f2 ?? fallback.LeftSecondaryRopeAnchor.Clone());
			Vec3f vec3f3;
			if (ropeAnchors == null)
			{
				vec3f3 = null;
			}
			else
			{
				ConfigVec3 rightPrimary = ropeAnchors.RightPrimary;
				vec3f3 = ((rightPrimary != null) ? rightPrimary.Resolve(fallback.RightPrimaryRopeAnchor) : null);
			}
			resolvedDraftEntityProfile.RightPrimaryRopeAnchor = (vec3f3 ?? fallback.RightPrimaryRopeAnchor.Clone());
			Vec3f vec3f4;
			if (ropeAnchors == null)
			{
				vec3f4 = null;
			}
			else
			{
				ConfigVec3 rightSecondary = ropeAnchors.RightSecondary;
				vec3f4 = ((rightSecondary != null) ? rightSecondary.Resolve(fallback.RightSecondaryRopeAnchor) : null);
			}
			resolvedDraftEntityProfile.RightSecondaryRopeAnchor = (vec3f4 ?? fallback.RightSecondaryRopeAnchor.Clone());
			resolvedDraftEntityProfile.WorkCollarTransform = (((workCollarTransform != null) ? workCollarTransform.Resolve(fallback.WorkCollarTransform) : null) ?? fallback.WorkCollarTransform.Clone());
			return resolvedDraftEntityProfile;
		}

		// Token: 0x04000076 RID: 118
		public string Name = "";

		// Token: 0x04000077 RID: 119
		public bool Enabled = true;

		// Token: 0x04000078 RID: 120
		public string[] Match = Array.Empty<string>();

		// Token: 0x04000079 RID: 121
		public float? ForwardBlocksPerSecond;

		// Token: 0x0400007A RID: 122
		public float? ReverseBlocksPerSecond;

		// Token: 0x0400007B RID: 123
		public RopeAnchorSetConfig RopeAnchors;

		// Token: 0x0400007C RID: 124
		public WorkCollarTransformConfig WorkCollarTransform;
	}
}
