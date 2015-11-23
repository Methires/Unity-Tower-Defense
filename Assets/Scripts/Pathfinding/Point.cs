namespace Assets.Scripts.Pathfinding
{
    public class Point
    {
        private int _x { get; set; }
        private int _y { get; set; }

        public Point(int x, int y)
        {
            this._x = x;
            this._y = y;
        }

        public int X
        {
            get { return this._x; }
            set { this._x = value; }
        }

        public int Y
        {
            get { return this._y; }
            set { this._y = value; }
        }
    } 
}