using System;
using System.Collections.Generic;
using UnityEngine;

public static class Skills
{
	public static Guid GetSkillId(Type type)
	{
		return Skills.BaseSkillAttribute.GetId(type);
	}

	public abstract class Core : Skills.BaseSkillAttribute
	{
	}

	public abstract class FishingTools : Skills.BaseSkillAttribute
	{
	}

	public abstract class FishGroup : Skills.BaseSkillAttribute
	{
	}

	public abstract class General : Skills.BaseSkillAttribute
	{
	}

	public abstract class Spawner : Skills.BaseSkillAttribute
	{
	}

	public abstract class ActiveSkill : Skills.BaseSkillAttribute
	{
	}

	public abstract class CostReduction : Skills.BaseSkillAttribute
	{
		public Skill SetCostReduction(float percent)
		{
			this.skillForReducedCost.SetCostReduction(percent);
			return this.skillForReducedCost;
		}

		[SerializeField]
		private Skill skillForReducedCost;
	}

	public abstract class PowerUp : Skills.ActiveSkill
	{
	}

	public abstract class Boost : Skills.ActiveSkill
	{
	}

	[Skills.IdAttribute("E72C5D9A-CCCC-426D-837C-BD63037C65AC")]
	public class None : Skills.BaseSkillAttribute
	{
	}

	[Skills.IdAttribute("EEC7FD5A-B495-409E-81F0-9C30A1E26D29")]
	public class SkillTier : Skills.Core
	{
	}

	[Skills.IdAttribute("588DC081-3A9A-487E-A26C-3F4222416C02")]
	public class DeepWaterTier : Skills.Core
	{
		public List<Color> WaterColors = new List<Color>();
	}

	[Skills.IdAttribute("37E70C8C-AEEE-43EF-BA46-511967D8B887")]
	public class PrestigeTier : Skills.Core
	{
	}

	[Skills.IdAttribute("37E70C8C-AEEE-43EF-BA46-511967D8B889")]
	public class Quests : Skills.Core
	{
	}

	[Skills.IdAttribute("797A05C7-F303-418F-B0E6-676933F795D1")]
	public class Quest_FishCount : Skills.Quests
	{
	}

	[Skills.IdAttribute("797A05C7-F303-418F-B0E6-676933F795DC")]
	public class Rod_FishTriesPerSecond : Skills.FishingTools
	{
	}

	[Skills.IdAttribute("95C432F7-1628-430C-993D-9077F8D792C8")]
	public class Rod_CatchChance : Skills.FishingTools
	{
	}

	[Skills.IdAttribute("B24B32E7-618E-45E8-9C02-F80AA97CF897")]
	public class Rod_ChanceForRareFish : Skills.FishingTools
	{
	}

	[Skills.IdAttribute("53C5EAC4-5561-485C-B638-956F5E625723")]
	public class Rod_ChanceForBiggerFish : Skills.FishingTools
	{
	}

	[Skills.IdAttribute("2D7548F2-7499-4B9E-B9B0-3E9023C64C38")]
	public class MoveableNets : Skills.FishingTools
	{
	}

	[Skills.IdAttribute("11A2B6D5-F0AE-48C3-867D-43514541FBE0")]
	public class ClaimLobsterAfterHours : Skills.FishingTools
	{
	}

	[Skills.IdAttribute("74F9513E-D0A5-4CB5-BE2A-45F15AC64082")]
	public class LobsterValue : Skills.FishingTools
	{
	}

	[Skills.IdAttribute("A4F9513E-D1AA-CCB5-BEEA-45F11ACC4082")]
	public class CrabValue : Skills.FishingTools
	{
	}

	[Skills.IdAttribute("EE5A631A-4F19-4682-B2B1-664734585DA9")]
	public class BossDamageModifier : Skills.General
	{
	}

	[Skills.IdAttribute("F0A660D8-E017-4A48-95F1-2D302E7CAF8A")]
	public class BoatCostReduction : Skills.CostReduction
	{
	}

	[Skills.IdAttribute("1AC2BA8F-1081-4D9A-AFBD-010BC922CAE1")]
	public class DeepWaterCostReduction : Skills.CostReduction
	{
	}

	[Skills.IdAttribute("EE5A631A-4F09-4685-B6B1-464734575DA9")]
	public class FishValue : Skills.General
	{
	}

