using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Xaml;

namespace ReSharper.RpCorrector
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<DaemonZone>, IRequire<ILanguageXamlZone>
    {
        
    }
}