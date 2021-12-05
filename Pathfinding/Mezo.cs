using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinding
{
    public class Mezo
    {
        public int x, y, tavolsag;
        public Mezo parent;
        public void tavolsagBeallit(int x, int y)
        {
            tavolsag = Math.Abs((x - this.x) + Math.Abs(y - this.y));
        }

    }
}
