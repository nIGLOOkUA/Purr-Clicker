using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PurrClicker.Services;

namespace PurrClicker.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly FarmingService _farmingService = new();
    private CancellationTokenSource? _cts;

    [ObservableProperty] private bool _useKeyboard = true;
    [ObservableProperty] private bool _useMouse = false;
    [ObservableProperty] private int _mouseX = 960;
    [ObservableProperty] private int _mouseY = 540;
    [ObservableProperty] private int _delayMs = 40;
    [ObservableProperty] private bool _isRunning;
    [ObservableProperty] private string _statusText = "Стан: Зупинено";
    [ObservableProperty] private string _buttonText = "Старт";

    [RelayCommand]
    private async Task ToggleFarmingAsync()
    {
        if (IsRunning)
        {
            _cts?.Cancel();
            IsRunning = false;
            ButtonText = "Старт";
            StatusText = "Стан: Зупинено";
        }
        else
        {
            if (!UseKeyboard && !UseMouse)
            {
                StatusText = "Оберіть хоча б один режим!";
                return;
            }

            _cts = new CancellationTokenSource();
            IsRunning = true;
            ButtonText = "Стоп";
            StatusText = "Стан: Фармить...";

            try
            {
                await _farmingService.StartAsync(UseKeyboard, UseMouse, MouseX, MouseY, DelayMs, _cts.Token);
            }
            catch (TaskCanceledException)
            {
                // Потік успішно скасовано
            }
        }
    }
}