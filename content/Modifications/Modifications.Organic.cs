using TC2.Base.Components;

namespace TC2.Base
{
	public sealed partial class ModInstance
	{
		private static void RegisterOrganicModifications(ref List<Modification.Definition> definitions)
		{
			definitions.Add(Modification.Definition.New<Organic.Data>
			(
				identifier: "organic.healfromdeath",
				category: "Organic",
				name: "Reanimatable Flesh",
				description: "Will come back to live if healed enough",
			
				can_add: static (ref Modification.Context context, in Organic.Data data, ref Modification.Handle handle, Span<Modification.Handle> modifications) =>
				{
					return context.GetComponent<HealFromDeath.Data>().IsNull();
				},

				apply_0: static (ref Modification.Context context, ref Organic.Data data, ref Modification.Handle handle, Span<Modification.Handle> modifications) =>
				{
					ref var healFromDeath = ref context.GetOrAddComponent<HealFromDeath.Data>();
					context.requirements_new.Add(Crafting.Requirement.Resource("salt.life", 15.00f));
				}
			));

			definitions.Add(Modification.Definition.New<Organic.Data>
			(
				identifier: "organic.rapidbiorhythem",
				category: "Protection",
				name: "Rapid Biorhythem",
				description: "Heals over time but slowly ages loosing max health",
			
				can_add: static (ref Modification.Context context, in Organic.Data data, ref Modification.Handle handle, Span<Modification.Handle> modifications) =>
				{
					return context.GetComponent<RapidBiorhythem.Data>().IsNull();
				},

				apply_0: static (ref Modification.Context context, ref Organic.Data data, ref Modification.Handle handle, Span<Modification.Handle> modifications) =>
				{
					ref var rapidBiorhythem = ref context.GetOrAddComponent<RapidBiorhythem.Data>();
					context.requirements_new.Add(Crafting.Requirement.Resource("salt.life", 35.00f));
				}
			));

			//Commented out until order salt exists
			//definitions.Add(Modification.Definition.New<RapidBiorhythem.Data>
			//(
			//	identifier: "rapidbiorhythem.stabelized_aging",
			//	category: "Organic",
			//	name: "Stabelized Aging",
			//	description: "Ages slower",
			//
			//	apply_0: static (ref Modification.Context context, ref RapidBiorhythem.Data data, ref Modification.Handle handle, Span<Modification.Handle> modifications) =>
			//	{
			//		data.age_per_tick -= Maths.Clamp(0.01f, 0.00f, data.age_per_tick);
			//		//if you really want to use 5 slots and a crap ton of order then you can stabelize aging
			//	},
			//
			//	apply_1: static (ref Modification.Context context, ref RapidBiorhythem.Data data, ref Modification.Handle handle, Span<Modification.Handle> modifications) =>
			//	{
			//		context.requirements_new.Add(Crafting.Requirement.Resource("salt.order", 20.00f));
			//	}
			//
			//));

			definitions.Add(Modification.Definition.New<Organic.Data>
			(
				identifier: "organic.chitin_exoskeleton",
				category: "Protection",
				name: "Chitin Exoskeleton",
				description: "More durable and armored",

				can_add: static (ref Modification.Context context, in Organic.Data data, ref Modification.Handle handle, Span<Modification.Handle> modifications) =>
				{
					return !context.GetComponent<Health.Data>().IsNull() && !modifications.HasModification(handle);
				},

				apply_0: static (ref Modification.Context context, ref Organic.Data data, ref Modification.Handle handle, Span<Modification.Handle> modifications) =>
				{
					ref var health = ref context.GetComponent<Health.Data>();
					if (!health.IsNull())
					{
						health.armor += 20;
						health.max *= 1.20f;
					}
				},

				apply_1: static (ref Modification.Context context, ref Organic.Data data, ref Modification.Handle handle, Span<Modification.Handle> modifications) =>
				{
					context.requirements_new.Add(Crafting.Requirement.Resource("chitin", 20.00f));
				}
			));
		}
	}
}

