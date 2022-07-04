using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;

using Keg;
using Keg.Engine;
using Keg.Engine.Game;
using Keg.Engine.Infrastructure;
using Keg.Extensions;

namespace TC2.Base.Components
{
	[IComponent.Data(Net.SendType.Reliable, 32u, 0)]
	public struct Data : IComponent, IBaseComponent, IBaseData
	{
		public float growth_speed;

		public Data()
		{
			growth_speed = 10;
		}
	}

	public static class Kudzu_Growth
	{
		[ISystem.Update(ISystem.Mode.Single, 0f, 0)]
		public static void OnUpdate(ISystem.Info info, Entity entity, [Source.Owned] in Data kudzu_growth)
		{
			ref Region.Data region = ref info.GetRegion();
			var terrain = region.GetTerrainHandle();
			
			val pos = this.pos;

			static void SetTileFunc(ref Tile tile, int x, int y, byte mask, ref (Block.Handle block, TileFlags tile_flags, int count) arg)
			{
				if ((tile.BlockID == 0 || arg.tile_flags.HasAll(TileFlags.Solid)) && !tile.Flags.HasAll(TileFlags.Solid))
				{
					tile.Reset();

					tile.Health = 255;
					tile.BlockID = 5;
					//tile.Flags |= arg.tile_flags;
				}
			}

			var args = (block: product.block, tile_flags: tile_flags, count: 0);
			terrain.IterateRect(pos, App.pixels_per_unit, ref args, SetTileFunc, dirty_flags: Chunk.DirtyFlags.Sync | Chunk.DirtyFlags.Neighbours | Chunk.DirtyFlags.Collider, iteration_flags: Terrain.IterationFlags.Create_If_Empty);
											
					
					
		}
	}
}
