using Avalonia.Controls;
using TechCraftLauncher.ViewModels;

namespace TechCraftLauncher.Views
{
    public partial class PotatoModsWindow : Window
    {
        public PotatoModsWindow()
        {
            InitializeComponent();
        }

        protected override void OnDataContextChanged(System.EventArgs e)
        {
            base.OnDataContextChanged(e);
            if (DataContext is PotatoModsViewModel vm)
            {
                vm.RequestClose += Close;
            }
        }
    }
}
