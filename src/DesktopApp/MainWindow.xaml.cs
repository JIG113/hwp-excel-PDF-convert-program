using System.Linq;
using System.Windows;
using DocToPdf.DesktopApp.ViewModels;

namespace DocToPdf.DesktopApp;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
    }

    private void OnDropFiles(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(DataFormats.FileDrop) is string[] files)
        {
            _viewModel.AddFiles(files.ToList());
        }
    }
}
