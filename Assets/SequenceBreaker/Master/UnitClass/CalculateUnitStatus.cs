using System;
using System.Collections.Generic;
using System.Linq;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.BattleUnit;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.Skills;
using SequenceBreaker.Play.Battle;
using UnityEngine;

namespace SequenceBreaker.Master.UnitClass
{
    public sealed class CalculateUnitStatus : MonoBehaviour
    {

        //Input data
        public UnitClass unit;

        //Output data
        public BattleUnit.BattleUnit battleUnit;

        // Output data to show magnification data
        public List<(int magnificationTargetID, int percentValue, double ratioValue, double totalValue, string percents)> summedOffenseList;
        public List<(int magnificationTargetID, int percentValue, double ratioValue, double totalValue, string percents)> summedDefenseList;
        public List<(int magnificationTargetID, int percentValue, double ratioValue, double totalValue, string percents)> summedNoneList;

        //output Ability Detail Text
        public string detailAbilityString;

        //public BattleEnvironment battleEnvironment;

        //output middle data
        private AbilityClass _ability;
        private CombatClass _combat;
        private OffenseMagnificationClass _offenseMagnification;
        private DefenseMagnificationClass _defenseMagnification;
        private UnitSkillMagnificationClass _unitSkillMagnification;
        private FeatureClass _feature;


        //input calculateUnitStatusMaster
        public CalculateUnitStatusMaster master;


        //Output Skill list
        public List<SkillsMasterClass> summedSkillList;

        //// Environment Parameter

        //// tuning style 
        ////public TuningStyleClass tuningStyleClass;
        //public TuningStyleMaster tuningStyleMaster;


        //// for combat status calculation
        //public float t5LevelCoefficient;
        //public float t5LevelPowLittle;
        //public float t5LevelPowBig;
        //public float t5AbilityCoefficient;
        //public float t5AbilityPowDenominator;

        //public float t4LevelCoefficient;
        //public float t4LevelPowLittle;
        //public float t4LevelPowBig;
        //public float t4AbilityCoefficient;
        //public float t4AbilityPowDenominator;

        //public float t3LevelCoefficient;
        //public float t3LevelPowLittle;
        //public float t3LevelPowBig;
        //public float t3AbilityCoefficient;
        //public float t3AbilityPowDenominator;
        //public float t3AbilityBasedCoefficient;

        //public float t2LevelCoefficient;
        //public float t2LevelPowLittle;
        //public float t2LevelPowBig;
        //public float t2AbilityCoefficient;
        //public float t2AbilityPowDenominator;
        //public float t2AbilityBasedCoefficient;


        ////efficient block
        //public float powerCoefficient;
        //public float criticalHitCoefficient;
        //public float numberOfAttacksCoefficient;
        //public float accuracyCoefficient;
        //public float mobilityCoefficient;
        //public float defenseCoefficient;
        //public float counterintelligenceCoefficient;
        //public float repairCoefficient;
        //public float shieldCoefficient;
        //public float hpCoefficient;


        //middle data
        private AbilityClass _frameTypeAbility;
        private AbilityClass _tuningStyleAbility;
        private AbilityClass _summedItemsAddAbility;

        private CombatClass _combatRaw;
        private CombatClass _combatItems;

        private ActionSkillClass _offenseEffectPowerActionSkill;
        private ActionSkillClass _triggerPossibilityActionSkill;
        private double _hateInitial;
        private double _hateMagnificationPerTurn;
        private double _absorbShieldRatio;
        private bool _isDamageControlAssist;


