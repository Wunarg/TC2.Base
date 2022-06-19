using TC2.Base.Components;

namespace TC2.Base
{
	public sealed partial class ModInstance
	{
		private static void RegisterOrganicAugments(ref List<Augment.Definition> definitions)
		{
			definitions.Add(Augment.Definition.New<Organic.Data>
			(
				identifier: "organic.healfromdeath",
				category: "Organic",
				name: "Reanimatable Flesh",
				description: "Will come back to live if healed enough",
			
				can_add: static (ref Augment.Context context, in Organic.Data data, ref Augment.Handle handle, Span<Augment.Handle> Augments) =>
				{
					return context.GetComponent<HealFromDeath.Data>().IsNull();
				},

				apply_0: static (ref Augment.Context context, ref Organic.Data data, ref Augment.Handle handle, Span<Augment.Handle> Augments) =>
				{
					ref var healFromDeath = ref context.GetOrAddComponent<HealFromDeath.Data>();
					context.requirements_new.Add(Crafting.Requirement.Resource("salt.life", 15.00f));
				}
			));

			definitions.Add(Augment.Definition.New<Organic.Data>
			(
				identifier: "organic.rapidbiorhythem",
				category: "Protection",
				name: "Rapid Biorhythem",
				description: "Heals over time but slowly ages loosing max health",
			
				can_add: static (ref Augment.Context context, in Organic.Data data, ref Augment.Handle handle, Span<Augment.Handle> Augments) =>
				{
					return context.GetComponent<RapidBiorhythem.Data>().IsNull();
				},

				apply_0: static (ref Augment.Context context, ref Organic.Data data, ref Augment.Handle handle, Span<Augment.Handle> Augments) =>
				{
					ref var rapidBiorhythem = ref context.GetOrAddComponent<RapidBiorhythem.Data>();
					context.requirements_new.Add(Crafting.Requirement.Resource("salt.life", 35.00f));
				}
			));

			//Commented out until order salt exists
			//definitions.Add(Augment.Definition.New<RapidBiorhythem.Data>
			//(
			//	identifier: "rapidbiorhythem.stabelized_aging",
			//	category: "Organic",
			//	name: "Stabelized Aging",
			//	description: "Ages slower",
			//
			//	apply_0: static (ref Augment.Context context, ref RapidBiorhythem.Data data, ref Augment.Handle handle, Span<Augment.Handle> Augments) =>
			//	{
			//		data.age_per_tick -= Maths.Clamp(0.01f, 0.00f, data.age_per_tick);
			//		//if you really want to use 5 slots and a crap ton of order then you can stabelize aging
			//	},
			//
			//	apply_1: static (ref Augment.Context context, ref RapidBiorhythem.Data data, ref Augment.Handle handle, Span<Augment.Handle> Augments) =>
			//	{
			//		context.requirements_new.Add(Crafting.Requirement.Resource("salt.order", 20.00f));
			//	}
			//
			//));

			definitions.Add(Augment.Definition.New<Organic.Data>
			(
				identifier: "organic.chitin_exoskeleton",
				category: "Protection",
				name: "Chitin Exoskeleton",
				description: "More durable and armored",

				can_add: static (ref Augment.Context context, in Organic.Data data, ref Augment.Handle handle, Span<Augment.Handle> Augments) =>
				{
					return !context.GetComponent<Health.Data>().IsNull() && !Augments.HasAugment(handle);
				},

				apply_0: static (ref Augment.Context context, ref Organic.Data data, ref Augment.Handle handle, Span<Augment.Handle> Augments) =>
				{
					ref var health = ref context.GetComponent<Health.Data>();
					if (!health.IsNull())
					{
						health.max *= 1.20f;
					}
					ref var armor = ref context.GetOrAddComponent<Armor.Data>();
					if (!armor.IsNull())
					{
						armor.material_type = Material.Type.Metal;
						armor.toughness += 50.00f;
					}
				},

				apply_1: static (ref Augment.Context context, ref Organic.Data data, ref Augment.Handle handle, Span<Augment.Handle> Augments) =>
				{
					context.requirements_new.Add(Crafting.Requirement.Resource("chitin", 20.00f));
				}
			));
		}
	}
}

