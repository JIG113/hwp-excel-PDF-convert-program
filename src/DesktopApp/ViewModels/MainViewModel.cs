using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Converter.Adapters.Hwp;
using Converter.Adapters.Office;
using Converter.Adapters.TextImage;
using Converter.Core.Conversion;
using Converter.Core.Jobs;
using DocToPdf.DesktopApp.Commands;
using DocToPdf.DesktopApp.Logging;
using Microsoft.Extensions.Logging;

namespace DocToPdf.DesktopApp.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly ILoggerFactory _loggerFactory;
    private string _outputFolder = Path.Combine(Path.GetTempPath(), "doc-to-pdf-output");
    private int _totalProgress;

    public MainViewModel()
    {
        Directory.CreateDirectory(_outputFolder);

        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddProvider(new FileLoggerProvider());
        });

        AddFilesCommand = new RelayCommand(() => { });
        SelectOutputFolderCommand = new RelayCommand(SelectOutputFolder);
        StartCommand = new RelayCommand(async () => await StartAsync(), () => Jobs.Count > 0);
    }

    public ObservableCollection<JobItemViewModel> Jobs { get; } = new();

    public RelayCommand AddFilesCommand { get; }
    public RelayCommand SelectOutputFolderCommand { get; }
    public RelayCommand StartCommand { get; }

    public int TotalProgress
    {
        get => _totalProgress;
        set { _totalProgress = value; OnPropertyChanged(); }
    }

    public void AddFiles(IReadOnlyCollection<string> files)
    {
        foreach (var file in files)
        {
            Jobs.Add(new JobItemViewModel { InputPath = file });
        }

        StartCommand.RaiseCanExecuteChanged();
    }

    private void SelectOutputFolder()
    {
        using var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _outputFolder = dialog.SelectedPath;
        }
    }

    private async Task StartAsync()
    {
        var logger = _loggerFactory.CreateLogger<OfficeConverter>();
        var converters = new IConverter[]
        {
            new HwpConverter(),
            new OfficeConverter(new ProcessCommandRunner(), logger),
            new TextImageConverter(),
        };

        var manager = new JobManager(new ConverterRouter(converters));
        var requests = Jobs.Select(j => new ConversionRequest(
            j.InputPath,
            Path.Combine(_outputFolder, Path.GetFileNameWithoutExtension(j.InputPath) + ".pdf"),
            new Dictionary<string, string>())).ToList();

        var results = await manager.RunBatchAsync(requests);

        foreach (var result in results)
        {
            var item = Jobs.First(x => x.InputPath == result.InputPath);
            item.Status = result.Status.ToString();
            item.Progress = result.Status == ConversionStatus.Succeeded ? 100 : 0;
            item.ErrorMessage = result.ErrorMessage;
        }

        TotalProgress = Jobs.Count == 0 ? 0 : Jobs.Average(j => j.Progress);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
