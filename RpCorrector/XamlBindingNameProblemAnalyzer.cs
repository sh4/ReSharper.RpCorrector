using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Daemon.Xaml.Highlightings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Xaml.Tree;
using JetBrains.ReSharper.Psi.Xaml.Tree.MarkupExtensions;
using JetBrains.Util;

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
        protected override void Run(IBindingMarkup element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            var propertyExpression = element?.Path as IPropertyExpression;
            if (propertyExpression == null || propertyExpression.Dot != null)
                return;
            var propertyReference = propertyExpression.PropertyReference as IMarkupPropertyReference;
            if (propertyReference == null)
                return;
            var info = propertyReference.Resolve();
            if (info.DeclaredElement.IsCSharpProperty() == false)
                return;
            var property = info.DeclaredElement as IProperty;
            var scalarType = property?.ReturnType?.GetScalarType();
            if (scalarType == null)
                return;
            if (IsSuperTypeByCLRName("Reactive.Bindings.IReadOnlyReactiveProperty", scalarType) == false)
                return;

            var highlighting = new XamlMissingReactivePropertyValueHighlighting(propertyReference);
            consumer.AddHighlighting(highlighting);
        }

        private bool IsSuperTypeByCLRName(string clrName, IDeclaredType type)
        {
            var interfaceType = TypeFactory.CreateTypeByCLRName(clrName, type.Module)?.GetTypeElement();
            if (interfaceType == null)
                return false;
            if (type.GetSuperType(interfaceType).IsEmpty())
                return false;
            return true;
        }
    }
}