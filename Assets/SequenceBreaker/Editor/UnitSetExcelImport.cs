using System.Collections.Generic;
using _00_Asset._07_ExcelImporter;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    [ExcelAsset]
    public class UnitSetExcelImport : ScriptableObject
    {
        public List<UnitSetExcel> unitSetExcelList;

    }
}
