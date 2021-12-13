﻿
namespace TC2.Base.Components
{
	public static partial class Melee
	{
		public enum Category: byte
		{
			Undefined = 0,

			Pointed,
			Bladed,
			Blunt,
			Misc
		}

		[Flags]
		public enum Flags: uint
		{
			None = 0,

			No_Handle = 1 << 0,
		}

		[IComponent.Data(Net.SendType.Reliable)]
		public partial struct Data: IComponent
		{
			public Sound.Handle sound_swing;
			public float sound_volume = 1.00f;
			public float sound_size = 2.00f;
			public float sound_pitch = 1.00f;

			public Vector2 swing_offset = new(1.00f, 1.00f);
			public float swing_rotation = -2.50f;

			[Statistics.Info("Base Damage", description: "Base damage", format: "{0:0}", comparison: Statistics.Comparison.Higher)]
			public float damage_base;

			[Statistics.Info("Bonus Damage", description: "Random additional damage", format: "{0:0}", comparison: Statistics.Comparison.Higher)]
			public float damage_bonus;

			[Statistics.Info("Primary Damage", description: "TODO: Desc", format: "{0:0}", comparison: Statistics.Comparison.Higher)]
			public float primary_damage_multiplier = 1.00f;

			[Statistics.Info("Secondary Damage", description: "TODO: Desc", format: "{0:0}", comparison: Statistics.Comparison.Higher)]
			public float secondary_damage_multiplier = 1.00f;

			[Statistics.Info("Terrain Damage", description: "Damage to terrain", format: "{0:0}", comparison: Statistics.Comparison.Higher)]
			public float terrain_damage_multiplier = 1.00f;

			[Statistics.Info("Charge Time", description: "How long you need to charge up before each attack", format: "{0:0.##}s", comparison: Statistics.Comparison.Lower)]
			public float charge_time;

			[Statistics.Info("Cooldown", description: "Time between attacking and charging up a new attack", format: "{0:0.##}s", comparison: Statistics.Comparison.Lower)]
			public float cooldown;

			[Statistics.Info("Reach", description: "Melee weapon range", format: "{0:0.##}", comparison: Statistics.Comparison.Higher)]
			public float max_distance;

			[Statistics.Info("Area of Effect", description: "Size of the affected area", format: "{0:0.##}", comparison: Statistics.Comparison.Higher)]
			public float aoe;

			[Statistics.Info("Thickness", description: "TODO: Desc", format: "{0:0.##}", comparison: Statistics.Comparison.Higher)]
			public float thickness = 0.30f;

			[Statistics.Info("Knockback", description: "Multiplies knockback of the damage", format: "{0:0.##}x", comparison: Statistics.Comparison.Higher)]
			public float knockback;

			[Statistics.Info("Yield", description: "Affects amount of material obtained from harvesting", format: "{0:P2}", comparison: Statistics.Comparison.Higher)]
			public float yield;

			[Statistics.Info("Penetration Falloff", description: "Modifies damage after each penetration", format: "{0:P2}", comparison: Statistics.Comparison.Lower)]
			public float penetration_falloff;

			[Statistics.Info("Penetration", description: "How many objects are hit in single strike", format: "{0:0}", comparison: Statistics.Comparison.Higher)]
			public int penetration;

			[Statistics.Info("Damage Type", description: "Type of damage dealt", format: "{0}", comparison: Statistics.Comparison.None)]
			public Damage.Type damage_type;

			[Statistics.Info("Category", description: "TODO: Desc", comparison: Statistics.Comparison.None)]
			public Melee.Category category;

			public Melee.Flags flags;

			public Physics.Layer hit_mask;
			
			[Statistics.Info("Auto attack", description: "Automatically attacks when fully charged up", format: "{0}", comparison: Statistics.Comparison.None)]
			public bool auto_attack = false;
		}

