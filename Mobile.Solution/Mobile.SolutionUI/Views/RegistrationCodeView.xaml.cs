using Mobile.Solution.UI.ViewModels;

namespace Mobile.Solution.UI.Views
{
    public partial class RegistrationCodeView
    {
        public RegistrationCodeView(RegistrationViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            Entry.TextChanged += (sender, args) =>
            {
                var _text = Entry.Text;
                if (_text.Length > 6)
                {
                    _text = _text.Remove(_text.Length - 1);
                    Entry.Text = _text;
                }
            };
        }
    }
}