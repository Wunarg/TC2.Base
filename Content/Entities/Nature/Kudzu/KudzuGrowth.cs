
namespace TC2.Base.Components
{
	public static class KudzuGrowth
	{
		[IComponent.Data(Net.SendType.Reliable)]
		public struct Data : IComponent
		{
			public float growth_speed;
			public Block.Handle baseBlock; 

			public List<Vector2> Arms;

			public Data()
			{
				growth_speed = 10;
				baseBlock = new Block.Handle("planks");
			}
		}

		[ISystem.Update(ISystem.Mode.Single)]
		public static void OnUpdate(ISystem.Info info, Entity entity, [Source.Owned] in Data kudzugrowth, [Source.Owned] in Transform.Data transform)
		{
			ref Region.Data region = ref info.GetRegion();
			var terrain = region.GetTerrainHandle();
			
			var pos = transform.position;
			var size = new Vector2(1.0f, 1.0f);

			static void SetTileFunc(ref Tile tile, int x, int y, byte mask, ref (Block.Handle block, TileFlags tile_flags, int count) arg)
			{
				if ((tile.BlockID == 0 || arg.tile_flags.HasAll(TileFlags.Solid)) && !tile.Flags.HasAll(TileFlags.Solid))
				{
					tile.Reset();

					tile.Health = 255;
					tile.BlockID = arg.block.id;
					tile.Flags |= arg.tile_flags;
				}
			}

			Block.Handle productblock =  kudzugrowth.baseBlock;
			ref var block = ref productblock.GetDefinition();
			var tile_flags = block.tile_flags | TileFlags.Solid;
			var args = (block: productblock, tile_flags: tile_flags, count: 0);

			var random = XorRandom.New();

			foreach (Vector2 Arm in kudzugrowth.Arms)
			{
				terrain.IterateRect(pos + Arm, size * App.pixels_per_unit, ref args, SetTileFunc, dirty_flags: Chunk.DirtyFlags.Sync | Chunk.DirtyFlags.Neighbours | Chunk.DirtyFlags.Collider, iteration_flags: Terrain.IterationFlags.Create_If_Empty);

				float dir = random.NextFloatRange(0.00f, 1.00f);

				if (dir > 0.75f)
				{
					Arm.X += 1.00f;
				}

			}
		}
	}
}
