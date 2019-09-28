using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemBase-", menuName = "Item/ItemBase", order = 1)]
public class ItemBaseMaster : ScriptableObject
{
	public string itemName;
	public string itemDescription;
	public Sprite icon;

	//1. CombatBaseValues
	[SerializeField] public CombatClass CombatBaseValue;

	// item level like 2, 3...
	[SerializeField] public int Level = 1;

	//2. Skill add
	[SerializeField] public List<SkillsMasterClass> SkillsMasterList;

	//3. Ability add value
	[SerializeField] public List<AddAbilityClass> AddAbilityList;


	//4. Offense or defense magnification set
	[SerializeField] public List<MagnificationMasterClass> MagnificationMasterList;


	// be calculated by coefficient
	public CombatClass CalculatedCombatValue()
	{
		CombatClass _calculated = new CombatClass();
		if (CombatBaseValue != null)
		{
			_calculated = CombatBaseValue.Copy();
			_calculated.Pow(Level);

		}
		return _calculated;
	}


	public string OneLineDescription()
	{
		//1. CombatBase Value

		string _description = null;
		if (CombatBaseValue != null)
		{
			if (CombatBaseValue.ShieldMax != 0) { _description += "Shield +" + CalculatedCombatValue().ShieldMax + " "; }
			if (CombatBaseValue.HitPointMax != 0) { _description += "HP +" + CalculatedCombatValue().HitPointMax + " "; }
			if (CombatBaseValue.Attack != 0) { _description += "Attack +" + CalculatedCombatValue().Attack + " "; }
			if (CombatBaseValue.Accuracy != 0) { _description += "Accuracy +" + CalculatedCombatValue().Accuracy + " "; }
			if (CombatBaseValue.Mobility != 0) { _description += "Mobility +" + CalculatedCombatValue().Mobility + " "; }
			if (CombatBaseValue.Defense != 0) { _description += "Defense +" + CalculatedCombatValue().Defense + " "; }
		}
		//2. Skill content
		foreach (SkillsMasterClass skill in SkillsMasterList)
		{
			_description += "[Skill: " + skill.name + "] ";
		}

		//3. Ability
		foreach (AddAbilityClass addAbility in AddAbilityList)
		{
			_description += addAbility.Ability + " +" + addAbility.ValueOfAbility + " ";
		}

		//4. offense or defense magnification
		_description += magnificationText(isShortText: true) + " ";

		return _description;

	}

	public string DetailDescription()
	{
		string _description = null;

		_description += itemName + "\n";

		//1. CombatBase Value
		if (CombatBaseValue != null)
		{
			if (CombatBaseValue.ShieldMax != 0) { _description += "(Shield +" + CalculatedCombatValue().ShieldMax + ")\n"; }
			if (CombatBaseValue.HitPointMax != 0) { _description += "(HP +" + CalculatedCombatValue().HitPointMax + ")\n"; }
			if (CombatBaseValue.Attack != 0) { _description += "(Attack +" + CalculatedCombatValue().Attack + ")\n"; }
			if (CombatBaseValue.Accuracy != 0) { _description += "(Accuracy +" + CalculatedCombatValue().Accuracy + ")\n"; }
			if (CombatBaseValue.Mobility != 0) { _description += "(Mobility +" + CalculatedCombatValue().Mobility + ")\n"; }
			if (CombatBaseValue.Defense != 0) { _description += "(Defense +" + CalculatedCombatValue().Defense + ")\n"; }
		}

		//2. Skill content
		foreach (SkillsMasterClass skill in SkillsMasterList)
		{
			_description += "[Skill acquisition] " + skill.name + "\n";
		}

		//3. Ability
		foreach (AddAbilityClass addAbility in AddAbilityList)
		{
			_description += addAbility.Ability + " +" + addAbility.ValueOfAbility + "\n";
		}

		//4. offense or defense magnification
		_description += magnificationText(isShortText: false) + " \n";

		return _description;
	}

	public string magnificationText(bool isShortText)
	{
		string _description = null;
		foreach (MagnificationMasterClass magnification in MagnificationMasterList)
		{
			string _offenseOrDefense = null;
			string _magnificationDetail = null;

			switch (magnification.OffenseOrDefense)
			{
				case OffenseOrDefense.none:
					_offenseOrDefense = "[";
					break;
				case OffenseOrDefense.Offense:
					if (isShortText) { _offenseOrDefense = "[O "; } else { _offenseOrDefense = "[Offense "; }
					break;
				case OffenseOrDefense.Defense:
					if (isShortText) { _offenseOrDefense = "[D "; } else { _offenseOrDefense = "[Defense "; }
					break;
				default:
					Debug.LogError("unexpected OffenseOrDefense value:" + magnification.OffenseOrDefense);
					break;
			}

			switch (magnification.MagnificationType)
			{
				case MagnificationType.MagnificationRatio:
					_magnificationDetail += "x" + magnification.MagnificationRatio.ToString("F2");
					break;
				case MagnificationType.AdditionalPercent:
					_magnificationDetail += "+" + (int)magnification.MagnificationPercent + "%";
					break;
				case MagnificationType.none:
					break;
				default:
					Debug.LogError("unexpected MagnificationType :" + magnification.MagnificationTarget + " "  + magnification.MagnificationType.ToString());
					break;

			}

			_description += _offenseOrDefense + magnification.MagnificationTarget + " " + _magnificationDetail + "]";
            if (!isShortText) { _description += "\n"; }

		}

		return _description;

	}

}
