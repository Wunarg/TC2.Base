﻿
namespace TC2.Base.Components
{
	public static partial class HeadBob
	{
		[IComponent.Data(Net.SendType.Unreliable)]
		public partial struct Data: IComponent
		{
			public Vector2 offset;
			public Vector2 multiplier;
			public float speed;
		}
	}

	public static partial class Head
	{
		[IComponent.Data(Net.SendType.Reliable)]
		public partial struct Data: IComponent
		{
			public float voice_pitch = 1.00f;

			public Sound.Handle sound_death = default;
			public Sound.Handle sound_hit = default;
			public Sound.Handle sound_pain = default;
			public Sound.Handle sound_attack = default;

			[Editor.Picker.Position(true, true)]
			public Vector2 offset_mouth;

			public byte frame_pain = default;
			public byte frame_dead = default;

			[Save.Ignore, Net.Ignore] public float next_sound = default;

			public Data()
			{

			}
		}

		[ISystem.VeryLateUpdate(ISystem.Mode.Single, interval: 0.20f), HasTag("dead", false, Source.Modifier.Owned)]
		public static void UpdateSprite(ISystem.Info info, [Source.Owned] ref Organic.State organic_state, [Source.Owned] in Head.Data head, [Source.Owned] ref Animated.Renderer.Data renderer)
		{
			renderer.sprite.frame.X = organic_state.pain_shared > 200.00f ? head.frame_pain : 0u;
		}

		[ISystem.AddFirst(ISystem.Mode.Single), HasTag("dead", true, Source.Modifier.Owned)]
		public static void OnDeath(ISystem.Info info, [Source.Owned] in Transform.Data transform, [Source.Owned] in Head.Data head, [Source.Owned] ref Animated.Renderer.Data renderer)
		{
			renderer.sprite.frame.X = head.frame_dead;

#if SERVER
			WorldNotification.Push(ref info.GetRegion(), "* Dies *", 0xffff0000, transform.position, lifetime: 1.00f);
#endif
		}

#if SERVER
		//[ISystem.Add(ISystem.Mode.Single), HasTag("dead", true, Source.Modifier.Owned)]
		//public static void OnDeath(ISystem.Info info, [Source.Owned] in Transform.Data transform, [Source.Owned] in Head.Data head)
		//{
		//	Sound.Play(ref info.GetRegion(), head.sound_death, transform.position);
		//}

		[ISystem.VeryLateUpdate(ISystem.Mode.Single), HasTag("dead", false, Source.Modifier.Owned)]
		public static void UpdateGroan(ISystem.Info info, [Source.Owned, Override] in Organic.Data organic, [Source.Owned] ref Organic.State organic_state, [Source.Owned] ref Head.Data head, [Source.Owned] in Transform.Data transform)
		{
			//if (info.WorldTime > head.next_sound)
			//{
			//	//App.WriteLine(organic_state.pain_shared);
			//	if (organic_state.pain_shared > 500.00f && organic_state.consciousness_shared > 0.30f)
			//	{
			//		ref var region = ref info.GetRegion();
			//		var random = XorRandom.New();
			//		Sound.Play(ref region, head.sound_pain, transform.position, volume: 0.20f + Maths.Clamp(organic_state.pain_shared / 2000.00f, 0.20f, 0.50f), pitch: random.NextFloatRange(0.70f, 1.10f) * (1.00f + Maths.Clamp((organic_state.pain_shared_new / 2000.00f), 0.00f, 0.10f)) * head.voice_pitch);
			//		head.next_sound = info.WorldTime + (Maths.Clamp(10.00f - (organic_state.pain_shared / 300.00f), 5.00f, 10.00f) * random.NextFloatRange(0.80f, 1.30f));
			//	}
			//}
		}
#endif

		[ISystem.Update(ISystem.Mode.Single), HasTag("dead", false, Source.Modifier.Owned)]
		public static void UpdateNoRotate(ISystem.Info info, [Source.Owned, Override] in Organic.Data organic, [Source.Owned] in Organic.State organic_state, [Source.Owned, Override] ref NoRotate.Data no_rotate, [Source.Owned] in Head.Data head)
		{
			no_rotate.multiplier = MathF.Round(organic_state.consciousness_shared * organic_state.efficiency * Maths.Lerp(0.20f, 1.00f, organic.motorics * organic.motorics)) * organic.coordination * organic.motorics;
			no_rotate.speed *= Maths.Lerp(0.90f, 1.00f, organic.motorics);
			no_rotate.bias += (1.00f - organic.motorics.Clamp01()) * 0.05f;
		}

		[ISystem.RemoveLast(ISystem.Mode.Single)]
		public static void OnRemoveHead([Source.Parent, Override] ref Organic.Data organic, [Source.Parent] ref Organic.State organic_state, [Source.Owned] in Head.Data head, [Source.Parent] in Joint.Base joint)
		{
			if (joint.flags.HasAll(Joint.Flags.Organic))
			{
				organic_state.consciousness_shared = 0.00f;
				organic_state.motorics_shared = 0.00f;
			}
		}

#if CLIENT
		[ISystem.Update(ISystem.Mode.Single)]
		public static void UpdateOffset(ISystem.Info info, [Source.Parent] in HeadBob.Data headbob, [Source.Owned] ref Animated.Renderer.Data renderer, [Source.Owned] in Head.Data head)
		{
			renderer.offset = headbob.offset;
		}

		//[ISystem.Update(ISystem.Mode.Single)]
		//public static void UpdateOffsetTrait(ISystem.Info info, [Source.Parent] in HeadBob.Data headbob, [Source.Owned, Pair.All] ref Animated.Renderer.Data renderer, [Source.Owned] in Head.Data head)
		//{
		//	//App.WriteLine($"{info.WorldTime}");
		//	renderer.offset = headbob.offset;
		//}

		[ISystem.Update(ISystem.Mode.Single)]
		public static void UpdateOffsetHair(ISystem.Info info, [Source.Parent] in HeadBob.Data headbob, [Source.Owned, Pair.Of<Hair.Data>] ref Animated.Renderer.Data renderer, [Source.Owned] in Head.Data head)
		{
			//App.WriteLine($"{info.WorldTime}");
			renderer.offset = headbob.offset;
		}

		[ISystem.Update(ISystem.Mode.Single)]
		public static void UpdateOffsetBeard(ISystem.Info info, [Source.Parent] in HeadBob.Data headbob, [Source.Owned, Pair.Of<Beard.Data>] ref Animated.Renderer.Data renderer, [Source.Owned] in Head.Data head)
		{
			//App.WriteLine($"{info.WorldTime}");
			renderer.offset = headbob.offset;
		}
#endif
	}
}