using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace glCourse
{
    static class Program
    {
        static void Main(string[] args)
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "OpenGL courseWork"
            };


            using var window = new Window(GameWindowSettings.Default, nativeWindowSettings);
            window.Run();

        }
    }
}
