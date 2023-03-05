using Silk.NET.GLFW;

namespace Riateu;

public static class KeyListener 
{
    private static bool[] keyPressed = new bool[350];

    internal static unsafe void KeyCallback(
        WindowHandle* handle, 
        Keys key, 
        int scancode, 
        InputAction action, 
        KeyModifiers mods) 
    {
        if (action == InputAction.Press)
            keyPressed[(int)key] = true;
        else if (action == InputAction.Release)
            keyPressed[(int)key] = false;
    }

    public static bool IsKeyPressed(Keys key) 
    {
        return keyPressed[(int)key];
    }

}