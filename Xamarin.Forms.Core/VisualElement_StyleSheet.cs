using System.Collections.Generic;
using System.Reflection;

using Xamarin.Forms.Internals;
using Xamarin.Forms.StyleSheets;

namespace Xamarin.Forms
{
	public partial class VisualElement : IStyleSelectable, IStylable
	{
		IList<string> IStyleSelectable.Classes
			=> StyleClass;

		BindableProperty IStylable.GetProperty(string key)
		{
			StylePropertyAttribute styleAttribute;
			if (!Internals.Registrar.StyleProperties.TryGetValue(key, out styleAttribute))
				return null;

			if (!styleAttribute.TargetType.GetTypeInfo().IsAssignableFrom(GetType().GetTypeInfo()))
				return null;

			if (styleAttribute.BindableProperty != null)
				return styleAttribute.BindableProperty;

			var bpField = GetType().GetField(styleAttribute.BindablePropertyName);
			if (bpField == null || !bpField.IsStatic)
				return null;

			return (styleAttribute.BindableProperty = bpField.GetValue(null) as BindableProperty);
		}

		void ApplyStyleSheets()
		{
			foreach (var styleSheet in this.GetStyleSheets())
				((IStyle)styleSheet).Apply(this);
		}

		//on parent set, or on parent stylesheet changed, reapply all
		void ApplyStyleSheetsOnParentSet()
		{
			var parent = Parent;
			if (parent == null)
				return;
			var sheets = new List<StyleSheet>();
			while (parent != null) {
				var visualParent = parent as VisualElement;
				var vpSheets = visualParent?.GetStyleSheets();
				if (vpSheets != null)
					sheets.AddRange(vpSheets);
				parent = parent.Parent;
			}
			for (var i = sheets.Count - 1; i >= 0; i--)
				((IStyle)sheets[i]).Apply(this);
		}
	}
}