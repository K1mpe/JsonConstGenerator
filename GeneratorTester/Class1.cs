using JsonConstGenerator;

namespace GeneratorTester
{
    [JsonConstGenerator("*.json")]
    public partial class MyConstants
    {
        public static void Blub()
        {
            var x = Flags.IsEnabled;

        }
    }
}
