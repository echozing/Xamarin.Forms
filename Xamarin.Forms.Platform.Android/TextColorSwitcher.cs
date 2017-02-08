using System;
using Android.Content.Res;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	/// <summary>
	/// Handles color state management for the TextColor/PlaceholderColor properties
	/// for Entry, Button, Picker, TimePicker, and DatePicker
	/// </summary>
	internal class TextColorSwitcher
	{
		static readonly int[][] s_colorStates = { new[] { global::Android.Resource.Attribute.StateEnabled }, new[] { -global::Android.Resource.Attribute.StateEnabled } };

		readonly ColorStateList _defaultTextColors;
		Color _currentTextColor;

		public TextColorSwitcher(ColorStateList textColors)
		{
			_defaultTextColors = textColors;
		}

		public void UpdateTextColor(TextView control, Color color, Action<ColorStateList> setColor = null, bool preserveDisabled = true)
		{
			if (color == _currentTextColor)
				return;

			if (setColor == null)
			{
				setColor = control.SetTextColor;
			}

			_currentTextColor = color;

			if (color.IsDefault)
			{
				setColor(_defaultTextColors);
			}
			else
			{
				if (preserveDisabled)
				{
					// Set the new enabled state color, preserving the default disabled state color
					int defaultDisabledColor = _defaultTextColors.GetColorForState(s_colorStates[1], color.ToAndroid());
					setColor(new ColorStateList(s_colorStates, new[] { color.ToAndroid().ToArgb(), defaultDisabledColor }));
				}
				else
				{
					var acolor = color.ToAndroid().ToArgb();
					setColor(new ColorStateList(s_colorStates, new[] { acolor, acolor }));
				}
			}
		}

	}
}