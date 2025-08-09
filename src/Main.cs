#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using XRL.World;
using XRL.World.Parts.Skill;

namespace UnlimitedCampfires
{
	[HarmonyPatch(typeof(Survival_Camp))]
	[HarmonyPatch(nameof(Survival_Camp.AttemptCamp))]
	[HarmonyPatch(new Type[] { typeof(GameObject) })]
	public class Survival_Camp_AttemptCamp
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var getPointsOfInterest = new CodeMatch[] {
				new(OpCodes.Ldarg_1),
				new(OpCodes.Ldstr, "PlayerCampfire"),
				new(OpCodes.Ldnull),
				new(OpCodes.Call, AccessTools.Method(
					type: typeof(GetPointsOfInterestEvent),
					name: nameof(GetPointsOfInterestEvent.GetOne),
					parameters: new Type[] { typeof(GameObject), typeof(string), typeof(Zone) }
				)),
			};
			var matcher = new CodeMatcher(instructions)
				.MatchStartForward(getPointsOfInterest);
			if (matcher.IsValid) {
				var labels = matcher.Labels;
				return matcher
					.RemoveInstructions(getPointsOfInterest.Length)
					.Insert(new CodeInstruction(OpCodes.Ldnull) {
						labels = labels,
					})
					.Instructions();
			} else {
				Logger.buildLog.Error("Failed");
				return instructions;
			}
		}
	}
}
