using JetBrains.ReSharper.Daemon.Xaml.Highlightings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Xaml.Impl.Util;
using JetBrains.ReSharper.Psi.Xaml.Tree;

namespace ReSharper.RpCorrector
{
    [StaticSeverityHighlighting(Severity.ERROR, 
        "XAMLErrors", 
        AttributeId = "ReSharper Error Underlined")]
    public class XamlMissingReactivePropertyValueHighlighting : XamlResolveErrorHighlighting
    {
        public XamlMissingReactivePropertyValueHighlighting(IMarkupPropertyReference reference)
            : base(reference, CreateTooltip(reference))
        {
        }

        private static string CreateTooltip(IMarkupPropertyReference reference)
        {
            var languageType = ReferenceUtil.GetProjectLanguage(reference.GetTreeNode());
            if (languageType.IsNullOrUnknown())
                return $"Cannot resolve symbol '{reference.GetName()}'";
            return $"'.Value' is not specified ReactiveProperty field or property '{reference.GetName()}'";
        }
    }
}