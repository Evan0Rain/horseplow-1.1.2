using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace HorsePlowMod
{
	// Token: 0x02000007 RID: 7
	public sealed class EntityHorsePlowSeat : EntityRideableSeat
	{
		// Token: 0x06000078 RID: 120 RVA: 0x0000231E File Offset: 0x0000051E
		public EntityHorsePlowSeat(IMountable mountableSupplier, string seatId, SeatConfig config) : base(mountableSupplier, seatId, config)
		{
			this.RideableClassName = "horseplow";
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000079 RID: 121 RVA: 0x0000234C File Offset: 0x0000054C
		public override EnumMountAngleMode AngleMode
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600007A RID: 122 RVA: 0x0000234F File Offset: 0x0000054F
		public override AnimationMetaData SuggestedAnimation
		{
			get
			{
				return this.suggestedAnimation;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00002357 File Offset: 0x00000557
		public override Vec3f LocalEyePos
		{
			get
			{
				return new Vec3f(this.config.EyeOffsetX, this.config.EyeHeight, 0f);
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00006280 File Offset: 0x00004480
		public override EntityPos SeatPosition
		{
			get
			{
				EntityPos pos = this.Entity.Pos;
				double num;
				double num2;
				EntityHorsePlow.LocalToWorld(0.0, -1.6, pos.Yaw, out num, out num2);
				this.seatPosition.SetFrom(pos);
				this.seatPosition.X += num;
				this.seatPosition.Y += 0.04;
				this.seatPosition.Z += num2;
				this.seatPosition.Yaw = pos.Yaw + 1.5707964f;
				return this.seatPosition;
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00002379 File Offset: 0x00000579
		public override void DidMount(EntityAgent entityAgent)
		{
			this.suggestedAnimation = EntityHorsePlowSeat.IdleAnimationMeta;
			base.DidMount(entityAgent);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000238F File Offset: 0x0000058F
		public void SetDriverAnimation(int driveDirection)
		{
			this.suggestedAnimation = ((driveDirection > 0) ? EntityHorsePlowSeat.ForwardAnimationMeta : ((driveDirection < 0) ? EntityHorsePlowSeat.ReverseAnimationMeta : EntityHorsePlowSeat.IdleAnimationMeta));
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000023B3 File Offset: 0x000005B3
		public override void DidUnmount(EntityAgent entityAgent)
		{
			this.suggestedAnimation = EntityHorsePlowSeat.IdleAnimationMeta;
			entityAgent.Pos.Roll = 0f;
			base.DidUnmount(entityAgent);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00006330 File Offset: 0x00004530
		private static AnimationMetaData CreateAnimationMeta(string animation, float speed)
		{
			return new AnimationMetaData
			{
				Code = animation,
				Animation = animation,
				AnimationSpeed = speed,
				Weight = 1f,
				BlendMode = 1,
				EaseInSpeed = 10f,
				EaseOutSpeed = 10f,
				SupressDefaultAnimation = true,
				WithFpVariant = false
			}.Init();
		}

		// Token: 0x0400006A RID: 106
		private const string IdleAnimation = "horseplow-hold-idle";

		// Token: 0x0400006B RID: 107
		private const string ForwardAnimation = "horseplow-push-walk";

		// Token: 0x0400006C RID: 108
		private const string ReverseAnimation = "horseplow-pull-walkback";

		// Token: 0x0400006D RID: 109
		private static readonly AnimationMetaData IdleAnimationMeta = EntityHorsePlowSeat.CreateAnimationMeta("horseplow-hold-idle", 1f);

		// Token: 0x0400006E RID: 110
		private static readonly AnimationMetaData ForwardAnimationMeta = EntityHorsePlowSeat.CreateAnimationMeta("horseplow-push-walk", 1f);

		// Token: 0x0400006F RID: 111
		private static readonly AnimationMetaData ReverseAnimationMeta = EntityHorsePlowSeat.CreateAnimationMeta("horseplow-pull-walkback", 1f);

		// Token: 0x04000070 RID: 112
		private readonly EntityPos seatPosition = new EntityPos();

		// Token: 0x04000071 RID: 113
		private AnimationMetaData suggestedAnimation = EntityHorsePlowSeat.IdleAnimationMeta;
	}
}
