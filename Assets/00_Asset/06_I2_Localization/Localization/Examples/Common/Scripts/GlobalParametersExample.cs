using _00_Asset._06_I2_Localization.Localization.Scripts.Utils;

namespace _00_Asset._06_I2_Localization.Localization.Examples.Common.Scripts
{

    public class GlobalParametersExample : RegisterGlobalParameters
	{
		public override string GetParameterValue( string paramName )
        {
            if (paramName == "WINNER")
                return "Javier";            // For your game, get this value from your Game Manager
            
            if (paramName == "NUM PLAYERS")
                return 5.ToString();

            return null;
        }

	}
}