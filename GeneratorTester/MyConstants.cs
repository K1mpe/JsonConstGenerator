using JsonConstGenerator;

namespace GeneratorTester
{
    [JsonConstGenerator("SubFolder/*.json", "*.json", UpperCamelCase = true)]
    public partial class MyConstants
    {

        private void Method()
        {
        }
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
