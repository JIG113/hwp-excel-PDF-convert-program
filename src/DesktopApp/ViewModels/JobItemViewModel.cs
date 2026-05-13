using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DocToPdf.DesktopApp.ViewModels;

public sealed class JobItemViewModel : INotifyPropertyChanged
{
    private string _status = "Pending";
    private int _progress;
    private string? _errorMessage;

    public required string InputPath { get; init; }
    public string FileName => System.IO.Path.GetFileName(InputPath);

    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    public int Progress
    {
        get => _progress;
        set { _progress = value; OnPropertyChanged(); }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
