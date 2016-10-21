using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.Infrastructure;

namespace SpecFlow.FsCheck
{
    /// <summary>
    /// This context lives in the test thread container, so has to be cleared out before every scenario.
    /// </summary>
    public class PropertyBasedTestContext
    {
        public ParamDefinitionDictionary ParamDefinitions = new ParamDefinitionDictionary();
        public ActualParamDictionary ActualParams { get; internal set; }
        internal List<Action<ITestExecutionEngine>> StepsToReplay = new List<Action<ITestExecutionEngine>>();

        public bool IsPreparation => IsPropertyBasedScenario && ActualParams == null;
        public bool IsPropertyBasedScenario { get; private set; }

        public void OnPropertyBasedScenarioStart()
        {
            Clear();
            IsPropertyBasedScenario = true;
        }

        public void OnPropertyBasedScenarioEnd()
        {
            IsPropertyBasedScenario = false;
            Clear();
        }

        private void Clear()
        {
            ParamDefinitions.Clear();
            ActualParams = null;
            StepsToReplay.Clear();
        }
    }
}