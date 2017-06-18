using System.Windows.Forms;

namespace FlaskManager.FlaskComponents
{
    internal class FlaskKeys
    {
        public Keys[] K;
        public FlaskKeys(Keys k1, Keys k2, Keys k3, Keys k4, Keys k5)
        {
            K = new Keys[5];
            K[0] = k1;
            K[1] = k2;
            K[2] = k3;
            K[3] = k4;
            K[4] = k5;
        }
    }
}
