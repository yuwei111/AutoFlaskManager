using System.Windows.Forms;

namespace FlaskManager.FlaskComponents
{
    class FlaskKeys
    {
        public Keys[] k;
        public FlaskKeys(Keys k1, Keys k2, Keys k3, Keys k4, Keys k5)
        {
            k = new Keys[5];
            k[0] = k1;
            k[1] = k2;
            k[2] = k3;
            k[3] = k4;
            k[4] = k5;
        }
    }
}
