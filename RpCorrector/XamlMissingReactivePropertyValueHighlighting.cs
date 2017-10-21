using JetBrains.ReSharper.Daemon.Xaml.Highlightings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Xaml.Impl.Util;
using JetBrains.ReSharper.Psi.Xaml.Tree;
using ReSharper.RpCorrector;

[assembly: RegisterConfigurableSeverity(
    XamlMissingReactivePropertyValueHighlighting.HighlightingId,
    "XAML",
    "XAMLErrors",
    XamlMissingReactivePropertyValueHighlighting.Message,
    "Missing ReactiveProperty's '.Value' field or property.",
    Severity.ERROR)]

namespace ReSharper.RpCorrector
{
    [ConfigurableSeverityHighlighting(HighlightingId, "XAML",
        AttributeId = "ReSharper Error Underlined",
        Languages = "XAML",
        OverlapResolve = OverlapResolveKind.ERROR,
        ToolTipFormatString = Message)]
    public class XamlMissingReactivePropertyValueHighlighting : XamlResolveErrorHighlightingBase
    {
        public const string Message = "'.Value' is not specified ReactiveProperty field or property '{0}'";
        public const string HighlightingId = "Xaml.MissingReactivePropertyValue";

        public XamlMissingReactivePropertyValueHighlighting(IMarkupPropertyReference reference)
            : base(reference, CreateTooltip(reference))
        {
        }

        private static string CreateTooltip(IMarkupPropertyReference reference)
        {
            var languageType = ReferenceUtil.GetProjectLanguage(reference.GetTreeNode());
            if (languageType.IsNullOrUnknown())
                return $"Cannot resolve symbol '{reference.GetName()}'";
            return string.Format(Message, reference.GetName());
        }
    }
}