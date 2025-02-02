﻿namespace TC2.Base.Components
{
	public static partial class Gun
	{
		public static readonly Texture.Handle texture_smoke = "BiggerSmoke_Light";
		public static readonly Texture.Handle texture_muzzle_flash = "MuzzleFlash";

		public static readonly Sound.Handle sound_gun_break = "gun_break";

		public enum Stage: uint
		{
			Ready,
			Fired,
			Cycling,
			Reloading,
			Jammed
		}

		[Flags]
		public enum Flags: uint
		{
			None = 0,

			Automatic = 1u << 0,
			Full_Reload = 1u << 1,

			Cycle_On_Shoot = 1u << 2,
			Manual_Cycle = 1u << 3,

			No_Particles = 1u << 4,

			Child_Projectiles = 1u << 5
		}

		public enum Type: byte
		{
			Undefined = 0,

			Handgun,
			Shotgun,
			SMG,
			Rifle,
			MachineGun,
			Cannon,
			AutoCannon,
			Launcher
		}

		public enum Feed: byte
		{
			Undefined = 0,

			Single,
			Cylinder,
			Drum,
			Clip,
			Magazine,
			Belt,
			Breech,
			Front,
			Funnel
		}

		public enum Action: byte
		{
			Undefined = 0,

			Revolver,
			Manual,
			Bolt,
			Lever,
			Pump,
			Blowback,
			Gas,
			Matchlock,
			Flintlock,
			Crank,
			Motor
		}

		//public enum Operation: byte
		//{
		//	Undefined = 0,

		//	Revolver,
		//	Bolt,
		//	Lever,
		//	Pump,
		//	Blowback,
		//	Gas,
		//	Crank
		//}

		[Flags]
		public enum Hints: uint
		{
			None = 0,

			//Can_Reload = 1 << 0,
			//Can_Shoot = 1 << 1,
			Cycled = 1 << 2,
			Loaded = 1 << 3,
			Wants_Reload = 1 << 4,
			Artillery = 1 << 5,
		}

		[IComponent.Data(Net.SendType.Reliable), IComponent.With<Gun.Data>]
		public partial struct Animation: IComponent
		{
			public uint frame_ready;
			public uint frame_ready_loaded;
			public uint frame_fired;
			public uint frame_cycling;
			public uint frame_reloading;
			public uint frame_jammed;
		}

		[IComponent.Data(Net.SendType.Reliable), IComponent.With<Gun.State>]
		public partial struct Data: IComponent
		{
			public static readonly Sound.Handle sound_jam_default = "gun_jam";

			[Editor.Picker.Position(true, true)]
			public Vector2 muzzle_offset = default;

			[Editor.Picker.Position(true, true)]
			public Vector2 particle_offset = default;

			public float particle_rotation = default;

			public Sound.Handle sound_shoot = default;
			public Sound.Handle sound_cycle = default;
			public Sound.Handle sound_reload = default;
			public Sound.Handle sound_empty = default;
			public Sound.Handle sound_jam = sound_jam_default;

			[Statistics.Info("Damage", description: "Damage dealt by the fired projectile.", format: "{0:0.##}x", comparison: Statistics.Comparison.Higher, priority: Statistics.Priority.High)]
			public float damage_multiplier = default;

			[Statistics.Info("Muzzle Velocity", description: "Velocity of the fired projectile.", format: "{0:0.##} m/s", comparison: Statistics.Comparison.Higher, priority: Statistics.Priority.Medium)]
			public float velocity_multiplier = default;

			[Statistics.Info("Spread", description: "Spread of the fired projectiles.", format: "{0:0.##}x", comparison: Statistics.Comparison.Lower, priority: Statistics.Priority.High)]
			public float jitter_multiplier = default;

			[Statistics.Info("Recoil", description: "Force applied after firing the weapon.", format: "{0:0.##}x", comparison: Statistics.Comparison.Lower, priority: Statistics.Priority.Medium)]
			public float recoil_multiplier = default;

			[Statistics.Info("Reload Speed", description: "Time to reload the weapon.", format: "{0:0.##}s", comparison: Statistics.Comparison.Lower, priority: Statistics.Priority.Medium)]
			public float reload_interval = default;

			[Statistics.Info("Cycle Speed", description: "Rate of fire.", format: "{0:0.##}s", comparison: Statistics.Comparison.Lower, priority: Statistics.Priority.High)]
			public float cycle_interval = default;

			[Statistics.Info("Stability", description: "Reliability, may result in a catastrophic failure if too low.", format: "{0:P2}", comparison: Statistics.Comparison.Higher, priority: Statistics.Priority.Low)]
			public float stability = 1.00f;

			[Statistics.Info("Failure Rate", description: "Chance of malfunction, such as jamming after being fired.", format: "{0:P2}", comparison: Statistics.Comparison.Lower, priority: Statistics.Priority.Low)]
			public float failure_rate = 0.00f;

			[Statistics.Info("Maximum Ammunition", description: "Ammo capacity.", format: "{0:0}", comparison: Statistics.Comparison.Higher, priority: Statistics.Priority.High)]
			public float max_ammo = default;

			[Statistics.Info("Ammunition Usage", description: "Ammo used per shot.", format: "{0:0}", comparison: Statistics.Comparison.Lower, priority: Statistics.Priority.Medium)]
			public float ammo_per_shot = 1.00f;

			[Statistics.Info("Barrel Count", description: "Number of barrels.", format: "{0:0}", comparison: Statistics.Comparison.Lower, priority: Statistics.Priority.Medium)]
			public int barrel_count = 1;

			[Statistics.Info("Loudness", description: "Loudness of the shot.", format: "{0:0.##}x", comparison: Statistics.Comparison.Lower, priority: Statistics.Priority.Low)]
			public float sound_volume = 1.25f;

			public float sound_size = 1.00f;
			public float sound_dist_multiplier = 4.00f;

			public float sound_pitch = 1.00f;

			public float flash_size = 1.00f;

			public float smoke_size = 1.00f;
			public float smoke_amount = 1.00f;

			public float shake_amount = 0.20f;

			public float heuristic_range = 30.00f;

			[Statistics.Info("Projectile Count", description: "Number of projectiles fired per shot.", format: "{0}", comparison: Statistics.Comparison.Higher, priority: Statistics.Priority.Medium)]
			public int projectile_count = 1;

			public Gun.Flags flags = default;

			[Statistics.Info("Ammo", description: "Ammunition type.", comparison: Statistics.Comparison.None, priority: Statistics.Priority.High)]
			public Material.Flags ammo_filter = default;

			[Statistics.Info("Operation", description: "Operation mode of the weapon.", comparison: Statistics.Comparison.None, priority: Statistics.Priority.Low)]
			public Gun.Action action = default;

			[Statistics.Info("Type", description: "Type of the weapon.", comparison: Statistics.Comparison.None, priority: Statistics.Priority.Low)]
			public Gun.Type type = default;

			[Statistics.Info("Feed", description: "Method of loading ammunition.", comparison: Statistics.Comparison.None, priority: Statistics.Priority.Low)]
			public Gun.Feed feed = default;

			public Data()
			{

			}
		}

		[IComponent.Data(Net.SendType.Unreliable)]
		public partial struct State: IComponent
		{
			public Gun.Stage stage;
			public Gun.Hints hints;

			public float muzzle_velocity;

			[Save.Ignore, Net.Ignore] public Vector2 last_recoil;
			[Save.Ignore, Net.Ignore] public float next_cycle;
			[Save.Ignore, Net.Ignore] public float next_reload;
		}

		public static bool TryCalculateTrajectory(Vector2 pos_muzzle, Vector2 pos_target, float speed, float gravity, out float? angle_shallow, out float? angle_steep)
		{
			angle_shallow = null;
			angle_steep = null;

			//ref var material = ref material_ammo.GetData();
			//if (material.IsNotNull() && material.projectile_prefab.TryGetPrefab(out var prefab_projectile))
			{
				//var pos_w_offset = transform.LocalToWorld(gun.muzzle_offset);
				//var dir = transform.GetDirection();

				//var velocity_jitter = 1.00f - (Maths.Clamp(gun.jitter_multiplier * 0.20f, 0.00f, 1.00f) * 0.50f);

				//if (prefab_projectile.Root.TryGetComponentData<Projectile.Data>(out var projectile, true))
				{
					//var random_multiplier = random.NextFloatRange(0.90f * velocity_jitter, 1.10f);


					//var random_multiplier = ((0.90f * velocity_jitter) + 1.10f) * 0.50f;

					var p = pos_target - pos_muzzle;
					p.Y *= -1;

					var v = speed; // velocity_multiplier * material.projectile_speed_mult;
					var g = gravity; /// region.GetGravity().Y * projectile.gravity;
					//var d = projectile.damp;

					var sqrt = MathF.Sqrt((v * v * v * v) - (g * (g * (p.X * p.X) + (2.00f * p.Y * (v * v)))));
					
					var a = MathF.Atan(((v * v) - sqrt) / (g * p.X));
					var b = MathF.Atan(((v * v) + sqrt) / (g * p.X));

					if (!float.IsNaN(a))
					{
						angle_shallow = a;

						if (!float.IsNaN(b))
						{
							if (MathF.Abs(a) < MathF.Abs(b))
							{
								angle_steep = b;
							}
							else
							{
								angle_steep = a;
								angle_shallow = b;
							}
						}
					}
					else if (!float.IsNaN(b))
					{
						angle_shallow = b;
					}
				}
			}

			return angle_shallow.HasValue || angle_steep.HasValue;
		}

#if CLIENT
		public static void DrawTrajectory(ref Region.Data region, IMaterial.Handle material_ammo, in Gun.Data gun, in Transform.Data transform)
		{
			var ts = Timestamp.Now();

			ref var material = ref material_ammo.GetData();
			if (material.IsNotNull() && material.projectile_prefab.TryGetPrefab(out var prefab_projectile))
			{
				var pos_w_offset = transform.LocalToWorld(gun.muzzle_offset);

				if (prefab_projectile.Root.TryGetComponentData<Projectile.Data>(out var projectile, true))
				{
					var vel = transform.GetDirection() * gun.velocity_multiplier * material.projectile_speed_mult; // * random_multiplier;

					var pos_a = pos_w_offset;
					var pos_b = pos_w_offset;

					//GUI.Text($"{vel}");

					var iter_count = 500;
					var iter_count_inv = 1.00f / iter_count;

					var pos_last = pos_a;
					var dist_delta = 0.00f;


					var line_len = 4.00f;
					var line_gap = 2.00f;

					for (int i = 0; i < iter_count; i++)
					{
						var pos = pos_b;
						var alpha = i * iter_count_inv;

						vel *= projectile.damp;
						vel += Region.gravity * App.fixed_update_interval_s * projectile.gravity;

						var step = vel * App.fixed_update_interval_s;
						pos += step;

						pos_a = pos_b;
						pos_b = pos;

						dist_delta += Vector2.Distance(pos_a, pos_b);

						if (dist_delta >= line_len)
						{
							var dir = (pos - pos_last).GetNormalized(out var len);
							len -= line_gap;

							var pos_line_a = pos_last;
							var pos_line_b = pos_last + (dir * len);

							if (!region.IsInLineOfSight(pos_line_a, pos_line_b))
							{
								//var pos_line_mid = (pos_line_a + pos_line_b) * 0.50f;
								//var dir_perp = dir.GetPerpendicular() * 10;

								//GUI.DrawLine((pos_line_mid - (dir_perp)).WorldToCanvas(), (pos_line_mid + (dir_perp)).WorldToCanvas(), GUI.col_button_yellow.WithAlphaMult(0.50f), 4.00f);


								break;
							}

							GUI.DrawLine((pos_line_a).WorldToCanvas(), (pos_line_b).WorldToCanvas(), GUI.col_button_yellow.WithAlphaMult(Maths.Clamp(1.00f - (i * iter_count_inv), 0.10f, 0.50f)), 4.00f);

							pos_last = pos;
							dist_delta = 0;
						}
					}
				}
			}

			var elapsed_ms = ts.GetMilliseconds();
			//GUI.Text($"trace in {elapsed_ms:0.0000} ms");
		}

		public struct HoldGUI: IGUICommand
		{
			public Transform.Data transform;
			public Vector2 world_position_target;

			public Gun.Data gun;
			public Inventory1.Data inventory;
			public Gun.State state;

			public void Draw()
			{
				var dir_a = (this.world_position_target - this.transform.GetInterpolatedPosition()).GetNormalized();
				var dir_b = this.transform.GetInterpolatedDirection();

				ref var region = ref Client.GetRegion();

				using (var window = GUI.Window.HUD("Crosshair", GUI.WorldToCanvas(this.world_position_target), size: new(100, 100)))
				{
					if (window.show)
					{
						if (this.state.stage == Gun.Stage.Reloading)
						{
							GUI.TitleCentered($"Reloading\n{MathF.Max(this.state.next_reload - region.GetFixedTime(), 0.00f):0.00}", pivot: new(0.50f));
						}
					}
				}

				//GUI.DrawLine((transform.position).WorldToCanvas(), (transform.position + (transform.GetDirection() * 10.00f)).WorldToCanvas(), Color32BGRA.Red);

				//GUI.DrawCrosshair(this.transform.GetInterpolatedPosition(), this.world_position_target, this.transform.GetInterpolatedDirection(), this.gun.jitter_multiplier, this.inventory[0].quantity, this.gun.max_ammo);
				Gun.DrawCrosshair(this.transform.GetInterpolatedPosition(), this.world_position_target, Vector2.Lerp(dir_a, dir_b, 0.25f), this.gun.jitter_multiplier, this.inventory[0].quantity, this.gun.max_ammo);
			}
		}

		public static void DrawCrosshair(Vector2 position_a, Vector2 position_b, Vector2 dir, float radius, float ammo_count, float ammo_count_max)
		{
			var dist = Vector2.Distance(position_a, position_b);
			var cpos_target = GUI.WorldToCanvas(position_a + (dir * dist));

			radius = MathF.Min(radius, 20.00f);
			radius *= MathF.Sqrt(dist);
			var line_length = MathF.Max(15.00f, radius * 2.00f);

			var color = new Color32BGRA(0xffff0000);

			if (ammo_count == 0)
			{
				color = 0xffffff00;
			}

			var color_circle = color.WithAlphaMult(0.30f);
			var color_line = color.WithAlphaMult(0.80f);

			GUI.DrawCircle(cpos_target, radius, color_circle);
			GUI.DrawLine(cpos_target + new Vector2(-line_length, 0), cpos_target + new Vector2(+line_length, 0), color_line, 1.00f);
			GUI.DrawLine(cpos_target + new Vector2(0, -line_length), cpos_target + new Vector2(0, +line_length), color_line, 1.00f);

			if (ammo_count_max > 0.00f)
			{
				var step = MathF.Tau / ammo_count_max;
				var count = (int)ammo_count_max;

				for (var i = 0; i < count; i++)
				{
					var (sin, cos) = MathF.SinCos(i * -step);

					var col = new Color32BGRA(0xffff0000);
					col = col.WithColorMult(i < ammo_count ? 1.00f : 0.10f);
					col = col.WithAlphaMult(0.60f);

					var r = 3.50f;
					GUI.DrawCircleFilled(cpos_target + (new Vector2(cos, sin) * 24), r, col);
				}
			}
		}

		//[ISystem.Render(ISystem.Mode.Single)]
		//public static void RenderLaserExample(ISystem.Info info, Entity entity,
		//[Source.Owned] in Transform.Data transform, [Source.Owned] in Control.Data control, [Source.Owned] in Gun.Data gun, [Source.Owned] in Animated.Renderer.Data renderer)
		//{
		//	var rmb = control.mouse.GetKey(Mouse.Key.Right);
		//	if (true || rmb)
		//	{
		//		ref var region = ref info.GetRegion();

		//		var aim_dir = transform.GetInterpolatedDirection();
		//		var pos_a = transform.LocalToWorldInterpolated(renderer.offset + gun.muzzle_offset); // + (aim_dir * 0.25f);
		//		var pos_b = pos_a + (aim_dir * 50.00f);

		//		var hit_pos = pos_b;

		//		Span<LinecastResult> results = stackalloc LinecastResult[16];
		//		if (region.TryLinecastAll(pos_a, pos_b, 0.00f, ref results, mask: Physics.Layer.Solid | Physics.Layer.World, exclude: Physics.Layer.Essence | Physics.Layer.Ignore_Bullet))
		//		{
		//			results.SortByDistance();

		//			foreach (ref var result in results)
		//			{
		//				if (result.material_type == Material.Type.Glass) continue;
		//				if (result.alpha <= 0.00f && !result.layer.HasAny(Physics.Layer.Static | Physics.Layer.World)) continue;

		//				hit_pos = result.world_position;

		//				break;
		//			}
		//		}

		//		var dist = Vector2.Distance(pos_a, hit_pos);

		//		var transform_tmp = transform;
		//		transform_tmp.SetPosition(hit_pos);

		//		Projectile.Renderer.Draw(transform_tmp, new()
		//		{
		//			color_a = 0x80ff0000,
		//			color_b = 0x80ff0000,
		//			length = dist
		//		});
		//	}
		//}

		[ISystem.GUI(ISystem.Mode.Single), HasTag("local", true, Source.Modifier.Parent)]
		public static void OnGUI(ISystem.Info info,
		[Source.Owned] in Gun.Data gun, [Source.Owned] in Gun.State state, [Source.Owned] in Transform.Data transform, [Source.Owned] in Control.Data control, [Source.Owned, Pair.Of<Gun.Data>] ref Inventory1.Data inventory,
		[Source.Parent] in Interactor.Data interactor, [Source.Parent] in Player.Data player)
		{
			if (player.IsLocal())
			{
				var gui = new HoldGUI()
				{
					transform = transform,
					world_position_target = control.mouse.position,
					state = state,
					inventory = inventory,
					gun = gun
				};
				gui.Submit();
			}
		}

		// TODO: Shithack
		[ISystem.GUI(ISystem.Mode.Single)]
		public static void OnGUIVehicle(ISystem.Info info,
		[Source.Owned] in Gun.Data gun, [Source.Owned] in Gun.State state, [Source.Owned] in Transform.Data transform, [Source.Owned] in Control.Data control, [Source.Owned, Pair.Of<Gun.Data>] ref Inventory1.Data inventory,
		[Source.Parent] in Vehicle.Data vehicle)
		{
			if (vehicle.ent_seat_driver.IsValid() && vehicle.ent_seat_driver == Client.GetControlledEntity())
			{
				var gui = new HoldGUI()
				{
					transform = transform,
					world_position_target = control.mouse.position,
					state = state,
					inventory = inventory,
					gun = gun
				};
				gui.Submit();
			}
		}
#endif

		[ISystem.VeryLateUpdate(ISystem.Mode.Single)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void UpdateAnimation([Source.Owned] in Gun.State gun_state, [Source.Owned] in Gun.Animation animation, [Source.Owned] ref Animated.Renderer.Data renderer)
		{
			var frame = 0u;

			switch (gun_state.stage)
			{
				case Gun.Stage.Ready:
				{
					frame = gun_state.hints.HasAll(Gun.Hints.Loaded) ? animation.frame_ready_loaded : animation.frame_ready;
				}
				break;

				case Gun.Stage.Fired:
				{
					frame = animation.frame_fired;
				}
				break;

				case Gun.Stage.Cycling:
				{
					frame = animation.frame_cycling;
				}
				break;

				case Gun.Stage.Reloading:
				{
					frame = animation.frame_reloading;
				}
				break;

				case Gun.Stage.Jammed:
				{
					frame = animation.frame_jammed;
				}
				break;
			}

			renderer.sprite.frame.Y = frame;
		}

		[ISystem.EarlyUpdate(ISystem.Mode.Single)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void UpdateLight1([Source.Owned] in Gun.State gun_state, [Source.Owned, Pair.Of<Gun.Data>] ref Light.Data light)
		{
			if (gun_state.stage == Gun.Stage.Fired)
			{
				light.intensity = 4.00f;
			}
		}

		[ISystem.LateUpdate(ISystem.Mode.Single)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void UpdateLight2([Source.Owned, Pair.Of<Gun.Data>] ref Light.Data light)
		{
			light.intensity = Maths.Lerp(light.intensity, 0.00f, 0.50f);
		}

		[ISystem.Update(ISystem.Mode.Single)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void UpdateReload<T>(ISystem.Info info, Entity entity,
		[Source.Owned] ref Gun.Data gun, [Source.Owned] ref Gun.State gun_state,
		[Source.Owned] in Transform.Data transform, [Source.Owned] in Control.Data control,
		[Source.Owned, Pair.Of<Gun.Data>] ref Inventory1.Data inventory_magazine, [Source.Any, Pair.Of<Storage.Data>] ref T inventory,
		[Source.Parent, Optional] in Specialization.Gunslinger.Data gunslinger) where T : unmanaged, IInventory
		{
#if SERVER
			if (gun_state.hints.HasAny(Gun.Hints.Wants_Reload))
			{
				gun_state.hints.SetFlag(Gun.Hints.Wants_Reload, false);
				gun_state.stage = Gun.Stage.Reloading;
				gun_state.Sync(entity);

				return;
			}
#endif

			if (gun_state.stage == Gun.Stage.Reloading)
			{
				var time = info.WorldTime;
				if (time < gun_state.next_reload) return;
				ref var region = ref info.GetRegion();

				if (inventory_magazine.resource.quantity >= gun.max_ammo)
				{
					gun_state.stage = Gun.Stage.Cycling;
				}
				else if (!gun.flags.HasAll(Gun.Flags.Full_Reload) && control.mouse.GetKeyDown(Mouse.Key.Left) && gun_state.hints.HasAll(Gun.Hints.Loaded))
				{
#if SERVER
					gun_state.stage = Gun.Stage.Cycling;
					gun_state.Sync(entity);

					return;
#endif
				}
				else
				{
					gun_state.next_reload = info.WorldTime + gunslinger.ApplyReloadSpeed(gun.reload_interval);

					ref var material_ammo = ref inventory_magazine.resource.material.GetData();
					if (material_ammo.IsNotNull() && !material_ammo.flags.HasAll(gun.ammo_filter)) inventory_magazine.resource.material = default;

					if (inventory_magazine.resource.material.id == 0 || inventory_magazine.resource.quantity <= float.Epsilon)
					{
						var count = inventory.Length;
						for (var i = 0; i < count; i++)
						{
							ref var resource = ref inventory[i];

							ref var material = ref resource.material.GetData();
							if (material.IsNotNull() && material.flags.HasAll(gun.ammo_filter))
							{
								inventory_magazine.resource.material = resource.material;
								gun_state.hints.SetFlag(Gun.Hints.Artillery, material.flags.HasAny(Material.Flags.Explosive));
								gun_state.muzzle_velocity = gun.velocity_multiplier * material.projectile_speed_mult;

								break;
							}
						}
					}

					if (inventory_magazine.resource.material != 0)
					{
#if SERVER
						gun_state.hints.SetFlag(Gun.Hints.Cycled, false);

						var amount = Maths.Clamp(MathF.Min(gun.max_ammo - inventory_magazine.resource.quantity, gun.flags.HasAll(Gun.Flags.Full_Reload) ? gun.max_ammo : gunslinger.ApplyBulkReload(1.00f)), 0.00f, gun.max_ammo);
						//App.WriteLine(amount);

						var done = true;

						if (Resource.Withdraw(ref inventory, ref inventory_magazine.resource, ref amount))
						{
							done = false;
							inventory_magazine.flags.SetFlag(Inventory.Flags.Dirty, true);

							Sound.Play(ref region, gun.sound_reload, transform.position);
						}

						if (done)
						{
							gun_state.stage = Gun.Stage.Cycling;
							gun_state.Sync(entity);

							return;
						}
#endif
					}
					else
					{
						gun_state.next_reload = info.WorldTime + 0.10f;
#if SERVER
						gun_state.stage = Gun.Stage.Ready;
						gun_state.Sync(entity);
#endif
						return;
					}
				}
			}
		}

		[ISystem.Update(ISystem.Mode.Single)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void OnUpdate(ISystem.Info info, Entity entity,
		[Source.Owned] ref Gun.Data gun, [Source.Owned] ref Gun.State gun_state, [Source.Owned] ref Body.Data body,
		[Source.Owned] in Transform.Data transform, [Source.Owned] in Control.Data control,
		[Source.Owned, Pair.Of<Gun.Data>] ref Inventory1.Data inventory_magazine,
		[Source.Owned, Optional(true)] ref Overheat.Data overheat)
		{
			var time = info.WorldTime;
			ref var region = ref info.GetRegion();
			if (gun_state.stage == Gun.Stage.Fired)
			{
				var pos_w_offset = transform.LocalToWorld(gun.muzzle_offset);
				var pos_w_offset_particle = transform.LocalToWorld(gun.muzzle_offset + gun.particle_offset);
				var dir = transform.GetDirection();
				var dir_particle = dir.RotateByRad(gun.particle_rotation);
				var random = XorRandom.New();
				var base_vel = body.GetVelocity();

				var failure_rate = gun.failure_rate;
				var stability = gun.stability;

				var force_jammed = false;

#if CLIENT
				Shake.Emit(ref region, transform.position, gun.shake_amount, gun.shake_amount * 1.25f, 16.00f);
#endif

				ref var material = ref inventory_magazine.resource.material.GetData();
				if (material.IsNotNull() && material.projectile_prefab.id != 0)
				{
					var velocity_jitter = Maths.Clamp(gun.jitter_multiplier * 0.20f, 0.00f, 1.00f) * 0.50f;
					var angle_jitter = Maths.Clamp(gun.jitter_multiplier, 0.00f, 25.00f);

					var recoil_mass = material.mass_per_unit * gun.ammo_per_shot;
					var recoil_speed = gun.velocity_multiplier * material.projectile_speed_mult;
					var recoil_force = -dir * ((recoil_mass * recoil_speed) * gun.recoil_multiplier * material.projectile_recoil_mult * App.tickrate * 20.00f);

					//recoil_force = Physics.LimitForce(ref body, recoil_force, new Vector2(50, 50));

					//App.WriteLine($"{body.GetMass() * gun.recoil_multiplier * App.tickrate * 150.00f}; {recoil_force.Length()}");

					//body.AddForceWorld(-dir * body.GetMass() * gun.recoil_multiplier * App.tickrate * 150.00f, pos_w_offset);
					//body.AddForce(recoil_force); //, pos_w_offset);
					body.AddForceWorld(recoil_force, pos_w_offset);
					gun_state.last_recoil = recoil_force;

#if SERVER
					var loaded_ammo = new Resource.Data()
					{
						material = inventory_magazine.resource.material,
						quantity = 0.00f
					};

					var amount = gun.ammo_per_shot;
					Resource.Withdraw(ref inventory_magazine, ref loaded_ammo, ref amount);

					var count = (material.projectile_count * gun.projectile_count) * (loaded_ammo.quantity / gun.ammo_per_shot);

					if (!overheat.IsNull())
					{
						if (overheat.heat_critical > 0.00f && material.projectile_heat > 0.00f)
						{
							var heat = ((gun.ammo_per_shot - amount) * material.projectile_heat) / MathF.Max(body.GetMass() * 0.10f, 1.00f);
							overheat.heat_current += heat;

							var heat_excess = MathF.Max(overheat.heat_current - overheat.heat_critical, 0.00f);
							if (heat_excess > 0.00f)
							{
								failure_rate = Maths.Clamp(failure_rate + (heat_excess * 0.01f), 0.00f, 1.00f);
								stability = Maths.Clamp(stability - (heat_excess * 0.005f), 0.00f, 1.00f);

								angle_jitter *= 1.00f + Maths.Clamp01(heat_excess * 0.01f);
								velocity_jitter *= 1.00f + Maths.Clamp01(heat_excess * 0.005f);
							}

							overheat.Sync(entity);
						}
					}

					//var faction_id = default(IFaction.Handle);
					
					//if (body_parent.IsNotNull())
					//{

					//}

					{
						for (var i = 0; i < count; i++)
						{
							//var random_multiplier = random.NextFloatRange(0.90f * velocity_jitter, 1.10f);
							var random_multiplier = 1.00f + random.NextFloat(velocity_jitter);
							//App.WriteLine(random_multiplier);

							var args =
							(
								damage_mult: gun.damage_multiplier * random_multiplier,
								vel: dir.RotateByDeg(random.NextFloat(angle_jitter * 0.50f * material.projectile_spread_mult)) * gun.velocity_multiplier * material.projectile_speed_mult * random_multiplier,
								ent_owner: body.GetParent(),
								ent_gun: entity,
								faction_id: body.GetFaction(),
								gun_flags: gun.flags
							);

							if (args.vel.LengthSquared() < (material.projectile_velocity_min * material.projectile_velocity_min))
							{
								force_jammed = true;
								break;
							}
							else
							{
								region.SpawnPrefab(material.projectile_prefab, pos_w_offset, rotation: args.vel.GetAngleRadians(), velocity: args.vel, angular_velocity: dir.GetParity()).ContinueWith(ent =>
								{
									ref var projectile = ref ent.GetComponent<Projectile.Data>();
									if (!projectile.IsNull())
									{
										projectile.damage_base *= args.damage_mult;
										projectile.damage_bonus *= args.damage_mult;
										projectile.velocity = args.vel;
										projectile.ent_owner = args.ent_owner;
										projectile.faction_id = args.faction_id;

										ent.SyncComponent(ref projectile);
									}

									ref var explosive = ref ent.GetComponent<Explosive.Data>();
									if (!explosive.IsNull())
									{
										explosive.ent_owner = args.ent_owner;

										ent.SyncComponent(ref explosive);
									}

									if (args.gun_flags.HasAll(Gun.Flags.Child_Projectiles))
									{
										ent.AddRelation(args.ent_gun, Relation.Type.Child);
									}
								});
							}
						}
					}

					if (stability < 1.00f && random.NextBool(failure_rate) && random.NextBool(1.00f - stability))
					{
						var explosion_data = new Explosion.Data()
						{
							power = 1.50f, // + (count * 0.80f),
							radius = 5.50f, // + (count * 2.50f),
							damage_entity = (gun.damage_multiplier * (1.00f + ((count - 1) * 3.80f))) * 200.00f,
							damage_terrain = (gun.damage_multiplier * (1.00f + (count * 0.50f))) * 130.00f,
							smoke_amount = 0.30f,
							sparks_amount = 2.00f,
							ent_owner = body.GetParent()
						};

						region.SpawnPrefab("explosion", transform.position).ContinueWith(x =>
						{
							ref var explosion = ref x.GetComponent<Explosion.Data>();
							if (!explosion.IsNull())
							{
								explosion.power = explosion_data.power;
								explosion.radius = explosion_data.radius;
								explosion.damage_entity = explosion_data.damage_entity;
								explosion.damage_terrain = explosion_data.damage_terrain;
								explosion.ent_owner = explosion_data.ent_owner;
								explosion.smoke_amount = explosion_data.smoke_amount;

								explosion.Sync(x);
							}
						});

						Sound.Play(ref region, sound_gun_break, pos_w_offset, volume: 1.50f, pitch: 1.10f, size: 1.50f);

						entity.Delete();
					}
#endif
				}

				if (force_jammed || gun.flags.HasAll(Gun.Flags.Cycle_On_Shoot))
				{
					gun_state.stage = Gun.Stage.Cycling;

#if SERVER
					if (force_jammed || random.NextBool(failure_rate))
					{
						//App.WriteLine("jammed");

						gun_state.stage = Gun.Stage.Jammed;
						entity.SyncComponent(ref gun_state);

						Sound.Play(ref region, gun.sound_jam, pos_w_offset, volume: 1.10f, pitch: 1.00f, size: 1.50f);
						WorldNotification.Push(ref region, "* Jammed *", 0xffff0000, transform.position, lifetime: 1.00f);
					}
#endif
				}
				else
				{
					gun_state.stage = Gun.Stage.Ready;
				}

#if CLIENT
				if (!gun.flags.HasAll(Gun.Flags.No_Particles))
				{
					if (gun.flash_size > 0.00f)
					{
						Particle.Spawn(ref region, new Particle.Data()
						{
							texture = texture_muzzle_flash,
							pos = transform.LocalToWorld(gun.muzzle_offset + gun.particle_offset + new Vector2(1.50f * gun.flash_size, 0.00f).RotateByRad(gun.particle_rotation)),
							lifetime = 0.25f,
							fps = 24,
							frame_count = 6,
							frame_count_total = 6,
							scale = gun.flash_size,
							lit = 1.00f,
							rotation = transform.rotation + gun.particle_rotation + (transform.scale.X < 0.00f ? MathF.PI : 0),
							vel = base_vel
						});
					}

					if (gun.smoke_size > 0.00f && gun.smoke_amount > 0.00f)
					{
						var smoke_count = (int)gun.smoke_amount;
						for (var i = 0; i < smoke_count; i++)
						{
							Particle.Spawn(ref region, new Particle.Data()
							{
								texture = texture_smoke,
								pos = pos_w_offset_particle + (dir_particle * i * 0.50f),
								lifetime = random.NextFloatRange(2.00f, 8.00f),
								fps = random.NextByteRange(12, 16),
								frame_count = 64,
								frame_count_total = 64,
								frame_offset = random.NextByteRange(0, 64),
								scale = random.NextFloatRange(0.05f, 0.10f) * gun.smoke_size,
								angular_velocity = random.NextFloatRange(-0.10f, 0.10f),
								vel = base_vel + (dir_particle * random.NextFloatRange(1.00f, 1.50f)),
								force = new Vector2(0, -random.NextFloatRange(0.00f, 0.20f)) + (dir_particle * random.NextFloatRange(0.05f, 0.20f)),
								rotation = random.NextFloat(10.00f),
								growth = random.NextFloatRange(0.15f, 0.30f),
								drag = random.NextFloatRange(0.07f, 0.10f),
								color_a = random.NextColor32Range(new Color32BGRA(255, 240, 240, 240), new Color32BGRA(255, 220, 220, 220)).WithAlphaMult(Maths.Clamp(0.40f + ((smoke_count - i) * 0.04f), 0.20f, 1.00f)),
								color_b = new Color32BGRA(000, 150, 150, 150)
							});
						}
					}
				}

				Sound.Play(gun.sound_shoot, pos_w_offset, volume: gun.sound_volume, pitch: gun.sound_pitch, size: gun.sound_size, priority: 0.70f, dist_multiplier: gun.sound_dist_multiplier);
#endif
			}

			switch (gun_state.stage)
			{
				case Gun.Stage.Cycling:
				{
					if (time < gun_state.next_cycle) break;

					if (gun_state.hints.HasAll(Gun.Hints.Cycled))
					{
						gun_state.stage = Gun.Stage.Ready;
					}
					else
					{
						var cycle_interval = gun.cycle_interval;
						//if (!gun.flags.HasAll(Gun.Flags.Automatic)) cycle_interval = gunslinger.ApplyShootSpeed(cycle_interval);

						gun_state.next_cycle = info.WorldTime + cycle_interval;
						gun_state.hints.SetFlag(Gun.Hints.Cycled, true);

#if SERVER
						Sound.Play(ref region, gun.sound_cycle, transform.position, volume: 0.50f);
#endif
					}
				}
				break;
			}
		}

		[ISystem.LateUpdate(ISystem.Mode.Single)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void OnReady(ISystem.Info info, Entity entity,
		[Source.Owned] ref Gun.Data gun, [Source.Owned] ref Gun.State gun_state,
		[Source.Owned] in Transform.Data transform, [Source.Owned] in Control.Data control, [Source.Owned] ref Body.Data body,
		[Source.Owned, Pair.Of<Gun.Data>] ref Inventory1.Data inventory_magazine)
		{
			gun_state.hints.SetFlag(Gun.Hints.Loaded, inventory_magazine.resource.quantity > float.Epsilon && inventory_magazine.resource.material.id != 0);

			if (gun_state.stage == Gun.Stage.Ready)
			{
				if (control.mouse.GetKeyDown(Mouse.Key.Left) && !gun_state.hints.HasAll(Gun.Hints.Cycled))
				{
#if SERVER
					gun_state.stage = Gun.Stage.Cycling;
					entity.SyncComponent(ref gun_state);
#endif
					return;
				}

				if (control.keyboard.GetKeyDown(Keyboard.Key.Reload) || (control.mouse.GetKeyDown(Mouse.Key.Left) && !gun_state.hints.HasAll(Gun.Hints.Loaded)))
				{
#if SERVER

					//gun_state.stage = Gun.Stage.Reloading;
					gun_state.hints.SetFlag(Gun.Hints.Wants_Reload, true);
					entity.SyncComponent(ref gun_state);
#endif
					return;
				}

				if (gun_state.hints.HasAll(Gun.Hints.Cycled) && (control.mouse.GetKeyDown(Mouse.Key.Left) || (control.mouse.GetKey(Mouse.Key.Left) && gun.flags.HasAll(Gun.Flags.Automatic))))
				{
					if (inventory_magazine.resource.quantity > float.Epsilon && inventory_magazine.resource.material.id != 0)
					{
#if SERVER
						gun_state.stage = Gun.Stage.Fired;
						gun_state.hints.SetFlag(Gun.Hints.Cycled, false);
						entity.SyncComponent(ref gun_state);
#endif
					}
					else
					{
						gun_state.stage = gun.flags.HasAll(Flags.Automatic) ? Gun.Stage.Ready : Gun.Stage.Cycling;
						gun_state.hints.SetFlag(Gun.Hints.Cycled, false);

#if SERVER
						ref var region = ref info.GetRegion();
						Sound.Play(ref region, gun.sound_empty, transform.position, volume: 0.50f);
						WorldNotification.Push(ref region, "* No ammo *", 0xffff0000, transform.position);
#endif
					}

					return;
				}
			}
			else if (gun_state.stage == Gun.Stage.Jammed)
			{
				if (control.mouse.GetKeyDown(Mouse.Key.Left) || control.keyboard.GetKeyDown(Keyboard.Key.Reload))
				{
					ref var region = ref info.GetRegion();
					var random = XorRandom.New();

					body.AddImpulse(transform.GetDirection().RotateByDeg(90.00f + random.NextFloatRange(-20.00f, 20.00f)) * MathF.Min(500, body.GetMass() * random.NextFloatRange(7.50f, 15.00f)));

#if SERVER
					if (random.NextBool(MathF.Max(0.10f, 1.00f - (0.40f + (gun.failure_rate * 5.00f)))))
					{
						//App.WriteLine("unjammed");

						//gun_state.stage = Gun.Stage.Reloading;

						gun_state.hints.SetFlag(Gun.Hints.Wants_Reload, true);
						gun_state.next_cycle = info.WorldTime + gun.reload_interval;

						entity.SyncComponent(ref gun_state);

						Sound.Play(ref region, gun.sound_jam, transform.position, volume: 0.10f, pitch: random.NextFloatRange(0.70f, 0.80f), size: 1.10f);
						WorldNotification.Push(ref region, "* Unjammed *", 0xff00ff00, transform.position);
					}
					else
					{
						gun_state.stage = Gun.Stage.Jammed;
						entity.SyncComponent(ref gun_state);

						Sound.Play(ref region, gun.sound_jam, transform.position, volume: 0.10f, pitch: random.NextFloatRange(0.70f, 0.80f), size: 1.10f);
						WorldNotification.Push(ref region, "* Jammed *", 0xffff0000, transform.position);
					}
#endif
				}
			}
		}
	}
}
