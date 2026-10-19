using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace HorsePlowMod
{
	// Token: 0x02000013 RID: 19
	public sealed class ItemWorkCollar : Item, IWearableShapeSupplier
	{
		// Token: 0x060000BA RID: 186 RVA: 0x00007AD8 File Offset: 0x00005CD8
		public Shape GetShape(ItemStack stack, Entity forEntity, string texturePrefixCode)
		{
			ICoreAPI coreAPI;
			if (forEntity == null)
			{
				coreAPI = null;
			}
			else
			{
				IWorldAccessor world = forEntity.World;
				coreAPI = ((world != null) ? world.Api : null);
			}
			ICoreAPI coreAPI2 = coreAPI;
			bool flag = coreAPI2 == null || stack == null;
			Shape result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Shape shape = Shape.TryGet(coreAPI2, ItemWorkCollar.AttachedShapePath);
				bool flag2 = shape == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					Shape shape2 = shape.Clone();
					ResolvedWorkCollarTransform transform = ItemWorkCollar.ResolveTransformForEntity(coreAPI2, forEntity);
					ItemWorkCollar.WrapShapeWithTransformRoot(shape2, transform);
					shape2.SubclassForStepParenting(texturePrefixCode, 0f);
					shape2.ResolveReferences(coreAPI2.World.Logger, ItemWorkCollar.AttachedShapePath.ToShortString());
					result = shape2;
				}
			}
			return result;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00007B78 File Offset: 0x00005D78
		private static ResolvedWorkCollarTransform ResolveTransformForEntity(ICoreAPI api, Entity entity)
		{
			HorsePlowModSystem modSystem = api.ModLoader.GetModSystem<HorsePlowModSystem>(true);
			ResolvedDraftEntityProfile resolvedDraftEntityProfile = (modSystem != null) ? modSystem.ResolveDraftProfile(entity) : null;
			return ((resolvedDraftEntityProfile != null) ? resolvedDraftEntityProfile.WorkCollarTransform : null) ?? ResolvedDraftEntityProfile.CreateEmergencyFallback().WorkCollarTransform;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00007BC0 File Offset: 0x00005DC0
		private static void WrapShapeWithTransformRoot(Shape shape, ResolvedWorkCollarTransform transform)
		{
			bool flag = ((shape != null) ? shape.Elements : null) == null || shape.Elements.Length == 0 || transform == null;
			if (!flag)
			{
				double num = (double)transform.Translation.X;
				double num2 = (double)transform.Translation.Y;
				double num3 = (double)transform.Translation.Z;
				ShapeElement[] elements = shape.Elements;
				ShapeElement shapeElement = new ShapeElement
				{
					Name = "horseplow-workcollar-transform-root",
					From = new double[]
					{
						num,
						num2,
						num3
					},
					To = new double[]
					{
						num,
						num2,
						num3
					},
					RotationOrigin = new double[]
					{
						num,
						num2,
						num3
					},
					RotationX = (double)transform.RotationDegrees.X,
					RotationY = (double)transform.RotationDegrees.Y,
					RotationZ = (double)transform.RotationDegrees.Z,
					ScaleX = (double)transform.Scale.X,
					ScaleY = (double)transform.Scale.Y,
					ScaleZ = (double)transform.Scale.Z,
					Children = elements
				};
				foreach (ShapeElement shapeElement2 in elements)
				{
					bool flag2 = shapeElement2 != null;
					if (flag2)
					{
						shapeElement2.ParentElement = shapeElement;
					}
				}
				shape.Elements = new ShapeElement[]
				{
					shapeElement
				};
			}
		}

		// Token: 0x040000AB RID: 171
		private static readonly AssetLocation AttachedShapePath = new AssetLocation("horseplow", "shapes/item/workcollar.json");
	}
}
