using System;
using System.IO;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace glCourse
{
    enum ShaderSourceType
    {
        VertexShader,
        FragmentShader,
    }

    public class Shader
    {
        public readonly int Handle;

        private readonly Dictionary<string, int> uniformLocations;

        public Shader(string path)
        {
            var shaderSources = ParseShaderSource(path);

            var vertexSource = shaderSources[0];
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            CompileShader(vertexShader);

            var fragmentSource = shaderSources[1];
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            CompileShader(fragmentShader);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            LinkProgram(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            uniformLocations = new Dictionary<string, int>();

            for (int i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(Handle, i, out _, out _);
                var location = GL.GetUniformLocation(Handle, key);

                uniformLocations.Add(key, location);
            }
        }

        private static string[] ParseShaderSource(string path)
        {
            var shaderSource = new List<string>(File.ReadAllText(path).Split("\n"));
            var vertexSource = new List<string>();
            var fragmentSource = new List<string>();

            ShaderSourceType type = ShaderSourceType.FragmentShader;
            foreach (var str in shaderSource)
            {
                if (str.Contains("#vertex"))
                {
                    type = ShaderSourceType.VertexShader;
                    continue;
                }
                if (str.Contains("#fragment"))
                {
                    type = ShaderSourceType.FragmentShader;
                    continue;
                }

                if (type == ShaderSourceType.VertexShader)
                    vertexSource.Add(str);
                if (type == ShaderSourceType.FragmentShader)
                    fragmentSource.Add(str);

            }

            return new[] { string.Join("\n", vertexSource), string.Join("\n", fragmentSource) };
        }

        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst linking Program({program})");
            }
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }


        public void SetInt(string name, int data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(uniformLocations[name], data);
        }

        public void SetFloat(string name, float data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(uniformLocations[name], data);
        }


        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(uniformLocations[name], true, ref data);
        }

        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(uniformLocations[name], data);
        }
    }
}