		[IComponent.Data(Net.SendType.Unreliable)]
		public struct State: IComponent
		{
			[Save.Ignore, Net.Ignore] public float next_hit;
			[Save.Ignore, Net.Ignore] public float last_hit;
			[Save.Ignore, Net.Ignore] public float start_charging;
		}

#if CLIENT
		[ISystem.Update(ISystem.Mode.Single)]
		public static void OnSpriteUpdate(ISystem.Info info, Entity entity, [Source.Owned] in Melee.Data melee, [Source.Owned] in Melee.State melee_state, [Source.Owned] ref Sprite.Renderer.Data renderer)
		{
			if (melee_state.start_charging != 0.00f)
			{
				float elapsed = (info.WorldTime - melee_state.start_charging);
				var scale = elapsed / melee.charge_time;
				scale = MathF.Min(1.00f, scale);
				
				Vector2 vec = new Vector2(-melee.swing_offset.X, melee.swing_offset.Y * 0.50f);
				renderer.offset = vec * scale * 0.50f;
				renderer.rotation = -melee.swing_rotation * scale * 0.50f;
			}
			else
			{
				var elapsed = info.WorldTime - melee_state.last_hit;
				var max = melee_state.next_hit - melee_state.last_hit;
				var alpha = 1.00f - Maths.Clamp(elapsed / (max * 0.80f), 0.00f, 1.00f);

				renderer.offset = melee.swing_offset * alpha * alpha;
				renderer.rotation = melee.swing_rotation * alpha * alpha;
			}
			
		}
#endif

		[ISystem.LateUpdate(ISystem.Mode.Single)]
		public static void Update(ISystem.Info info, Entity entity,
		[Source.Owned] in Melee.Data melee, [Source.Owned] ref Melee.State melee_state,
		[Source.Owned] in Transform.Data transform, [Source.Owned] in Control.Data control, [Source.Owned] in Body.Data body)
		{
			if (info.WorldTime >= melee_state.next_hit)
			if (control.mouse.GetKey(Mouse.Key.Left))
			{
				if (melee_state.start_charging == 0.00f)
				{
					melee_state.start_charging = info.WorldTime; //Start charging
				}
			}

			if (melee_state.start_charging != 0.00f)
			{
				if (!control.mouse.GetKey(Mouse.Key.Left) && info.WorldTime - melee_state.start_charging < melee.charge_time)
				{
					melee_state.start_charging = 0.00f; //Interrupt charging
				}
			}

			if (melee_state.start_charging != 0.00f)
			if (info.WorldTime - melee_state.start_charging >= melee.charge_time)
			if (!control.mouse.GetKey(Mouse.Key.Left) || melee.auto_attack || melee.charge_time + melee.cooldown <= 0.30f)
			{
				//TODO: should not attack if simply dropping or loosing grip on the while its fully charged
				melee_state.start_charging = 0.00f; //Actually attack
				var random = XorRandom.New();
				ref var region = ref info.GetRegion();

				melee_state.last_hit = info.WorldTime;
				melee_state.next_hit = info.WorldTime + melee.cooldown;

				var dir = (control.mouse.position - transform.position).GetNormalized(out var len);
				len = MathF.Min(len, melee.max_distance);

#if CLIENT
				Sound.Play(melee.sound_swing, transform.position, volume: melee.sound_volume, random.NextFloatRange(0.90f, 1.10f) * melee.sound_pitch, size: melee.sound_size);
#endif

				Span<LinecastResult> hits = stackalloc LinecastResult[16];
				if (region.TryLinecastAll(transform.position, transform.position + (dir * len), melee.thickness, ref hits, mask: melee.hit_mask))
				{
					var parent = body.GetParent();

					var damage_base = melee.damage_base;
					var damage_bonus = random.NextFloatRange(0.00f, melee.damage_bonus);
					var damage = damage_base + damage_bonus;

					var modifier = 1.00f;
					var flags = Damage.Flags.None;

					var penetration = melee.penetration;

					var hit_terrain = false;

					for (var i = 0; i < hits.Length && penetration >= 0; i++)
					{
						ref var hit = ref hits[i];
						if (hit.entity == parent || hit.entity_parent == parent || hit.entity == entity) continue;
						var is_terrain = !hit.entity.IsValid();

						if (is_terrain)
						{
							if (hit_terrain) continue;
							hit_terrain = true;
						}

#if CLIENT
						var shake_mult = Maths.Clamp(melee.knockback, 0.00f, 1.00f);
						Camera.Shake(ref region, transform.position, 0.40f * shake_mult, 0.40f * shake_mult, radius: 2.00f);
#endif

#if SERVER
						Damage.Hit(entity, parent, hit.entity, hit.world_position, dir, -dir, damage * modifier, hit.material_type, melee.damage_type, knockback: melee.knockback, size: melee.aoe, flags: flags, yield: melee.yield, primary_damage_multiplier: melee.primary_damage_multiplier, secondary_damage_multiplier: melee.secondary_damage_multiplier, terrain_damage_multiplier: melee.terrain_damage_multiplier);
#endif

						flags |= Damage.Flags.No_Sound;
						modifier *= melee.penetration_falloff;
						penetration--;
					}
				}
			}
		}
	}
}
