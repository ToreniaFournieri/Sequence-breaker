using UnityEngine;
using System.Collections;

namespace KohmaiWorks.Scroller
{
    public class Data
    {
        public string reactText;
        public string unitName;
        public string unitHealth;
        public string firstLine;
        public string mainText;

        /// <summary>
        /// We will store the cell size in the model so that the cell view can update it
        /// </summary>
        public float cellSize;
        public int nestLevel;

        public Affiliation affiliation;
    }

}