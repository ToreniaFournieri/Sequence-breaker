using System.Collections.Generic;
using SequenceBreaker.Master.Skills;
using UnityEngine;

namespace SequenceBreaker.Master.Units
{
    [CreateAssetMenu(fileName = "TuningStyleMaster", menuName = "Environment/TuningStyleMaster", order = 3)]
    public class TuningStyleMaster : ScriptableObject
    {
        //for each Tuning Type.
        public List<SkillsMasterClass> 壁skills;
        public List<SkillsMasterClass> 楯skills;
        public List<SkillsMasterClass> 駆skills;
        public List<SkillsMasterClass> 戦skills;
        public List<SkillsMasterClass> 載skills;
        public List<SkillsMasterClass> 偵skills;
        public List<SkillsMasterClass> 妨skills;
        public List<SkillsMasterClass> 揮skills;
        public List<SkillsMasterClass> 救skills;
        public List<SkillsMasterClass> 砲skills;
        public List<SkillsMasterClass> 炸skills;
        public List<SkillsMasterClass> 狙skills;


        public List<SkillsMasterClass> GetSkills(TuningStyle tuningStyle)
        {
            List<SkillsMasterClass> skills = new List<SkillsMasterClass>();

            switch (tuningStyle)
            {
                case TuningStyle.壁:
                    skills = 壁skills;
                    break;
                case TuningStyle.楯:
                    skills = 壁skills;
                    break;
                case TuningStyle.駆:
                    skills = 壁skills;
                    break;
                case TuningStyle.戦:
                    skills = 壁skills;
                    break;
                case TuningStyle.載:
                    skills = 壁skills;
                    break;
                case TuningStyle.偵:
                    skills = 壁skills;
                    break;
                case TuningStyle.妨:
                    skills = 壁skills;
                    break;
                case TuningStyle.揮:
                    skills = 壁skills;
                    break;
                case TuningStyle.救:
                    skills = 壁skills;
                    break;
                case TuningStyle.砲:
                    skills = 壁skills;
                    break;
                case TuningStyle.炸:
                    skills = 壁skills;
                    break;
                case TuningStyle.狙:
                    skills = 狙skills;
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