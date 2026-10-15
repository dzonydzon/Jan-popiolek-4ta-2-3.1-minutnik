using System;
using Avalonia.Controls;
using Avalonia.Threading;

namespace okno;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private TimeSpan remainingTime = TimeSpan.FromMinutes(25);
    private bool isWorkPhase = true;
    private bool isRunning;
    private bool hasStarted;
    private int completedCycles;

    public MainWindow()
    {
        InitializeComponent();

        timer.Tick += OnTimerTick;
        WorkDurationInput.ValueChanged += OnDurationValueChanged;
        BreakDurationInput.ValueChanged += OnDurationValueChanged;
        UpdateDisplay();
    }

    private void OnStartPauseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (isRunning)
        {
            timer.Stop();
            isRunning = false;
        }
        else
        {
            if (!hasStarted)
            {
                remainingTime = GetCurrentPhaseDuration();
                hasStarted = true;
            }

            timer.Start();
            isRunning = true;
        }

        UpdateDisplay();
    }

    private void OnResetClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        timer.Stop();
        isRunning = false;
        hasStarted = false;
        isWorkPhase = true;
        completedCycles = 0;
        remainingTime = GetCurrentPhaseDuration();
        UpdateDisplay();
    }

    private void OnDurationValueChanged(object? sender, EventArgs e)
    {
        if (!isRunning && !hasStarted)
        {
            remainingTime = GetCurrentPhaseDuration();
            UpdateDisplay();
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        remainingTime -= TimeSpan.FromSeconds(1);

        if (remainingTime <= TimeSpan.Zero)
        {
            if (isWorkPhase)
            {
                completedCycles++;
            }

            isWorkPhase = !isWorkPhase;
            remainingTime = GetCurrentPhaseDuration();
        }

        UpdateDisplay();
    }

    private TimeSpan GetCurrentPhaseDuration()
    {
        var minutes = isWorkPhase
            ? WorkDurationInput.Value ?? 25
            : BreakDurationInput.Value ?? 5;

        return TimeSpan.FromMinutes((double)minutes);
    }

    private void UpdateDisplay()
    {
        TimerText.Text = $"{(int)remainingTime.TotalMinutes:00}:{remainingTime.Seconds:00}";
        PhaseText.Text = isWorkPhase ? "PRACA" : "PRZERWA";
        PhaseIndicator.Background = isWorkPhase ? Avalonia.Media.Brushes.IndianRed : Avalonia.Media.Brushes.SeaGreen;
        CycleCountText.Text = completedCycles.ToString("D2");
        StartPauseButton.Content = isRunning ? "Pauza" : hasStarted ? "Wznów" : "Start";
    }
}