using FsCheck;

namespace SpecFlow.FsCheck
{
    public class ParamDefinitionDictionary : ParamDictionaryBase<object>
    {
        internal ParamDefinitionDictionary() : base()
        {
        }

        public void Add<TItem>(string label, Arbitrary<TItem> arb)
        {
            base.Add(label, arb);
        }
    }
}