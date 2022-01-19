using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Asteroids2022
{
    public class TPLAYER
    {
        public Vector2 pos;
        public Vector2 vel;
        public Vector2 acc;

        public double angle;
        public double avel;

        public double thrust;
        public void DoPhysics(float dt)
        {
            if((float)(avel * avel) != 0.0f)
            {
                pos += vel * dt + (acc - acc * (float)Math.Cos(avel * dt)) / (float)(avel * avel);
                vel += (acc * (float)Math.Sin(avel * dt)) / (float)avel;
            }
            else
            {
                vel += acc * dt / 2.0f;
                pos += vel * dt;
                vel += acc * dt / 2.0f;
            }

            angle += avel * dt;
            angle = (angle + 2.0 * Math.Atan2(0, -1)) % (2.0 * Math.Atan2(0, -1));

            avel = 0;
            acc = Vector2.Zero;
        }

        public class Bullet
        {
            public Vector2 pos;
            public Vector2 vel;
            public float life;
            public float mass = 100.0f;

            public static uint COUNT = 0;
            public static Bullet[] LIST = new Bullet[32];

            public static float[] posses
            {
                get
                {
                    float[] ps = new float[64];
                    for(int i = 0; i < 32; i++)
                    {
                        ps[2 * i] = LIST[i].pos.X;
                        ps[i * 2 + 1] = LIST[i].pos.Y;
                    }
                    return ps;
                }
            }

            public float kenergy
            {
                get
                {
                    return mass * vel.LengthSquared / 2.0f;
                }
            }

            public float momentum
            {
                get
                {
                    return mass * vel.Length;
                }
            }

            public static void Spawn(Vector2 pos, Vector2 vel)
            {
                Bullet b = new Bullet() { pos = pos, vel = vel, life = 3.0f };
                
                for(uint i = COUNT; i > 0; i--)
                {
                    LIST[i] = LIST[i - 1];
                }

                LIST[0] = b;

                COUNT = Math.Min(COUNT + 1, 31);

                Console.WriteLine($"Spawned a bullet! {COUNT}");
            }

            public static void Update(float dt)
            {
                for(int i = 0; i < COUNT; i++)
                {
                    Bullet b = LIST[i];

                    if (b.life <= 0.0f) 
                    {
                        COUNT--;
                        Console.WriteLine($"POOF! {COUNT}");
                    }
                    else
                    {
                        b.pos += b.vel * dt;
                        b.life -= - dt;
                    }
                }
            }
        }
    }
}
