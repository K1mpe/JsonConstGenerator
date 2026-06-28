using JsonConstGenerator;

namespace GeneratorTester
{
    [JsonConstGenerator("*.json", "SubFolder/example.json", UpperCamelCase = true)]
    public partial class MyConstants
    {

    }


    public class Class2
    {
        public void Method2()
        {
            var x = MyConstants.Numbers.DecimalValue;
            var y = MyConstants.FromSecondExample;


            var a = MyConstants.Permissions.Companies.Read;


            var b = MyConstants.CollectorSettings.Global.Enabled.NodePath;

            var c = MyConstants.GetNodeValue<bool>(b);
        }

    }

}
