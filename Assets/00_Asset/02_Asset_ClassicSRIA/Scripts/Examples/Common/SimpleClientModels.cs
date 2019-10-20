

namespace _00_Asset._02_Asset_ClassicSRIA.Scripts.Examples.Common
{
	public class SimpleClientModel
	{
		public string ClientName;
		public string Location;
		public float Availability01, ContractChance01, LongTermClient01;
		public bool IsOnline;

		public float AverageScore01 { get { return (Availability01 + ContractChance01 + LongTermClient01) / 3; } }

		public void SetRandom()
		{
			Availability01 = CUtil.RandF();
			ContractChance01 = CUtil.RandF();
			LongTermClient01 = CUtil.RandF();
			IsOnline = CUtil.Rand(2) == 0;
		}
	}


	public class ExpandableSimpleClientModel : SimpleClientModel
	{
		// View size related
		public bool Expanded;
		public float NonExpandedSize;
	}
}
