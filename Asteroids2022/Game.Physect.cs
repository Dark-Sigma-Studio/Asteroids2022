using OpenTK.Mathematics;

namespace Asteroids2022
{
    public partial class Game
    {
        public class Physect
        /*
            [ ] position
            [ ] velocity
            [ ] acceleration
            [ ] mass

            [ ] Momentum
            [ ] Kinetic energy

            [ ] Impulse
            [ ] Force
        */
        {
            public Vector2 pos;
            public Vector2 vel;
            public float mass;
        }
    }
}
