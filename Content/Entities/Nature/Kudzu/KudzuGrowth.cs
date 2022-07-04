
namespace TC2.Base.Components
{
	public static class KudzuGrowth
	{
		[IComponent.Data(Net.SendType.Unreliable)]
		public struct Data : IComponent
		{
			public float growth_speed;
			
			public Block.Handle baseBlock; 

			public FixedArray32<Vector2> Arms;

			public int ArmCount;
			[Save.Ignore, Net.Ignore] public float next_growth = default;
			public Data()
			{
				growth_speed = 0.05f; //10
				ArmCount = 0;
				baseBlock = new Block.Handle("kudzu");
			}
		}

		[ISystem.Update(ISystem.Mode.Single)]
		public static void OnUpdate(ISystem.Info info, Entity entity, [Source.Owned] ref Data kudzugrowth, [Source.Owned] in Transform.Data transform)
		{
#if SERVER
			if (info.WorldTime >= kudzugrowth.next_growth)
			{
				kudzugrowth.next_growth = info.WorldTime + kudzugrowth.growth_speed;
				
				if (kudzugrowth.ArmCount < 5)
				{
					kudzugrowth.Arms[kudzugrowth.ArmCount + 1] = new Vector2(0.00f, 0.00f);
					kudzugrowth.ArmCount += 1; 
				}
				

				ref Region.Data region = ref info.GetRegion();
				var terrain = region.GetTerrainHandle();
				
				var pos = transform.position;
				var size = new Vector2(2.0f, 2.0f);
				var speed = 0.20f; // 4.0f;

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
				var tile_flags = block.tile_flags;
				

				var random = XorRandom.New();
				var args = (block: productblock, tile_flags: tile_flags, count: 0);
				for (int i = 0; i < 32; i++)
				{
					
					ref Vector2 Arm = ref kudzugrowth.Arms[i];
					if(random.NextBool(0.50f))
					{
						args = (block: productblock, tile_flags: tile_flags | TileFlags.Solid, count: 0);
					}
					terrain.IterateRect(pos + Arm, size, ref args, SetTileFunc, dirty_flags: Chunk.DirtyFlags.Sync | Chunk.DirtyFlags.Neighbours | Chunk.DirtyFlags.Collider, iteration_flags: Terrain.IterationFlags.Create_If_Empty);
						


					float dir = random.NextFloatRange(0.00f, 1.00f);

					if (dir > 0.75f)
					{
						Arm.X += speed;
					}
					else if (dir > 0.50f)
					{
						Arm.X -= speed;
					}
					else if (dir > 0.25f)
					{
						Arm.Y += speed;
					}
					else
					{
						Arm.Y -= speed;
					}
					//App.WriteLine(Arm);
				}
			}
#endif
		}
	}

}
