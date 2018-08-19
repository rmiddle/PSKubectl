using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;
using KubeClient;
using YamlDotNet.Serialization;
using System.IO;
using System.Runtime.InteropServices;

namespace Kubectl {
    [Cmdlet(VerbsCommon.Set, "KubeConfig", SupportsShouldProcess = true)]
    [OutputType(new[] { typeof(K8sConfig) })]
    public sealed class SetKubeConfigCmdlet : AsyncCmdlet {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public K8sConfig Config;

        protected override async Task ProcessRecordAsync(CancellationToken cancellationToken) {
            await base.ProcessRecordAsync(cancellationToken);
            Serializer serializer = new SerializerBuilder().Build();
            string yaml = serializer.Serialize(Config);
            string configPath = ConfigHelpers.LocateConfig();
            if (ShouldProcess(configPath, "update")) {
                await File.WriteAllTextAsync(configPath, yaml); // Do not pass cancellationToken to not corrupt config file
            }
        }
    }
}