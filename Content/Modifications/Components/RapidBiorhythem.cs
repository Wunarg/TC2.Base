
namespace TC2.Base.Components
{
	public static partial class RapidBiorhythem
	{

		[IComponent.Data(Net.SendType.Reliable)]
		public partial struct Data: IComponent
		{
			[Statistics.Info("Primary Regen", description: "Regenerates this much health per tick", format: "{0:0.##}", comparison: Statistics.Comparison.Higher)]
			public float heal_amount_primary = 0.5f;

			[Statistics.Info("Secondary Regen", description: "Regenerates this much health per tick", format: "{0:0.##}", comparison: Statistics.Comparison.Higher)]
			public float heal_amount_secondary = 1f;

			[Statistics.Info("Aging rate", description: "Looses this much max health per tick", format: "{0:0.##}", comparison: Statistics.Comparison.Higher)]
			public float age_per_tick = 0.05f;
		}

		[ISystem.EarlyUpdate(ISystem.Mode.Single)]
		public static void OnModified(ISystem.Info info, Entity entity, [Source.Owned] in RapidBiorhythem.Data data, [Source.Owned] ref Health.Data health)
		{
			if (health.max > data.age_per_tick)
			{
				health.max -= data.age_per_tick;

				if (health.primary < 1.0f)
				{
					var heal_normalized = Maths.Normalize(data.heal_amount_primary * 0.25f, health.max);				
					var heal_amount = MathF.Min(Maths.Clamp(1.00f - health.primary, 0.00f, 1.00f), heal_normalized);
					health.primary += heal_amount;
					entity.MarkModified<Health.Data>(sync: true);
				}

				if (health.secondary < 1.0f)
				{
					var heal_normalized = Maths.Normalize(data.heal_amount_secondary * 0.25f, health.max);				
					var heal_amount = MathF.Min(Maths.Clamp(1.00f - health.secondary, 0.00f, 1.00f), heal_normalized);
					health.secondary += heal_amount;
					entity.MarkModified<Health.Data>(sync: true);
				}
			}
		}
	}
}
