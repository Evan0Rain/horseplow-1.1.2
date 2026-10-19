using System;
using System.Runtime.CompilerServices;
using Jaunt.Behaviors;
using Jaunt.Systems;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace HorsePlowMod
{
	// Token: 0x02000002 RID: 2
	public sealed class EntityHorsePlow : Entity, ISeatInstSupplier, IMountableListener
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override bool IsCreature
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		public override bool ApplyGravity
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002050 File Offset: 0x00000250
		public override bool IsInteractable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002053 File Offset: 0x00000253
		public override float MaterialDensity
		{
			get
			{
				return 3000f;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000005 RID: 5 RVA: 0x0000205A File Offset: 0x0000025A
		public override double FrustumSphereRadius
		{
			get
			{
				return base.FrustumSphereRadius * 3.0;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000006 RID: 6 RVA: 0x0000206C File Offset: 0x0000026C
		// (set) Token: 0x06000007 RID: 7 RVA: 0x00002080 File Offset: 0x00000280
		private long LeftHorseId
		{
			get
			{
				return this.WatchedAttributes.GetLong("horsePlowLeftHorseId", -1L);
			}
			set
			{
				this.WatchedAttributes.SetLong("horsePlowLeftHorseId", value);
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002094 File Offset: 0x00000294
		// (set) Token: 0x06000009 RID: 9 RVA: 0x000020A8 File Offset: 0x000002A8
		private long RightHorseId
		{
			get
			{
				return this.WatchedAttributes.GetLong("horsePlowRightHorseId", -1L);
			}
			set
			{
				this.WatchedAttributes.SetLong("horsePlowRightHorseId", value);
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000020BC File Offset: 0x000002BC
		// (set) Token: 0x0600000B RID: 11 RVA: 0x000020CF File Offset: 0x000002CF
		private int LeftRopeClothId
		{
			get
			{
				return this.WatchedAttributes.GetInt("horsePlowLeftRopeClothId", 0);
			}
			set
			{
				this.WatchedAttributes.SetInt("horsePlowLeftRopeClothId", value);
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000020E3 File Offset: 0x000002E3
		// (set) Token: 0x0600000D RID: 13 RVA: 0x000020F6 File Offset: 0x000002F6
		private int RightRopeClothId
		{
			get
			{
				return this.WatchedAttributes.GetInt("horsePlowRightRopeClothId", 0);
			}
			set
			{
				this.WatchedAttributes.SetInt("horsePlowRightRopeClothId", value);
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600000E RID: 14 RVA: 0x0000210A File Offset: 0x0000030A
		// (set) Token: 0x0600000F RID: 15 RVA: 0x0000211D File Offset: 0x0000031D
		private int LeftSecondaryRopeClothId
		{
			get
			{
				return this.WatchedAttributes.GetInt("horsePlowLeftSecondaryRopeClothId", 0);
			}
			set
			{
				this.WatchedAttributes.SetInt("horsePlowLeftSecondaryRopeClothId", value);
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000010 RID: 16 RVA: 0x00002131 File Offset: 0x00000331
		// (set) Token: 0x06000011 RID: 17 RVA: 0x00002144 File Offset: 0x00000344
		private int RightSecondaryRopeClothId
		{
			get
			{
				return this.WatchedAttributes.GetInt("horsePlowRightSecondaryRopeClothId", 0);
			}
			set
			{
				this.WatchedAttributes.SetInt("horsePlowRightSecondaryRopeClothId", value);
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002158 File Offset: 0x00000358
		// (set) Token: 0x06000013 RID: 19 RVA: 0x0000216B File Offset: 0x0000036B
		private bool BladesLowered
		{
			get
			{
				return this.WatchedAttributes.GetBool("horsePlowLowered", true);
			}
			set
			{
				this.WatchedAttributes.SetBool("horsePlowLowered", value);
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000014 RID: 20 RVA: 0x0000217F File Offset: 0x0000037F
		private float ForwardBlocksPerSecond
		{
			get
			{
				return this.WatchedAttributes.GetFloat("horsePlowForwardBlocksPerSecond", EntityHorsePlow.EmergencyDraftProfile.ForwardBlocksPerSecond);
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000015 RID: 21 RVA: 0x0000219B File Offset: 0x0000039B
		private float ReverseBlocksPerSecond
		{
			get
			{
				return this.WatchedAttributes.GetFloat("horsePlowReverseBlocksPerSecond", EntityHorsePlow.EmergencyDraftProfile.ReverseBlocksPerSecond);
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000016 RID: 22 RVA: 0x000021B7 File Offset: 0x000003B7
		private EntityAgent LeftHorse
		{
			get
			{
				return this.ResolveHorse(this.LeftHorseId, ref this.leftHorseCache);
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000017 RID: 23 RVA: 0x000021CB File Offset: 0x000003CB
		private EntityAgent RightHorse
		{
			get
			{
				return this.ResolveHorse(this.RightHorseId, ref this.rightHorseCache);
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000257C File Offset: 0x0000077C
		private EntityBehaviorSeatable GetSeatableBehavior()
		{
			EntityBehaviorSeatable result;
			if ((result = this.seatableCache) == null)
			{
				result = (this.seatableCache = this.GetBehavior<EntityBehaviorSeatable>());
			}
			return result;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000025A8 File Offset: 0x000007A8
		private static IMountableSeat FindControllingSeat(EntityBehaviorSeatable seatable)
		{
			bool flag = ((seatable != null) ? seatable.Seats : null) == null;
			IMountableSeat result;
			if (flag)
			{
				result = null;
			}
			else
			{
				foreach (IMountableSeat mountableSeat in seatable.Seats)
				{
					bool flag2 = mountableSeat.CanControl && mountableSeat.Passenger != null;
					if (flag2)
					{
						return mountableSeat;
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600001A RID: 26 RVA: 0x000021DF File Offset: 0x000003DF
		private Entity ControllingPassenger
		{
			get
			{
				IMountableSeat mountableSeat = EntityHorsePlow.FindControllingSeat(this.GetSeatableBehavior());
				return (mountableSeat != null) ? mountableSeat.Passenger : null;
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002614 File Offset: 0x00000814
		public IMountableSeat CreateSeat(IMountable mountable, string seatId, SeatConfig config)
		{
			return new EntityHorsePlowSeat(mountable, seatId, config);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002630 File Offset: 0x00000830
		public override void AfterInitialized(bool onFirstSpawn)
		{
			base.AfterInitialized(onFirstSpawn);
			this.seatableCache = this.GetBehavior<EntityBehaviorSeatable>();
			this.visualAnchorStateInitialized = false;
			bool flag = this.World.Side == 1;
			if (flag)
			{
				this.RefreshConfiguredDraftProfiles();
			}
			this.RebindStoredVisualRope(this.LeftRopeClothId, this.LeftHorse);
			this.RebindStoredVisualRope(this.LeftSecondaryRopeClothId, this.LeftHorse);
			this.RebindStoredVisualRope(this.RightRopeClothId, this.RightHorse);
			this.RebindStoredVisualRope(this.RightSecondaryRopeClothId, this.RightHorse);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000026C0 File Offset: 0x000008C0
		private HorsePlowModSystem GetHorsePlowModSystem()
		{
			bool flag;
			if (this.horsePlowModSystemCache == null)
			{
				IWorldAccessor world = this.World;
				flag = (((world != null) ? world.Api : null) != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.horsePlowModSystemCache = this.World.Api.ModLoader.GetModSystem<HorsePlowModSystem>(true);
			}
			return this.horsePlowModSystemCache;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000271C File Offset: 0x0000091C
		private void RefreshConfiguredDraftProfiles()
		{
			bool flag = this.World.Side != 1;
			if (!flag)
			{
				this.leftDraftProfileConfigured = false;
				this.rightDraftProfileConfigured = false;
				EntityAgent leftHorse = this.LeftHorse;
				EntityAgent rightHorse = this.RightHorse;
				bool flag2 = leftHorse != null;
				if (flag2)
				{
					this.EnsureConfiguredDraftProfile(true, leftHorse);
				}
				bool flag3 = rightHorse != null;
				if (flag3)
				{
					this.EnsureConfiguredDraftProfile(false, rightHorse);
				}
				this.UpdateConfiguredTeamSpeeds();
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x0000278C File Offset: 0x0000098C
		private void EnsureConfiguredDraftProfile(bool left, EntityAgent horse)
		{
			bool flag = this.World.Side != 1 || horse == null;
			if (!flag)
			{
				bool flag2 = left ? this.leftDraftProfileConfigured : this.rightDraftProfileConfigured;
				if (!flag2)
				{
					HorsePlowModSystem horsePlowModSystem = this.GetHorsePlowModSystem();
					ResolvedDraftEntityProfile resolvedDraftEntityProfile = ((horsePlowModSystem != null) ? horsePlowModSystem.ResolveDraftProfile(horse) : null) ?? EntityHorsePlow.EmergencyDraftProfile;
					if (left)
					{
						this.WatchedAttributes.SetFloat("horsePlowLeftForwardBlocksPerSecond", resolvedDraftEntityProfile.ForwardBlocksPerSecond);
						this.WatchedAttributes.SetFloat("horsePlowLeftReverseBlocksPerSecond", resolvedDraftEntityProfile.ReverseBlocksPerSecond);
						this.StoreHorseAnchor(true, false, resolvedDraftEntityProfile.LeftPrimaryRopeAnchor);
						this.StoreHorseAnchor(true, true, resolvedDraftEntityProfile.LeftSecondaryRopeAnchor);
						this.leftDraftProfileConfigured = true;
					}
					else
					{
						this.WatchedAttributes.SetFloat("horsePlowRightForwardBlocksPerSecond", resolvedDraftEntityProfile.ForwardBlocksPerSecond);
						this.WatchedAttributes.SetFloat("horsePlowRightReverseBlocksPerSecond", resolvedDraftEntityProfile.ReverseBlocksPerSecond);
						this.StoreHorseAnchor(false, false, resolvedDraftEntityProfile.RightPrimaryRopeAnchor);
						this.StoreHorseAnchor(false, true, resolvedDraftEntityProfile.RightSecondaryRopeAnchor);
						this.rightDraftProfileConfigured = true;
					}
					this.UpdateConfiguredTeamSpeeds();
				}
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000028AC File Offset: 0x00000AAC
		private void UpdateConfiguredTeamSpeeds()
		{
			bool flag = this.World.Side != 1;
			if (!flag)
			{
				HorsePlowModSystem horsePlowModSystem = this.GetHorsePlowModSystem();
				HorsePlowConfig horsePlowConfig = ((horsePlowModSystem != null) ? horsePlowModSystem.Config : null) ?? new HorsePlowConfig();
				ResolvedDraftEntityProfile resolvedDraftEntityProfile = ((horsePlowModSystem != null) ? horsePlowModSystem.ResolveDraftProfile(null) : null) ?? horsePlowConfig.ResolveProfile(null);
				bool hasLeft = this.LeftHorseId >= 0L;
				bool hasRight = this.RightHorseId >= 0L;
				float @float = this.WatchedAttributes.GetFloat("horsePlowLeftForwardBlocksPerSecond", resolvedDraftEntityProfile.ForwardBlocksPerSecond);
				float float2 = this.WatchedAttributes.GetFloat("horsePlowLeftReverseBlocksPerSecond", resolvedDraftEntityProfile.ReverseBlocksPerSecond);
				float float3 = this.WatchedAttributes.GetFloat("horsePlowRightForwardBlocksPerSecond", resolvedDraftEntityProfile.ForwardBlocksPerSecond);
				float float4 = this.WatchedAttributes.GetFloat("horsePlowRightReverseBlocksPerSecond", resolvedDraftEntityProfile.ReverseBlocksPerSecond);
				float num = horsePlowConfig.CombineTeamSpeed(@float, hasLeft, float3, hasRight, resolvedDraftEntityProfile.ForwardBlocksPerSecond);
				float num2 = horsePlowConfig.CombineTeamSpeed(float2, hasLeft, float4, hasRight, resolvedDraftEntityProfile.ReverseBlocksPerSecond);
				HorsePlowUserConfig horsePlowUserConfig = ((horsePlowModSystem != null) ? horsePlowModSystem.UserConfig : null) ?? new HorsePlowUserConfig();
				float num3 = HorsePlowUserConfig.SanitizeMultiplier(horsePlowUserConfig.ForwardSpeedMultiplier);
				float num4 = HorsePlowUserConfig.SanitizeMultiplier(horsePlowUserConfig.ReverseSpeedMultiplier);
				float num5 = num * num3;
				float num6 = num2 * num4;
				this.WatchedAttributes.SetFloat("horsePlowForwardBlocksPerSecond", num5);
				this.WatchedAttributes.SetFloat("horsePlowReverseBlocksPerSecond", num6);
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002A1C File Offset: 0x00000C1C
		private void StoreHorseAnchor(bool left, bool secondary, Vec3f value)
		{
			bool flag = value == null;
			if (!flag)
			{
				bool flag2 = left && !secondary;
				if (flag2)
				{
					this.WatchedAttributes.SetFloat("horsePlowLeftPrimaryAnchorX", value.X);
					this.WatchedAttributes.SetFloat("horsePlowLeftPrimaryAnchorY", value.Y);
					this.WatchedAttributes.SetFloat("horsePlowLeftPrimaryAnchorZ", value.Z);
				}
				else if (left)
				{
					this.WatchedAttributes.SetFloat("horsePlowLeftSecondaryAnchorX", value.X);
					this.WatchedAttributes.SetFloat("horsePlowLeftSecondaryAnchorY", value.Y);
					this.WatchedAttributes.SetFloat("horsePlowLeftSecondaryAnchorZ", value.Z);
				}
				else
				{
					bool flag3 = !secondary;
					if (flag3)
					{
						this.WatchedAttributes.SetFloat("horsePlowRightPrimaryAnchorX", value.X);
						this.WatchedAttributes.SetFloat("horsePlowRightPrimaryAnchorY", value.Y);
						this.WatchedAttributes.SetFloat("horsePlowRightPrimaryAnchorZ", value.Z);
					}
					else
					{
						this.WatchedAttributes.SetFloat("horsePlowRightSecondaryAnchorX", value.X);
						this.WatchedAttributes.SetFloat("horsePlowRightSecondaryAnchorY", value.Y);
						this.WatchedAttributes.SetFloat("horsePlowRightSecondaryAnchorZ", value.Z);
					}
				}
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002B78 File Offset: 0x00000D78
		private Vec3f GetHorseAnchor(bool left, bool secondary)
		{
			Vec3f vec3f = left ? (secondary ? EntityHorsePlow.EmergencyDraftProfile.LeftSecondaryRopeAnchor : EntityHorsePlow.EmergencyDraftProfile.LeftPrimaryRopeAnchor) : (secondary ? EntityHorsePlow.EmergencyDraftProfile.RightSecondaryRopeAnchor : EntityHorsePlow.EmergencyDraftProfile.RightPrimaryRopeAnchor);
			bool flag = left && !secondary;
			Vec3f result;
			if (flag)
			{
				result = new Vec3f(this.WatchedAttributes.GetFloat("horsePlowLeftPrimaryAnchorX", vec3f.X), this.WatchedAttributes.GetFloat("horsePlowLeftPrimaryAnchorY", vec3f.Y), this.WatchedAttributes.GetFloat("horsePlowLeftPrimaryAnchorZ", vec3f.Z));
			}
			else if (left)
			{
				result = new Vec3f(this.WatchedAttributes.GetFloat("horsePlowLeftSecondaryAnchorX", vec3f.X), this.WatchedAttributes.GetFloat("horsePlowLeftSecondaryAnchorY", vec3f.Y), this.WatchedAttributes.GetFloat("horsePlowLeftSecondaryAnchorZ", vec3f.Z));
			}
			else
			{
				bool flag2 = !secondary;
				if (flag2)
				{
					result = new Vec3f(this.WatchedAttributes.GetFloat("horsePlowRightPrimaryAnchorX", vec3f.X), this.WatchedAttributes.GetFloat("horsePlowRightPrimaryAnchorY", vec3f.Y), this.WatchedAttributes.GetFloat("horsePlowRightPrimaryAnchorZ", vec3f.Z));
				}
				else
				{
					result = new Vec3f(this.WatchedAttributes.GetFloat("horsePlowRightSecondaryAnchorX", vec3f.X), this.WatchedAttributes.GetFloat("horsePlowRightSecondaryAnchorY", vec3f.Y), this.WatchedAttributes.GetFloat("horsePlowRightSecondaryAnchorZ", vec3f.Z));
				}
			}
			return result;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002D10 File Offset: 0x00000F10
		private void ClearConfiguredDraftProfile(bool left)
		{
			if (left)
			{
				this.leftDraftProfileConfigured = false;
				this.WatchedAttributes.RemoveAttribute("horsePlowLeftForwardBlocksPerSecond");
				this.WatchedAttributes.RemoveAttribute("horsePlowLeftReverseBlocksPerSecond");
				this.WatchedAttributes.RemoveAttribute("horsePlowLeftPrimaryAnchorX");
				this.WatchedAttributes.RemoveAttribute("horsePlowLeftPrimaryAnchorY");
				this.WatchedAttributes.RemoveAttribute("horsePlowLeftPrimaryAnchorZ");
				this.WatchedAttributes.RemoveAttribute("horsePlowLeftSecondaryAnchorX");
				this.WatchedAttributes.RemoveAttribute("horsePlowLeftSecondaryAnchorY");
				this.WatchedAttributes.RemoveAttribute("horsePlowLeftSecondaryAnchorZ");
			}
			else
			{
				this.rightDraftProfileConfigured = false;
				this.WatchedAttributes.RemoveAttribute("horsePlowRightForwardBlocksPerSecond");
				this.WatchedAttributes.RemoveAttribute("horsePlowRightReverseBlocksPerSecond");
				this.WatchedAttributes.RemoveAttribute("horsePlowRightPrimaryAnchorX");
				this.WatchedAttributes.RemoveAttribute("horsePlowRightPrimaryAnchorY");
				this.WatchedAttributes.RemoveAttribute("horsePlowRightPrimaryAnchorZ");
				this.WatchedAttributes.RemoveAttribute("horsePlowRightSecondaryAnchorX");
				this.WatchedAttributes.RemoveAttribute("horsePlowRightSecondaryAnchorY");
				this.WatchedAttributes.RemoveAttribute("horsePlowRightSecondaryAnchorZ");
			}
			this.UpdateConfiguredTeamSpeeds();
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002E54 File Offset: 0x00001054
		private ClothManager GetClothManager()
		{
			bool flag;
			if (this.clothManagerCache == null)
			{
				IWorldAccessor world = this.World;
				flag = (((world != null) ? world.Api : null) != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.clothManagerCache = this.World.Api.ModLoader.GetModSystem<ClothManager>(true);
			}
			return this.clothManagerCache;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002EB0 File Offset: 0x000010B0
		private void RebindStoredVisualRope(int clothId, EntityAgent horse)
		{
			bool flag = clothId == 0;
			if (!flag)
			{
				ClothManager clothManager = this.GetClothManager();
				ClothSystem clothSystem = (clothManager != null) ? clothManager.GetClothSystem(clothId) : null;
				bool flag2 = clothSystem == null;
				if (!flag2)
				{
					clothSystem.CanPull = false;
					clothSystem.CanRip = false;
					clothSystem.OnPinnnedEntityLoaded(this);
					bool flag3 = horse != null;
					if (flag3)
					{
						clothSystem.OnPinnnedEntityLoaded(horse);
					}
					bool flag4 = horse != null;
					if (flag4)
					{
						foreach (ClothPoint clothPoint in clothSystem.Ends)
						{
							Entity pinnedToEntity = clothPoint.PinnedToEntity;
							long? num = (pinnedToEntity != null) ? new long?(pinnedToEntity.EntityId) : null;
							long entityId = horse.EntityId;
							bool flag5 = num.GetValueOrDefault() == entityId & num != null;
							if (flag5)
							{
								clothPoint.NoAttachTransform = true;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002F94 File Offset: 0x00001194
		private void UpdateVisualProxyAnchorsIfNeeded(EntityAgent leftHorse, EntityAgent rightHorse, bool force)
		{
			long num = (leftHorse != null) ? leftHorse.EntityId : -1L;
			long num2 = (rightHorse != null) ? rightHorse.EntityId : -1L;
			float num3 = (leftHorse != null) ? leftHorse.Pos.Yaw : 0f;
			float num4 = (rightHorse != null) ? rightHorse.Pos.Yaw : 0f;
			bool flag = force || !this.visualAnchorStateInitialized || num != this.lastVisualAnchorLeftHorseId || num2 != this.lastVisualAnchorRightHorseId || Math.Abs(base.Pos.Yaw - this.lastVisualAnchorPlowYaw) > 1E-05f || Math.Abs(num3 - this.lastVisualAnchorLeftYaw) > 1E-05f || Math.Abs(num4 - this.lastVisualAnchorRightYaw) > 1E-05f;
			bool flag2 = !flag;
			if (!flag2)
			{
				this.visualAnchorStateInitialized = true;
				this.lastVisualAnchorLeftHorseId = num;
				this.lastVisualAnchorRightHorseId = num2;
				this.lastVisualAnchorPlowYaw = base.Pos.Yaw;
				this.lastVisualAnchorLeftYaw = num3;
				this.lastVisualAnchorRightYaw = num4;
				bool flag3 = this.LeftRopeClothId != 0 && leftHorse != null;
				if (flag3)
				{
					this.UpdateVisualProxyAnchor(this.LeftRopeClothId, leftHorse, this.GetHorseAnchor(true, false), EntityHorsePlow.LeftPrimaryPlowRopeOffset);
				}
				bool flag4 = this.LeftSecondaryRopeClothId != 0 && leftHorse != null;
				if (flag4)
				{
					this.UpdateVisualProxyAnchor(this.LeftSecondaryRopeClothId, leftHorse, this.GetHorseAnchor(true, true), EntityHorsePlow.LeftSecondaryPlowRopeOffset);
				}
				bool flag5 = this.RightRopeClothId != 0 && rightHorse != null;
				if (flag5)
				{
					this.UpdateVisualProxyAnchor(this.RightRopeClothId, rightHorse, this.GetHorseAnchor(false, false), EntityHorsePlow.RightPrimaryPlowRopeOffset);
				}
				bool flag6 = this.RightSecondaryRopeClothId != 0 && rightHorse != null;
				if (flag6)
				{
					this.UpdateVisualProxyAnchor(this.RightSecondaryRopeClothId, rightHorse, this.GetHorseAnchor(false, true), EntityHorsePlow.RightSecondaryPlowRopeOffset);
				}
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00003160 File Offset: 0x00001360
		private void UpdateVisualProxyAnchor(int clothId, EntityAgent horse, Vec3f horseLocalOffset, Vec3f plowLocalOffset)
		{
			bool flag = clothId == 0 || horse == null;
			if (!flag)
			{
				ClothManager clothManager = this.GetClothManager();
				ClothSystem clothSystem = (clothManager != null) ? clothManager.GetClothSystem(clothId) : null;
				bool flag2 = clothSystem == null;
				if (!flag2)
				{
					clothSystem.CanPull = false;
					clothSystem.CanRip = false;
					Vec3f vec3f = EntityHorsePlow.RotateLocalAnchorOffset(horseLocalOffset, horse.Pos.Yaw);
					Vec3f vec3f2 = EntityHorsePlow.RotateLocalAnchorOffset(plowLocalOffset, base.Pos.Yaw);
					foreach (ClothPoint clothPoint in clothSystem.Ends)
					{
						Entity pinnedToEntity = clothPoint.PinnedToEntity;
						long? num = (pinnedToEntity != null) ? new long?(pinnedToEntity.EntityId) : null;
						long entityId = horse.EntityId;
						bool flag3 = num.GetValueOrDefault() == entityId & num != null;
						if (flag3)
						{
							clothPoint.PinTo(horse, vec3f.Clone());
							clothPoint.NoAttachTransform = true;
						}
						else
						{
							Entity pinnedToEntity2 = clothPoint.PinnedToEntity;
							num = ((pinnedToEntity2 != null) ? new long?(pinnedToEntity2.EntityId) : null);
							entityId = this.EntityId;
							bool flag4 = num.GetValueOrDefault() == entityId & num != null;
							if (flag4)
							{
								clothPoint.PinTo(this, vec3f2.Clone());
								clothPoint.NoAttachTransform = false;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000032C0 File Offset: 0x000014C0
		private static Vec3f RotateLocalAnchorOffset(Vec3f localOffset, float yaw)
		{
			bool flag = localOffset == null;
			Vec3f result;
			if (flag)
			{
				result = null;
			}
			else
			{
				double num;
				double num2;
				EntityHorsePlow.LocalToWorld((double)localOffset.X, (double)localOffset.Z, yaw, out num, out num2);
				result = new Vec3f((float)num, localOffset.Y, (float)num2);
			}
			return result;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000021F8 File Offset: 0x000003F8
		public void DidMount(EntityAgent entityAgent)
		{
		}

		// Token: 0x0600002A RID: 42 RVA: 0x0000330C File Offset: 0x0000150C
		public void DidUnmount(EntityAgent entityAgent)
		{
			base.Pos.Motion.X = 0.0;
			base.Pos.Motion.Z = 0.0;
			bool flag = this.World.Side == 1;
			if (flag)
			{
				EntityHorsePlow.MarkForDismountPositionResync(this);
				EntityHorsePlow.MarkForDismountPositionResync(this.LeftHorse);
				EntityHorsePlow.MarkForDismountPositionResync(this.RightHorse);
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00003380 File Offset: 0x00001580
		private static void MarkForDismountPositionResync(Entity entity)
		{
			bool flag = entity == null;
			if (!flag)
			{
				entity.IsTeleport = true;
				entity.Pos.Motion.X = 0.0;
				entity.Pos.Motion.Y = 0.0;
				entity.Pos.Motion.Z = 0.0;
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000033EC File Offset: 0x000015EC
		public override string GetInfoText()
		{
			string infoText = base.GetInfoText();
			string text = Lang.Get(this.BladesLowered ? "horseplow:tooltip-blade-lowered" : "horseplow:tooltip-blade-raised", Array.Empty<object>());
			string text2 = Lang.Get((this.LeftHorseId >= 0L) ? "horseplow:tooltip-left-hitched" : "horseplow:tooltip-left-empty", Array.Empty<object>());
			string text3 = Lang.Get((this.RightHorseId >= 0L) ? "horseplow:tooltip-right-hitched" : "horseplow:tooltip-right-empty", Array.Empty<object>());
			string text4 = Lang.Get("horseplow:tooltip-unhitch-help", Array.Empty<object>());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			defaultInterpolatedStringHandler..ctor(3, 4);
			defaultInterpolatedStringHandler.AppendFormatted(text);
			defaultInterpolatedStringHandler.AppendLiteral("\n");
			defaultInterpolatedStringHandler.AppendFormatted(text2);
			defaultInterpolatedStringHandler.AppendLiteral("\n");
			defaultInterpolatedStringHandler.AppendFormatted(text3);
			defaultInterpolatedStringHandler.AppendLiteral("\n");
			defaultInterpolatedStringHandler.AppendFormatted(text4);
			string text5 = defaultInterpolatedStringHandler.ToStringAndClear();
			return string.IsNullOrWhiteSpace(infoText) ? text5 : (infoText.TrimEnd() + "\n" + text5);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000034F8 File Offset: 0x000016F8
		public override void OnInteract(EntityAgent byEntity, ItemSlot itemslot, Vec3d hitPosition, EnumInteractMode mode)
		{
			EntityPlayer entityPlayer;
			bool flag;
			if (mode == 1)
			{
				entityPlayer = (byEntity as EntityPlayer);
				flag = (entityPlayer == null);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (flag2)
			{
				base.OnInteract(byEntity, itemslot, hitPosition, mode);
			}
			else
			{
				bool flag3 = entityPlayer.Controls.CtrlKey && (itemslot == null || itemslot.Empty);
				if (flag3)
				{
					bool flag4 = this.World.Side == 1;
					if (flag4)
					{
						bool flag5 = this.LeftHorseId >= 0L || this.RightHorseId >= 0L;
						if (flag5)
						{
							this.UnhitchAll(entityPlayer);
						}
						else
						{
							this.TryPackPlowIntoBackpack(entityPlayer);
						}
					}
				}
				else
				{
					object obj;
					if (itemslot == null)
					{
						obj = null;
					}
					else
					{
						ItemStack itemstack = itemslot.Itemstack;
						obj = ((itemstack != null) ? itemstack.Collectible : null);
					}
					bool flag6 = obj is ItemRope;
					if (flag6)
					{
						this.HandleRopeHitchInteraction(entityPlayer, itemslot);
					}
					else
					{
						IMountableSeat mountedOn = entityPlayer.MountedOn;
						EntityBehavior entityBehavior = ((mountedOn != null) ? mountedOn.MountSupplier : null) as EntityBehavior;
						Entity entity;
						if ((entity = ((entityBehavior != null) ? entityBehavior.entity : null)) == null)
						{
							IMountableSeat mountedOn2 = entityPlayer.MountedOn;
							entity = ((mountedOn2 != null) ? mountedOn2.Entity : null);
						}
						Entity entity2 = entity;
						bool flag7 = entity2 != null;
						if (!flag7)
						{
							bool shiftKey = entityPlayer.Controls.ShiftKey;
							if (shiftKey)
							{
								bool flag8 = this.World.Side == 1;
								if (flag8)
								{
									this.BladesLowered = !this.BladesLowered;
									this.ClearLaneHistory();
									this.World.PlaySoundAt(EntityHorsePlow.CollectSoundCode, this, null, true, 32f, 1f);
									EntityHorsePlow.SendStatus(entityPlayer, this.BladesLowered ? "horseplow:blades-lowered" : "horseplow:blades-raised");
								}
							}
							else
							{
								EnumHandling enumHandling = 0;
								foreach (EntityBehavior entityBehavior2 in base.SidedProperties.Behaviors)
								{
									entityBehavior2.OnInteract(byEntity, itemslot, hitPosition, mode, ref enumHandling);
									bool flag9 = enumHandling == 3;
									if (flag9)
									{
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003708 File Offset: 0x00001908
		public override void OnGameTick(float dt)
		{
			base.OnGameTick(dt);
			EntityBehaviorSeatable seatableBehavior = this.GetSeatableBehavior();
			IMountableSeat mountableSeat = EntityHorsePlow.FindControllingSeat(seatableBehavior);
			Entity entity = (mountableSeat != null) ? mountableSeat.Passenger : null;
			bool flag = seatableBehavior != null;
			if (flag)
			{
				seatableBehavior.Controller = entity;
			}
			int num;
			int turnDirection;
			EntityHorsePlow.GetDriveInput((mountableSeat != null) ? mountableSeat.Controls : null, out num, out turnDirection);
			EntityHorsePlowSeat entityHorsePlowSeat = mountableSeat as EntityHorsePlowSeat;
			bool flag2 = entityHorsePlowSeat != null;
			if (flag2)
			{
				entityHorsePlowSeat.SetDriverAnimation((entity == null) ? 0 : num);
			}
			EntityAgent leftHorse = this.LeftHorse;
			EntityAgent rightHorse = this.RightHorse;
			bool flag3 = this.CanUseAsDraftHorse(leftHorse) && this.CanUseAsDraftHorse(rightHorse);
			bool flag4 = this.World.Side == 1 && !flag3;
			if (flag4)
			{
				bool flag5 = this.IsLogicallyAttachedHorse(true, leftHorse);
				if (flag5)
				{
					this.LockAttachedHorseToSlot(true, leftHorse);
				}
				bool flag6 = this.IsLogicallyAttachedHorse(false, rightHorse);
				if (flag6)
				{
					this.LockAttachedHorseToSlot(false, rightHorse);
				}
			}
			bool flag7 = this.World.Side == 1;
			if (flag7)
			{
				this.visualRopeMaintenanceAccumulator += Math.Clamp(dt, 0f, 0.1f);
				bool flag8 = this.visualRopeMaintenanceAccumulator >= 0.25f;
				bool flag9 = flag8;
				if (flag9)
				{
					this.visualRopeMaintenanceAccumulator %= 0.25f;
				}
				this.ValidateAttachedHorses(flag8);
			}
			leftHorse = this.LeftHorse;
			rightHorse = this.RightHorse;
			bool flag10 = this.CanUseAsDraftHorse(leftHorse) && this.CanUseAsDraftHorse(rightHorse);
			bool assemblyMoved = false;
			bool flag11 = this.IsLogicallyAttachedHorse(true, leftHorse);
			if (flag11)
			{
				EntityHorsePlow.SuppressHorseMovement(leftHorse);
			}
			bool flag12 = this.IsLogicallyAttachedHorse(false, rightHorse);
			if (flag12)
			{
				EntityHorsePlow.SuppressHorseMovement(rightHorse);
			}
			bool flag13 = flag10;
			if (flag13)
			{
				bool flag14 = this.HasMovementAuthority(entity);
				if (flag14)
				{
					assemblyMoved = this.UpdateRigidAssembly(dt, leftHorse, rightHorse, (entity == null) ? 0 : num, turnDirection);
				}
			}
			else
			{
				this.StopHorizontalMotion();
				EntityHorsePlow.StopHorseAnimation(leftHorse);
				EntityHorsePlow.StopHorseAnimation(rightHorse);
				this.fatigueAccumulator = 0f;
			}
			this.visualAnchorRefreshAccumulator += Math.Clamp(dt, 0f, 0.1f);
			bool flag15 = this.visualAnchorRefreshAccumulator >= 0.25f;
			bool flag16 = flag15;
			if (flag16)
			{
				this.visualAnchorRefreshAccumulator %= 0.25f;
			}
			this.UpdateVisualProxyAnchorsIfNeeded(leftHorse, rightHorse, flag15);
			bool flag17 = this.World.Side == 2;
			if (flag17)
			{
				this.UpdateWheelVisualAnimationFromMotion(dt);
				this.UpdateBladeVisualState();
			}
			bool flag18 = this.World.Side != 1;
			if (!flag18)
			{
				bool flag19 = !flag10;
				if (!flag19)
				{
					bool flag20 = !this.CanWork(flag10, entity, assemblyMoved, num);
					if (flag20)
					{
						this.fatigueAccumulator = 0f;
					}
					else
					{
						bool flag21 = this.TryTillCenterBlock();
						bool flag22 = flag21;
						if (flag22)
						{
							this.World.PlaySoundAt(EntityHorsePlow.DirtSoundCode, base.Pos.X, base.Pos.Y, base.Pos.Z, null, true, 16f, 0.45f);
						}
						this.fatigueAccumulator += dt;
						bool flag23 = this.fatigueAccumulator >= 0.25f;
						if (flag23)
						{
							this.fatigueAccumulator %= 0.25f;
							this.ApplyDraftFatigue(leftHorse);
							this.ApplyDraftFatigue(rightHorse);
						}
					}
				}
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003A80 File Offset: 0x00001C80
		private bool HasMovementAuthority(Entity controllingPassenger)
		{
			bool flag = this.World.Side == 1;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				ICoreClientAPI coreClientAPI = this.World.Api as ICoreClientAPI;
				bool flag2 = coreClientAPI == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					IClientPlayer player = coreClientAPI.World.Player;
					result = (controllingPassenger == ((player != null) ? player.Entity : null));
				}
			}
			return result;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003AE4 File Offset: 0x00001CE4
		private bool UpdateRigidAssembly(float dt, EntityAgent leftHorse, EntityAgent rightHorse, int driveDirection, int turnDirection)
		{
			float num = Math.Clamp(dt, 0f, 0.1f);
			float num2 = (driveDirection > 0) ? (this.ForwardBlocksPerSecond * num) : ((driveDirection < 0) ? (-this.ReverseBlocksPerSecond * num) : 0f);
			float num3 = 0f;
			bool flag = driveDirection != 0 && turnDirection != 0;
			if (flag)
			{
				float num4 = (driveDirection > 0) ? 0.58f : 0.34f;
				float num5 = (driveDirection < 0) ? -1f : 1f;
				num3 = (float)turnDirection * num4 * num * num5;
			}
			int num6 = Math.Max(1, Math.Max((int)Math.Ceiling((double)Math.Abs(num2) / 0.055), (int)Math.Ceiling((double)Math.Abs(num3) / 0.035)));
			double num7 = (double)(num2 / (float)num6);
			float num8 = num3 / (float)num6;
			bool flag2 = false;
			for (int i = 0; i < num6; i++)
			{
				float num9 = base.Pos.Yaw + num8;
				double num10;
				double num11;
				EntityHorsePlow.LocalToWorld(0.0, num7, num9, out num10, out num11);
				double plowX = base.Pos.X + num10;
				double plowZ = base.Pos.Z + num11;
				EntityHorsePlow.AssemblyPose pose;
				bool flag3 = !this.TryResolveAssemblyPose(plowX, plowZ, num9, leftHorse, rightHorse, out pose);
				if (flag3)
				{
					break;
				}
				this.ApplyAssemblyPose(pose, leftHorse, rightHorse);
				flag2 |= (Math.Abs(num7) > 1E-06 || (double)Math.Abs(num8) > 1E-06);
			}
			this.StopHorizontalMotion();
			EntityHorsePlow.SetHorseAnimation(leftHorse, flag2, driveDirection < 0);
			EntityHorsePlow.SetHorseAnimation(rightHorse, flag2, driveDirection < 0);
			return flag2;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003CA4 File Offset: 0x00001EA4
		private bool TryResolveAssemblyPose(double plowX, double plowZ, float yaw, EntityAgent leftHorse, EntityAgent rightHorse, out EntityHorsePlow.AssemblyPose pose)
		{
			pose = default(EntityHorsePlow.AssemblyPose);
			double num;
			bool flag = !this.TryResolveGroundedY(this, plowX, plowZ, base.Pos.Y, out num);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				double num2;
				double num3;
				EntityHorsePlow.LocalToWorld(0.9, 3.55, yaw, out num2, out num3);
				double num4;
				double num5;
				EntityHorsePlow.LocalToWorld(-0.9, 3.55, yaw, out num4, out num5);
				double num6 = plowX + num2;
				double num7 = plowZ + num3;
				double num8 = plowX + num4;
				double num9 = plowZ + num5;
				double num10;
				double num11;
				bool flag2 = !this.TryResolveGroundedY(leftHorse, num6, num7, leftHorse.Pos.Y, out num10) || !this.TryResolveGroundedY(rightHorse, num8, num9, rightHorse.Pos.Y, out num11);
				if (flag2)
				{
					result = false;
				}
				else
				{
					double num12 = Math.Min(num, Math.Min(num10, num11));
					double num13 = Math.Max(num, Math.Max(num10, num11));
					bool flag3 = num13 - num12 > 1.2;
					if (flag3)
					{
						result = false;
					}
					else
					{
						pose = new EntityHorsePlow.AssemblyPose(plowX, num, plowZ, yaw, num6, num10, num7, num8, num11, num9);
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003DDC File Offset: 0x00001FDC
		private bool TryResolveGroundedY(Entity entity, double x, double z, double referenceY, out double groundedY)
		{
			groundedY = referenceY;
			bool flag = ((entity != null) ? entity.CollisionBox : null) == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				double num = referenceY + 1.0625;
				double num2 = referenceY - 1.25;
				for (double num3 = num; num3 >= num2; num3 -= 0.0625)
				{
					this.groundProbeCandidate.Set(x, num3, z);
					bool flag2 = this.World.CollisionTester.IsColliding(this.World.BlockAccessor, entity.CollisionBox, this.groundProbeCandidate, false);
					if (!flag2)
					{
						this.groundProbeSupport.Set(x, num3 - 0.0625, z);
						bool flag3 = !this.World.CollisionTester.IsColliding(this.World.BlockAccessor, entity.CollisionBox, this.groundProbeSupport, false);
						if (!flag3)
						{
							groundedY = num3;
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003EE8 File Offset: 0x000020E8
		private void ApplyAssemblyPose(EntityHorsePlow.AssemblyPose pose, EntityAgent leftHorse, EntityAgent rightHorse)
		{
			base.Pos.SetPos(pose.PlowX, pose.PlowY, pose.PlowZ);
			base.Pos.Yaw = pose.Yaw;
			EntityHorsePlow.ZeroMotion(this);
			EntityHorsePlow.ApplyHorsePose(leftHorse, pose.LeftX, pose.LeftY, pose.LeftZ, pose.Yaw);
			EntityHorsePlow.ApplyHorsePose(rightHorse, pose.RightX, pose.RightY, pose.RightZ, pose.Yaw);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003F78 File Offset: 0x00002178
		private static void ApplyHorsePose(EntityAgent horse, double x, double y, double z, float plowYaw)
		{
			bool flag = horse == null;
			if (!flag)
			{
				horse.Pos.SetPos(x, y, z);
				float num = plowYaw + EntityHorsePlow.HorseVisualYawOffset;
				horse.Pos.Yaw = num;
				horse.BodyYaw = num;
				horse.BodyYawServer = num;
				EntityHorsePlow.ZeroMotion(horse);
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003FCC File Offset: 0x000021CC
		private static void SuppressHorseMovement(EntityAgent horse)
		{
			bool flag = horse == null;
			if (!flag)
			{
				horse.Controls.StopAllMovement();
				horse.ServerControls.StopAllMovement();
				EntityHorsePlow.ZeroMotion(horse);
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00004004 File Offset: 0x00002204
		private bool IsLogicallyAttachedHorse(bool left, EntityAgent horse)
		{
			bool flag = horse == null || !horse.Alive || !this.IsSupportedHorse(horse);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				long num = left ? this.LeftHorseId : this.RightHorseId;
				result = (num >= 0L && num == horse.EntityId);
			}
			return result;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x0000405C File Offset: 0x0000225C
		private void LockAttachedHorseToSlot(bool left, EntityAgent horse)
		{
			bool flag = horse == null;
			if (!flag)
			{
				EntityHorsePlow.SuppressHorseMovement(horse);
				double x = left ? 0.9 : -0.9;
				double num;
				double num2;
				EntityHorsePlow.LocalToWorld(x, 3.55, base.Pos.Yaw, out num, out num2);
				double x2 = base.Pos.X + num;
				double z = base.Pos.Z + num2;
				double y = horse.Pos.Y;
				double num3;
				bool flag2 = this.TryResolveGroundedY(horse, x2, z, base.Pos.Y, out num3);
				if (flag2)
				{
					y = num3;
				}
				EntityHorsePlow.ApplyHorsePose(horse, x2, y, z, base.Pos.Yaw);
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x0000411C File Offset: 0x0000231C
		private static void ZeroMotion(Entity entity)
		{
			bool flag = entity == null;
			if (!flag)
			{
				entity.Pos.Motion.X = 0.0;
				entity.Pos.Motion.Y = 0.0;
				entity.Pos.Motion.Z = 0.0;
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00004180 File Offset: 0x00002380
		private static void GetDriveInput(EntityControls controls, out int driveDirection, out int turnDirection)
		{
			driveDirection = 0;
			turnDirection = 0;
			bool flag = controls == null;
			if (!flag)
			{
				bool flag2 = controls.Forward && !controls.Backward;
				if (flag2)
				{
					driveDirection = 1;
				}
				else
				{
					bool flag3 = controls.Backward && !controls.Forward;
					if (flag3)
					{
						driveDirection = -1;
					}
				}
				bool flag4 = controls.Left && !controls.Right;
				if (flag4)
				{
					turnDirection = 1;
				}
				else
				{
					bool flag5 = controls.Right && !controls.Left;
					if (flag5)
					{
						turnDirection = -1;
					}
				}
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00004210 File Offset: 0x00002410
		private void UpdateWheelVisualAnimationFromMotion(float dt)
		{
			double x = base.Pos.X;
			double z = base.Pos.Z;
			bool flag = !this.wheelVisualPositionInitialized;
			if (flag)
			{
				this.wheelVisualPositionInitialized = true;
				this.lastWheelVisualX = x;
				this.lastWheelVisualZ = z;
				EntityHorsePlow.StopHorseAnimation(this.LeftHorse);
				EntityHorsePlow.StopHorseAnimation(this.RightHorse);
				IAnimationManager animManager = this.AnimManager;
				bool flag2 = ((animManager != null) ? animManager.Animator : null) != null;
				if (flag2)
				{
					this.StopWheelVisualAnimations();
				}
			}
			else
			{
				double num = x - this.lastWheelVisualX;
				double num2 = z - this.lastWheelVisualZ;
				this.lastWheelVisualX = x;
				this.lastWheelVisualZ = z;
				double num3 = Math.Sqrt(num * num + num2 * num2);
				bool flag3 = num3 <= 0.0002500000118743628 || dt <= 0f;
				if (flag3)
				{
					EntityHorsePlow.StopHorseAnimation(this.LeftHorse);
					EntityHorsePlow.StopHorseAnimation(this.RightHorse);
					IAnimationManager animManager2 = this.AnimManager;
					bool flag4 = ((animManager2 != null) ? animManager2.Animator : null) != null;
					if (flag4)
					{
						this.StopWheelVisualAnimations();
					}
				}
				else
				{
					float num4 = Math.Clamp(dt, 0.001f, 0.1f);
					float num5 = (float)(num3 / (double)num4);
					bool flag5 = num5 < 0.03f;
					if (flag5)
					{
						EntityHorsePlow.StopHorseAnimation(this.LeftHorse);
						EntityHorsePlow.StopHorseAnimation(this.RightHorse);
						IAnimationManager animManager3 = this.AnimManager;
						bool flag6 = ((animManager3 != null) ? animManager3.Animator : null) != null;
						if (flag6)
						{
							this.StopWheelVisualAnimations();
						}
					}
					else
					{
						double num6;
						double num7;
						EntityHorsePlow.LocalToWorld(0.0, 1.0, base.Pos.Yaw, out num6, out num7);
						double num8 = num * num6 + num2 * num7;
						int num9 = (num8 >= 0.0) ? 1 : -1;
						EntityHorsePlow.SetHorseAnimation(this.LeftHorse, true, num9 < 0);
						EntityHorsePlow.SetHorseAnimation(this.RightHorse, true, num9 < 0);
						IAnimationManager animManager4 = this.AnimManager;
						bool flag7 = ((animManager4 != null) ? animManager4.Animator : null) == null;
						if (!flag7)
						{
							float num10 = Math.Max(0.01f, this.ForwardBlocksPerSecond);
							float animationSpeed = Math.Clamp(num5 / num10, 0.05f, 4f);
							this.UpdateWheelVisualAnimation(num9, animationSpeed);
						}
					}
				}
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00004454 File Offset: 0x00002654
		private void UpdateWheelVisualAnimation(int direction, float animationSpeed)
		{
			bool flag = this.AnimManager == null;
			if (!flag)
			{
				string text = (direction > 0) ? "walk" : "walkback";
				string text2 = (direction > 0) ? "walkback" : "walk";
				bool flag2 = this.wheelAnimationDirection != direction;
				if (flag2)
				{
					this.AnimManager.StopAnimation(text2);
					this.AnimManager.StopAnimation(text);
					bool flag3 = !this.TryStartPlowVisualAnimation(text, animationSpeed, 5f);
					if (flag3)
					{
						this.wheelAnimationDirection = 0;
						this.wheelAnimationSpeed = 0f;
					}
					else
					{
						this.wheelAnimationDirection = direction;
						this.wheelAnimationSpeed = animationSpeed;
					}
				}
				else
				{
					AnimationMetaData animationMetaData;
					bool flag4 = this.AnimManager.ActiveAnimationsByAnimCode.TryGetValue(text, out animationMetaData);
					if (flag4)
					{
						animationMetaData.AnimationSpeed = animationSpeed;
						this.wheelAnimationSpeed = animationSpeed;
					}
					else
					{
						bool flag5 = this.TryStartPlowVisualAnimation(text, animationSpeed, 5f);
						if (flag5)
						{
							this.wheelAnimationDirection = direction;
							this.wheelAnimationSpeed = animationSpeed;
						}
					}
				}
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x0000454C File Offset: 0x0000274C
		private void StopWheelVisualAnimations()
		{
			bool flag = this.AnimManager == null;
			if (!flag)
			{
				bool flag2 = this.wheelAnimationDirection != 0 || this.AnimManager.IsAnimationActive(new string[]
				{
					"walk"
				}) || this.AnimManager.IsAnimationActive(new string[]
				{
					"walkback"
				});
				if (flag2)
				{
					this.AnimManager.StopAnimation("walk");
					this.AnimManager.StopAnimation("walkback");
				}
				this.wheelAnimationDirection = 0;
				this.wheelAnimationSpeed = 0f;
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000045E4 File Offset: 0x000027E4
		private void UpdateBladeVisualState()
		{
			IAnimationManager animManager = this.AnimManager;
			bool flag = ((animManager != null) ? animManager.Animator : null) == null;
			if (!flag)
			{
				bool bladesLowered = this.BladesLowered;
				bool flag2 = !this.bladeVisualStateInitialized;
				if (flag2)
				{
					this.bladeVisualStateInitialized = true;
					this.lastBladeVisualState = bladesLowered;
					bool flag3 = !bladesLowered;
					if (flag3)
					{
						this.PlayBladeVisualAnimation(false);
					}
				}
				else
				{
					bool flag4 = this.lastBladeVisualState == bladesLowered;
					if (!flag4)
					{
						this.lastBladeVisualState = bladesLowered;
						this.PlayBladeVisualAnimation(bladesLowered);
					}
				}
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00004664 File Offset: 0x00002864
		private void PlayBladeVisualAnimation(bool lowered)
		{
			bool flag = this.AnimManager == null;
			if (!flag)
			{
				string text = lowered ? "blade-lower" : "blade-raise";
				string text2 = lowered ? "blade-raise" : "blade-lower";
				this.AnimManager.StopAnimation(text2);
				this.AnimManager.StopAnimation(text);
				this.TryStartPlowVisualAnimation(text, 1f, 10f);
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000046D0 File Offset: 0x000028D0
		private bool TryStartPlowVisualAnimation(string animationCode, float animationSpeed, float weight)
		{
			IAnimationManager animManager = this.AnimManager;
			IAnimator animator = (animManager != null) ? animManager.Animator : null;
			bool flag = animator == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = animator.GetAnimationState(animationCode) == null;
				result = (!flag2 && animManager.StartAnimation(EntityHorsePlow.CreatePlowAnimationMeta(animationCode, animationSpeed, weight)));
			}
			return result;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00004724 File Offset: 0x00002924
		private static AnimationMetaData CreatePlowAnimationMeta(string animationCode, float animationSpeed, float weight)
		{
			return new AnimationMetaData
			{
				Code = animationCode,
				Animation = animationCode,
				AnimationSpeed = animationSpeed,
				Weight = weight,
				BlendMode = 1,
				EaseInSpeed = 20f,
				EaseOutSpeed = 20f,
				ClientSide = true
			}.Init();
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00004780 File Offset: 0x00002980
		private bool CanWork(bool completeTeam, Entity controllingPassenger, bool assemblyMoved, int driveDirection)
		{
			bool flag = !this.BladesLowered || controllingPassenger == null || !completeTeam;
			return !flag && assemblyMoved && driveDirection > 0;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000047B8 File Offset: 0x000029B8
		private bool CanUseAsDraftHorse(EntityAgent horse)
		{
			bool flag = !this.IsSupportedHorse(horse) || !horse.Alive || horse.Swimming;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				EntityBehaviorJauntRideable behavior = horse.GetBehavior<EntityBehaviorJauntRideable>();
				result = (behavior == null || !behavior.AnyMounted());
			}
			return result;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00004804 File Offset: 0x00002A04
		private void HandleRopeHitchInteraction(EntityPlayer player, ItemSlot ropeSlot)
		{
			int? num;
			if (ropeSlot == null)
			{
				num = null;
			}
			else
			{
				ItemStack itemstack = ropeSlot.Itemstack;
				if (itemstack == null)
				{
					num = null;
				}
				else
				{
					ITreeAttribute attributes = itemstack.Attributes;
					num = ((attributes != null) ? new int?(attributes.GetInt("clothId", 0)) : null);
				}
			}
			int? num2 = num;
			int valueOrDefault = num2.GetValueOrDefault();
			bool flag = valueOrDefault == 0;
			if (flag)
			{
				bool flag2 = this.World.Side == 1;
				if (flag2)
				{
					EntityHorsePlow.SendStatus(player, "horseplow:rope-first-horse");
				}
			}
			else
			{
				ClothManager clothManager = this.GetClothManager();
				ClothSystem clothSystem = (clothManager != null) ? clothManager.GetClothSystem(valueOrDefault) : null;
				bool flag3 = clothSystem == null;
				if (flag3)
				{
					bool flag4 = this.World.Side == 1;
					if (flag4)
					{
						EntityHorsePlow.SendStatus(player, "horseplow:rope-invalid");
					}
				}
				else
				{
					ClothPoint clothPoint = null;
					ClothPoint clothPoint2 = null;
					EntityAgent entityAgent = null;
					foreach (ClothPoint clothPoint3 in clothSystem.Ends)
					{
						Entity pinnedToEntity = clothPoint3.PinnedToEntity;
						long? num3 = (pinnedToEntity != null) ? new long?(pinnedToEntity.EntityId) : null;
						long entityId = player.EntityId;
						bool flag5 = num3.GetValueOrDefault() == entityId & num3 != null;
						if (flag5)
						{
							clothPoint2 = clothPoint3;
						}
						else
						{
							EntityAgent entityAgent2 = clothPoint3.PinnedToEntity as EntityAgent;
							bool flag6 = entityAgent2 != null && this.IsSupportedHorse(entityAgent2);
							if (flag6)
							{
								clothPoint = clothPoint3;
								entityAgent = entityAgent2;
							}
						}
					}
					bool flag7 = entityAgent == null || clothPoint == null || clothPoint2 == null;
					if (flag7)
					{
						bool flag8 = this.World.Side == 1;
						if (flag8)
						{
							EntityHorsePlow.SendStatus(player, "horseplow:rope-first-horse");
						}
					}
					else
					{
						bool flag9 = !EntityHorsePlow.HasWorkCollar(entityAgent);
						if (flag9)
						{
							bool flag10 = this.World.Side == 1;
							if (flag10)
							{
								EntityHorsePlow.SendStatus(player, "horseplow:workcollar-required");
							}
						}
						else
						{
							EntityHorsePlow entityHorsePlow = EntityHorsePlow.FindPlowUsingHorse(entityAgent);
							bool flag11 = entityHorsePlow != null && entityHorsePlow != this;
							if (flag11)
							{
								bool flag12 = this.World.Side == 1;
								if (flag12)
								{
									EntityHorsePlow.SendStatus(player, "horseplow:horse-already-hitched");
								}
							}
							else
							{
								bool flag13 = this.LeftHorseId == entityAgent.EntityId || this.RightHorseId == entityAgent.EntityId;
								if (flag13)
								{
									bool flag14 = this.World.Side == 1;
									if (flag14)
									{
										EntityHorsePlow.SendStatus(player, "horseplow:horse-already-hitched");
									}
								}
								else
								{
									bool flag15 = this.LeftHorseId >= 0L && this.RightHorseId >= 0L;
									if (flag15)
									{
										bool flag16 = this.World.Side == 1;
										if (flag16)
										{
											EntityHorsePlow.SendStatus(player, "horseplow:team-full");
										}
									}
									else
									{
										bool flag17 = this.ChooseLeftHitch(entityAgent);
										bool flag18 = this.World.Side != 1;
										if (!flag18)
										{
											bool flag19 = flag17;
											if (flag19)
											{
												this.LeftHorseId = entityAgent.EntityId;
												this.leftHorseCache = entityAgent;
											}
											else
											{
												this.RightHorseId = entityAgent.EntityId;
												this.rightHorseCache = entityAgent;
											}
											this.EnsureConfiguredDraftProfile(flag17, entityAgent);
											Vec3f horseAnchor = this.GetHorseAnchor(flag17, false);
											Vec3f horseAnchor2 = this.GetHorseAnchor(flag17, true);
											Vec3f plowOffset = flag17 ? EntityHorsePlow.LeftPrimaryPlowRopeOffset : EntityHorsePlow.RightPrimaryPlowRopeOffset;
											Vec3f plowOffset2 = flag17 ? EntityHorsePlow.LeftSecondaryPlowRopeOffset : EntityHorsePlow.RightSecondaryPlowRopeOffset;
											this.LockAttachedHorseToSlot(flag17, entityAgent);
											this.RemoveTemporaryHitchRope(clothManager, clothSystem, entityAgent);
											int num4 = this.CreateVisualProxyRope(entityAgent, horseAnchor, plowOffset);
											int num5 = this.CreateVisualProxyRope(entityAgent, horseAnchor2, plowOffset2);
											bool flag20 = flag17;
											if (flag20)
											{
												this.LeftRopeClothId = num4;
												this.LeftSecondaryRopeClothId = num5;
												EntityHorsePlow.SendStatus(player, "horseplow:left-hitched");
											}
											else
											{
												this.RightRopeClothId = num4;
												this.RightSecondaryRopeClothId = num5;
												EntityHorsePlow.SendStatus(player, "horseplow:right-hitched");
											}
											ropeSlot.Itemstack.Attributes.RemoveAttribute("clothId");
											ropeSlot.Itemstack.Attributes.RemoveAttribute("ropeHeldByEntityId");
											ropeSlot.TakeOut(1);
											ropeSlot.MarkDirty();
											this.World.PlaySoundAt(EntityHorsePlow.CollectSoundCode, this, null, true, 32f, 1f);
											this.ClearLaneHistory();
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00004C5C File Offset: 0x00002E5C
		private void RemoveTemporaryHitchRope(ClothManager clothManager, ClothSystem rope, EntityAgent horse)
		{
			bool flag = clothManager == null || rope == null;
			if (!flag)
			{
				if (horse != null)
				{
					EntityBehaviorRopeTieable behavior = horse.GetBehavior<EntityBehaviorRopeTieable>();
					if (behavior != null)
					{
						behavior.Detach(rope);
					}
				}
				rope.WalkPoints(delegate(ClothPoint point)
				{
					bool pinned = point.Pinned;
					if (pinned)
					{
						point.UnPin();
					}
				});
				clothManager.UnregisterCloth(rope.ClothId);
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00004CC8 File Offset: 0x00002EC8
		private int CreateVisualProxyRope(EntityAgent horse, Vec3f horseAnchorOffset, Vec3f plowOffset)
		{
			bool flag = this.World.Side != 1 || horse == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				ClothManager clothManager = this.GetClothManager();
				bool flag2 = clothManager == null;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					Vec3f vec3f = EntityHorsePlow.RotateLocalAnchorOffset(horseAnchorOffset, horse.Pos.Yaw);
					Vec3f vec3f2 = EntityHorsePlow.RotateLocalAnchorOffset(plowOffset, base.Pos.Yaw);
					Vec3d vec3d = new Vec3d(horse.Pos.X + (double)vec3f.X, horse.Pos.Y + (double)vec3f.Y, horse.Pos.Z + (double)vec3f.Z);
					Vec3d vec3d2 = new Vec3d(base.Pos.X + (double)vec3f2.X, base.Pos.Y + (double)vec3f2.Y, base.Pos.Z + (double)vec3f2.Z);
					double num = vec3d2.X - vec3d.X;
					double num2 = vec3d2.Y - vec3d.Y;
					double num3 = vec3d2.Z - vec3d.Z;
					double num4 = Math.Sqrt(num * num + num2 * num2 + num3 * num3);
					Vec3d vec3d3 = vec3d2.Clone();
					double num5 = Math.Max(0.25, 1.7);
					bool flag3 = num4 > 1E-06;
					if (flag3)
					{
						double num6 = num5 / num4;
						vec3d3.Set(vec3d.X + num * num6, vec3d.Y + num2 * num6, vec3d.Z + num3 * num6);
					}
					ClothSystem clothSystem = ClothSystem.CreateRope(this.World.Api, clothManager, vec3d, vec3d3, null, 2f);
					bool flag4 = clothSystem == null;
					if (flag4)
					{
						result = 0;
					}
					else
					{
						clothSystem.FirstPoint.PinTo(horse, vec3f);
						clothSystem.FirstPoint.NoAttachTransform = true;
						clothSystem.LastPoint.PinTo(this, vec3f2);
						clothSystem.LastPoint.NoAttachTransform = false;
						clothSystem.CanPull = false;
						clothSystem.CanRip = false;
						clothManager.RegisterCloth(clothSystem);
						result = clothSystem.ClothId;
					}
				}
			}
			return result;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00004EF4 File Offset: 0x000030F4
		private bool ChooseLeftHitch(Entity horse)
		{
			bool flag = this.LeftHorseId < 0L && this.RightHorseId >= 0L;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = this.RightHorseId < 0L && this.LeftHorseId >= 0L;
				if (flag2)
				{
					result = false;
				}
				else
				{
					double num;
					double num2;
					EntityHorsePlow.LocalToWorld(1.0, 0.0, base.Pos.Yaw, out num, out num2);
					double num3 = horse.Pos.X - base.Pos.X;
					double num4 = horse.Pos.Z - base.Pos.Z;
					result = (num3 * num + num4 * num2 >= 0.0);
				}
			}
			return result;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004FC4 File Offset: 0x000031C4
		private static bool HasWorkCollar(EntityAgent horse)
		{
			EntityBehaviorAttachable entityBehaviorAttachable = (horse != null) ? horse.GetBehavior<EntityBehaviorAttachable>() : null;
			bool flag = ((entityBehaviorAttachable != null) ? entityBehaviorAttachable.Inventory : null) == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (ItemSlot itemSlot in entityBehaviorAttachable.Inventory)
				{
					ItemStack itemstack = itemSlot.Itemstack;
					bool flag2;
					if (itemstack == null)
					{
						flag2 = false;
					}
					else
					{
						CollectibleObject collectible = itemstack.Collectible;
						bool? flag3;
						if (collectible == null)
						{
							flag3 = null;
						}
						else
						{
							AssetLocation code = collectible.Code;
							flag3 = ((code != null) ? new bool?(code.Equals(EntityHorsePlow.WorkCollarCode)) : null);
						}
						bool? flag4 = flag3;
						flag2 = flag4.GetValueOrDefault();
					}
					bool flag5 = flag2;
					if (flag5)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00005098 File Offset: 0x00003298
		private bool MaintainVisualRope(int clothId, EntityAgent horse)
		{
			bool flag = clothId == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				ClothManager clothManager = this.GetClothManager();
				ClothSystem clothSystem = (clothManager != null) ? clothManager.GetClothSystem(clothId) : null;
				bool flag2 = clothSystem == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					clothSystem.CanPull = false;
					clothSystem.CanRip = false;
					clothSystem.OnPinnnedEntityLoaded(this);
					bool flag3 = horse != null;
					if (flag3)
					{
						clothSystem.OnPinnnedEntityLoaded(horse);
						foreach (ClothPoint clothPoint in clothSystem.Ends)
						{
							Entity pinnedToEntity = clothPoint.PinnedToEntity;
							long? num = (pinnedToEntity != null) ? new long?(pinnedToEntity.EntityId) : null;
							long entityId = horse.EntityId;
							bool flag4 = num.GetValueOrDefault() == entityId & num != null;
							if (flag4)
							{
								clothPoint.NoAttachTransform = true;
							}
						}
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000517C File Offset: 0x0000337C
		private void EnsureVisualRopes(bool left, EntityAgent horse)
		{
			bool flag = horse == null || this.World.Side != 1;
			if (!flag)
			{
				int num = left ? this.LeftRopeClothId : this.RightRopeClothId;
				int num2 = left ? this.LeftSecondaryRopeClothId : this.RightSecondaryRopeClothId;
				bool flag2 = this.MaintainVisualRope(num, horse);
				bool flag3 = this.MaintainVisualRope(num2, horse);
				bool flag4 = flag2 && flag3;
				if (!flag4)
				{
					this.LockAttachedHorseToSlot(left, horse);
					Vec3f horseAnchor = this.GetHorseAnchor(left, false);
					Vec3f horseAnchor2 = this.GetHorseAnchor(left, true);
					Vec3f plowOffset = left ? EntityHorsePlow.LeftPrimaryPlowRopeOffset : EntityHorsePlow.RightPrimaryPlowRopeOffset;
					Vec3f plowOffset2 = left ? EntityHorsePlow.LeftSecondaryPlowRopeOffset : EntityHorsePlow.RightSecondaryPlowRopeOffset;
					bool flag5 = !flag2;
					if (flag5)
					{
						num = this.CreateVisualProxyRope(horse, horseAnchor, plowOffset);
						if (left)
						{
							this.LeftRopeClothId = num;
						}
						else
						{
							this.RightRopeClothId = num;
						}
					}
					bool flag6 = !flag3;
					if (flag6)
					{
						num2 = this.CreateVisualProxyRope(horse, horseAnchor2, plowOffset2);
						if (left)
						{
							this.LeftSecondaryRopeClothId = num2;
						}
						else
						{
							this.RightSecondaryRopeClothId = num2;
						}
					}
				}
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000021FB File Offset: 0x000003FB
		private void ValidateAttachedHorses(bool maintainVisualRopes)
		{
			this.ValidateHorseSlot(true, this.LeftHorse, maintainVisualRopes);
			this.ValidateHorseSlot(false, this.RightHorse, maintainVisualRopes);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00005294 File Offset: 0x00003494
		private void ValidateHorseSlot(bool left, EntityAgent horse, bool maintainVisualRopes)
		{
			long num = left ? this.LeftHorseId : this.RightHorseId;
			bool flag = num < 0L || horse == null;
			if (!flag)
			{
				bool flag2 = !this.IsEntityWhitelisted(horse);
				if (flag2)
				{
					this.UnhitchHorseSlot(left, true, false);
				}
				else
				{
					this.EnsureConfiguredDraftProfile(left, horse);
					bool flag3 = !EntityHorsePlow.HasWorkCollar(horse);
					if (flag3)
					{
						this.UnhitchHorseSlot(left, true, false);
					}
					else
					{
						double num2 = horse.Pos.DistanceTo(base.Pos.XYZ);
						double num3 = Math.Abs(horse.Pos.Y - base.Pos.Y);
						bool flag4 = !horse.Alive || !this.IsSupportedHorse(horse) || num2 > 8.0 || num3 > 2.5;
						if (flag4)
						{
							this.UnhitchHorseSlot(left, false, true);
						}
						else if (maintainVisualRopes)
						{
							this.EnsureVisualRopes(left, horse);
						}
					}
				}
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00005398 File Offset: 0x00003598
		private bool TryPackPlowIntoBackpack(EntityPlayer player)
		{
			bool flag = this.World.Side != 1 || ((player != null) ? player.Player : null) == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.LeftHorseId >= 0L || this.RightHorseId >= 0L;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = this.ControllingPassenger != null;
					if (flag3)
					{
						EntityHorsePlow.SendStatus(player, "horseplow:pickup-dismount-first");
						result = false;
					}
					else
					{
						Item item = this.World.GetItem(EntityHorsePlow.PlowItemCode);
						bool flag4 = item == null;
						if (flag4)
						{
							this.World.Logger.Error("[HorsePlow] Cannot pack plow: item {0} could not be resolved.", new object[]
							{
								EntityHorsePlow.PlowItemCode
							});
							result = false;
						}
						else
						{
							IPlayerInventoryManager inventoryManager = player.Player.InventoryManager;
							IInventory inventory = (inventoryManager != null) ? inventoryManager.GetOwnInventory("backpack") : null;
							bool flag5 = inventory == null;
							if (flag5)
							{
								EntityHorsePlow.SendStatus(player, "horseplow:pickup-backpack-full");
								result = false;
							}
							else
							{
								DummySlot dummySlot = new DummySlot(new ItemStack(item, 1));
								for (int i = 0; i < inventory.Count; i++)
								{
									ItemSlot itemSlot = inventory[i];
									bool flag6 = itemSlot == null || !itemSlot.Empty;
									if (!flag6)
									{
										bool flag7 = !itemSlot.CanHold(dummySlot);
										if (!flag7)
										{
											int num = dummySlot.TryPutInto(this.World, itemSlot, 1);
											bool flag8 = num != 1;
											if (!flag8)
											{
												itemSlot.MarkDirty();
												inventoryManager.NotifySlot(player.Player, itemSlot);
												this.RemoveVisualDraftRope(this.LeftRopeClothId, null);
												this.RemoveVisualDraftRope(this.LeftSecondaryRopeClothId, null);
												this.RemoveVisualDraftRope(this.RightRopeClothId, null);
												this.RemoveVisualDraftRope(this.RightSecondaryRopeClothId, null);
												this.LeftRopeClothId = 0;
												this.LeftSecondaryRopeClothId = 0;
												this.RightRopeClothId = 0;
												this.RightSecondaryRopeClothId = 0;
												this.ClearLaneHistory();
												this.Die(3, null);
												return true;
											}
										}
									}
								}
								EntityHorsePlow.SendStatus(player, "horseplow:pickup-backpack-full");
								result = false;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000055CC File Offset: 0x000037CC
		private void UnhitchAll(EntityPlayer player)
		{
			bool flag = false;
			bool flag2 = this.LeftHorseId >= 0L || this.LeftRopeClothId != 0 || this.LeftSecondaryRopeClothId != 0;
			if (flag2)
			{
				flag |= this.UnhitchHorseSlot(true, true, false);
			}
			bool flag3 = this.RightHorseId >= 0L || this.RightRopeClothId != 0 || this.RightSecondaryRopeClothId != 0;
			if (flag3)
			{
				flag |= this.UnhitchHorseSlot(false, true, false);
			}
			EntityHorsePlow.SendStatus(player, flag ? "horseplow:all-unhitched" : "horseplow:nothing-to-unhitch");
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00005654 File Offset: 0x00003854
		private bool UnhitchHorseSlot(bool left, bool refundRope, bool playBreakSound)
		{
			EntityAgent entityAgent = left ? this.LeftHorse : this.RightHorse;
			int num = left ? this.LeftRopeClothId : this.RightRopeClothId;
			int num2 = left ? this.LeftSecondaryRopeClothId : this.RightSecondaryRopeClothId;
			bool flag = entityAgent != null || (left ? this.LeftHorseId : this.RightHorseId) >= 0L || num != 0 || num2 != 0;
			bool flag2 = this.RemoveVisualDraftRope(num, entityAgent);
			bool flag3 = this.RemoveVisualDraftRope(num2, entityAgent);
			this.ClearHorseSlot(left, playBreakSound);
			bool flag4 = refundRope && flag;
			if (flag4)
			{
				this.DropReturnedRope(entityAgent);
			}
			return flag || flag2 || flag3;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000056F8 File Offset: 0x000038F8
		private bool RemoveVisualDraftRope(int clothId, EntityAgent horse)
		{
			bool flag = clothId == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				ClothManager clothManager = this.GetClothManager();
				ClothSystem clothSystem = (clothManager != null) ? clothManager.GetClothSystem(clothId) : null;
				bool flag2 = clothSystem == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					clothSystem.WalkPoints(delegate(ClothPoint point)
					{
						Entity pinnedToEntity = point.PinnedToEntity;
						long? num = (pinnedToEntity != null) ? new long?(pinnedToEntity.EntityId) : null;
						long? num2 = num;
						long entityId = this.EntityId;
						bool flag3;
						if (!(num2.GetValueOrDefault() == entityId & num2 != null))
						{
							if (horse != null)
							{
								num2 = num;
								entityId = horse.EntityId;
								flag3 = (num2.GetValueOrDefault() == entityId & num2 != null);
							}
							else
							{
								flag3 = false;
							}
						}
						else
						{
							flag3 = true;
						}
						bool flag4 = flag3;
						if (flag4)
						{
							point.UnPin();
						}
					});
					clothManager.UnregisterCloth(clothId);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00005770 File Offset: 0x00003970
		private void DropReturnedRope(EntityAgent horse)
		{
			Item item = this.World.GetItem(EntityHorsePlow.RopeItemCode);
			bool flag = item == null;
			if (!flag)
			{
				Vec3d vec3d = ((horse != null) ? horse.Pos.XYZ : null) ?? base.Pos.XYZ;
				this.World.SpawnItemEntity(new ItemStack(item, 1), new Vec3d(vec3d.X, vec3d.Y + 0.5, vec3d.Z), null);
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000057F0 File Offset: 0x000039F0
		private void ClearHorseSlot(bool left, bool playBreakSound)
		{
			EntityAgent horse = left ? this.LeftHorse : this.RightHorse;
			EntityHorsePlow.StopHorseAnimation(horse);
			if (left)
			{
				this.LeftHorseId = -1L;
				this.LeftRopeClothId = 0;
				this.LeftSecondaryRopeClothId = 0;
				this.leftHorseCache = null;
			}
			else
			{
				this.RightHorseId = -1L;
				this.RightRopeClothId = 0;
				this.RightSecondaryRopeClothId = 0;
				this.rightHorseCache = null;
			}
			this.ClearConfiguredDraftProfile(left);
			this.StopHorizontalMotion();
			this.fatigueAccumulator = 0f;
			this.ClearLaneHistory();
			if (playBreakSound)
			{
				this.World.PlaySoundAt(EntityHorsePlow.ToolBreakSoundCode, this, null, true, 32f, 1f);
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000058A8 File Offset: 0x00003AA8
		private EntityAgent ResolveHorse(long id, ref Entity cache)
		{
			bool flag = id < 0L;
			EntityAgent result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = cache == null || cache.EntityId != id;
				if (flag2)
				{
					cache = this.World.GetEntityById(id);
				}
				result = (cache as EntityAgent);
			}
			return result;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000058F8 File Offset: 0x00003AF8
		private bool TryTillCenterBlock()
		{
			BlockPos blockPos = new BlockPos((int)Math.Floor(base.Pos.X), (int)Math.Floor(base.Pos.Y + 0.25) - 1, (int)Math.Floor(base.Pos.Z));
			bool flag = EntityHorsePlow.SamePosition(this.lastTilledPosition, blockPos);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.lastTilledPosition = blockPos.Copy();
				BlockPos blockPos2 = blockPos.UpCopy(1);
				Block block = this.World.BlockAccessor.GetBlock(blockPos2);
				bool flag2 = this.IsInvolvedFarmingFallow(block);
				if (flag2)
				{
					result = this.RemoveInvolvedFarmingFallow(block, blockPos2, blockPos);
				}
				else
				{
					bool flag3 = block.BlockMaterial > 0;
					if (flag3)
					{
						bool flag4 = !EntityHorsePlow.CanClearSurfaceVegetation(block);
						if (flag4)
						{
							return false;
						}
						this.World.BlockAccessor.SetBlock(0, blockPos2);
					}
					bool flag5 = this.World.BlockAccessor.GetBlockEntity(blockPos) is BlockEntityFarmland;
					if (flag5)
					{
						result = false;
					}
					else
					{
						Block block2 = this.World.BlockAccessor.GetBlock(blockPos);
						string str;
						TreeAttribute treeAttribute;
						bool flag6 = !this.TryGetTillableSoilData(block2, blockPos, out str, out treeAttribute);
						if (flag6)
						{
							result = false;
						}
						else
						{
							Block block3 = this.World.GetBlock(new AssetLocation("game", "farmland-dry-" + str));
							bool flag7 = ((block3 != null) ? block3.Code : null) == null || block3.BlockId == 0;
							if (flag7)
							{
								result = false;
							}
							else
							{
								this.World.BlockAccessor.SetBlock(block3.BlockId, blockPos);
								BlockEntityFarmland blockEntityFarmland = this.World.BlockAccessor.GetBlockEntity(blockPos) as BlockEntityFarmland;
								bool flag8 = blockEntityFarmland != null;
								if (flag8)
								{
									blockEntityFarmland.OnCreatedFromSoil(block2, treeAttribute);
									blockEntityFarmland.MarkDirty(true, null);
								}
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00005AE8 File Offset: 0x00003CE8
		private bool TryGetTillableSoilData(Block soil, BlockPos soilPos, out string fertility, out TreeAttribute existingFertilityData)
		{
			fertility = null;
			existingFertilityData = null;
			bool flag = ((soil != null) ? soil.Code : null) == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = soil.Code.Domain == "game" && soil.Code.Path.StartsWith("soil-", StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					bool flag3 = !soil.Variant.TryGetValue("fertility", ref fertility);
					if (flag3)
					{
						result = false;
					}
					else
					{
						string a;
						bool flag4 = soil.Variant.TryGetValue("layer", ref a) && a != "7";
						result = !flag4;
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00005BA4 File Offset: 0x00003DA4
		private bool IsInvolvedFarmingFallow(Block block)
		{
			bool flag = block == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this.involvedFarmingFallowResolved;
				if (flag2)
				{
					this.involvedFarmingFallowCache = this.World.GetBlock(EntityHorsePlow.FallowBlockCode);
					this.involvedFarmingFallowResolved = true;
				}
				result = (this.involvedFarmingFallowCache != null && block == this.involvedFarmingFallowCache);
			}
			return result;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00005C04 File Offset: 0x00003E04
		private bool RemoveInvolvedFarmingFallow(Block fallowBlock, BlockPos fallowPos, BlockPos farmlandPos)
		{
			bool flag = !this.IsInvolvedFarmingFallow(fallowBlock);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				fallowBlock.SpawnDropsAndRemoveBlock(this.World, fallowPos, null, 1f);
				BlockEntityFarmland blockEntityFarmland = this.World.BlockAccessor.GetBlockEntity(farmlandPos) as BlockEntityFarmland;
				bool flag2 = blockEntityFarmland != null;
				if (flag2)
				{
					for (int i = 0; i < 3; i++)
					{
						blockEntityFarmland.Nutrients[i] += 5f;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00005C8C File Offset: 0x00003E8C
		private static bool CanClearSurfaceVegetation(Block block)
		{
			bool flag = ((block != null) ? block.Code : null) == null || block.BlockId == 0;
			return flag || (block.BlockMaterial == 13 && block.CropProps == null && block.Replaceable >= 6000 && !block.IsLiquid());
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00005CF4 File Offset: 0x00003EF4
		private void ApplyDraftFatigue(EntityAgent horse)
		{
			EntityBehaviorJauntStamina entityBehaviorJauntStamina = (horse != null) ? horse.GetBehavior<EntityBehaviorJauntStamina>() : null;
			if (entityBehaviorJauntStamina != null)
			{
				entityBehaviorJauntStamina.FatigueEntity(0.09f, new FatigueSource
				{
					Source = 1,
					SourceEntity = horse,
					CauseEntity = this,
					SourcePos = horse.Pos.XYZ
				});
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00005D4C File Offset: 0x00003F4C
		private static void SetHorseAnimation(EntityAgent horse, bool moving, bool reverse)
		{
			bool flag = ((horse != null) ? horse.AnimManager : null) == null;
			if (!flag)
			{
				bool flag2 = !moving;
				if (flag2)
				{
					EntityHorsePlow.StopHorseAnimation(horse);
				}
				else
				{
					string text = reverse ? "walkback" : "Walk";
					string text2 = reverse ? "Walk" : "walkback";
					horse.AnimManager.StopAnimation(text2);
					IAnimator animator = horse.AnimManager.Animator;
					bool flag3 = ((animator != null) ? animator.GetAnimationState(text) : null) != null;
					if (flag3)
					{
						horse.AnimManager.StartAnimation(text);
					}
				}
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00005DE0 File Offset: 0x00003FE0
		private static void StopHorseAnimation(EntityAgent horse)
		{
			IAnimationManager animationManager = (horse != null) ? horse.AnimManager : null;
			bool flag = animationManager == null;
			if (!flag)
			{
				animationManager.StopAnimation("Walk");
				animationManager.StopAnimation("walk");
				animationManager.StopAnimation("walkback");
				animationManager.StopAnimation("Walkback");
				animationManager.StopAnimation("Trot");
				animationManager.StopAnimation("trot");
				animationManager.StopAnimation("Canter");
				animationManager.StopAnimation("canter");
				animationManager.StopAnimation("Gallop");
				animationManager.StopAnimation("gallop");
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x0000221C File Offset: 0x0000041C
		private void StopHorizontalMotion()
		{
			base.Pos.Motion.X = 0.0;
			base.Pos.Motion.Z = 0.0;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00005E80 File Offset: 0x00004080
		private static EntityHorsePlow FindPlowUsingHorse(Entity horse)
		{
			bool flag = horse == null;
			EntityHorsePlow result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Entity[] entitiesAround = horse.World.GetEntitiesAround(horse.Pos.XYZ, 16f, 16f, delegate(Entity entity)
				{
					EntityHorsePlow entityHorsePlow2 = entity as EntityHorsePlow;
					return entityHorsePlow2 != null && (entityHorsePlow2.LeftHorseId == horse.EntityId || entityHorsePlow2.RightHorseId == horse.EntityId);
				});
				foreach (Entity entity2 in entitiesAround)
				{
					EntityHorsePlow entityHorsePlow = entity2 as EntityHorsePlow;
					bool flag2 = entityHorsePlow != null;
					if (flag2)
					{
						return entityHorsePlow;
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00005F24 File Offset: 0x00004124
		private bool IsEntityWhitelisted(Entity entity)
		{
			HorsePlowModSystem horsePlowModSystem = this.GetHorsePlowModSystem();
			return horsePlowModSystem != null && horsePlowModSystem.IsEntityWhitelisted(entity);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00005F4C File Offset: 0x0000414C
		private bool IsSupportedHorse(Entity entity)
		{
			EntityAgent entityAgent = entity as EntityAgent;
			bool flag = entityAgent == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this.IsEntityWhitelisted(entityAgent);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = entityAgent.GetBehavior<EntityBehaviorRopeTieable>() == null;
					result = (!flag3 && (entityAgent.GetBehavior<EntityBehaviorJauntRideable>() != null || EntityHorsePlow.HasWorkCollar(entityAgent)));
				}
			}
			return result;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00005FAC File Offset: 0x000041AC
		internal static void LocalToWorld(double x, double z, float yawRadians, out double worldX, out double worldZ)
		{
			double num = (double)GameMath.Sin(yawRadians);
			double num2 = (double)GameMath.Cos(yawRadians);
			worldX = x * num + z * num2;
			worldZ = x * num2 - z * num;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00005FE0 File Offset: 0x000041E0
		private static bool SamePosition(BlockPos left, BlockPos right)
		{
			return left != null && right != null && left.X == right.X && left.Y == right.Y && left.Z == right.Z;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00002251 File Offset: 0x00000451
		private void ClearLaneHistory()
		{
			this.lastTilledPosition = null;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000225B File Offset: 0x0000045B
		private static void SendStatus(EntityPlayer player, string langCode)
		{
			IServerPlayer serverPlayer = player.Player as IServerPlayer;
			if (serverPlayer != null)
			{
				serverPlayer.SendMessage(GlobalConstants.GeneralChatGroup, Lang.Get(langCode, Array.Empty<object>()), 4, null);
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00002287 File Offset: 0x00000487
		public override bool CanCollect(Entity byEntity)
		{
			return false;
		}

		// Token: 0x04000001 RID: 1
		private const string LeftHorseIdAttribute = "horsePlowLeftHorseId";

		// Token: 0x04000002 RID: 2
		private const string RightHorseIdAttribute = "horsePlowRightHorseId";

		// Token: 0x04000003 RID: 3
		private const string LoweredAttribute = "horsePlowLowered";

		// Token: 0x04000004 RID: 4
		private const string ForwardSpeedAttribute = "horsePlowForwardBlocksPerSecond";

		// Token: 0x04000005 RID: 5
		private const string ReverseSpeedAttribute = "horsePlowReverseBlocksPerSecond";

		// Token: 0x04000006 RID: 6
		private const string LeftForwardSpeedAttribute = "horsePlowLeftForwardBlocksPerSecond";

		// Token: 0x04000007 RID: 7
		private const string LeftReverseSpeedAttribute = "horsePlowLeftReverseBlocksPerSecond";

		// Token: 0x04000008 RID: 8
		private const string RightForwardSpeedAttribute = "horsePlowRightForwardBlocksPerSecond";

		// Token: 0x04000009 RID: 9
		private const string RightReverseSpeedAttribute = "horsePlowRightReverseBlocksPerSecond";

		// Token: 0x0400000A RID: 10
		private const string LeftPrimaryAnchorXAttribute = "horsePlowLeftPrimaryAnchorX";

		// Token: 0x0400000B RID: 11
		private const string LeftPrimaryAnchorYAttribute = "horsePlowLeftPrimaryAnchorY";

		// Token: 0x0400000C RID: 12
		private const string LeftPrimaryAnchorZAttribute = "horsePlowLeftPrimaryAnchorZ";

		// Token: 0x0400000D RID: 13
		private const string LeftSecondaryAnchorXAttribute = "horsePlowLeftSecondaryAnchorX";

		// Token: 0x0400000E RID: 14
		private const string LeftSecondaryAnchorYAttribute = "horsePlowLeftSecondaryAnchorY";

		// Token: 0x0400000F RID: 15
		private const string LeftSecondaryAnchorZAttribute = "horsePlowLeftSecondaryAnchorZ";

		// Token: 0x04000010 RID: 16
		private const string RightPrimaryAnchorXAttribute = "horsePlowRightPrimaryAnchorX";

		// Token: 0x04000011 RID: 17
		private const string RightPrimaryAnchorYAttribute = "horsePlowRightPrimaryAnchorY";

		// Token: 0x04000012 RID: 18
		private const string RightPrimaryAnchorZAttribute = "horsePlowRightPrimaryAnchorZ";

		// Token: 0x04000013 RID: 19
		private const string RightSecondaryAnchorXAttribute = "horsePlowRightSecondaryAnchorX";

		// Token: 0x04000014 RID: 20
		private const string RightSecondaryAnchorYAttribute = "horsePlowRightSecondaryAnchorY";

		// Token: 0x04000015 RID: 21
		private const string RightSecondaryAnchorZAttribute = "horsePlowRightSecondaryAnchorZ";

		// Token: 0x04000016 RID: 22
		private const string LeftRopeClothIdAttribute = "horsePlowLeftRopeClothId";

		// Token: 0x04000017 RID: 23
		private const string RightRopeClothIdAttribute = "horsePlowRightRopeClothId";

		// Token: 0x04000018 RID: 24
		private const string LeftSecondaryRopeClothIdAttribute = "horsePlowLeftSecondaryRopeClothId";

		// Token: 0x04000019 RID: 25
		private const string RightSecondaryRopeClothIdAttribute = "horsePlowRightSecondaryRopeClothId";

		// Token: 0x0400001A RID: 26
		private static readonly AssetLocation WorkCollarCode = new AssetLocation("horseplow", "workcollar");

		// Token: 0x0400001B RID: 27
		private static readonly AssetLocation PlowItemCode = new AssetLocation("horseplow", "creature-horseplow");

		// Token: 0x0400001C RID: 28
		private static readonly AssetLocation RopeItemCode = new AssetLocation("game", "rope");

		// Token: 0x0400001D RID: 29
		private static readonly AssetLocation CollectSoundCode = new AssetLocation("game:sounds/player/collect");

		// Token: 0x0400001E RID: 30
		private static readonly AssetLocation DirtSoundCode = new AssetLocation("game:sounds/block/dirt");

		// Token: 0x0400001F RID: 31
		private static readonly AssetLocation ToolBreakSoundCode = new AssetLocation("game:sounds/effect/toolbreak");

		// Token: 0x04000020 RID: 32
		private static readonly AssetLocation FallowBlockCode = new AssetLocation("invfarming:soilfallow");

		// Token: 0x04000021 RID: 33
		private const double HorseSideOffset = 0.9;

		// Token: 0x04000022 RID: 34
		private const double HorseForwardOffset = 3.55;

		// Token: 0x04000023 RID: 35
		private const double MaximumAttachedDistance = 8.0;

		// Token: 0x04000024 RID: 36
		private const double MaximumVerticalDifference = 2.5;

		// Token: 0x04000025 RID: 37
		private const double GroundProbeIncrement = 0.0625;

		// Token: 0x04000026 RID: 38
		private const double MaximumStepUp = 1.0625;

		// Token: 0x04000027 RID: 39
		private const double MaximumStepDown = 1.25;

		// Token: 0x04000028 RID: 40
		private const double MaximumTeamHeightSpread = 1.2;

		// Token: 0x04000029 RID: 41
		private const double AssemblySubstepDistance = 0.055;

		// Token: 0x0400002A RID: 42
		private const double AssemblySubstepYaw = 0.035;

		// Token: 0x0400002B RID: 43
		private static readonly float HorseVisualYawOffset = 1.5707964f;

		// Token: 0x0400002C RID: 44
		private static readonly ResolvedDraftEntityProfile EmergencyDraftProfile = ResolvedDraftEntityProfile.CreateEmergencyFallback();

		// Token: 0x0400002D RID: 45
		private static readonly Vec3f LeftPrimaryPlowRopeOffset = new Vec3f(0.41f, 0.7f, 1.46f);

		// Token: 0x0400002E RID: 46
		private static readonly Vec3f LeftSecondaryPlowRopeOffset = new Vec3f(1.34f, 0.7f, 1.46f);

		// Token: 0x0400002F RID: 47
		private static readonly Vec3f RightPrimaryPlowRopeOffset = new Vec3f(-0.41f, 0.7f, 1.46f);

		// Token: 0x04000030 RID: 48
		private static readonly Vec3f RightSecondaryPlowRopeOffset = new Vec3f(-1.34f, 0.7f, 1.46f);

		// Token: 0x04000031 RID: 49
		private const double VisualRopeLength = 1.7;

		// Token: 0x04000032 RID: 50
		private const float ForwardTurnRateRadiansPerSecond = 0.58f;

		// Token: 0x04000033 RID: 51
		private const float ReverseTurnRateRadiansPerSecond = 0.34f;

		// Token: 0x04000034 RID: 52
		private const float FatigueIntervalSeconds = 0.25f;

		// Token: 0x04000035 RID: 53
		private const float FatiguePerHorsePerInterval = 0.09f;

		// Token: 0x04000036 RID: 54
		private const float VisualRopeMaintenanceIntervalSeconds = 0.25f;

		// Token: 0x04000037 RID: 55
		private const float VisualAnchorRefreshIntervalSeconds = 0.25f;

		// Token: 0x04000038 RID: 56
		private const string WheelForwardAnimation = "walk";

		// Token: 0x04000039 RID: 57
		private const string WheelBackwardAnimation = "walkback";

		// Token: 0x0400003A RID: 58
		private const string BladeLowerAnimation = "blade-lower";

		// Token: 0x0400003B RID: 59
		private const string BladeRaiseAnimation = "blade-raise";

		// Token: 0x0400003C RID: 60
		private const float WheelMovementEpsilon = 0.00025f;

		// Token: 0x0400003D RID: 61
		private const float WheelMinimumVisualSpeed = 0.03f;

		// Token: 0x0400003E RID: 62
		private const float WheelMaximumAnimationSpeed = 4f;

		// Token: 0x0400003F RID: 63
		private BlockPos lastTilledPosition;

		// Token: 0x04000040 RID: 64
		private Entity leftHorseCache;

		// Token: 0x04000041 RID: 65
		private Entity rightHorseCache;

		// Token: 0x04000042 RID: 66
		private HorsePlowModSystem horsePlowModSystemCache;

		// Token: 0x04000043 RID: 67
		private EntityBehaviorSeatable seatableCache;

		// Token: 0x04000044 RID: 68
		private ClothManager clothManagerCache;

		// Token: 0x04000045 RID: 69
		private readonly Vec3d groundProbeCandidate = new Vec3d();

		// Token: 0x04000046 RID: 70
		private readonly Vec3d groundProbeSupport = new Vec3d();

		// Token: 0x04000047 RID: 71
		private Block involvedFarmingFallowCache;

		// Token: 0x04000048 RID: 72
		private bool involvedFarmingFallowResolved;

		// Token: 0x04000049 RID: 73
		private float fatigueAccumulator;

		// Token: 0x0400004A RID: 74
		private float visualRopeMaintenanceAccumulator;

		// Token: 0x0400004B RID: 75
		private float visualAnchorRefreshAccumulator;

		// Token: 0x0400004C RID: 76
		private bool visualAnchorStateInitialized;

		// Token: 0x0400004D RID: 77
		private long lastVisualAnchorLeftHorseId = -1L;

		// Token: 0x0400004E RID: 78
		private long lastVisualAnchorRightHorseId = -1L;

		// Token: 0x0400004F RID: 79
		private float lastVisualAnchorPlowYaw;

		// Token: 0x04000050 RID: 80
		private float lastVisualAnchorLeftYaw;

		// Token: 0x04000051 RID: 81
		private float lastVisualAnchorRightYaw;

		// Token: 0x04000052 RID: 82
		private bool leftDraftProfileConfigured;

		// Token: 0x04000053 RID: 83
		private bool rightDraftProfileConfigured;

		// Token: 0x04000054 RID: 84
		private int wheelAnimationDirection;

		// Token: 0x04000055 RID: 85
		private float wheelAnimationSpeed;

		// Token: 0x04000056 RID: 86
		private bool wheelVisualPositionInitialized;

		// Token: 0x04000057 RID: 87
		private double lastWheelVisualX;

		// Token: 0x04000058 RID: 88
		private double lastWheelVisualZ;

		// Token: 0x04000059 RID: 89
		private bool bladeVisualStateInitialized;

		// Token: 0x0400005A RID: 90
		private bool lastBladeVisualState;

		// Token: 0x02000003 RID: 3
		private readonly struct AssemblyPose
		{
			// Token: 0x06000066 RID: 102 RVA: 0x00006134 File Offset: 0x00004334
			public AssemblyPose(double plowX, double plowY, double plowZ, float yaw, double leftX, double leftY, double leftZ, double rightX, double rightY, double rightZ)
			{
				this.PlowX = plowX;
				this.PlowY = plowY;
				this.PlowZ = plowZ;
				this.Yaw = yaw;
				this.LeftX = leftX;
				this.LeftY = leftY;
				this.LeftZ = leftZ;
				this.RightX = rightX;
				this.RightY = rightY;
				this.RightZ = rightZ;
			}

			// Token: 0x17000012 RID: 18
			// (get) Token: 0x06000067 RID: 103 RVA: 0x000022B9 File Offset: 0x000004B9
			public double PlowX { get; }

			// Token: 0x17000013 RID: 19
			// (get) Token: 0x06000068 RID: 104 RVA: 0x000022C1 File Offset: 0x000004C1
			public double PlowY { get; }

			// Token: 0x17000014 RID: 20
			// (get) Token: 0x06000069 RID: 105 RVA: 0x000022C9 File Offset: 0x000004C9
			public double PlowZ { get; }

			// Token: 0x17000015 RID: 21
			// (get) Token: 0x0600006A RID: 106 RVA: 0x000022D1 File Offset: 0x000004D1
			public float Yaw { get; }

			// Token: 0x17000016 RID: 22
			// (get) Token: 0x0600006B RID: 107 RVA: 0x000022D9 File Offset: 0x000004D9
			public double LeftX { get; }

			// Token: 0x17000017 RID: 23
			// (get) Token: 0x0600006C RID: 108 RVA: 0x000022E1 File Offset: 0x000004E1
			public double LeftY { get; }

			// Token: 0x17000018 RID: 24
			// (get) Token: 0x0600006D RID: 109 RVA: 0x000022E9 File Offset: 0x000004E9
			public double LeftZ { get; }

			// Token: 0x17000019 RID: 25
			// (get) Token: 0x0600006E RID: 110 RVA: 0x000022F1 File Offset: 0x000004F1
			public double RightX { get; }

			// Token: 0x1700001A RID: 26
			// (get) Token: 0x0600006F RID: 111 RVA: 0x000022F9 File Offset: 0x000004F9
			public double RightY { get; }

			// Token: 0x1700001B RID: 27
			// (get) Token: 0x06000070 RID: 112 RVA: 0x00002301 File Offset: 0x00000501
			public double RightZ { get; }
		}
	}
}
