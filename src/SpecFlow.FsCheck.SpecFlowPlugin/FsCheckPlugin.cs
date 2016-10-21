using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecFlow.FsCheck;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Plugins;

[assembly: RuntimePlugin(typeof(FsCheckPlugin))]

namespace SpecFlow.FsCheck
{
    public class FsCheckPlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters)
        {
            runtimePluginEvents.CustomizeTestThreadDependencies += (sender, args) =>
            {
                args.ObjectContainer.RegisterTypeAs<PropertyBasedTestRunner, ITestRunner>();
            };
        }
    }
}
