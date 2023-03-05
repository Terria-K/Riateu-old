using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Loader;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Riateu;

public abstract class Game 
{
    private Glfw glfw;
    private unsafe WindowHandle* glfwWindow;
    private int width;
    private int height;
    private string title;
    private GL gl;
    private GraphicsDevice graphicsDevice;

    public GraphicsDevice GraphicsDevice => graphicsDevice;

    public GL GL => gl;

    public Game(int width, int height, string title) 
    {
        this.width = width;
        this.height = height;
        this.title = title;
        glfw = Glfw.GetApi();
    }

    private void ErrorMessageCallback(Silk.NET.GLFW.ErrorCode code, string message) 
    {
        Console.WriteLine($"[{code}] {message}");
    }

    private unsafe void Init() 
    {
        glfw.SetErrorCallback(ErrorMessageCallback);
        if (!glfw.Init())
            throw new InvalidOperationException("Unable to start GLFW");

        glfw.DefaultWindowHints();
        glfw.WindowHint(WindowHintBool.Visible, false);
        glfw.WindowHint(WindowHintBool.Resizable, true);
        glfw.WindowHint(WindowHintBool.Maximized, true);

        glfwWindow = glfw.CreateWindow(width, height, title, null, null);
        if (glfwWindow == null) 
            throw new InvalidOperationException("Failed to create a window");
        
        glfw.SetCursorPosCallback(glfwWindow, MouseListener.MousePositionCallback);
        glfw.SetMouseButtonCallback(glfwWindow, MouseListener.MouseButtonCallback);
        glfw.SetScrollCallback(glfwWindow, MouseListener.MouseScrollCallback);

        glfw.SetKeyCallback(glfwWindow, KeyListener.KeyCallback);

        glfw.MakeContextCurrent(glfwWindow);

        glfw.SwapInterval(1);

        glfw.ShowWindow(glfwWindow);
        gl = new GL(new WindowGlNativeContext());
        graphicsDevice = new GraphicsDevice(gl);
    }

    public void Run() 
    {
        Init();
        Initialize();
        var timer = Stopwatch.StartNew();
        var lastTime = new TimeSpan();
        GameTime gameTime = new GameTime(timer);
        unsafe 
        {
            while (!glfw.WindowShouldClose(glfwWindow)) 
            {
                glfw.PollEvents();

                var currTime = gameTime.Elapsed;
                TimeSpan diff = currTime - lastTime;
                lastTime = currTime;
                var fixedTarget = (float)TimeSpan.FromSeconds(1f/ 60f).TotalSeconds;

                Update(fixedTarget);

                Render();

                glfw.SwapBuffers(glfwWindow);
            }
        }
        
    }

    protected abstract void KeyDown(IKeyboard keyboard, Key key, int keyCode);

    protected abstract void Initialize();
    protected abstract void Update(double delta);
    protected abstract void Render();
    protected abstract void Unload();
}

public class WindowGlNativeContext : INativeContext
{
    private readonly UnmanagedLibrary _l;
    [DllImport("opengl32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static unsafe extern IntPtr wglGetProcAddress(sbyte* functionName);

    public WindowGlNativeContext()
    {
        // The base library, with functions that exist in all versions
        _l = new UnmanagedLibrary("opengl32.dll");
    }

    public unsafe bool TryGetProcAddress(string proc, out nint addr, int? slot = null)
    {
        // Firstly, we try to get the function in the base library
        if (_l.TryLoadFunction(proc, out addr))
        {
            return true;
        }
        
        // If we fail, we assume that this is an extended function that we need to query Windows for

        // Buffer for out ASCII null-terminated string
        var asciiName = new byte[proc.Length + 1];
        Encoding.ASCII.GetBytes(proc, asciiName);

        // We ask the GC not to move the buffer
        fixed (byte* name = asciiName)
        {
            // Query Windows for the extended OpenGL function
            addr = wglGetProcAddress((sbyte*)name);
            
            // If the address is not null -> we succeeded
            if (addr != IntPtr.Zero)
            {
                return true;
            }
        }

        // We failed to get the function
        return false;
    }
    
    public nint GetProcAddress(string proc, int? slot = null)
    {
        if (TryGetProcAddress(proc, out var address, slot))
        {
            return address;
        }

        throw new InvalidOperationException("No function was found with the name " + proc + ".");
    }
    
    public void Dispose() => _l.Dispose();
}