using JetBrains.ReSharper.Daemon.Xaml.Highlightings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Xaml.Impl.Util;
using JetBrains.ReSharper.Psi.Xaml.Tree;
using ReSharper.RpCorrector;

[assembly: RegisterConfigurableSeverity(
    XamlMissingReactivePropertyValueHighlighting.HIGHLIGHTING_ID,
    "XAML",
    "XAMLErrors",
    XamlMissingReactivePropertyValueHighlighting.MESSAGE,
    "Missing ReactiveProperty's '.Value' field or property.",
    Severity.ERROR)]

namespace ReSharper.RpCorrector
{
    [ConfigurableSeverityHighlighting(HIGHLIGHTING_ID, "XAML",
        AttributeId = "ReSharper Error Underlined",
        Languages = "XAML",
        OverlapResolve = OverlapResolveKind.ERROR,
        ToolTipFormatString = MESSAGE)]
    public class XamlMissingReactivePropertyValueHighlighting : XamlResolveErrorHighlightingBase
    {
        public const string MESSAGE = "'.Value' is not specified ReactiveProperty field or property '{0}'";
        public const string HIGHLIGHTING_ID = "Xaml.MissingReactivePropertyValue";

        public XamlMissingReactivePropertyValueHighlighting(IMarkupPropertyReference reference)
            : base(reference, CreateTooltip(reference))
        {
        }

        private static string CreateTooltip(IMarkupPropertyReference reference)
        {
            var languageType = ReferenceUtil.GetProjectLanguage(reference.GetTreeNode());
            if (languageType.IsNullOrUnknown())
                return $"Cannot resolve symbol '{reference.GetName()}'";
            return string.Format(MESSAGE, reference.GetName());
        }
    }
}