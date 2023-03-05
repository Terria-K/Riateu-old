using Silk.NET.GLFW;
using Silk.NET.Input;

namespace Riateu;

public static class MouseListener 
{
    private static double scrollX, scrollY;
    private static double xPos, yPos, lastX, lastY;
    private static bool[] mouseButtonPressed = new bool[3];
    private static bool isDragging;

    public static float X => (float)xPos;
    public static float Y => (float)yPos;
    public static float LastX => (float)lastX;
    public static float LastY => (float)lastY;
    public static float DistanceX => (float)(lastX - xPos);
    public static float DistanceY => (float)(lastY - yPos);
    public static float ScrollX => (float)scrollX;
    public static float ScrollY => (float)scrollY;
    public static bool IsDragging => isDragging;


    internal static unsafe void MousePositionCallback(WindowHandle* window, double xpos, double ypos) 
    {
        lastX = xPos;
        lastY = yPos;
        xPos = xpos;
        yPos = ypos;
        isDragging = mouseButtonPressed[0] || mouseButtonPressed[1] || mouseButtonPressed[2];
    }

    internal static unsafe void MouseButtonCallback(
        WindowHandle* window, 
        Silk.NET.GLFW.MouseButton button, Silk.NET.GLFW.InputAction action, Silk.NET.GLFW.KeyModifiers mods) 
    {
        if (action == Silk.NET.GLFW.InputAction.Press) 
        {
            if ((int)button < mouseButtonPressed.Length)
                mouseButtonPressed[(int)button] = true;
        }
        else if (action == Silk.NET.GLFW.InputAction.Release && (int)button < mouseButtonPressed.Length)
        {
            mouseButtonPressed[(int)button] = false;
            isDragging = false;
        }
    }

    internal static unsafe void MouseScrollCallback(WindowHandle* window, double offsetX, double offsetY) 
    {
        scrollX = offsetX;
        scrollY = offsetY;
    }

    internal static void End() 
    {
        scrollX = 0;
        scrollY = 0;
        lastX = xPos;
        lastY = yPos;
    }

    public static bool MouseButtonDown(int button) 
    {
        if (button < mouseButtonPressed.Length)
            return mouseButtonPressed[button];
        return false;
    }
}