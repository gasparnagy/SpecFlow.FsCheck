using System;
using System.Collections.Generic;
using BoDi;
using FsCheck;
using TechTalk.SpecFlow.Infrastructure;

namespace SpecFlow.FsCheck
{
    public class ConstraintsBase : IContainerDependentObject
    {
        private IObjectContainer scenarioContainer;
        protected PropertyBasedTestContext PropertyBasedTestContext => scenarioContainer.Resolve<PropertyBasedTestContext>();

        protected TArg AsParam<TArg>(string label, Gen<TArg> gen)
        {
            return AsParam(label, Arb.From(gen));
        }

        protected TArg AsParam<TArg>(string label, Arbitrary<TArg> arb)
        {
            if (!PropertyBasedTestContext.IsPreparation)
                return (TArg)PropertyBasedTestContext.ActualParams[label];

            if (!PropertyBasedTestContext.ParamDefinitions.ContainsKey(label))
                PropertyBasedTestContext.ParamDefinitions.Add(label, arb);
            return default(TArg);
        }

        protected TResult AsFormula<TResult>(Func<ParamDictionaryBase<object>, TResult> formula)
        {
            if (!PropertyBasedTestContext.IsPreparation)
                return formula(PropertyBasedTestContext.ActualParams);

            return default(TResult);
        }

        void IContainerDependentObject.SetObjectContainer(IObjectContainer container)
        {
            scenarioContainer = container;
        }
    }
}