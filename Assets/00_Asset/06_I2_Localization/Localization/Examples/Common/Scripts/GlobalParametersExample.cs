namespace I2.Loc
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