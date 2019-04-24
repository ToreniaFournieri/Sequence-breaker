using UnityEngine;
using System.Collections;

namespace KohmaiWorks.Scroller
{
    public class Data
    {
        public string firstLine;
        public string someText;

        /// <summary>
        /// We will store the cell size in the model so that the cell view can update it
        /// </summary>
        public float cellSize;

        public Affiliation affiliation;
    }

}