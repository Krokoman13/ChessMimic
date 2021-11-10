using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public struct Coordinate
    {
        public int x;
        public int y;

        public Coordinate(int x, int y)
        {
            this.x = -1;
            this.y = -1;

            setX(x);
            setY(y);
        }

        public Coordinate(string cord)
        {
            this.x = -1;
            this.y = -1;

            if (cord.Length < 2)
            { 
            }

            setX(GetX(cord[0]));
            setY(GetY(cord[1]));
        }

        public static int GetX(char x)
        {
            int newX = -1;

            switch (x)
            {
                case 'a':
                    newX = 0;
                    break;
                case 'b':
                    newX = 1;
                    break;
                case 'c':
                    newX = 2;
                    break;
                case 'd':
                    newX = 3;
                    break;
                case 'e':
                    newX = 4;
                    break;
                case 'f':
                    newX = 5;
                    break;
                case 'g':
                    newX = 6;
                    break;
                case 'h':
                    newX = 7;
                    break;
            }

            return newX;
        }

        private void setX(int x)
        {
            //if (x < 0) x = 0;
            //else if (x > 7) x = 7;

            this.x = x;
        }

        public static int GetY(char y)
        {
            return (y - '0' - 1);
        }

        private void setY(int y)
        {
            if (y < 0) y = 0;
            else if (y > 7) y = 7;

            this.y = y;
        }

        public string Notation()
        {
            char one = 'a';
            char two = '1';

            one = XToChar(x);

            two = YToChar(y);

            return one.ToString() + two.ToString();
        }

        public static char YToChar(int y)
        {
            return (char)((y + 1) + '0');
        }

        public static char XToChar(int x)
        {
            switch (x)
            {
                case 0:
                    return 'a';
                case 1:
                    return 'b';
                case 2:
                    return 'c';
                case 3:
                    return 'd';
                case 4:
                    return 'e';
                case 5:
                    return 'f';
                case 6:
                    return 'g';
                case 7:
                    return 'h';
                default:
                    return ' ';
            }
        }

        public static bool operator ==(Coordinate left, Coordinate right)
        {
            return (left.x == right.x && left.y == right.y);
        }

        public static bool operator !=(Coordinate left, Coordinate right)
        {
            return (left.x != right.x || left.y != right.y);
        }

        public static Coordinate operator +(Coordinate left, Coordinate right)
        {
            return new Coordinate(left.x + right.x, left.x + right.y);
        }

        public override string ToString()
        {
            return Notation();
        }
    }
}
