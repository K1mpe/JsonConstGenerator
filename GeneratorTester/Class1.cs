using JsonConstGenerator;

namespace GeneratorTester
{
    [JsonConstGenerator("example.json", "example2.json", UpperCamelCase = true)]
    public partial class MyConstants
    {
    }


    public class Class2
    {
        public void Method2()
        {
            var x = MyConstants.Numbers.DecimalValue;
            var y = MyConstants.FromSecondExample;

            string[] permissions = MyConstants.Permissions;
        }
    }
}
