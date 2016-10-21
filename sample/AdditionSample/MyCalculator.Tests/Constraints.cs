using System.Linq;
using FsCheck;
using SpecFlow.FsCheck;
using TechTalk.SpecFlow;

namespace MyCalculator.Tests
{
    [Binding]
    public class Constraints : ConstraintsBase
    {
        [StepArgumentTransformation("any number")]
        public int AnyNumber()
        {
            return AsParam("any", Arb.Default.Int32());
            //could be constrainded: AsParam("any", Gen.Choose(0, 100));
        }

        [StepArgumentTransformation("any number '([a-z]+)'")]
        public int AnyNumberLabeled(string label)
        {
            return AsParam(label, Arb.Default.Int32());
        }

        [StepArgumentTransformation("the first number")]
        public int TheFirstNumber()
        {
            return AsFormula(actualParams => (int)actualParams.First());
        }

        [StepArgumentTransformation(@"the same as ([a-z]+)\+([a-z]+)")]
        public int TheSameAsAddition(string op1Label, string op2Label)
        {
            return AsFormula(actualParams => Addition.Add((int)actualParams[op1Label], (int)actualParams[op2Label]));
        }
    }
}
