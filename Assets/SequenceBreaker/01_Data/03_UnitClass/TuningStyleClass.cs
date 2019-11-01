using System.Collections.Generic;
using System.Diagnostics;
using SequenceBreaker._01_Data._01_Skills;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SequenceBreaker._01_Data._03_UnitClass
{
//    [CreateAssetMenu(fileName = "T", menuName = "Unit/TuningStyleClass", order = 17)]
    public sealed class TuningStyleClass : MonoBehaviour
    {
        //for each Tuning Type.
        public List<SkillsMasterClass> commanderSkills;
        public List<SkillsMasterClass> destroyerSkills;
        public List<SkillsMasterClass> fighterSkills;
        public List<SkillsMasterClass> gunnerSkills;
        public List<SkillsMasterClass> jammerSkills;
        public List<SkillsMasterClass> lancerSkills;
        public List<SkillsMasterClass> medicSkills;
        public List<SkillsMasterClass> reconnoiterSkills;
        public List<SkillsMasterClass> sniperSkills;
        public List<SkillsMasterClass> tankSkills;
        
        
        public List<SkillsMasterClass> GetSkills(TuningStyle tuningStyle)
        {
            List<SkillsMasterClass> skills = new List<SkillsMasterClass>();
            
            switch (tuningStyle)
            {
                case TuningStyle.Commander:
                    skills = commanderSkills;
                    break;
                case TuningStyle.Destroyer:
                    skills = destroyerSkills;
                    break;
                case TuningStyle.Fighter:
                    skills = fighterSkills;
                    break;
                case TuningStyle.Gunner:
                    skills = gunnerSkills;
                    break;
                case TuningStyle.Jammer:
                    skills = jammerSkills;
                    break;
                case TuningStyle.Lancer:
                    skills = lancerSkills;
                    break;
                case TuningStyle.Medic:
                    skills = medicSkills;
                    break;
                case TuningStyle.Reconnoiter:
                    skills = reconnoiterSkills;
                    break;
                case TuningStyle.Sniper:
                    skills = sniperSkills;
                    break;
                case TuningStyle.Tank:
                    skills = tankSkills;
                    break;
                case TuningStyle.None:
                    break;
                default:
                    Debug.LogError("unexpected values :" + tuningStyle);
                    break;
                
            }

            return skills;
        }

        
    }

   
    
}
