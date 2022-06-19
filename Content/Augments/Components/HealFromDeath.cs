
namespace TC2.Base.Components
{
	public static partial class HealFromDeath
	{

		[IComponent.Data(Net.SendType.Reliable)]
		public partial struct Data: IComponent
		{

		}

		[ISystem.Modified<Health.Data>(ISystem.Mode.Single)]
		[Exclude<Player.Data>(Source.Modifier.Shared)]
		[HasTag("dead", true, Source.Modifier.Owned)]
		public static void OnModified(ISystem.Info info, Entity entity,
		[Source.Owned] ref Organic.Data orgData, [Source.Owned] in HealFromDeath.Data data, 
		[Source.Owned] ref Organic.State orgState, [Source.Owned] in Health.Data health)
		{
			if (health.integrity > 0.30f && health.durability > 0.30f)
			{
				//orgState.consciousness_shared_new += 0.5f;
				orgState.consciousness_shared += 0.5f;
				entity.SyncComponent<Organic.State>(ref orgState);
			}
		}
	}
}