	[Skills.IdAttribute("7F75FBEA-C51B-4AC9-9559-33E453FDE7B6")]
	public class MinimumFishWeightIncrease : Skills.General
	{
	}

	[Skills.IdAttribute("8D3EEE3B-CAD2-49BD-A555-85B59F798A97")]
	public class ShoalAfterSeconds : Skills.General
	{
	}

	[Skills.IdAttribute("F170AAD3-1FCB-488C-A5E1-5DA400F4BB04")]
	public class ShoalAmount : Skills.General
	{
	}

	[Skills.IdAttribute("A4782E00-071E-4791-8B2A-712C5B37F98E")]
	public class ShoalSpawnChance : Skills.General
	{
	}

	[Skills.IdAttribute("19A7824F-6D61-4F77-9899-9F41238EA15D")]
	public class RareAfterSeconds : Skills.General
	{
	}

	[Skills.IdAttribute("AE3FC716-FFE9-4049-AA31-6DEA10D4106B")]
	public class RareSpawnChance : Skills.General
	{
	}

	[Skills.IdAttribute("D753F421-7E10-46FE-8B33-9200BCC0FDBD")]
	public class CapacityAfkHours : Skills.General
	{
	}

	[Skills.IdAttribute("2133A4C0-138C-455A-B3BE-4A1102B4E3D0")]
	public class CollectedStars : Skills.General
	{
	}

	[Skills.IdAttribute("CB2F560E-8674-45F6-B12A-71CC6F56D0CC")]
	public class AfkCollectedValueIncrease : Skills.General
	{
	}

	[Skills.IdAttribute("CDE722DD-F634-41A2-95AB-EF10D0CFE45E")]
	public class PackageValue : Skills.General
	{
	}

	[Skills.IdAttribute("9ECADFF9-FB03-4FF9-8C2E-DC5DA59DE325")]
	public class PackageMultiplier : Skills.General
	{
	}

	[Skills.IdAttribute("21CADF39-3B03-4FC9-5C2E-6C5D153DE354")]
	public class FreeSpins : Skills.General
	{
	}

	[Skills.IdAttribute("FEE2AA50-1189-45BB-856B-19878026BB34")]
	public class PassiveCashGain : Skills.General
	{
	}

	[Skills.IdAttribute("56D83C92-1BDA-4361-B496-045CB9930B5A")]
	public class FishValueMultiplier : Skills.General
	{
	}

	[Skills.IdAttribute("5011011B-5361-449D-8D34-3AED836A500C")]
	public class FishingExperienceGain : Skills.General
	{
	}

	[Skills.IdAttribute("1241011B-B361-A4C2-8D00-312383605009")]
	public class FishingExperienceGainMultiplier : Skills.General
	{
	}

	[Skills.IdAttribute("5341011B-5A61-449D-8D34-3AEA836D5023")]
	public class AllIncome : Skills.General
	{
	}

	[Skills.IdAttribute("1321011B-1161-4A2D-8DA4-3AED8A6AAAAC")]
	public class InGameFishValueModifier : Skills.General
	{
	}

	[Skills.IdAttribute("23A6FE67-4B49-4B06-8D28-BCA61A41D8BD")]
	public class CriticalStrikeFishValueChance : Skills.General
	{
	}

	[Skills.IdAttribute("C2FB2C74-4709-4F06-BDB4-DDD0DE6203DF")]
	public class CriticalStrikeFishValueMultiplier : Skills.General
	{
	}

	[Skills.IdAttribute("AC5BFF00-75D7-4BB2-8577-765C9B135680")]
	public class CriticalStrikeBossDamageChance : Skills.General
	{
	}

	[Skills.IdAttribute("CA55B195-46A5-4ACC-A1D6-030A1E0137A9")]
	public class CriticalStrikeBossDamageMultiplier : Skills.General
	{
	}

	[Skills.IdAttribute("40CC7C57-7860-4AFE-9D9F-B067EA5544E0")]
	public class PackageSpawnModifier : Skills.General
	{
	}

	[Skills.IdAttribute("BF029969-5299-4E51-A360-E9F958B67166")]
	public class BirdCatchAbility : Skills.General
	{
	}

	[Skills.IdAttribute("FF739C43-146F-4882-9C31-726AC7577C1F")]
	public class BirdValueModifier : Skills.General
	{
	}

