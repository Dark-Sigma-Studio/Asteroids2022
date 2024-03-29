﻿using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using OpenTK.Mathematics;
using OpenTK.Platform;
using OpenTK.Windowing;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Asteroids2022
{
    public partial class Game : GameWindow
    {

        #region Shader things

        public struct Shader
        {
            public int id;

            public static Shader Load(string location, ShaderType type)
            {
                // Declarations
                int id = GL.CreateShader(type);

                // Processing
                GL.ShaderSource(id, File.ReadAllText(location));
                GL.CompileShader(id);

                // Error checking
                string infolog = GL.GetShaderInfoLog(id);
                if (!string.IsNullOrEmpty(infolog))
                {
                    throw new Exception(infolog);
                }

                // Return
                return new Shader() { id = id };
            }
        }

        public struct ShaderProgram
        {
            #region Fields
            public int id;
            #endregion

            #region Methods
            public static ShaderProgram Load(string vertlocation, string fraglocation)
            {
                // Declarations
                int id = GL.CreateProgram();

                Shader vert = Shader.Load(vertlocation, ShaderType.VertexShader);
                Shader frag = Shader.Load(fraglocation, ShaderType.FragmentShader);

                // Processing
                GL.AttachShader(id, vert.id);
                GL.AttachShader(id, frag.id);
                GL.LinkProgram(id);

                // Cleanup
                GL.DetachShader(id, vert.id);
                GL.DetachShader(id, frag.id);
                GL.DeleteShader(vert.id);
                GL.DeleteShader(frag.id);

                // Error checking
                string infolog = GL.GetProgramInfoLog(id);
                if (!string.IsNullOrEmpty(infolog))
                {
                    throw new Exception(infolog);
                }

                // Retern
                return new ShaderProgram() { id = id };
            }
            #endregion
        }

        #endregion

        private int vbo;
        private int vao;
        public ShaderProgram sprog = new ShaderProgram() { id = 0 };
        private readonly float[] verteces =
        {
            -1.0f, -1.0f, 0.0f,
            -1.0f, 3.0f, 0.0f,
            3.0f, -1.0f, 0.0f
        };

        Stopwatch sw;
        double ellapsedtime;
        double deltat;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        int uiResolution = -1;
        int utplaypos = -1;
        int utplayangle = -1;
        int ucampos = -1;
        int ubullets = -1;
        int ubullnumb = -1;

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.125f, 0.175f, 0.257f, 1.0f);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, verteces.Length * sizeof(float), verteces, BufferUsageHint.StaticDraw);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            sprog = ShaderProgram.Load("Resources/Screen.vert", "Resources/Screen.frag");
            GL.UseProgram(sprog.id);

            uiResolution = GL.GetUniformLocation(sprog.id, "iResolution");
            utplaypos = GL.GetUniformLocation(sprog.id, "tplaypos");
            utplayangle = GL.GetUniformLocation(sprog.id, "tplayangle");
            ucampos = GL.GetUniformLocation(sprog.id, "campos");
            ubullets = GL.GetUniformLocation(sprog.id, "bullets");
            ubullnumb = GL.GetUniformLocation(sprog.id, "bullnumb");

            for(int i = 0; i < 32; i++)
            {
                TPLAYER.Bullet.LIST[i] = new TPLAYER.Bullet() { vel = Vector2.Zero, mass = 100.0f, pos = Vector2.Zero };
            }

            sw = Stopwatch.StartNew();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        TPLAYER player = new TPLAYER();
        Vector2 campos = new Vector2(0.0f, 0.0f);

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            #region Boilerplate
            base.OnUpdateFrame(args);
            //-------------------------------------//
            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            #endregion

            deltat = sw.ElapsedMilliseconds / 1000.0 - ellapsedtime;
            ellapsedtime += deltat;

            if(KeyboardState.IsKeyDown(Keys.Up))
            {
                player.acc = new Vector2((float)(Math.Cos(player.angle)), (float)Math.Sin(player.angle)) * 0.1f;
            }
            if(KeyboardState.IsKeyDown(Keys.Left))
            {
                player.avel = 2.0;
            }
            if(KeyboardState.IsKeyDown(Keys.Right))
            {
                player.avel = -2.0;
            }
            if(KeyboardState.IsKeyPressed(Keys.F))
            {
                Vector2 dir = new Vector2((float)Math.Cos(player.angle), (float)Math.Sin(player.angle));
                TPLAYER.Bullet.Spawn(player.pos, dir * 1f + player.vel);
            }

            player.DoPhysics((float)deltat);
            TPLAYER.Bullet.Update((float)deltat);

            campos += (player.pos - campos) * (float)deltat * (player.pos - campos).Length * 5.0f;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            #region Boilerplate
            base.OnRenderFrame(args);
            //---------------------------------------//
            GL.Clear(ClearBufferMask.ColorBufferBit);
            #endregion
            //=============================================================================//

            GL.UseProgram(sprog.id);

            GL.Uniform2(uiResolution, Size);
            GL.Uniform2(utplaypos, player.pos);
            GL.Uniform1(utplayangle, (float)player.angle);
            GL.Uniform2(ucampos, campos);
            GL.Uniform1(ubullnumb, TPLAYER.Bullet.COUNT);

            GL.Uniform2(ubullets, 32, TPLAYER.Bullet.posses);

            GL.BindVertexArray(vao);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            //=============================================================================//

            SwapBuffers();
        }
    }
}
