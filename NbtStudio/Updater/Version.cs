using System;
using System.Linq;

namespace NbtStudio
{
    public class Version : IComparable<Version>
    {
        private int[] Dots;
        public Version(params int[] dots)
        {
            Dots = dots;
        }

        public Version(string str)
        {
            ParseDots(str);
        }

        public Version(System.Version version)
        {
            Dots = new int[] { version.Major, version.Minor, version.Build, version.Revision };
        }

        protected void ParseDots(string str)
        {
            Dots = str.Split('.').Select(x => int.Parse(x)).ToArray();
        }

        public static bool operator >(Version v1, Version v2)
        {
            return v1.CompareTo(v2) > 0;
        }

        public static bool operator <(Version v1, Version v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        public int CompareTo(Version other)
        {
            for (int i = 0; i < Math.Min(Dots.Length, other.Dots.Length); i++)
            {
                int compare = Dots[i].CompareTo(other.Dots[i]);
                if (compare != 0)
                    return compare;
            }
            return Dots.Length.CompareTo(other.Dots.Length);
        }

        public override string ToString() => ToString(true);
        public string ToString(bool trailing)
        {
            if (trailing)
                return String.Join(".", Dots);
            else
            {
                int stop = Dots.Length;
                for (int i = Dots.Length - 1; i >= 0; i--)
                {
                    if (Dots[i] != 0)
                    {
                        stop = i + 1;
                        break;
                    }
                }
                return String.Join(".", Dots.Take(stop));
            }
        }
    }
}
