using System;
using System.Diagnostics;

namespace Riateu;

public struct GameTime 
{
    private Stopwatch timer;
    public TimeSpan Elapsed => TimeSpan.FromTicks(timer.Elapsed.Ticks);

    public GameTime(Stopwatch stopwatch) 
    {
        timer = stopwatch;
    }
}