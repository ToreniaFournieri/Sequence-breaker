using _00_Asset.StompyRobot.SRF.Scripts.Helpers;

namespace _00_Asset.StompyRobot.SRDebugger.Scripts.Internal
{
    public class OptionDefinition
    {
        private OptionDefinition(string name, string category, int sortPriority)
        {
            Name = name;
            Category = category;
            SortPriority = sortPriority;
        }

        public OptionDefinition(string name, string category, int sortPriority, MethodReference method)
            : this(name, category, sortPriority)
        {
            Method = method;
        }

        public OptionDefinition(string name, string category, int sortPriority, PropertyReference property)
            : this(name, category, sortPriority)
        {
            Property = property;
        }

        public string Name { get; private set; }
        public string Category { get; private set; }
        public int SortPriority { get; private set; }
        public MethodReference Method { get; private set; }
        public PropertyReference Property { get; private set; }
    }
}
