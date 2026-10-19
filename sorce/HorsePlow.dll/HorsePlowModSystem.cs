using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Jaunt.Behaviors;
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
	// Token: 0x0200000F RID: 15
	public sealed class HorsePlowModSystem : ModSystem
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000096 RID: 150 RVA: 0x0000243E File Offset: 0x0000063E
		// (set) Token: 0x06000097 RID: 151 RVA: 0x00002446 File Offset: 0x00000646
		public HorsePlowConfig Config { get; private set; } = new HorsePlowConfig();

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000098 RID: 152 RVA: 0x0000244F File Offset: 0x0000064F
		// (set) Token: 0x06000099 RID: 153 RVA: 0x00002457 File Offset: 0x00000657
		public HorsePlowUserConfig UserConfig { get; private set; } = new HorsePlowUserConfig();

		// Token: 0x0600009A RID: 154 RVA: 0x00002460 File Offset: 0x00000660
		public override void Start(ICoreAPI api)
		{
			api.RegisterItemClass("ItemWorkCollar", typeof(ItemWorkCollar));
			api.RegisterEntity("EntityHorsePlow", typeof(EntityHorsePlow));
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00006C8C File Offset: 0x00004E8C
		public override void AssetsLoaded(ICoreAPI api)
		{
			base.AssetsLoaded(api);
			IAsset asset = api.Assets.TryGet(HorsePlowModSystem.BundledProfilesAsset, true);
			bool flag = asset == null;
			if (flag)
			{
				api.Logger.Warning("[HorsePlow] Bundled profile asset {0} is missing; no entities will be whitelisted until it is restored.", new object[]
				{
					HorsePlowModSystem.BundledProfilesAsset
				});
				this.Config = new HorsePlowConfig();
			}
			else
			{
				try
				{
					string text = asset.ToText();
					this.Config = (JsonSerializer.Deserialize<HorsePlowConfig>(text, HorsePlowModSystem.ProfileJsonOptions) ?? new HorsePlowConfig());
				}
				catch (Exception ex)
				{
					api.Logger.Error("[HorsePlow] Could not read bundled profile asset {0}: {1}", new object[]
					{
						HorsePlowModSystem.BundledProfilesAsset,
						ex.Message
					});
					this.Config = new HorsePlowConfig();
				}
			}
			this.Config.Normalize();
			this.profileCache.Clear();
			this.whitelistCache.Clear();
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00006D80 File Offset: 0x00004F80
		public override void StartServerSide(ICoreServerAPI api)
		{
			base.StartServerSide(api);
			this.sapi = api;
			this.LoadUserConfig(api);
			api.Event.OnPlayerInteractEntity += new OnInteractDelegate(this.OnPlayerInteractEntity);
			this.serverEatSuppressTickId = api.Event.RegisterGameTickListener(new Action<float>(this.SuppressWorkCollarEatAnimationsServer), 500, 0);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00006DE0 File Offset: 0x00004FE0
		private void LoadUserConfig(ICoreServerAPI api)
		{
			HorsePlowUserConfig horsePlowUserConfig = null;
			try
			{
				horsePlowUserConfig = api.LoadModConfig<HorsePlowUserConfig>("horseplow.json");
			}
			catch (Exception ex)
			{
				api.Logger.Warning("[HorsePlow] Could not read {0}; neutral speed multipliers will be used. {1}", new object[]
				{
					"horseplow.json",
					ex.Message
				});
			}
			this.UserConfig = (horsePlowUserConfig ?? new HorsePlowUserConfig());
			this.UserConfig.Normalize();
			api.StoreModConfig<HorsePlowUserConfig>(this.UserConfig, "horseplow.json");
			api.Logger.Notification("[HorsePlow] Using bundled draft profiles from {0}.", new object[]
			{
				HorsePlowModSystem.BundledProfilesAsset
			});
			api.Logger.Notification("[HorsePlow] Global speed multipliers: forward={0:0.###}x, reverse={1:0.###}x", new object[]
			{
				this.UserConfig.ForwardSpeedMultiplier,
				this.UserConfig.ReverseSpeedMultiplier
			});
			ILogger logger = api.Logger;
			string text = "[HorsePlow] Draft profiles active: {0} explicit profiles, whitelist entries={1}, team speed mode={2}";
			object[] array = new object[3];
			int num = 0;
			DraftEntityProfile[] entityProfiles = this.Config.EntityProfiles;
			array[num] = ((entityProfiles != null) ? entityProfiles.Length : 0);
			int num2 = 1;
			string[] hitchWhitelist = this.Config.HitchWhitelist;
			array[num2] = ((hitchWhitelist != null) ? hitchWhitelist.Length : 0);
			array[2] = this.Config.TeamSpeedMode;
			logger.Notification(text, array);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00006F28 File Offset: 0x00005128
		public ResolvedDraftEntityProfile ResolveDraftProfile(Entity entity)
		{
			bool flag = ((entity != null) ? entity.Code : null) == null;
			ResolvedDraftEntityProfile result;
			if (flag)
			{
				result = this.Config.ResolveProfile(null);
			}
			else
			{
				ValueTuple<string, string> key = new ValueTuple<string, string>(entity.Code.Domain ?? "", entity.Code.Path ?? "");
				ResolvedDraftEntityProfile resolvedDraftEntityProfile;
				bool flag2 = this.profileCache.TryGetValue(key, out resolvedDraftEntityProfile);
				if (flag2)
				{
					result = resolvedDraftEntityProfile;
				}
				else
				{
					ResolvedDraftEntityProfile resolvedDraftEntityProfile2 = this.Config.ResolveProfile(entity);
					this.profileCache[key] = resolvedDraftEntityProfile2;
					result = resolvedDraftEntityProfile2;
				}
			}
			return result;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00006FC8 File Offset: 0x000051C8
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
				ValueTuple<string, string> key = new ValueTuple<string, string>(entity.Code.Domain ?? "", entity.Code.Path ?? "");
				bool flag3;
				bool flag2 = this.whitelistCache.TryGetValue(key, out flag3);
				if (flag2)
				{
					result = flag3;
				}
				else
				{
					bool flag4 = this.Config.IsEntityWhitelisted(entity);
					this.whitelistCache[key] = flag4;
					result = flag4;
				}
			}
			return result;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x0000248F File Offset: 0x0000068F
		public override void StartClientSide(ICoreClientAPI api)
		{
			base.StartClientSide(api);
			this.capi = api;
			this.clientEatSuppressTickId = api.Event.RegisterGameTickListener(new Action<float>(this.SuppressWorkCollarEatAnimationsClient), 100, 0);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x0000705C File Offset: 0x0000525C
		private void SuppressWorkCollarEatAnimationsServer(float dt)
		{
			ICoreServerAPI coreServerAPI = this.sapi;
			object obj;
			if (coreServerAPI == null)
			{
				obj = null;
			}
			else
			{
				IServerWorldAccessor world = coreServerAPI.World;
				obj = ((world != null) ? world.LoadedEntities : null);
			}
			bool flag = obj == null;
			if (!flag)
			{
				foreach (Entity entity in this.sapi.World.LoadedEntities.Values)
				{
					this.SuppressWorkCollarEatAnimation(entity);
				}
			}
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000070E8 File Offset: 0x000052E8
		private void SuppressWorkCollarEatAnimationsClient(float dt)
		{
			ICoreClientAPI coreClientAPI = this.capi;
			object obj;
			if (coreClientAPI == null)
			{
				obj = null;
			}
			else
			{
				IClientWorldAccessor world = coreClientAPI.World;
				obj = ((world != null) ? world.LoadedEntities : null);
			}
			bool flag = obj == null;
			if (!flag)
			{
				Entity[] array = this.capi.World.LoadedEntities.Values.ToArray<Entity>();
				foreach (Entity entity in array)
				{
					this.SuppressWorkCollarEatAnimation(entity);
				}
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x0000715C File Offset: 0x0000535C
		private void SuppressWorkCollarEatAnimation(Entity entity)
		{
			EntityAgent entityAgent = entity as EntityAgent;
			bool flag = entityAgent == null;
			if (!flag)
			{
				bool flag2 = !this.IsEntityWhitelisted(entityAgent);
				if (!flag2)
				{
					bool flag3 = entityAgent.GetBehavior<EntityBehaviorRopeTieable>() == null;
					if (!flag3)
					{
						bool flag4 = HorsePlowModSystem.FindWorkCollarSlot(entityAgent) == null;
						if (!flag4)
						{
							HorsePlowModSystem.SuppressEatAnimation(entityAgent);
						}
					}
				}
			}
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x000071B8 File Offset: 0x000053B8
		private static void SuppressEatAnimation(EntityAgent draftEntity)
		{
			IAnimationManager animationManager = (draftEntity != null) ? draftEntity.AnimManager : null;
			bool flag = animationManager == null;
			if (!flag)
			{
				bool flag2 = animationManager.IsAnimationActive(new string[]
				{
					"Eat"
				});
				if (flag2)
				{
					animationManager.StopAnimation("Eat");
				}
				bool flag3 = animationManager.IsAnimationActive(new string[]
				{
					"eat"
				});
				if (flag3)
				{
					animationManager.StopAnimation("eat");
				}
			}
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x0000722C File Offset: 0x0000542C
		private static ItemSlot FindWorkCollarSlot(EntityAgent draftEntity)
		{
			EntityBehaviorAttachable entityBehaviorAttachable = (draftEntity != null) ? draftEntity.GetBehavior<EntityBehaviorAttachable>() : null;
			bool flag = ((entityBehaviorAttachable != null) ? entityBehaviorAttachable.Inventory : null) == null;
			ItemSlot result;
			if (flag)
			{
				result = null;
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
							flag3 = ((code != null) ? new bool?(code.Equals(HorsePlowModSystem.WorkCollarCode)) : null);
						}
						bool? flag4 = flag3;
						flag2 = flag4.GetValueOrDefault();
					}
					bool flag5 = flag2;
					if (flag5)
					{
						return itemSlot;
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00007300 File Offset: 0x00005500
		private static bool HasWorkCollar(EntityAgent draftEntity)
		{
			return HorsePlowModSystem.FindWorkCollarSlot(draftEntity) != null;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x0000731C File Offset: 0x0000551C
		private void OnPlayerInteractEntity(Entity target, IPlayer byPlayer, ItemSlot slot, Vec3d hitPosition, int mode, ref EnumHandling handling)
		{
			bool flag = handling != 0;
			if (!flag)
			{
				bool flag2 = mode != 1;
				if (!flag2)
				{
					EntityAgent entityAgent = target as EntityAgent;
					bool flag3 = entityAgent == null || !this.IsSupportedDraftEntity(entityAgent);
					if (!flag3)
					{
						EntityPlayer entityPlayer = (byPlayer != null) ? byPlayer.Entity : null;
						bool flag4 = entityPlayer == null || !entityPlayer.Controls.CtrlKey;
						if (!flag4)
						{
							bool flag5 = slot != null && !slot.Empty;
							if (!flag5)
							{
								int num = this.RemoveAllRopesFromHorse(entityAgent, entityPlayer);
								bool flag6 = num <= 0;
								if (!flag6)
								{
									IServerPlayer serverPlayer = entityPlayer.Player as IServerPlayer;
									bool flag7 = serverPlayer != null;
									if (flag7)
									{
										IServerPlayer serverPlayer2 = serverPlayer;
										int generalChatGroup = GlobalConstants.GeneralChatGroup;
										string text;
										if (num != 1)
										{
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
											defaultInterpolatedStringHandler..ctor(15, 1);
											defaultInterpolatedStringHandler.AppendFormatted<int>(num);
											defaultInterpolatedStringHandler.AppendLiteral(" ropes removed.");
											text = defaultInterpolatedStringHandler.ToStringAndClear();
										}
										else
										{
											text = "Rope removed.";
										}
										serverPlayer2.SendMessage(generalChatGroup, text, 4, null);
									}
									handling = 3;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00007428 File Offset: 0x00005628
		private int RemoveAllRopesFromHorse(EntityAgent selectedHorse, EntityPlayer player)
		{
			EntityBehaviorRopeTieable behavior = selectedHorse.GetBehavior<EntityBehaviorRopeTieable>();
			int[] array;
			if (behavior == null)
			{
				array = null;
			}
			else
			{
				IntArrayAttribute clothIds = behavior.ClothIds;
				array = ((clothIds != null) ? clothIds.value : null);
			}
			int[] array2 = array;
			bool flag = array2 == null || array2.Length == 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int[] array3 = (int[])array2.Clone();
				ClothManager modSystem = this.sapi.ModLoader.GetModSystem<ClothManager>(true);
				bool flag2 = modSystem == null;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					int num = 0;
					foreach (int num2 in array3)
					{
						bool flag3 = num2 == 0;
						if (!flag3)
						{
							ClothSystem clothSystem = modSystem.GetClothSystem(num2);
							bool flag4 = clothSystem == null;
							if (flag4)
							{
								IntArrayAttribute clothIds2 = behavior.ClothIds;
								if (clothIds2 != null)
								{
									clothIds2.RemoveInt(num2);
								}
							}
							else
							{
								bool flag5 = false;
								Entity[] array5 = Enumerable.DistinctBy<Entity, long>(from point in clothSystem.Ends
								select point.PinnedToEntity into entity
								where entity != null
								select entity, (Entity entity) => entity.EntityId).ToArray<Entity>();
								foreach (Entity entity3 in array5)
								{
									bool flag6 = entity3.EntityId == selectedHorse.EntityId;
									if (flag6)
									{
										flag5 = true;
									}
								}
								bool flag7 = !flag5;
								if (flag7)
								{
									IntArrayAttribute clothIds3 = behavior.ClothIds;
									if (clothIds3 != null)
									{
										clothIds3.RemoveInt(num2);
									}
								}
								else
								{
									bool flag8 = false;
									foreach (Entity entity2 in array5)
									{
										flag8 |= HorsePlowModSystem.NormalizeHeldRopeItem(entity2, num2);
										EntityBehaviorRopeTieable behavior2 = entity2.GetBehavior<EntityBehaviorRopeTieable>();
										if (behavior2 != null)
										{
											behavior2.Detach(clothSystem);
										}
									}
									foreach (ClothPoint clothPoint in clothSystem.Ends)
									{
										EntityItem entityItem = clothPoint.PinnedToEntity as EntityItem;
										bool flag9;
										if (entityItem != null)
										{
											ItemStack itemstack = entityItem.Itemstack;
											flag9 = (((itemstack != null) ? itemstack.Collectible : null) is ItemRope);
										}
										else
										{
											flag9 = false;
										}
										bool flag10 = flag9;
										if (flag10)
										{
											entityItem.Itemstack.Attributes.RemoveAttribute("clothId");
											entityItem.Itemstack.Attributes.RemoveAttribute("ropeHeldByEntityId");
											flag8 = true;
										}
									}
									clothSystem.WalkPoints(delegate(ClothPoint point)
									{
										bool pinned = point.Pinned;
										if (pinned)
										{
											point.UnPin();
										}
									});
									modSystem.UnregisterCloth(num2);
									bool flag11 = !flag8;
									if (flag11)
									{
										this.RefundRope(player);
									}
									num++;
								}
							}
						}
					}
					IntArrayAttribute clothIds4 = behavior.ClothIds;
					bool flag12 = clothIds4 != null && clothIds4.value.Length == 0;
					if (flag12)
					{
						selectedHorse.WatchedAttributes.RemoveAttribute("clothIds");
					}
					bool flag13 = num > 0;
					if (flag13)
					{
						this.sapi.World.PlaySoundAt(HorsePlowModSystem.CollectSoundCode, selectedHorse, null, true, 32f, 1f);
					}
					result = num;
				}
			}
			return result;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x0000776C File Offset: 0x0000596C
		private static bool NormalizeHeldRopeItem(Entity entity, int clothId)
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
				bool found = false;
				entityAgent.WalkInventory(delegate(ItemSlot slot)
				{
					bool empty = slot.Empty;
					bool result2;
					if (empty)
					{
						result2 = true;
					}
					else
					{
						ItemStack itemstack = slot.Itemstack;
						bool flag2 = !(((itemstack != null) ? itemstack.Collectible : null) is ItemRope);
						if (flag2)
						{
							result2 = true;
						}
						else
						{
							bool flag3 = slot.Itemstack.Attributes.GetInt("clothId", 0) != clothId;
							if (flag3)
							{
								result2 = true;
							}
							else
							{
								slot.Itemstack.Attributes.RemoveAttribute("clothId");
								slot.Itemstack.Attributes.RemoveAttribute("ropeHeldByEntityId");
								slot.MarkDirty();
								found = true;
								result2 = false;
							}
						}
					}
					return result2;
				});
				result = found;
			}
			return result;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x000077C4 File Offset: 0x000059C4
		private void RefundRope(EntityPlayer player)
		{
			Item item = this.sapi.World.GetItem(HorsePlowModSystem.RopeItemCode);
			bool flag = item == null;
			if (!flag)
			{
				ItemStack itemStack = new ItemStack(item, 1);
				bool flag2 = player.TryGiveItemStack(itemStack);
				if (!flag2)
				{
					this.sapi.World.SpawnItemEntity(itemStack, new Vec3d(player.Pos.X, player.Pos.Y + 0.5, player.Pos.Z), null);
				}
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000784C File Offset: 0x00005A4C
		public override void Dispose()
		{
			bool flag = this.sapi != null;
			if (flag)
			{
				this.sapi.Event.OnPlayerInteractEntity -= new OnInteractDelegate(this.OnPlayerInteractEntity);
				bool flag2 = this.serverEatSuppressTickId != 0L;
				if (flag2)
				{
					this.sapi.Event.UnregisterGameTickListener(this.serverEatSuppressTickId);
					this.serverEatSuppressTickId = 0L;
				}
			}
			bool flag3 = this.capi != null && this.clientEatSuppressTickId != 0L;
			if (flag3)
			{
				this.capi.Event.UnregisterGameTickListener(this.clientEatSuppressTickId);
				this.clientEatSuppressTickId = 0L;
			}
			this.sapi = null;
			this.capi = null;
			this.profileCache.Clear();
			this.whitelistCache.Clear();
			base.Dispose();
		}

		// Token: 0x060000AC RID: 172 RVA: 0x0000791C File Offset: 0x00005B1C
		private bool IsSupportedDraftEntity(Entity entity)
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
					result = (!flag3 && (entityAgent.GetBehavior<EntityBehaviorJauntRideable>() != null || HorsePlowModSystem.HasWorkCollar(entityAgent)));
				}
			}
			return result;
		}

		// Token: 0x04000092 RID: 146
		private static readonly AssetLocation WorkCollarCode = new AssetLocation("horseplow", "workcollar");

		// Token: 0x04000093 RID: 147
		private static readonly AssetLocation BundledProfilesAsset = new AssetLocation("horseplow", "config/draftprofiles.json");

		// Token: 0x04000094 RID: 148
		private static readonly AssetLocation RopeItemCode = new AssetLocation("game", "rope");

		// Token: 0x04000095 RID: 149
		private static readonly AssetLocation CollectSoundCode = new AssetLocation("game:sounds/player/collect");

		// Token: 0x04000096 RID: 150
		private static readonly JsonSerializerOptions ProfileJsonOptions = new JsonSerializerOptions
		{
			IncludeFields = true,
			PropertyNameCaseInsensitive = true,
			AllowTrailingCommas = true,
			ReadCommentHandling = 1
		};

		// Token: 0x04000097 RID: 151
		private const string UserConfigFilename = "horseplow.json";

		// Token: 0x04000098 RID: 152
		private const int ServerEatAnimationSuppressIntervalMs = 500;

		// Token: 0x04000099 RID: 153
		private const int ClientEatAnimationSuppressIntervalMs = 100;

		// Token: 0x0400009C RID: 156
		[TupleElementNames(new string[]
		{
			"Domain",
			"Path"
		})]
		private readonly ConcurrentDictionary<ValueTuple<string, string>, ResolvedDraftEntityProfile> profileCache = new ConcurrentDictionary<ValueTuple<string, string>, ResolvedDraftEntityProfile>();

		// Token: 0x0400009D RID: 157
		[TupleElementNames(new string[]
		{
			"Domain",
			"Path"
		})]
		private readonly ConcurrentDictionary<ValueTuple<string, string>, bool> whitelistCache = new ConcurrentDictionary<ValueTuple<string, string>, bool>();

		// Token: 0x0400009E RID: 158
		private ICoreServerAPI sapi;

		// Token: 0x0400009F RID: 159
		private ICoreClientAPI capi;

		// Token: 0x040000A0 RID: 160
		private long serverEatSuppressTickId;

		// Token: 0x040000A1 RID: 161
		private long clientEatSuppressTickId;
	}
}
