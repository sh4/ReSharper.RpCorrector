using System.Linq;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Daemon.Xaml.Highlightings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Xaml.Tree;
using JetBrains.ReSharper.Psi.Xaml.Tree.MarkupExtensions;

namespace ReSharper.RpCorrector
{
    [ElementProblemAnalyzer(typeof(IBindingMarkup),
        HighlightingTypes = new []
        {
            typeof(XamlMissingReactivePropertyValueHighlighting),
            typeof(XamlErrorHighlighting)
        })]
    public class XamlBindingNameProblemAnalyzer : ElementProblemAnalyzer<IBindingMarkup>
    {
        private const string ReactivePropertyInterface = "Reactive.Bindings.IReadOnlyReactiveProperty";

        protected override void Run(IBindingMarkup element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            var reference = GetMarkupPropertyReference(element);
            if (reference == null)
                return;
            var scalarType = GetCSharpPropertyScalarType(reference);
            if (scalarType == null || IsSuperTypeByClrName(ReactivePropertyInterface, scalarType) == false)
                return;

            var highlighting = new XamlMissingReactivePropertyValueHighlighting(reference);
            consumer.AddHighlighting(highlighting);
        }

        private static IMarkupPropertyReference GetMarkupPropertyReference(IBindingMarkup element)
        {
            if (!(element?.Path is IPropertyExpression propertyExpression) || propertyExpression.Dot != null)
                return null;
            return propertyExpression.PropertyReference as IMarkupPropertyReference;
        }

        private static IDeclaredType GetCSharpPropertyScalarType(IReference reference)
        {
            var info = reference.Resolve();
            if (info.DeclaredElement.IsCSharpProperty() == false)
                return null;
            var property = info.DeclaredElement as IProperty;
            return property?.ReturnType.GetScalarType();
        }

        private static bool IsSuperTypeByClrName(string clrName, IDeclaredType type)
        {
            var interfaceType = TypeFactory.CreateTypeByCLRName(clrName, type.Module).GetTypeElement();
            return interfaceType != null && type.GetSuperType(interfaceType).Any();
        }
    }
}