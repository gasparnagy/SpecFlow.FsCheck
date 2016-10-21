using System.Collections.Generic;

namespace SpecFlow.FsCheck
{
    public class ActualParamDictionary : ParamDictionaryBase<object>
    {
        internal ActualParamDictionary(IEnumerable<KeyValuePair<string, object>> items) : base(items)
        {
        }
    }
}