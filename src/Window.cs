using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace glCourse
{
    public class Window : GameWindow
    {

        private readonly float[] vertices =
        {
            0.5f, 0.5f, 0.0f, 1.0f, 1.0f,
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,
            -0.5f, 0.5f, 0.0f, 0.0f, 1.0f
        };

        private readonly uint[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int vertexBuffer;
        private int vertexArray;
        private int elementBuffer;
        private Shader shader;
        private Texture texture;

        private Camera camera;
        private bool firstMove = true;
        private Vector2 lastPos;

        private double time;


        // private Stopwatch timer;


        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {

        }


        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            vertexArray = GL.GenVertexArray();
            GL.BindVertexArray(vertexArray);

            vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            elementBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            shader = new Shader("res/default.shader");
            shader.Use();

            var vertexLocation = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            texture = Texture.LoadFromFile("res/container.png");
            texture.Use(TextureUnit.Texture0);

            camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

            CursorGrabbed = true;

            // timer = new Stopwatch();
            // timer.Start();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            time += 4.0 * e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            // double timeValue = timer.Elapsed.TotalSeconds;
            // float greenValue = (float)Math.Sin(timeValue) / 2.0f + 0.5f;
            // shader.SetVector3("ourColor", new OpenTK.Mathematics.Vector3(0.0f, greenValue, 0.0f));

            GL.BindVertexArray(vertexArray);

            texture.Use(TextureUnit.Texture0);
            shader.Use();

            var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(time));
            shader.SetMatrix4("model", Matrix4.Identity);
            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());


            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                camera.Position += camera.Front * cameraSpeed * (float)e.Time;
            }

            if (input.IsKeyDown(Keys.S))
            {
                camera.Position -= camera.Front * cameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.A))
            {
                camera.Position -= camera.Right * cameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.D))
            {
                camera.Position += camera.Right * cameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.Space))
            {
                camera.Position += camera.Up * cameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                camera.Position -= camera.Up * cameraSpeed * (float)e.Time;
            }

            var mouse = MouseState;

            if (firstMove)
            {
                lastPos = new Vector2(mouse.X, mouse.Y);
                firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - lastPos.X;
                var deltaY = mouse.Y - lastPos.Y;
                lastPos = new Vector2(mouse.X, mouse.Y);

                camera.Yaw += deltaX * sensitivity;
                camera.Pitch -= deltaY * sensitivity;
            }

        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            camera.AspectRatio = Size.X / (float)Size.Y;
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(vertexBuffer);
            GL.DeleteVertexArray(vertexArray);

            GL.DeleteProgram(shader.Handle);

            base.OnUnload();
        }
    }
}
