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
				category: "Organic",
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
		}
	}
}

