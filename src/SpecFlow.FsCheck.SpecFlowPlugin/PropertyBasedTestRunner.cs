using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BoDi;
using FsCheck;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace SpecFlow.FsCheck
{
    public class PropertyBasedTestRunner : ITestRunner
    {
        private readonly ITestExecutionEngine normalExecutionEngine;
        private readonly ITestExecutionEngine nullExecutionEngine;
        private readonly PropertyBasedTestContext propertyBasedTestContext;

        public int ThreadId { get; private set; }

        class NullBindingInvoker : IBindingInvoker
        {
            public object InvokeBinding(IBinding binding, IContextManager contextManager, object[] arguments, ITestTracer testTracer,
                out TimeSpan duration)
            {
                duration = TimeSpan.Zero;
                return null;
            }
        }

        public PropertyBasedTestRunner(ITestExecutionEngine executionEngine, IObjectContainer container)
        {
            this.normalExecutionEngine = executionEngine;
            var nullContainer = new ObjectContainer(container);
            nullContainer.RegisterInstanceAs((IBindingInvoker)new NullBindingInvoker());
            this.nullExecutionEngine = nullContainer.Resolve<ITestExecutionEngine>();

            //TODO: check with newer SpecFlow if this is still necessary
            container.RegisterTypeAs<PropertyBasedTestContext, PropertyBasedTestContext>();
            propertyBasedTestContext = container.Resolve<PropertyBasedTestContext>();
        }

        public FeatureContext FeatureContext
        {
            get { return GetCurrentExecutionEngine().FeatureContext; }
        }

        public ScenarioContext ScenarioContext
        {
            get { return GetCurrentExecutionEngine().ScenarioContext; }
        }

        private ITestExecutionEngine GetCurrentExecutionEngine()
            => propertyBasedTestContext.IsPreparation ? nullExecutionEngine : normalExecutionEngine;

        public void OnTestRunStart()
        {
            GetCurrentExecutionEngine().OnTestRunStart();
        }

        public void InitializeTestRunner(int threadId)
        {
            ThreadId = threadId;
        }

        public void OnFeatureStart(FeatureInfo featureInfo)
        {
            GetCurrentExecutionEngine().OnFeatureStart(featureInfo);
        }

        public void OnFeatureEnd()
        {
            GetCurrentExecutionEngine().OnFeatureEnd();
        }

        public void OnScenarioStart(ScenarioInfo scenarioInfo)
        {
            if (scenarioInfo.Tags.Contains("propertyBased"))
            {
                propertyBasedTestContext.OnPropertyBasedScenarioStart();
                propertyBasedTestContext.StepsToReplay.Add(e => e.OnScenarioStart(scenarioInfo));
            }

            GetCurrentExecutionEngine().OnScenarioStart(scenarioInfo);
        }

        public void CollectScenarioErrors()
        {
            if (propertyBasedTestContext.IsPreparation)
                propertyBasedTestContext.StepsToReplay.Add(e => e.OnAfterLastStep());

            GetCurrentExecutionEngine().OnAfterLastStep();
        }

        public void OnScenarioEnd()
        {
            try
            {
                if (propertyBasedTestContext.IsPreparation)
                    propertyBasedTestContext.StepsToReplay.Add(e => e.OnScenarioEnd());

                GetCurrentExecutionEngine().OnScenarioEnd();

                if (propertyBasedTestContext.IsPreparation)
                {
                    Property property = CreateProperty(propertyBasedTestContext.ParamDefinitions);
                    property.VerboseCheckThrowOnFailure();
                }
            }
            finally
            {
                if (propertyBasedTestContext.IsPropertyBasedScenario)
                    propertyBasedTestContext.OnPropertyBasedScenarioEnd();
            }
        }

        private Property CreateProperty(ParamDictionaryBase<object> paramDefinitions)
        {
            if (paramDefinitions.Count > 3)
                throw new NotSupportedException("Maximum 3 property parameter is allowed!");

            if (paramDefinitions.Count == 0)
                throw new NotSupportedException("At least one input parameter is necessary for property based tesing!");

            var paramDefinitionsList = paramDefinitions.ToArray();
            var paramTypes = paramDefinitionsList.Select(pd => pd.GetType().GetGenericArguments()[0]).ToArray();

            var paramExpressions = paramTypes.Select(Expression.Parameter).ToArray();

            var genericActionTypes = new[] { typeof(Action), typeof(Action<>), typeof(Action<,>), typeof(Action<,,>) };
            var actionType = genericActionTypes[paramDefinitions.Count].MakeGenericType(paramTypes);
            var methodToInvoke = GetType().GetMethod(nameof(CallStepsWithActualParams), BindingFlags.Instance | BindingFlags.NonPublic, null, new []{ typeof(ParamDictionaryBase<object>), typeof(object[]) }, null);

            var expressionLambdaMethod = FindExpressionLambdaMethod(actionType);

            var callStepsWithActualParamsCall = Expression.Call(Expression.Constant(this), methodToInvoke, 
                Expression.Constant(paramDefinitions),
                Expression.NewArrayInit(typeof(object), paramExpressions.Select(pe => Expression.Convert(pe, typeof(object)))));

            var actionExpression = expressionLambdaMethod.Invoke(null, new object[] { callStepsWithActualParamsCall, paramExpressions });
            var actionCompileMethod = actionExpression.GetType().GetMethod(nameof(Expression<Action>.Compile), new Type[0]);

            var action = actionCompileMethod.Invoke(actionExpression, null);
            var propForAllMetod = FindForAllMethod(actionType, paramTypes);

            return (Property)propForAllMetod.Invoke(null, 
                paramDefinitionsList.Concat(new [] { action }).ToArray());
        }

        private MethodInfo FindExpressionLambdaMethod(Type actionType)
        {
            var genericMethod = typeof(Expression)
                .GetMembers(BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.Name == nameof(Expression.Lambda) && m.MemberType == MemberTypes.Method)
                .Cast<MethodInfo>()
                .Where(m => m.IsGenericMethodDefinition)
                .Single(m =>
                {
                    var parameterInfos = m.GetParameters();
                    return parameterInfos.Count() == 2 &&
                           parameterInfos[0].ParameterType == typeof(Expression) &&
                           parameterInfos[1].ParameterType == typeof(IEnumerable<ParameterExpression>);
                });
            return genericMethod.MakeGenericMethod(actionType);
        }

        private MethodInfo FindForAllMethod(Type actionType, params Type[] paramTypes)
        {
            var genericMethod = typeof (Prop)
                .GetMembers(BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.Name == nameof(Prop.ForAll) && m.MemberType == MemberTypes.Method)
                .Cast<MethodInfo>()
                .Single(m =>
                {
                    var parameterInfos = m.GetParameters();
                    return parameterInfos.Count() == paramTypes.Length + 1 &&
                           IsGenericTypeOf(typeof(Arbitrary<>), parameterInfos.First().ParameterType) &&
                           IsGenericTypeOf(actionType.GetGenericTypeDefinition(), parameterInfos.Last().ParameterType);
                });
            return genericMethod.MakeGenericMethod(paramTypes);
        }

        private bool IsGenericTypeOf(Type tGen, Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == tGen;
        }

        private void CallStepsWithActualParams(ParamDictionaryBase<object> paramDefinitions, params object[] parameters)
        {
            propertyBasedTestContext.ActualParams = new ActualParamDictionary(
                paramDefinitions.KeyValuePairs.Select((pd, i) => new KeyValuePair<string, object>(pd.Key, parameters[i])));
               
            foreach (var action in propertyBasedTestContext.StepsToReplay)
            {
                action(GetCurrentExecutionEngine());
            }
        }

        public void OnTestRunEnd()
        {
            GetCurrentExecutionEngine().OnTestRunEnd();
        }

        public void Given(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            if (propertyBasedTestContext.IsPreparation)
                propertyBasedTestContext.StepsToReplay.Add(e => e.Step(StepDefinitionKeyword.Given, keyword, text, multilineTextArg, tableArg));

            GetCurrentExecutionEngine().Step(StepDefinitionKeyword.Given, keyword, text, multilineTextArg, tableArg);
        }

        public void When(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            if (propertyBasedTestContext.IsPreparation)
                propertyBasedTestContext.StepsToReplay.Add(e => e.Step(StepDefinitionKeyword.When, keyword, text, multilineTextArg, tableArg));

            GetCurrentExecutionEngine().Step(StepDefinitionKeyword.When, keyword, text, multilineTextArg, tableArg);
        }

        public void Then(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            if (propertyBasedTestContext.IsPreparation)
                propertyBasedTestContext.StepsToReplay.Add(e => e.Step(StepDefinitionKeyword.Then, keyword, text, multilineTextArg, tableArg));

            GetCurrentExecutionEngine().Step(StepDefinitionKeyword.Then, keyword, text, multilineTextArg, tableArg);
        }

        public void And(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            if (propertyBasedTestContext.IsPreparation)
                propertyBasedTestContext.StepsToReplay.Add(e => e.Step(StepDefinitionKeyword.And, keyword, text, multilineTextArg, tableArg));

            GetCurrentExecutionEngine().Step(StepDefinitionKeyword.And, keyword, text, multilineTextArg, tableArg);
        }

        public void But(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            if (propertyBasedTestContext.IsPreparation)
                propertyBasedTestContext.StepsToReplay.Add(e => e.Step(StepDefinitionKeyword.But, keyword, text, multilineTextArg, tableArg));

            GetCurrentExecutionEngine().Step(StepDefinitionKeyword.But, keyword, text, multilineTextArg, tableArg);
        }

        public void Pending()
        {
            GetCurrentExecutionEngine().Pending();
        }
    }
}
