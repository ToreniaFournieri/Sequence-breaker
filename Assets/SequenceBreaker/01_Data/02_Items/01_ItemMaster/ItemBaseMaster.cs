using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "ItemBase-", menuName = "Item/ItemBase", order = 1)]
sealed public class ItemBaseMaster : ScriptableObject
{
    [FormerlySerializedAs("itemID")] public int itemId;

    public string itemName;
	public string itemDescription;
	public Sprite icon;

	//1. CombatBaseValues
	[FormerlySerializedAs("CombatBaseValue")] [SerializeField] public CombatClass combatBaseValue;

	// item level like 2, 3...
	[FormerlySerializedAs("Level")] [SerializeField] public int level = 0;

	//2. Skill add
	[FormerlySerializedAs("SkillsMasterList")] [SerializeField] public List<SkillsMasterClass> skillsMasterList;

	//3. Ability add value
	[FormerlySerializedAs("AddAbilityList")] [SerializeField] public List<AddAbilityClass> addAbilityList;


	//4. Offense or defense magnification set
	[FormerlySerializedAs("MagnificationMasterList")] [SerializeField] public List<MagnificationMasterClass> magnificationMasterList;


	// be calculated by coefficient
	public CombatClass CalculatedCombatValue()
	{
		CombatClass calculated = new CombatClass();
		if (combatBaseValue != null)
		{
			calculated = combatBaseValue.Copy();
			calculated.Pow(level);

		}
		return calculated;
	}


	public string OneLineDescription()
	{
		//1. CombatBase Value

		string description = null;
		if (combatBaseValue != null)
		{
			if (combatBaseValue.shieldMax != 0) { description += "Shield +" + CalculatedCombatValue().shieldMax + " "; }
			if (combatBaseValue.hitPointMax != 0) { description += "HP +" + CalculatedCombatValue().hitPointMax + " "; }
			if (combatBaseValue.attack != 0) { description += "Attack +" + CalculatedCombatValue().attack + " "; }
			if (combatBaseValue.accuracy != 0) { description += "Accuracy +" + CalculatedCombatValue().accuracy + " "; }
			if (combatBaseValue.mobility != 0) { description += "Mobility +" + CalculatedCombatValue().mobility + " "; }
			if (combatBaseValue.defense != 0) { description += "Defense +" + CalculatedCombatValue().defense + " "; }
		}
		//2. Skill content
		foreach (SkillsMasterClass skill in skillsMasterList)
		{
			description += "[Skill: " + ((Object) skill).name + "] ";
		}

		//3. Ability
		foreach (AddAbilityClass addAbility in addAbilityList)
		{
			description += addAbility.ability + " +" + addAbility.valueOfAbility + " ";
		}

		//4. offense or defense magnification
		description += MagnificationText(isShortText: true) + " ";

		return description;

	}

	public string DetailDescription()
	{
		string description = null;

		description += itemName + "\n";

		//1. CombatBase Value
		if (combatBaseValue != null)
		{
			if (combatBaseValue.shieldMax != 0) { description += "(Shield +" + CalculatedCombatValue().shieldMax + ")\n"; }
			if (combatBaseValue.hitPointMax != 0) { description += "(HP +" + CalculatedCombatValue().hitPointMax + ")\n"; }
			if (combatBaseValue.attack != 0) { description += "(Attack +" + CalculatedCombatValue().attack + ")\n"; }
			if (combatBaseValue.accuracy != 0) { description += "(Accuracy +" + CalculatedCombatValue().accuracy + ")\n"; }
			if (combatBaseValue.mobility != 0) { description += "(Mobility +" + CalculatedCombatValue().mobility + ")\n"; }
			if (combatBaseValue.defense != 0) { description += "(Defense +" + CalculatedCombatValue().defense + ")\n"; }
		}

		//2. Skill content
		foreach (SkillsMasterClass skill in skillsMasterList)
		{
			description += "[Skill acquisition] " + ((Object) skill).name + "\n";
		}

		//3. Ability
		foreach (AddAbilityClass addAbility in addAbilityList)
		{
			description += addAbility.ability + " +" + addAbility.valueOfAbility + "\n";
		}

		//4. offense or defense magnification
		description += MagnificationText(isShortText: false) + " \n";

		return description;
	}

	public string MagnificationText(bool isShortText)
	{
		string description = null;
		foreach (MagnificationMasterClass magnification in magnificationMasterList)
		{
			string offenseOrDefense = null;
			string magnificationDetail = null;

			switch (magnification.offenseOrDefense)
			{
				case OffenseOrDefense.None:
					offenseOrDefense = "[";
					break;
				case OffenseOrDefense.Offense:
					if (isShortText) { offenseOrDefense = "[O "; } else { offenseOrDefense = "[Offense "; }
					break;
				case OffenseOrDefense.Defense:
					if (isShortText) { offenseOrDefense = "[D "; } else { offenseOrDefense = "[Defense "; }
					break;
				default:
					Debug.LogError("unexpected OffenseOrDefense value:" + magnification.offenseOrDefense);
					break;
			}

			switch (magnification.magnificationType)
			{
				case MagnificationType.MagnificationRatio:
					magnificationDetail += "x" + magnification.magnificationRatio.ToString("F2");
					break;
				case MagnificationType.AdditionalPercent:
					magnificationDetail += "+" + (int)magnification.magnificationPercent + "%";
					break;
				case MagnificationType.None:
					break;
				default:
					Debug.LogError("unexpected MagnificationType :" + magnification.magnificationTarget + " "  + magnification.magnificationType.ToString());
					break;

			}

			description += offenseOrDefense + magnification.magnificationTarget + " " + magnificationDetail + "]";
            if (!isShortText) { description += "\n"; }

		}

		return description;

	}

}
