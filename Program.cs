using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdditiveNC
{        
    class Program
    {
        //Some simple classes to make data formatting a smidge easier.
        class CLIHatches
        {
            public int id = -1;
            public int numberofhatches = -1;
            public List<CLIHatch> hatches = new List<CLIHatch>();
        }
        class CLIHatch:IFormattable
        {
            public int startx = -1;
            public int starty = -1;
            public int endx = -1;
            public int endy = -1;
            public String ToString(string format, IFormatProvider formatprovider) { return String.Format("{0},{1} - {2},{3}",startx,starty,endx,endy); }
        }
        class Polyline
        {
            public int id =-1;
            public int direction=-1;
            public int numberofpoints=-1;
            public List<Tuple<int, int>> points=new List<Tuple<int,int>>();
        }

        static void Main(string[] args)
        {
            double unitsmultiplier = .001; //Units default to mm, or .001m. 
            Queue<string> lines = new Queue<string>(System.IO.File.ReadAllLines("test_block.cli"));
            string line = lines.Dequeue();
            while(!(line.Contains("$$GEOMETRYSTART"))) //Parse header
            {
                if (line.Contains("$$UNITS/"))
                {
                    unitsmultiplier = .001 * Convert.ToDouble(line.Substring("$$UNITS/".Length));
                }
                line = lines.Dequeue();
            }
            while((!line.Contains("$$GEOMETRYEND"))) //Parse Geometry
            {
                if(line.Contains("$$POLYLINE/"))
                {
                    Polyline p = parsepolyline(line);
                }
                if(line.Contains("$$HATCHES/"))
                {
                    CLIHatches h = parsehatches(line);
                }
                line = lines.Dequeue();
            }
        }
        static Polyline parsepolyline(String line)
        {
            Polyline p = new Polyline();
            List<String> values = new List<String>(line.Split(','));
            //Parse ID
            if (values[0].Contains("$$POLYLINE/"))
            {
                values[0]=values[0].Substring("$$POLYLINE/".Length);
                
            }
            p.id = Convert.ToInt32(values[0]);
            values.RemoveAt(0);

            //Parse Direction
            p.direction = Convert.ToInt32(values[0]);
            values.RemoveAt(0);
            //Parse count.
            p.numberofpoints = Convert.ToInt32(values[0]);
            values.RemoveAt(0);
            for (int i = 0; i < p.numberofpoints;i++ )
            {
                int x = Convert.ToInt32(values[0]);
                int y = Convert.ToInt32(values[1]);
                Tuple<int, int> point = new Tuple<int, int>(x, y);
                p.points.Add(point);
                values.RemoveRange(0, 2);
            }
            return p;
        }
        static CLIHatches parsehatches(String line)
        {
            CLIHatches h = new CLIHatches();
            List<String> values = new List<String>(line.Split(','));
            if (values[0].Contains("$$HATCHES/"))
            {
                values[0] = values[0].Substring("$$HATCHES/".Length);

            }
            h.id = Convert.ToInt32(values[0]);
            values.RemoveAt(0);
            //Parse count.
            h.numberofhatches = Convert.ToInt32(values[0]);
            values.RemoveAt(0);
            for (int i = 0; i < h.numberofhatches; i++)
            {
                CLIHatch hatch = new CLIHatch();
                 hatch.startx = Convert.ToInt32(values[0]);
                hatch.starty = Convert.ToInt32(values[1]);
                hatch.endx = Convert.ToInt32(values[2]);
                hatch.endy = Convert.ToInt32(values[3]);
                h.hatches.Add(hatch);
                values.RemoveRange(0, 4);
            }
            return h;
        }
    }
}