	[Skills.IdAttribute("74B2D366-9CFB-449B-9E00-11C7D39FAE45")]
	public class SpawnFishOnSwipeChance : Skills.General
	{
	}

	[Skills.IdAttribute("EC2C2344-ED8A-4FF0-9314-31ED548BF30A")]
	public class FishValueSwipeMultiplier : Skills.General
	{
	}

	[Skills.IdAttribute("2F83A084-F402-4EC5-B872-3DE0A740F5CE")]
	public class FishValueBoatMultiplier : Skills.General
	{
	}

	[Skills.IdAttribute("B4825006-97E0-45CE-B055-18AAEDA22F98")]
	public class SpawnPinkyChance : Skills.General
	{
	}

	[Skills.IdAttribute("284518E1-1D6B-CAAA-3122-1E5B12D2B6AB")]
	public class SpawnWinterChance : Skills.General
	{
	}

	[Skills.IdAttribute("384518E5-7D6A-4AA9-8125-6E5BB2DCB6AA")]
	public class SpawnPinkyAfterSeconds : Skills.General
	{
	}

	[Skills.IdAttribute("08A44583-F27A-403F-ACF4-6C4DE5544023")]
	public class SpawnCrabAfterSeconds : Skills.General
	{
	}

	[Skills.IdAttribute("19EB5812-4660-41C4-B3F0-76F85EF3245E")]
	public class DoubleSurfaceFishChance : Skills.General
	{
	}

	[Skills.IdAttribute("5BF87DC4-54EE-41AA-877B-F0B081693582")]
	public class TierSkillsPriceReduction : Skills.General
	{
	}

	[Skills.IdAttribute("3EAA4C73-FA2C-4F35-928A-B827A712B6F3")]
	public class SpawnFishValueFishChance : Skills.General
	{
	}

	[Skills.IdAttribute("57F11BC1-AC98-41F6-9C9D-94DD9A9CC054")]
	public class FishValueFishMultiplier : Skills.General
	{
	}

	[Skills.IdAttribute("B7F1BBC1-AC98-4FF4-9999-94119A9BC054")]
	public class CargoModifier : Skills.General
	{
	}

	[Skills.IdAttribute("56D83C92-1BDA-4361-B496-045CB9931B5C")]
	public class HolidayFishValueMultiplier : Skills.General
	{
	}

	[Skills.IdAttribute("26D83C41-2BDC-B36C-A497-245CB9931B51")]
	public class WinterFishValueMultiplier : Skills.General
	{
	}

	[Skills.IdAttribute("A4825006-97A0-45CE-B055-17AAEDA22F98")]
	public class SpawnHeartFishChance : Skills.General
	{
	}

	[Skills.IdAttribute("676EFE32-B080-4A12-AB79-057524841831")]
	public class HeartFishValueMultiplier : Skills.General
	{
	}

	[Skills.IdAttribute("F7AA72EE-7028-409A-804D-A4441DE3D841")]
	public class RushTimeBonus : Skills.General
	{
	}

	[Skills.IdAttribute("B7999F83-A04A-4211-9E65-2B1304EC65F5")]
	public class GoldFishBonus : Skills.General
	{
	}

	[Skills.IdAttribute("119C2945-2E2E-493A-866E-6658163EA84D")]
	public class WheelJackpotAmountBonus : Skills.General
	{
	}

	[Skills.IdAttribute("5C3A499E-C1AA-4490-A8F1-C5D1EBFF7D5A")]
	public class WheelHighestMultiplierBonus : Skills.General
	{
	}

	[Skills.IdAttribute("9894E72E-7C08-4BFF-BC8A-73617B3C9F82")]
	public class WheelHighestMultiplierChanceBonus : Skills.General
	{
	}

	[Skills.IdAttribute("08EE98C0-15C6-4D97-A77B-AE2D8D4DCE69")]
	public class WheelJackpotChanceBonus : Skills.General
	{
	}

	[Skills.IdAttribute("074E98C0-A0C6-4D97-867B-662D8D4DCE5A")]
	public class IdleDoubleUpMultiplierBonus : Skills.General
	{
	}

	[Skills.IdAttribute("2CFD7A62-8E92-46CE-B8D0-336443F66CB3")]
	public class GiantismChance : Skills.General
	{
	}

