using _00_Asset.I2.Localization.Scripts.Utils;

namespace _00_Asset.I2.Localization.Examples.Common.Scripts
{

    public class GlobalParametersExample : RegisterGlobalParameters
	{
		public override string GetParameterValue( string ParamName )
        {
            if (ParamName == "WINNER")
                return "Javier";            // For your game, get this value from your Game Manager
            
            if (ParamName == "NUM PLAYERS")
                return 5.ToString();

            return null;
        }

	}
}