        static CalculateUnitStatus _instance = null;
        public static CalculateUnitStatus Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CalculateUnitStatus>();
                }
                return _instance;
            }

        }


        public void Init(UnitClass unitClass)
        {



            _ability = ScriptableObject.CreateInstance<AbilityClass>();

            unit = unitClass;

            //Debug.Log("unit name in CalculateUnitStatus :" + unit.TrueName());

            //(1) Ability
            // (1-1) Ability calculation -> CoreFrameAddAbility
            // CoreFrame.FrameType.AddAbility + CoreFrame.TuningStyle.AddAbility

            _frameTypeAbility = ScriptableObject.CreateInstance<AbilityClass>();
            switch (unit.coreFrame.frameType)
            {
                case FrameType.Caterpillar:
                    _frameTypeAbility.Set(6, 3, 6, 3, 2, 0, 0);

                    break;
                case FrameType.ReverseJointLegs:
                    //                    _frameTypeAbility = new AbilityClass(3, 3, 3, 8, 2, 0, 2);
                    _frameTypeAbility.Set(3, 3, 3, 8, 2, 0, 2);

                    break;
                case FrameType.SpiderLegs:
                    //                    _frameTypeAbility = new AbilityClass(4, 3, 5, 4, 3, 0, 1);
                    _frameTypeAbility.Set(4, 3, 5, 4, 3, 0, 1);
                    break;
                case FrameType.TwoLegs:
                    _frameTypeAbility.Set(5, 3, 4, 5, 2, 0, 1);
                    break;
                case FrameType.Wheel:
                    _frameTypeAbility.Set(2, 6, 3, 4, 2, 0, 3);
                    break;
                default:
                    Debug.LogError("CoreFrame.FrameType unexpected value, need FrameType case update! :" + unit.coreFrame.frameType);
                    break;
            }
            _ability.AddUp(_frameTypeAbility);

            _tuningStyleAbility = ScriptableObject.CreateInstance<AbilityClass>();

            switch (unit.coreFrame.tuningStyle)
            {
                case TuningStyle.Commander:
                    _tuningStyleAbility.Set(0, 0, 0, 2, 2, 2, 0);
                    break;
                case TuningStyle.Destroyer:
                    _tuningStyleAbility.Set(2, 0, 2, 2, 0, 0, 0);
                    break;
                case TuningStyle.Fighter:
                    _tuningStyleAbility.Set(2, 0, 0, 2, 2, 0, 0);
                    break;
                case TuningStyle.Gunner:
                    _tuningStyleAbility.Set(3, 0, 0, 0, 2, 0, 0);
                    break;
                case TuningStyle.Jammer:
                    _tuningStyleAbility.Set(0, 2, 0, 0, 2, 2, 0);
                    break;
                case TuningStyle.Lancer:
                    _tuningStyleAbility.Set(3, 1, 0, 0, 2, 0, 0);
                    break;
                case TuningStyle.Medic:
                    _tuningStyleAbility.Set(0, 5, 2, 0, 0, 1, 0);
                    break;
                case TuningStyle.Reconnoiter:
                    _tuningStyleAbility.Set(1, 1, 2, 3, 0, 0, 0);
                    break;
                case TuningStyle.Sniper:
                    _tuningStyleAbility.Set(0, 0, 0, 0, 5, 0, 1);
                    break;

                case TuningStyle.Tank:
                    _tuningStyleAbility.Set(0, 2, 4, 0, 0, 0, 0);
                    break;

                default:
                    Debug.LogError("CoreFrame.TuningStyle unexpected value, need TuningStyle case update! :" + unit.coreFrame.tuningStyle);
                    break;
            }

            _ability.AddUp(_tuningStyleAbility);

            // (1-2) Ability calculation -> SummedItemsAddAbility
            _summedItemsAddAbility = ScriptableObject.CreateInstance<AbilityClass>();
            _summedItemsAddAbility.Set(0, 0, 0, 0, 0, 0, 0);

            if (unit.itemList != null)
            {
                foreach (Item item in unit.itemList)
                {
                    if (item != null)
                    {
                        _summedItemsAddAbility.AddUp(item.TotaledAbility());
                    }
                }
            }

            // (1-3) Ability calculation -> CalculatedAbility
            // Formula:
            //  Ability = CoreFrame.Ability + Pilot.AddAbility + SummedItemsAddAbility
            _ability.AddUp(unit.pilot.addAbility);
            _ability.AddUp(_summedItemsAddAbility);


            // (1-4) Offense/Defense/UnitSkill Magnification

            //(4)Offense/Defense/UnitSkill Magnification

            List<MagnificationMasterClass> itemMagnificationMasterList = new List<MagnificationMasterClass>();
            if (unit.itemList != null)
            {
                foreach (Item item in unit.itemList)
                {
                    if (item != null)
                    {
                        //item has prefix, base and suffix. each one has magnification master class list. collect them all
                        if (item.prefixItem != null)
                        {
                            foreach (MagnificationMasterClass magnificationMaster in item.prefixItem
                                .magnificationMasterList)
                            {
                                itemMagnificationMasterList.Add(magnificationMaster);
                            }
                        }

                        if (item.baseItem != null)
                        {
                            foreach (MagnificationMasterClass magnificationMaster in item.baseItem
                                .magnificationMasterList)
                            {
                                itemMagnificationMasterList.Add(magnificationMaster);
                            }
                        }

                        if (item.suffixItem != null)
                        {
                            foreach (MagnificationMasterClass magnificationMaster in item.suffixItem
                                .magnificationMasterList)
                            {
                                itemMagnificationMasterList.Add(magnificationMaster);
                            }
                        }
                    }
                }
            }

            // Deduplication
            MagnificationComparer comparer = new MagnificationComparer();
            IEnumerable<MagnificationMasterClass> deduplicationMagnification = itemMagnificationMasterList.Distinct(comparer);


            // this List is magnificationTarget
            // magnificationTargetID is stand for enum magnificationTarget.

            List<(int magnificationTargetID, int _percent, double _ratio, List<int> _percentList)> magnificationOffenseList
                = new List<(int magnificationTargetID, int _percentSummed, double _ratioSummed, List<int> _percentList)>();
            List<(int magnificationTargetID, int _percent, double _ratio, List<int> _percentList)> magnificationDefenseList
                = new List<(int magnificationTargetID, int _percentSummed, double _ratioSummed, List<int> _percentList)>();
            List<(int magnificationTargetID, int _percent, double _ratio, List<int> _percentList)> magnificationNoneList
                = new List<(int magnificationTargetID, int _percentSummed, double _ratioSummed, List<int> _percentList)>();

            // final value
            summedOffenseList = new List<(int magnificationTargetID, int percentValue, double ratioValue, double totalValue, string)>();
            summedDefenseList = new List<(int magnificationTargetID, int percentValue, double ratioValue, double totalValue, string)>();
            summedNoneList = new List<(int magnificationTargetID, int percentValue, double ratioValue, double totalValue, string)>();


            // get length of enum magnification Target
            int totalMagnificationTargetLength = Enum.GetNames(typeof(MagnificationTarget)).Length;

            // initialize
            for (int i = 0; i < totalMagnificationTargetLength; i++)
            {
                magnificationOffenseList.Add((i, 0, 1.0, null));
                magnificationDefenseList.Add((i, 0, 1.0, null));
                magnificationNoneList.Add((i, 0, 1.0, null));

                summedOffenseList.Add((i, 0, 1.0, 1.0, null));
                summedDefenseList.Add((i, 0, 1.0, 1.0, null));
                summedNoneList.Add((i, 0, 1.0, 1.0, null));

            }

            foreach (MagnificationMasterClass magnificationClass in deduplicationMagnification)
            {
                //for test
                (int magnificationTargetID, int _percent, double _ratio, List<int> _percentList) magnification;

                switch (magnificationClass.offenseOrDefense)
                {

                    case OffenseOrDefense.Offense:
                        magnification = MagnificationTypeModule(magnificationClass);

                        if (magnificationOffenseList[magnification.magnificationTargetID]._percentList != null)
                        {
                            foreach (int i in magnificationOffenseList[magnification.magnificationTargetID]._percentList)
                            {
                                magnification._percentList.Add(i);
                            }
                        }

                        magnification = (magnification.magnificationTargetID,
                                magnificationOffenseList[magnification.magnificationTargetID]._percent + magnification._percent,
                                magnificationOffenseList[magnification.magnificationTargetID]._ratio * magnification._ratio,
                                magnification._percentList
                            );

                        magnificationOffenseList[magnification.magnificationTargetID] = magnification;

                        break;
                    case OffenseOrDefense.Defense:
                        magnification = MagnificationTypeModule(magnificationClass);
                        if (magnificationDefenseList[magnification.magnificationTargetID]._percentList != null)
                        {
                            foreach (int i in magnificationDefenseList[magnification.magnificationTargetID]._percentList)
                            {
                                magnification._percentList.Add(i);
                            }
                        }
                        magnification = (magnification.magnificationTargetID,
                                magnificationDefenseList[magnification.magnificationTargetID]._percent + magnification._percent,
                                magnificationDefenseList[magnification.magnificationTargetID]._ratio * magnification._ratio,
                                magnification._percentList
                            );

                        magnificationDefenseList[magnification.magnificationTargetID] = magnification;


                        break;
                    case OffenseOrDefense.None:
                        magnification = MagnificationTypeModule(magnificationClass);
                        if (magnificationNoneList[magnification.magnificationTargetID]._percentList != null)
                        {
                            foreach (int i in magnificationNoneList[magnification.magnificationTargetID]._percentList)
                            {
                                magnification._percentList.Add(i);
                            }
                        }
                        magnification = (magnification.magnificationTargetID,
                                magnificationNoneList[magnification.magnificationTargetID]._percent + magnification._percent,
                                magnificationNoneList[magnification.magnificationTargetID]._ratio * magnification._ratio,
                                magnification._percentList
                            );

                        magnificationNoneList[magnification.magnificationTargetID] = magnification;

                        break;
                    default:
                        Debug.LogError("unexpected OffenseOrDefense Value:" + magnificationClass.offenseOrDefense);
                        break;
                }


            }


            foreach ((int magnificationTargetID, int percentValue, double ratioValue, List<int> percentList) calculated in magnificationOffenseList)
            {
                double total = 1.0 / (1.0 - calculated.percentValue * 0.01) * calculated.ratioValue;

                string percents = null;
                if (calculated.percentList != null)
                {
                    calculated.percentList.Sort();

                    foreach (int i in calculated.percentList)
                    {
                        percents += i + " ";
                    }

                }

                summedOffenseList[calculated.magnificationTargetID] =
                (
                    calculated.magnificationTargetID,
                    calculated.percentValue,
                    calculated.ratioValue,
                    total,
                    percents
                );
            }

            foreach ((int magnificationTargetID, int percentValue, double ratioValue, List<int> percentList) calculated in magnificationDefenseList)
            {
                double total = (1.0 - calculated.percentValue * 0.01) * calculated.ratioValue;

                string percents = null;
                if (calculated.percentList != null)
                {
                    calculated.percentList.Sort();
                    foreach (int i in calculated.percentList)
                    {
                        percents += i + " ";
                    }
                }

                summedDefenseList[calculated.magnificationTargetID] =
                (
                    calculated.magnificationTargetID,
                    calculated.percentValue,
                    calculated.ratioValue,
                    total,
                    percents
                );
            }

            foreach ((int magnificationTargetID, int percentValue, double ratioValue, List<int> percentList) calculated in magnificationNoneList)
            {
                double total = 1.0 / (1.0 - calculated.percentValue * 0.01) * calculated.ratioValue;

                string percents = null;
                if (calculated.percentList != null)
                {
                    calculated.percentList.Sort();
                    foreach (int i in calculated.percentList)
                    {
                        percents += i + " ";
                    }
                }
                summedNoneList[calculated.magnificationTargetID] =
                (
                    calculated.magnificationTargetID,
                    calculated.percentValue,
                    calculated.ratioValue,
                    total,
                    percents
                );
            }

            // 0:none, 1:Critical, 2:Kinetic, 3:Chemical, 4:Thermal, 5:VsBeast, 6:VsCyborg, 7:VsDrone, 8:VsRobot, 9:VsTitan, 10:OptimumRangeBonus
            // after 11, should only affect status, so use OffenseOrDefense.none
            // 11:Shield, 12:HitPoint, 13: NumberOfAttacks, 14: MinRange, 15: MaxRange, 16:Accuracy, 17:Mobility, 18:Attack, 19:Defence

            //            _offenseMagnification = new OffenseMagnificationClass(
            //                summedOffenseList[10].totalValue,
            //                summedOffenseList[1].totalValue,
            //                summedOffenseList[2].totalValue,
            //                summedOffenseList[3].totalValue,
            //                summedOffenseList[4].totalValue,
            //                summedOffenseList[5].totalValue,
            //                summedOffenseList[6].totalValue,
            //                summedOffenseList[7].totalValue,
            //                summedOffenseList[8].totalValue,
            //                summedOffenseList[9].totalValue);

            _offenseMagnification = ScriptableObject.CreateInstance<OffenseMagnificationClass>();
            _offenseMagnification.Set(summedOffenseList[10].totalValue,
                summedOffenseList[1].totalValue,
                summedOffenseList[2].totalValue,
                summedOffenseList[3].totalValue,
                summedOffenseList[4].totalValue,
                summedOffenseList[5].totalValue,
                summedOffenseList[6].totalValue,
                summedOffenseList[7].totalValue,
                summedOffenseList[8].totalValue,
                summedOffenseList[9].totalValue);

            //            _defenseMagnification = new DefenseMagnificationClass(
            //                summedDefenseList[1].totalValue,
            //                summedDefenseList[2].totalValue,
            //                summedDefenseList[3].totalValue,
            //                summedDefenseList[4].totalValue,
            //                summedDefenseList[5].totalValue,
            //                summedDefenseList[6].totalValue,
            //                summedDefenseList[7].totalValue,
            //                summedDefenseList[8].totalValue,
            //                summedDefenseList[9].totalValue,
            //                summedNoneList[11].totalValue,
            //                summedNoneList[12].totalValue,
            //                summedNoneList[13].totalValue,
            //                summedNoneList[14].totalValue,
            //                summedNoneList[15].totalValue,
            //                summedNoneList[16].totalValue,
            //                summedNoneList[17].totalValue,
            //                summedNoneList[18].totalValue,
            //                summedNoneList[19].totalValue
            //            );

            _defenseMagnification = ScriptableObject.CreateInstance<DefenseMagnificationClass>();
            _defenseMagnification.Set(summedDefenseList[1].totalValue,
                summedDefenseList[2].totalValue,
                summedDefenseList[3].totalValue,
                summedDefenseList[4].totalValue,
                summedDefenseList[5].totalValue,
                summedDefenseList[6].totalValue,
                summedDefenseList[7].totalValue,
                summedDefenseList[8].totalValue,
                summedDefenseList[9].totalValue,
                summedNoneList[11].totalValue,
                summedNoneList[12].totalValue,
                summedNoneList[13].totalValue,
                summedNoneList[14].totalValue,
                summedNoneList[15].totalValue,
                summedNoneList[16].totalValue,
                summedNoneList[17].totalValue,
                summedNoneList[18].totalValue,
                summedNoneList[19].totalValue);

            //            _offenseEffectPowerActionSkill = new ActionSkillClass(1, 1, 1, 1, 1, 1, 1, 1);

            _offenseEffectPowerActionSkill = ScriptableObject.CreateInstance<ActionSkillClass>();
            _offenseEffectPowerActionSkill.Set(1, 1, 1, 1, 1, 1, 1, 1);

            //            _triggerPossibilityActionSkill = new ActionSkillClass(1, 1, 1, 1, 1, 1, 1, 1);
            _triggerPossibilityActionSkill = ScriptableObject.CreateInstance<ActionSkillClass>();
            _triggerPossibilityActionSkill.Set(1, 1, 1, 1, 1, 1, 1, 1);

            //            _unitSkillMagnification = new UnitSkillMagnificationClass(_offenseEffectPowerActionSkill, _triggerPossibilityActionSkill);

            _unitSkillMagnification = ScriptableObject.CreateInstance<UnitSkillMagnificationClass>();
            _unitSkillMagnification.Set(_offenseEffectPowerActionSkill, _triggerPossibilityActionSkill);


            //(2) Combat
            // (2-1) Set environment values


            // each ability's coefficient
            {

            }

            // (2-3) First Combat calculation -> CombatRaw
            // Formula:
            //    CombatRaw.Shield = CoreFrame.Shield (fixed, independent on Level)
            //    CombatRaw.HitPoint = CoreFrame.HitPoint (fixed, independent on Level)
            //    CombatRaw.Others values: (
            //                                  LevelCoefficient *(Level^LevelPowLittle)
            //                                  +Level^LevelPowBig
            //                                  +Level * Ability
            //                                  +Level^(Ability / PowDenominator)*AbilityCoefficient
            //                             )*XXXCoefficient
            //     + Unit.CoreFrame.Shield or HitPoint;
            _combatRaw = ScriptableObject.CreateInstance<CombatClass>();


            //Tier 5
            //            t5LevelPowLittle = (float)0.372;
            //            t5LevelPowBig = (float)1.475;
            //            t5AbilityCoefficient = (float)2.6;
            //            t5LevelCoefficient = (float)100.0;
            //            t5AbilityPowDenominator = (float)20.0;
            //            shieldCoefficient = (float)0.82;
            //            hpCoefficient = (float)0.67;

            _combatRaw.shieldMax =
                //Unit.CoreFrame.Shield +
                (int)((
                          (master.t5LevelCoefficient * Mathf.Pow(unit.level, master.t5LevelPowLittle))
                          + Mathf.Pow(unit.level, master.t5LevelPowBig)
                          + unit.level * _ability.generation
                          + Mathf.Pow(unit.level, _ability.generation / master.t5AbilityPowDenominator) * master.t5AbilityCoefficient
                      ) * master.shieldCoefficient
                      + unit.coreFrame.shield);

            _combatRaw.hitPointMax =
                //Unit.CoreFrame.HP +
                (int)((
                          (master.t5LevelCoefficient * Mathf.Pow(unit.level, master.t5LevelPowLittle))
                          + Mathf.Pow(unit.level, master.t5LevelPowBig)
                          + unit.level * _ability.stability
                          + Mathf.Pow(unit.level, _ability.stability / master.t5AbilityPowDenominator) * master.t5AbilityCoefficient
                      ) * master.hpCoefficient
                      + unit.coreFrame.hp
                );


            //Tier 4
            //            t4LevelCoefficient = (float)100.0;
            //            t4LevelPowLittle = (float)0.372;
            //            t4LevelPowBig = (float)1.475;
            //            t4AbilityCoefficient = (float)2.6;
            //            t4AbilityPowDenominator = (float)20.0;
            //            powerCoefficient = (float)0.4;
            //            defenseCoefficient = (float)0.4;

            _combatRaw.attack = (int)((
                                          (master.t4LevelCoefficient * Mathf.Pow(unit.level, master.t4LevelPowLittle))
                                          + Mathf.Pow(unit.level, master.t4LevelPowBig)
                                          + unit.level * _ability.power
                                          + Mathf.Pow(unit.level, _ability.power / master.t4AbilityPowDenominator) * master.t4AbilityCoefficient
                                      ) * master.powerCoefficient);

            _combatRaw.defense = (int)((
                                           (master.t4LevelCoefficient * Mathf.Pow(unit.level, master.t4LevelPowLittle))
                                           + Mathf.Pow(unit.level, master.t4LevelPowBig)
                                           + unit.level * _ability.stability
                                           + Mathf.Pow(unit.level, _ability.stability / master.t4AbilityPowDenominator) * master.t4AbilityCoefficient
                                       ) * master.defenseCoefficient);

            // Tier 3
            //            t3LevelCoefficient = (float)100.0;
            //            t3LevelPowLittle = (float)0.34;
            //            t3LevelPowBig = (float)1.00;
            //            t3AbilityCoefficient = (float)2.1;
            //            t3AbilityPowDenominator = (float)20.0;
            //            t3AbilityBasedCoefficient = (float)0.3;
            //            mobilityCoefficient = (float)0.233;
            //            accuracyCoefficient = (float)0.233;

            _combatRaw.mobility = (int)((
                                            (master.t3LevelCoefficient * Mathf.Pow(unit.level, master.t3LevelPowLittle))
                                            + Mathf.Pow(unit.level, master.t3LevelPowBig)
                                            + unit.level * _ability.responsiveness * master.t3AbilityBasedCoefficient
                                            + Mathf.Pow(unit.level, _ability.responsiveness / master.t3AbilityPowDenominator) * master.t3AbilityCoefficient
                                        ) * master.mobilityCoefficient);

            _combatRaw.accuracy = (int)((
                                            (master.t3LevelCoefficient * Mathf.Pow(unit.level, master.t3LevelPowLittle))
                                            + Mathf.Pow(unit.level, master.t3LevelPowBig)
                                            + unit.level * _ability.precision * master.t3AbilityBasedCoefficient
                                            + Mathf.Pow(unit.level, _ability.precision / master.t3AbilityPowDenominator) * master.t3AbilityCoefficient
                                        ) * master.accuracyCoefficient);

            //Tier 2
            //            t2LevelCoefficient = (float)100.0;
            //            t2LevelPowLittle = (float)0.1;
            //            t2LevelPowBig = (float)0.7;
            //            t2AbilityCoefficient = (float)2.1;
            //            t2AbilityPowDenominator = (float)20.0;
            //            t2AbilityBasedCoefficient = (float)0.0;
            //            counterintelligenceCoefficient = (float)1.2;
            //            repairCoefficient = (float)1.2;

            _combatRaw.counterintelligence = (int)((
                                                       (master.t2LevelCoefficient * Mathf.Pow(unit.level, master.t2LevelPowLittle))
                                                       + Mathf.Pow(unit.level, master.t2LevelPowBig)
                                                       + unit.level * _ability.intelligence * master.t2AbilityBasedCoefficient
                                                       + Mathf.Pow(unit.level, _ability.intelligence / master.t2AbilityPowDenominator) * master.t2AbilityCoefficient
                                                   ) * master.counterintelligenceCoefficient);

            _combatRaw.repair = (int)((
                                          (master.t2LevelCoefficient * Mathf.Pow(unit.level, master.t2LevelPowLittle))
                                          + Mathf.Pow(unit.level, master.t2LevelPowBig)
                                          + unit.level * _ability.generation * master.t2AbilityBasedCoefficient
                                          + Mathf.Pow(unit.level, _ability.generation / master.t2AbilityPowDenominator) * master.t2AbilityCoefficient
                                      ) * master.repairCoefficient);

            // Tier 1
            // critical hit = Level * Luck * CriticalHitCoefficient
            //            criticalHitCoefficient = (float)0.01;

            _combatRaw.criticalHit = (int)(
                (unit.level * _ability.luck * master.criticalHitCoefficient)
            );

            // Number of attacks
            // numberOfAttacksCoefficient + ( level / 4 )
            _combatRaw.numberOfAttacks = (int)(master.numberOfAttacksCoefficient + (unit.level / 4.0));

            // Min range and Max range, default are 1.
            _combatRaw.minRange = 1;
            _combatRaw.maxRange = 6;

            // Sample set kinetic Attack ratio
            _combatRaw.kineticAttackRatio = 1.0;

            // (2-4)Core Skill consideration, coreFrame skill and Pilot skill ->CombatBaseSkillConsidered
            // Formula:
            //    CombatCoreSkillConsidered.someValue = (CombatRaw.someValue + (CoreFrame and Pilot).skills.addCombat.someValue) * (CoreFrame and Pilot).skills.amplifyCombat.someValue

            // some code here. omitted


            // (2-5)Skill consideration of items equipped -> CombatItemSkillConsidered
            // Formula:
            //    CombatItemSkillConsidered.someValue = (CombatCoreSkillConsidered.someValue + itemList.skills.addCombat.someValue) * itemList.skills.amplifyCombat.someValue

            _combatRaw.criticalHit = _combatRaw.criticalHit + 18;
            // some code here. omitted

            // (2-6)Base Magnification 



            // this is for only Defense magnification. 2019.9.29
            // 11:BaseShield, 12:BaseHitPoint, 13:BaseNumberOfAttacks, 14:MiniRange, 15:MaxRange, 16:BaseAccuracy, 17:BaseMobility, 18:BaseAttack, 19:BaseDefense

            _combatRaw.shieldMax = (int)(_combatRaw.shieldMax * _defenseMagnification.shield);
            _combatRaw.hitPointMax = (int)(_combatRaw.hitPointMax * _defenseMagnification.hitPoint);
            _combatRaw.numberOfAttacks = (int)(_combatRaw.numberOfAttacks * _defenseMagnification.numberOfAttacks);
            _combatRaw.minRange = (int)(_combatRaw.minRange * _defenseMagnification.minRange);
            _combatRaw.maxRange = (int)(_combatRaw.maxRange * _defenseMagnification.maxRange);
            _combatRaw.accuracy = (int)(_combatRaw.accuracy * _defenseMagnification.accuracy);
            _combatRaw.mobility = (int)(_combatRaw.mobility * _defenseMagnification.mobility);
            _combatRaw.attack = (int)(_combatRaw.attack * _defenseMagnification.attack);
            _combatRaw.defense = (int)(_combatRaw.defense * _defenseMagnification.defense);

            // (2-7)Add combat value of items equipped -> CombatItemEquipped
            // Formula:
            //    CombatItemEquipped.someValue = CombatItemSkillConsidered.someValue + itemList.addCombat.someValue * AmplifyEquipmentRate.someParts

            _combatItems = ScriptableObject.CreateInstance<CombatClass>();

            if (unit.itemList != null)
            {
                foreach (Item item in unit.itemList)
                {
                    if (item != null)
                    {
                        _combatItems.Add(item.TotaledCombat());
                    }
                }
            }

            // some code here. omitted

            // (2-8)Finalize
            // Formula:
            //    CombatCalculated = CombatItemEquipped

            _combat = ScriptableObject.CreateInstance<CombatClass>();
            _combat.Add(_combatRaw);
            if (_combatItems != null) { _combat.Add(_combatItems); }


            //(3) Feature
            // (3-1) Set environment values
            _absorbShieldRatio = 0.1;
            _hateInitial = 10;
            _hateMagnificationPerTurn = 0.667;
            //_optimumRangeBonusDefault = 1.2;
            //_criticalMagnificationDefault = 1.4; this will obsolete
            // (3-2) 
            //   double absorbShieldInitial, bool damageControlAssist, double hateInitial, double hateMagnificationPerTurn)
            // Formula:
            //   absorbShieldInitial = CoreFrame.addFeature.absorbShield + Pilot.addFeature.absorbShield
            //   damageControlAssist = CoreFrame.TuningStyle.damageControlAssist
            //   hateInitial = (default value) + Pilot.addFeature.hate
            //   hateMagnificationPerTurn = (default value) * Pilot.addFeature.hateMagnificationPerTurn

            _isDamageControlAssist = false;

            switch (unit.coreFrame.tuningStyle)
            {
                case TuningStyle.Medic:
                    _isDamageControlAssist = true;
                    break;
                case TuningStyle.Commander:
                    break;
                default:
                    break;
            }


            //            _feature = new FeatureClass(_absorbShieldRatio, _isDamageControlAssist,
            //                _hateInitial, _hateMagnificationPerTurn);

            _feature = ScriptableObject.CreateInstance<FeatureClass>();
            _feature.Set(_absorbShieldRatio, _isDamageControlAssist,
                _hateInitial, _hateMagnificationPerTurn);

            //            battleUnit = new BattleUnit.BattleUnit(1, unitClass.TrueName(), Affiliation.None, unitClass.unitType, _ability,
            //                _combat, _feature, _offenseMagnification,
            //                _defenseMagnification, _unitSkillMagnification);

            battleUnit = ScriptableObject.CreateInstance<BattleUnit.BattleUnit>();
            battleUnit.Set(1, unitClass.TrueName(), Affiliation.None, unitClass.unitType, _ability,
                _combat, _feature, _offenseMagnification,
                _defenseMagnification, _unitSkillMagnification);


            // Summed Skill calculation
            // note: skill is not in battleUnit. is set effectClass, outside.
            List<SkillsMasterClass> tuningSkills = master.tuningStyleMaster.GetSkills(unit.coreFrame.tuningStyle);
            summedSkillList.Clear();
            foreach (var skill in tuningSkills)
            {
                summedSkillList.Add(skill);
            }

            if (unit.itemList != null)
            {
                foreach (var item in unit.itemList)
                {
                    if (item != null)
                    {
                        if (item.prefixItem != null)
                        {
                            foreach (var skill in item.prefixItem.skillsMasterList)
                            {
                                summedSkillList.Add(skill);
                            }
                        }

                        if (item.baseItem != null)
                        {
                            foreach (var skill in item.baseItem.skillsMasterList)
                            {
                                summedSkillList.Add(skill);
                            }
                        }

                        if (item.suffixItem != null)
                        {
                            foreach (var skill in item.suffixItem.skillsMasterList)
                            {
                                summedSkillList.Add(skill);
                            }
                        }
                    }

                }
            }


            //set detail Ability strings
            detailAbilityString =
                "Experience: " + unit.experience + " (to next level: " + unit.toNextLevel + ") \n"
                + battleUnit.combat.shieldMax + " Shield \n"
                + battleUnit.combat.hitPointMax + " HP\n"
                + battleUnit.combat.attack + " Attack \n"
                + battleUnit.combat.accuracy + " Accuracy \n"
                + battleUnit.combat.mobility + " Mobility \n"
                + battleUnit.combat.defense + " Defense\n"
                + battleUnit.combat.numberOfAttacks + " Number of Attacks\n"
                + "(P:" + battleUnit.ability.power + " G:" + battleUnit.ability.generation
                + " S:" + battleUnit.ability.stability + " R:" + battleUnit.ability.responsiveness
                + " P:" + battleUnit.ability.precision + " I:" + battleUnit.ability.intelligence
                + " L:" + battleUnit.ability.luck + ")\n"
                + " \n"

                + "<Offense> \n"
                + "[Critical: x" + Math.Round(battleUnit.offenseMagnification.critical * 100) / 100 + "] "
                + " ("
                + " x" + Math.Round(summedOffenseList[1].ratioValue, 3)
                + " & {" + summedOffenseList[1].percents + "}) \n"
                + "[Kinetic: x" + Math.Round(battleUnit.offenseMagnification.kinetic * 100) / 100 + "] "
                + " ("
                + " x" + Math.Round(summedOffenseList[2].ratioValue, 3)
                + " & {" + summedOffenseList[2].percents + "}) \n"

                + "[Chemical: x" + Math.Round(battleUnit.offenseMagnification.chemical * 100) / 100 + "] "
                + " ("
                + " x" + Math.Round(summedOffenseList[3].ratioValue, 3)
                + " & {" + summedOffenseList[3].percents + "}) \n"

                + "[Thermal: x" + Math.Round(battleUnit.offenseMagnification.thermal * 100) / 100 + "] "
                + " ("
                + " x" + Math.Round(summedOffenseList[4].ratioValue, 3)
                + " & {" + summedOffenseList[4].percents + "}) \n"

                + "[OptimumRangeBonus: x" + Math.Round(battleUnit.offenseMagnification.optimumRangeBonus * 100) / 100 +
                "] "
                + " ("
                + " x" + Math.Round(summedOffenseList[10].ratioValue, 3)
                + " & {" + summedOffenseList[10].percents + "}) \n"

                + " \n"

                + "<Defense> \n"
                + "[Critical: x" + Math.Round(battleUnit.defenseMagnification.critical * 100) / 100 + "] "
                + " ("
                + " x" + Math.Round(summedDefenseList[1].ratioValue, 3)
                + " & {" + summedDefenseList[1].percents + "}) \n"

                + "[Kinetic: x" + Math.Round(battleUnit.defenseMagnification.kinetic * 100) / 100 + "] "
                + " ("
                + " x" + Math.Round(summedDefenseList[2].ratioValue, 3)
                + " & {" + summedDefenseList[2].percents + "}) \n"

                + "[Chemical: x" + Math.Round(battleUnit.defenseMagnification.chemical * 100) / 100 + "] "
                + " ("
                + " x" + Math.Round(summedDefenseList[3].ratioValue, 3)
                + " & {" + summedDefenseList[3].percents + "}) \n"

                + "[Thermal: x" + Math.Round(battleUnit.defenseMagnification.thermal * 100) / 100 + "]"
                + " ("
                + " x" + Math.Round(summedDefenseList[4].ratioValue, 3)
                + " & {" + summedDefenseList[4].percents + "}) \n"

                + " \n"
                + "[Skills] \n";

            if (summedSkillList.Count == 0)
            {
                detailAbilityString += " (none)";
            }

            foreach (var skill in summedSkillList)
            {
                detailAbilityString += skill.skillName + " (usage: " + skill.usageCount + ", veiledTurn: " + skill.veiledTurn + ")\n";
            }



            //Console.WriteLine("unit name in CalculateUnitStatus :" + unit.TrueName());
            //Debug.Log("unit name in CalculateUnitStatus :" + unit.TrueName());


        }

        private (int magnificationTargetID, int _percent, double _ratio, List<int> _percentList) MagnificationTypeModule(MagnificationMasterClass magnificationClass)
        {
            int magnificationTargetId = (int)magnificationClass.magnificationTarget;
            int percentSummed = 0;
            //double _fixedRatioSummed = 1.0;
            double ratioSummed = 1.0;
            List<int> percentList = new List<int>();
            switch (magnificationClass.magnificationType)
            {
                case MagnificationType.None:
                    break;
                case MagnificationType.AdditionalPercent:
                    // values and Enum ID is equal
                    percentSummed += (int)magnificationClass.magnificationPercent;
                    percentList.Add((int)magnificationClass.magnificationPercent);
                    break;
                case MagnificationType.MagnificationRatio:
                    ratioSummed *= magnificationClass.magnificationRatio;
                    break;
                default:
                    Debug.LogError("unexpected value:" + magnificationClass.magnificationType);
                    break;
            }

            return (magnificationTargetId, percentSummed, ratioSummed, percentList);
        }


        private class MagnificationComparer : IEqualityComparer<MagnificationMasterClass>
        {
            public bool Equals(MagnificationMasterClass iLhs, MagnificationMasterClass iRhs)
            {
                if (
                    iLhs.offenseOrDefense == iRhs.offenseOrDefense &&
                    iLhs.magnificationPercent == iRhs.magnificationPercent &&
                    Math.Abs(iLhs.magnificationRatio - iRhs.magnificationRatio) < Double.Epsilon &&
                    iLhs.magnificationTarget == iRhs.magnificationTarget &&
                    iLhs.magnificationType == iRhs.magnificationType
                )
                {
                    return true;
                }
                return false;
            }

            public int GetHashCode(MagnificationMasterClass iObj)
            {
                return iObj.offenseOrDefense.GetHashCode()
                       ^ iObj.magnificationPercent.GetHashCode()
                       ^ iObj.magnificationRatio.GetHashCode()
                       ^ iObj.magnificationTarget.GetHashCode()
                       ^ iObj.magnificationType.GetHashCode();
            }
        }

    }

}