	[Skills.IdAttribute("B7B9F520-398F-476C-B862-456F7D63CD04")]
	public class GiantismValueMultiplier : Skills.General
	{
	}

	[Skills.IdAttribute("EA4D1C3D-1B88-4E2B-8297-C0C0F3DC48F2")]
	public class StimValueMultiplierMax : Skills.General
	{
	}

	[Skills.IdAttribute("F24A1F08-87D6-4FC7-9852-4094512F5B0A")]
	public class StimValueMultiplierRate : Skills.General
	{
	}

	[Skills.IdAttribute("20498B13-353D-4121-9B52-A86E1852DCB2")]
	public class PackageAdMultiplier : Skills.General
	{
	}

	[Skills.IdAttribute("D660148F-0D8F-451A-A36E-BB8F81F75D82")]
	public class FireworkAmericanoFishChance : Skills.General
	{
	}

	[Skills.IdAttribute("FB11163F-4FA1-4909-8FDA-2C9924232FFB")]
	public class FireworkFishAmount : Skills.General
	{
	}

	[Skills.IdAttribute("2D04749E-5D29-4F01-82FD-D9D93C92BC76")]
	public class FireworkBossDamageMultiplier : Skills.General
	{
	}

	[Skills.IdAttribute("0EE46279-9B9E-4FF3-908A-17D56835198B")]
	public class FireworkMaxAmount : Skills.General
	{
	}

	[Skills.IdAttribute("21639AAE-FCDD-4EE4-A40B-141BFE3F21FD")]
	public class AmericanoCurrentDWFishMultiplier : Skills.General
	{
	}

	[Skills.IdAttribute("887BAB8B-8F57-4645-A1B9-9D70A4E8E70D")]
	public class CommonFishPerSecond : Skills.Spawner
	{
	}

	[Skills.IdAttribute("063604B5-0D9E-4896-9E4A-3D7234CF2CBC")]
	public class ShoalPowerUp : Skills.PowerUp
	{
	}

	[Skills.IdAttribute("9BC15D5C-5635-4DB7-BF70-935BBAAD57B5")]
	public class AutoFishPowerUp : Skills.PowerUp
	{
	}

	[Skills.IdAttribute("D1F5FBAE-C4B0-4765-97E8-75BE210E1A48")]
	public class BoostGoldFishing : Skills.Boost
	{
	}

	[Skills.IdAttribute("77462121-544D-4DB4-85D8-5F0853E7A8C6")]
	public class BoostRushTime : Skills.Boost
	{
	}

	[Skills.IdAttribute("E5CFBA5B-1450-4BB0-84BC-1F0E8D71C997")]
	public class BoostSpawnBossFish : Skills.Boost
	{
	}

	[Skills.IdAttribute("95CD7029-2DC7-4090-AAC1-0A3A539BF4CA")]
	public class BoostDailyGift : Skills.Boost
	{
	}

	[Skills.IdAttribute("E5CD7029-2AC7-4090-AAC1-0A3A539BF4CC")]
	public class HeartFishing : Skills.PowerUp
	{
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class IdAttribute : Attribute
	{
		public IdAttribute(string idAsString)
		{
			this.id = new Guid(idAsString);
		}

		public Guid Id
		{
			get
			{
				return this.id;
			}
		}

		private Guid id = default(Guid);
	}

	[Serializable]
	public abstract class BaseSkillAttribute : ISkillAttribute
	{
		public Guid GetId()
		{
			return Skills.BaseSkillAttribute.GetId(base.GetType());
		}

		public static Guid GetId(Type type)
		{
			Guid guid = Guid.Empty;
			if (!Skills.BaseSkillAttribute.cachedIds.TryGetValue(type, out guid))
			{
				Skills.IdAttribute idAttribute = (Skills.IdAttribute)Attribute.GetCustomAttribute(type, typeof(Skills.IdAttribute));
				if (idAttribute == null)
				{
					throw new InvalidOperationException("The 'Id' attribute is missing for Type: '" + type + "'.");
				}
				guid = idAttribute.Id;
				Skills.BaseSkillAttribute.cachedIds.Add(type, guid);
			}
			return guid;
		}

		private static Dictionary<Type, Guid> cachedIds = new Dictionary<Type, Guid>();
	}
}
