
namespace TC2.Base.Components
{
	public static class KudzuGrowth
	{
		[IComponent.Data(Net.SendType.Unreliable)]
		public struct Data : IComponent
		{
			public float growth_speed;
			public float max_arms = 10; //Hard cap at 32
			public float damage = 10.0f;

			public float growth_distance = 0.20f;
			public float concentrate_chance = 0.20f;
			
			public Block.Handle baseBlock; 

			public FixedArray32<Vector2> Arms;

			public int ArmCount;
			public Physics.Layer hit_mask = Physics.Layer.Destructible; 
			public Physics.Layer hit_exclude = Physics.Layer.Static;

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
			var pos = transform.position;
			ref Region.Data region = ref info.GetRegion();
#if SERVER
			if (info.WorldTime >= kudzugrowth.next_growth)
			{
				kudzugrowth.next_growth = info.WorldTime + kudzugrowth.growth_speed;
				
				var random = XorRandom.New();

				if (kudzugrowth.ArmCount < kudzugrowth.max_arms && random.NextBool(0.10f))
				{
					kudzugrowth.Arms[kudzugrowth.ArmCount + 1] = new Vector2(0.00f, 0.00f);
					kudzugrowth.ArmCount += 1; 
				}
			
				var terrain = region.GetTerrainHandle();
				
				var size = new Vector2(3.0f, 3.0f);

				static void SetTileFunc(ref Tile tile, int x, int y, byte mask, ref (Block.Handle block, TileFlags tile_flags, int blocked_count, int support_count, int kudzu_count) arg)
				{
					if ((tile.BlockID == 0 || arg.tile_flags.HasAll(TileFlags.Solid)) && !tile.Flags.HasAll(TileFlags.Solid) && tile.BlockID != arg.block.id)
					{
						tile.Reset();

						tile.Health = 255;
						tile.BlockID = arg.block.id;
						tile.Flags |= arg.tile_flags;
						arg.kudzu_count++;
					}
				}

				static void CountTileFunc(ref Tile tile, int x, int y, byte mask, ref (Block.Handle block, TileFlags tile_flags, int blocked_count, int support_count, int kudzu_count) arg)
				{
					if(tile.BlockID != arg.block.id)
					{
						if (tile.Flags.HasAll(TileFlags.Solid))
						{
							arg.blocked_count++;
						}
						if (tile.BlockID != 0)
						{
							ref var block = ref tile.Block;
							if(block.friction > 0.200)
							{
								arg.support_count++;
							}
						}
					}
					else
					{
						arg.kudzu_count++;
					}
				}

				static void CountKudzuFunc(ref Tile tile, int x, int y, byte mask, ref (Block.Handle block, TileFlags tile_flags, int blocked_count, int support_count, int kudzu_count) arg)
				{
					if(tile.BlockID == arg.block.id)	
					{
						arg.kudzu_count++;
					}
				}

				Block.Handle productblock =  kudzugrowth.baseBlock;
				ref var block = ref productblock.GetDefinition();
				var tile_flags = block.tile_flags;

				var speed = kudzugrowth.growth_distance;
				
				for (int i = 0; i < kudzugrowth.ArmCount; i++)
				{
					var args = (block: productblock, tile_flags: tile_flags | TileFlags.Solid, blocked_count: 0, support_count: 0, kudzu_count: 0);
					ref Vector2 Arm = ref kudzugrowth.Arms[i];
					var armlength = Arm.Length();
					//Check for kudzu at current position
					terrain.IterateRect(pos + Arm, size*2.0f, ref args, CountKudzuFunc, dirty_flags: Chunk.DirtyFlags.Sync | Chunk.DirtyFlags.Neighbours | Chunk.DirtyFlags.Collider, iteration_flags: Terrain.IterationFlags.Create_If_Empty);


					if((args.kudzu_count < 1 && armlength > 1.00f) || //DEATH (Doesnt die if in kudzu or next to the core)
						(random.NextFloatRange(0.00f, 1.00f) > 0.999f)) //Randomrecall
					{
						//Arm = new Vector2(0.00f, 0.00f);
						for(int j = i; j < kudzugrowth.ArmCount-1; j++) //Move all other arms one back in the array
						{
							kudzugrowth.Arms[j].X = kudzugrowth.Arms[j+1].X;
							kudzugrowth.Arms[j].Y = kudzugrowth.Arms[j+1].Y;
						}
						i--;
						kudzugrowth.ArmCount--;
						//App.WriteLine("ARM DEATH");
					}
					else
					{
						Vector2 offset = random.NextUnitVector2Range(speed, speed);
						/*var dirrandom = random.NextFloatRange(0.00f, 1.00f);
						if(dirrandom > 0.75f)
						{
							offset.X = speed;
						}
						else if(dirrandom > 0.50f)
						{
							offset.X = -speed;
						}
						else if(dirrandom > 0.25f)
						{
							offset.Y = speed;
						}
						else
						{
							offset.Y = -speed;	
						}*/

						var hitsomething = false;

						Span<LinecastResult> results = stackalloc LinecastResult[4];
						if (region.TryLinecastAll(pos + Arm, pos + Arm + offset, size.X, ref results, mask: kudzugrowth.hit_mask, exclude: kudzugrowth.hit_exclude))
						{
							results.Sort(static (a, b) => a.alpha.CompareTo(b.alpha));

							var damage = kudzugrowth.damage;

							var flags = Damage.Flags.None;

							if (results.Length > 0)
							{
								int current = 0;
								for (var j = 0; j < results.Length; j++)
								{	
									ref var hit = ref results[j];
									if(!hit.IsNull() && hit.entity != entity)
									{
										hitsomething = true;
										Damage.Hit(entity, entity, hit.entity, hit.world_position, offset, -offset, damage, hit.material_type, Damage.Type.Crush, knockback: 0.25f, size: 2.0f, xp_modifier: 0.80f, flags: flags, yield: 0.00f, primary_damage_multiplier: 1.00f, secondary_damage_multiplier: 1.00f, terrain_damage_multiplier: 1.00f);
										j = results.Length;
									}
								}
								
							}
						}
						
						if(!hitsomething) //Only grow if not blocked
						{
							args = (block: productblock, tile_flags: tile_flags | TileFlags.Solid, blocked_count: 0, support_count: 0, kudzu_count: 0);
							terrain.IterateRect(pos + Arm + offset, size * 4.00f, ref args, CountTileFunc, dirty_flags: Chunk.DirtyFlags.Sync | Chunk.DirtyFlags.Neighbours | Chunk.DirtyFlags.Collider, iteration_flags: Terrain.IterationFlags.Create_If_Empty);
							int supportcount = args.support_count;
							
							args = (block: productblock, tile_flags: tile_flags | TileFlags.Solid, blocked_count: 0, support_count: 0, kudzu_count: 0);
							terrain.IterateRect(pos + Arm + offset, size, ref args, CountTileFunc, dirty_flags: Chunk.DirtyFlags.Sync | Chunk.DirtyFlags.Neighbours | Chunk.DirtyFlags.Collider, iteration_flags: Terrain.IterationFlags.Create_If_Empty);
							int kudzucount = args.kudzu_count; //kudzu only counts the close by things
							int blockedcount = args.blocked_count;
							//App.WriteLine(blockedcount);

							if((supportcount > 0 || armlength < 2.00f || (kudzucount + blockedcount >= size.X && kudzucount > 0)) 
								&& blockedcount < size.X)
							{
								//Move
								Arm += offset;
								kudzugrowth.Sync(entity); //For the particle visualization

								//Create kudzu
								args = (block: productblock, tile_flags: tile_flags | TileFlags.Solid, blocked_count: 0, support_count: 0, kudzu_count: 0);
								terrain.IterateRect(pos + Arm, size, ref args, SetTileFunc, dirty_flags: Chunk.DirtyFlags.Sync | Chunk.DirtyFlags.Neighbours | Chunk.DirtyFlags.Collider, iteration_flags: Terrain.IterationFlags.Create_If_Empty);

								if (args.kudzu_count > 0 && random.NextBool(kudzugrowth.concentrate_chance))
								{
									ref var RandomArm = ref kudzugrowth.Arms[random.NextIntRange(0, kudzugrowth.ArmCount - 1)];
									RandomArm.X = Arm.X;
									RandomArm.Y = Arm.Y;
								}
							}
						}
					}
				}
			}
#endif

#if CLIENT

			var random = XorRandom.New();
			for (int i = 0; i < kudzugrowth.ArmCount; i++)
			{
				ref Vector2 Arm = ref kudzugrowth.Arms[i];


				var particle = Particle.New("spark", pos + Arm, 0.10f);
				particle.frame_count = 3;
				particle.frame_count_total = 3;
				particle.fps = 30;
				particle.rotation = random.NextFloat(MathF.PI);
				particle.vel = random.NextUnitVector2Range(2, 6);
				particle.angular_velocity = random.NextFloat(10);
				particle.force = new Vector2(0, 0.00f);

				Particle.Spawn(ref region, particle);

			}
#endif

		}
	}

}
