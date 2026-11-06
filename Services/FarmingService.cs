using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using PurrClicker.Native;

namespace PurrClicker.Services;

public class FarmingService
{
    private readonly List<ushort> _allowedKeys;
    private readonly Random _rng = new();

    public FarmingService()
    {
        // A-Z (0x41..0x5A) + 0-9 (0x30..0x39)
        var allKeys = Enumerable.Range(0x41, 26)
            .Concat(Enumerable.Range(0x30, 10))
            .Select(x => (ushort)x)
            .ToList();

        // Фільтрація: виключаємо F (0x46), M (0x4D), R (0x52) та Esc (0x1B)
        ushort keyF = 0x46, keyM = 0x4D, keyR = 0x52, keyEsc = 0x1B;
        _allowedKeys = allKeys.Where(k => k != keyF && k != keyM && k != keyR && k != keyEsc).ToList();
    }

    public async Task StartAsync(bool useKeyboard, bool useMouse, int mouseX, int mouseY, int delayMs, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var tasks = new List<Task>();

            if (useKeyboard)
                tasks.Add(Task.Run(PressRandomKey, ct));

            if (useMouse)
                tasks.Add(Task.Run(() => ClickMouse(mouseX, mouseY), ct));

            if (tasks.Count > 0)
                await Task.WhenAll(tasks);

            int jitter = _rng.Next(-10, 11);
            int finalDelay = Math.Max(10, delayMs + jitter);

            await Task.Delay(finalDelay, ct);
        }
    }

    private void PressRandomKey()
    {
        ushort vk = _allowedKeys[_rng.Next(_allowedKeys.Count)];
        var inputs = new WinApi.INPUT[2];

        inputs[0] = new WinApi.INPUT { type = WinApi.INPUT_KEYBOARD };
        inputs[0].u.ki.wVk = vk;

        inputs[1] = new WinApi.INPUT { type = WinApi.INPUT_KEYBOARD };
        inputs[1].u.ki.wVk = vk;
        inputs[1].u.ki.dwFlags = WinApi.KEYEVENTF_KEYUP;

        WinApi.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(WinApi.INPUT)));
    }

    private void ClickMouse(int x, int y)
    {
        WinApi.SetCursorPos(x, y);

        var inputs = new WinApi.INPUT[2];

        inputs[0] = new WinApi.INPUT { type = WinApi.INPUT_MOUSE };
        inputs[0].u.mi.dwFlags = WinApi.MOUSEEVENTF_LEFTDOWN;

        inputs[1] = new WinApi.INPUT { type = WinApi.INPUT_MOUSE };
        inputs[1].u.mi.dwFlags = WinApi.MOUSEEVENTF_LEFTUP;

        WinApi.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(WinApi.INPUT)));
    }
}