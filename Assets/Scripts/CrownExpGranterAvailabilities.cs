using System;

public class CrownExpGranterAvailabilities
{
	public CrownExpGranterAvailabilities(CrownExpGranterManager crownExpGranterManager, SkillTreeManager skillTreeManager, ResourceManager resourceManager, TournamentManager tournamentManager)
	{
		this.crownExpGranterManager = crownExpGranterManager;
		this.skillTreeManager = skillTreeManager;
		this.resourceManager = resourceManager;
		this.tournamentManager = tournamentManager;
	}

	public int GetCrownExpAmountAtLocation(GranterLocation location)
	{
		if (!this.skillTreeManager.IsSkillTreeEnabled)
		{
			return 0;
		}
		if (location.GranterType == GranterType.Ad)
		{
			AdPlacement locationTypeAsInt = (AdPlacement)location.LocationTypeAsInt;
			switch (locationTypeAsInt + 1)
			{
			case AdPlacement.GemChest:
				return 0;
			case AdPlacement.DoubleUp:
				return 10;
			case AdPlacement.SpinWheel:
				return 10;
			case AdPlacement.SpinWheelDialog:
				return 10;
			case AdPlacement.QuestExtra:
				return 10;
			case AdPlacement.PackageExtra:
				return 10;
			case AdPlacement.GemChestExtra:
				return 10;
			case AdPlacement.DailyCatchContinue:
				return 10;
			case AdPlacement.GetMoreFireworks:
				return 10;
			case (AdPlacement)9:
				return 10;
			}
		}
		else if (location.GranterType == GranterType.IAP)
		{
			IAPPlacement locationTypeAsInt2 = (IAPPlacement)location.LocationTypeAsInt;
			switch (locationTypeAsInt2 + 1)
			{
			case IAPPlacement.GemPack1:
				return 0;
			case IAPPlacement.GemPack2:
				return 300;
			case IAPPlacement.GemPack3:
				return 2500;
			case IAPPlacement.GemPackWhale:
				return 6000;
			case IAPPlacement.SpecialOffer:
				return 20000;
			case IAPPlacement.SpecialOfferStarter:
				return 0;
			case IAPPlacement.SpecialOfferSmall:
				return 500;
			case IAPPlacement.SpecialOfferAwesome:
				return 500;
			case IAPPlacement.SpecialOfferUltimate:
				return 2500;
			case IAPPlacement.SpecialOfferAdventCalendar:
				return 6000;
			case IAPPlacement.SpecialOfferValentineOffer:
				return 3000;
			case IAPPlacement.SpecialOfferEasterOffer:
				return 3000;
			case IAPPlacement.SpecialOffer4thJulyOffer:
				return 3000;
			case IAPPlacement.SpecialOffer4thJulyRocketsOffer:
				return 10000;
			}
		}
		return 0;
	}

	public bool IsCrownExpAvailableAtLocation(GranterLocation location)
	{
		if (!this.skillTreeManager.IsSkillTreeEnabled)
		{
			return false;
		}
		if (location.GranterType == GranterType.Ad)
		{
			AdPlacement locationTypeAsInt = (AdPlacement)location.LocationTypeAsInt;
			switch (locationTypeAsInt + 1)
			{
			case AdPlacement.GemChest:
				return false;
			case AdPlacement.DoubleUp:
				return this.Default_CrownExpAvailability_Ads();
			case AdPlacement.SpinWheel:
				return this.Default_CrownExpAvailability_Ads();
			case AdPlacement.SpinWheelDialog:
				return this.Default_CrownExpAvailability_Ads() && this.IsCrownExpAvailableAt_Wheel();
			case AdPlacement.QuestExtra:
				return this.Default_CrownExpAvailability_Ads() && this.IsCrownExpAvailableAt_Wheel();
			case AdPlacement.PackageExtra:
				return this.Default_CrownExpAvailability_Ads();
			case AdPlacement.GemChestExtra:
				return this.Default_CrownExpAvailability_Ads();
			case AdPlacement.DailyCatchContinue:
				return this.Default_CrownExpAvailability_Ads();
			case AdPlacement.GetMoreFireworks:
				return this.Default_CrownExpAvailability_Ads();
			case (AdPlacement)9:
				return this.Default_CrownExpAvailability_Ads();
			}
		}
		else if (location.GranterType == GranterType.IAP)
		{
			IAPPlacement locationTypeAsInt2 = (IAPPlacement)location.LocationTypeAsInt;
			switch (locationTypeAsInt2 + 1)
			{
			case IAPPlacement.GemPack1:
				return false;
			case IAPPlacement.GemPack2:
				return this.Default_CrownExpAvailability_IAPs();
			case IAPPlacement.GemPack3:
				return this.Default_CrownExpAvailability_IAPs();
			case IAPPlacement.GemPackWhale:
				return this.Default_CrownExpAvailability_IAPs();
			case IAPPlacement.SpecialOffer:
				return this.Default_CrownExpAvailability_IAPs();
			case IAPPlacement.SpecialOfferStarter:
				return false;
			case IAPPlacement.SpecialOfferSmall:
				return this.Default_CrownExpAvailability_IAPs();
			case IAPPlacement.SpecialOfferAwesome:
				return this.Default_CrownExpAvailability_IAPs();
			case IAPPlacement.SpecialOfferUltimate:
				return this.Default_CrownExpAvailability_IAPs();
			case IAPPlacement.SpecialOfferAdventCalendar:
				return this.Default_CrownExpAvailability_IAPs();
			case IAPPlacement.SpecialOfferValentineOffer:
			case IAPPlacement.SpecialOfferEasterOffer:
			case IAPPlacement.SpecialOffer4thJulyOffer:
			case IAPPlacement.SpecialOffer4thJulyRocketsOffer:
				return this.Default_CrownExpAvailability_IAPs();
			}
		}
		return false;
	}

	private bool Default_CrownExpAvailability_IAPs()
	{
		return !this.tournamentManager.IsInsideTournament;
	}

	private bool Default_CrownExpAvailability_Ads()
	{
		return !this.tournamentManager.IsInsideTournament;
	}

	private bool IsCrownExpAvailableAt_Wheel()
	{
		return this.resourceManager.GetFreeSpinAmount() <= 0;
	}

	private CrownExpGranterManager crownExpGranterManager;

	private SkillTreeManager skillTreeManager;

	private ResourceManager resourceManager;

	private TournamentManager tournamentManager;
}
