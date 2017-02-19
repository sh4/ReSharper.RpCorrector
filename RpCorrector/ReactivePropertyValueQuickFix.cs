using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
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

        public ReactivePropertyValueQuickFix(XamlMissingReactivePropertyValueHighlighting highlighting)
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

        public override string Text => "Add '.Value' to ReactiveProperty field or property";
        public override bool IsAvailable(IUserDataHolder cache) => _reference != null;
    }
}