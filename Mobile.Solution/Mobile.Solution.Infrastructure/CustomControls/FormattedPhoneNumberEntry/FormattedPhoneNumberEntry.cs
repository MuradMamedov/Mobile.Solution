using Xamarin.Forms;

namespace Mobile.Solution.Infrastructure.CustomControls
{
    public class FormattedPhoneNumberEntry : Entry
    {
        public bool ShouldReactToTextChanges { get; set; } = true;

        protected override void OnPropertyChanged(string propertyName = null)
        {
            var _limit = 15;

            if (nameof(Text).Equals(propertyName))
            {
                if (!ShouldReactToTextChanges) return;
                ShouldReactToTextChanges = false;

                var _text = Text;
                if (_text.Length > _limit)
                {
                    _text = _text.Remove(_text.Length - 1);
                    Text = _text;
                    ShouldReactToTextChanges = true;
                    return;
                }

                var oldText = Text;
                var number = GetNumber(oldText);
                var newText = number > 999999999 ? $"{number:(###)-###-##-##}" : $"{number:### ### ## ##}";

                Text = number == 0 ? "" : newText;
                ShouldReactToTextChanges = true;
            }
            base.OnPropertyChanged(propertyName);
        }

        public static ulong GetNumber(string input)
        {
            ulong number = 0;
            ulong multiply = 1;
            for (var i = input.Length - 1; i >= 0; i--)
                if (char.IsDigit(input[i]))
                {
                    number += (ulong) (input[i] - '0') * multiply;
                    multiply *= 10;
                }
            return number;
        }
    }
}