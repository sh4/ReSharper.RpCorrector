using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Daemon.Xaml.Highlightings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Xaml.Impl.Util;
using JetBrains.ReSharper.Psi.Xaml.Tree;
using JetBrains.ReSharper.Psi.Xaml.Tree.MarkupExtensions;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReSharper.RpCorrector
{
    [QuickFix]
    public class ReactivePropertyValueQuickFix : QuickFixBase
    {
        private readonly IMarkupPropertyReference _reference;

        public ReactivePropertyValueQuickFix(XamlMisassignedBindingReactivePropertyHighlighting highlighting)
        {
            _reference = highlighting.Reference as IMarkupPropertyReference;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var node = _reference.GetTreeNode();
            var markupAttribute = node.Parent as IMarkupAttribute;
            markupAttribute?.SetStringValue($"{_reference.GetName()}.Value");
            return null;
        }

        public override string Text
        {
            get { return "Add '.Value' to ReactiveProperty field or property"; }
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            return _reference != null;
        }
    }

    [StaticSeverityHighlighting(Severity.ERROR, 
        "XAMLErrors", 
        AttributeId = "ReSharper Error Underlined")]
    public class XamlMisassignedBindingReactivePropertyHighlighting : XamlResolveErrorHighlighting
    {
        public XamlMisassignedBindingReactivePropertyHighlighting(IMarkupPropertyReference reference)
            : base(reference, CreateTooltip(reference))
        {
            
        }

        private static string CreateTooltip(IMarkupPropertyReference reference)
        {
            var languageType = ReferenceUtil.GetProjectLanguage(reference.GetTreeNode());
            if (languageType.IsNullOrUnknown())
                return $"Cannot resolve symbol '{reference.GetName()}'";

            return string.Format(
                "'.Value' is not specified ReactiveProperty field or property '{0}'",
                reference.GetName());
        }
    }

    [ElementProblemAnalyzer(typeof(IBindingMarkup),
        HighlightingTypes = new []
        {
            typeof(XamlMisassignedBindingReactivePropertyHighlighting),
            typeof(XamlErrorHighlighting)
        })]
    public class XamlBindingNameProblemAnalyzer : ElementProblemAnalyzer<IBindingMarkup>
    {
        public XamlBindingNameProblemAnalyzer()
        {
            
        }
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

            var highlighting = new XamlMisassignedBindingReactivePropertyHighlighting(propertyReference);
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