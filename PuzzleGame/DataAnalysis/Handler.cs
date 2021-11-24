using System;
using FileReading;
using System.Collections.Generic;

namespace DataAnalysis
{
    public class DataWriter
    {
        public delegate void DataConsumerDelegate(string type, DateTime time, int x, int y, string obj);

        public static DataConsumerDelegate dataDel;

        static DataWriter()
        {
            dataDel = DataConsumer;
        }

        public static void DataConsumer(string type, DateTime time, int x, int y, string obj)
        {
            FileStreams.writeToFile("../../../../working_result.txt", "" + type + ":" + ((DateTimeOffset)time).ToUnixTimeMilliseconds() + ":" + x + ":" + y + ":" + obj + "\n");
        }

        public static void deleteFile(string path)
        {
            FileStreams.writeToNewFile(path, "");
        }



        public static void AnalizeAndWrite(string s = "../../../../working_result.txt", string sr = "../../../../result.csv", string sep = ";")
        {
            List<DataReader.Probe> probes = DataReader.ReadAndParse(s);
            long timetotal = 0;
            for (int i = 0; i < probes.Count; ++i)
            {
                //Console.WriteLine("Test number " + i + "\n" + probes[i].ToString() + "\n\n");
                if (i == 0)
                {
                    FileReading.FileStreams.writeToFile(sr, "" + probes[i].ToStringCSV(true, sep: sep) + "\n");
                }
                else
                {
                    FileReading.FileStreams.writeToFile(sr, "" + (i) + sep + probes[i].ToStringCSV(sep: sep) + "\n");
                    timetotal += probes[i].totaltime;
                }
            }
            FileReading.FileStreams.writeToFile(sr, "" + "Total time:" + sep + timetotal + "\n");
        }
    }

    public class DataReader
    {
        public enum EventTypes
        {
            Probe_start, // start of probe
            Probe_end, // end of probe
            Mouse_up, // botton of mouse released (end activity, start pause) - for statistick about clicking if needed
            Mouse_down, // button of mouse pressed (start activity)
            Object_get, // user take object(start moving)
            Object_loose // user release object (end moving, start pause)
        }
        public class EventData
        {
            public EventTypes type;
            public long time;
            public Position pos;
            public string obj = "";

            public EventData(EventTypes type, string time, string x, string y, string obj)
            {
                this.type = type;
                this.time = long.Parse(time);
                pos = new Position(int.Parse(x), int.Parse(y));
                this.obj = obj;
            }
        }

        public class Position
        {
            public int x, y;
            public Position(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static bool operator ==(Position a, Position b) {
                return a.x == b.x && a.y == b.y;
            }
            public static bool operator !=(Position a, Position b)
            {
                return !(a == b);
            }

            public override bool Equals(object a)
            {
                return base.Equals(a);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        public class MovableObject
        {
            public string name;
            public List<Position> pos;
            public int ret_to_start_pos;

            public MovableObject(string name, Position pos1)
            {
                ret_to_start_pos = 0;
                this.name = name;
                pos = new List<Position>();
                pos.Add(pos1);
            }
            public MovableObject(MovableObject a)
            {
                ret_to_start_pos = 0;
                this.name = a.name;
                pos = new List<Position>();
                pos.Add(a.pos[a.pos.Count-1]);
            }
        }

        public class Probe
        {
            public string name_attemp = "";
            public List<long> pauses = new List<long>();
            public List<long> moves = new List<long>();
            public long totaltime = 0;
            //public List<int> returnes = new List<int>();
            public Dictionary<string, MovableObject> obj = new Dictionary<string, MovableObject>();

            public override string ToString()
            {
                string s = "" + name_attemp;
                long lsum = 0;
                int isum = 0;
                s += "Pauses: \n";
                foreach(long i in pauses)
                {
                    lsum += i;
                    s += "" + i + ", ";
                }
                s += "\nTotal Sum: " + lsum + "\n";
                lsum = 0;
                s += "Moves: \n";
                foreach (long i in moves)
                {
                    lsum += i;
                    s += "" + i + ", ";
                }
                s += "\nTotal Sum: " + lsum + "\n";
                s += "returnes for each object: \n";
                foreach (MovableObject i in obj.Values)
                {
                    s += "\t" + i.name + ": " + i.ret_to_start_pos + "\n";
                    isum += i.ret_to_start_pos;
                }
                s += "Total Sum: " + isum + "\n";

                return s;
            }


            public string ToStringCSV(bool f = false, string sep = ",")
            {
                string s = "";
                if (f)
                {
                    foreach(var i in name_attemp.Split(","))
                    {
                        s += i + sep;
                    }
                    return "\n\n" + s;
                }
                s = "" + name_attemp + sep;
                long lsum = 0;
                int isum = 0;
                //s += "Pauses: \n";
                foreach (long i in pauses)
                {
                    lsum += i;
                    //s += "" + i + ", ";
                }
                //s += "\nTotal Sum: " + lsum + "\n";
                s += "" + lsum + sep;
                lsum = 0;
                //s += "Moves: \n";
                foreach (long i in moves)
                {
                    lsum += i;
                    //s += "" + i + ", ";
                }
                //s += "\nTotal Sum: " + lsum + "\n";
                s += "" + lsum + sep;
                s += "" + pauses.Count + sep + moves.Count + sep;
                //s += "returnes for each object: \n";
                List<MovableObject> valList = new List<MovableObject>(obj.Values);
                valList.Sort((a, b) => { return a.name.CompareTo(b.name); });
                string ids = "";
                foreach (MovableObject i in valList)
                {
                    //s += "\t" + i.name + ": " + i.ret_to_start_pos + "\n";
                    //s += "" + i.name + ": " + i.ret_to_start_pos + sep;
                    isum += i.ret_to_start_pos;
                    ids += "" + i.name;
                }
                //s += "Total Sum: " + isum + "\n";
                s += "" + isum + sep;
                s += "" + pauses[0] + sep;
                var lenum = obj.Values.GetEnumerator();
                if (lenum.MoveNext()) {
                    s += "" + lenum.Current.name + sep;
                }
                s += ids + sep;
                s += "" + totaltime + sep;

                return s;


                /*

    1) Продолжительность пауз между передвижением монеток/спичек в мс
    2) Длительность передвижений монеток/спичек в мс
    3) Количество пауз на пробу
    4) Количество шагов (передвижений монеток/спичек) на пробу
    5) Количество возвратов монеток/спичек на пробу (когда монетку/спичку перемещают с её места и возвращают обратно)
    6) Длительность паузы до первого шага в пробе в мс
    7) ID монетки/спички взятой первой в пробе
    8) ID монеток/спичек, перемещаемых за пробу
    9) Общее время пробы в мс
    10) Общее время решения задачи в с
    11) Номер пробы на испытуемого, в которой он впервые передвинул спичку/монетку связанную с решением

                */
            }
        }

        public static List<Probe> ReadAndParse(string path)
        {
            List<EventData> history = new List<EventData>();
            Dictionary<string, MovableObject> objlist = new Dictionary<string, MovableObject>();
            try
            {
                int linei = 0;
                foreach (string s in FileStreams.readFileAllLines(path))
                {
                    ++linei;
                    string[] res = s.Split(':');
                    if (res[0] == "Probe_start")
                    {
                        if (history.Count != 0 && history[history.Count - 1].type != EventTypes.Probe_end)
                        {
                            throw new FormatException("Broken data! " + linei);
                        }
                        history.Add(new EventData(EventTypes.Probe_start, res[1], res[2], res[3], res[4]));
                        continue;
                    }
                    if (res[0] == "Probe_end")
                    {
                        history.Add(new EventData(EventTypes.Probe_end, res[1], res[2], res[3], res[4]));
                        continue;
                    }
                    if (res[0] == "Mouse_up")
                    {
                        history.Add(new EventData(EventTypes.Mouse_up, res[1], res[2], res[3], res[4]));
                        continue;
                    }
                    if (res[0] == "Mouse_down")
                    {
                        history.Add(new EventData(EventTypes.Mouse_down, res[1], res[2], res[3], res[4]));
                        continue;
                    }
                    if (res[0] == "Object_get")
                    {
                        history.Add(new EventData(EventTypes.Object_get, res[1], res[2], res[3], res[4]));
                        if (objlist.TryGetValue(res[4], out MovableObject obj))
                        {
                            //obj.pos.Add(history[history.Count - 1].pos);
                        }
                        else
                        {
                            objlist.Add(res[4], new MovableObject(res[4], history[history.Count - 1].pos));
                        }
                        continue;
                    }
                    if (res[0] == "Object_loose")
                    {
                        history.Add(new EventData(EventTypes.Object_loose, res[1], res[2], res[3], res[4]));
                        if (objlist.TryGetValue(res[4], out MovableObject obj))
                        {
                            obj.pos.Add(history[history.Count - 1].pos);
                        }
                        else
                        {
                            throw new FormatException("Broken data! " + linei);
                        }
                    }
                    // Other cathchers dont realized, because for this task they are not needed (front does not working yet).
                }
            }
            catch (Exception e)
            {
                throw new FormatException("Broken data!", e);
            }

            return ParseReaded(history, objlist);
        }
            /*

1) Продолжительность пауз между передвижением монеток/спичек в мс
2) Длительность передвижений монеток/спичек в мс
3) Количество пауз на пробу
4) Количество шагов (передвижений монеток/спичек) на пробу
5) Количество возвратов монеток/спичек на пробу (когда монетку/спичку перемещают с её места и возвращают обратно)
6) Длительность паузы до первого шага в пробе в мс
7) ID монетки/спички взятой первой в пробе
8) ID монеток/спичек, перемещаемых за пробу
9) Общее время пробы в мс
10) Общее время решения задачи в с
11) Номер пробы на испытуемого, в которой он впервые передвинул спичку/монетку связанную с решением

            */

        public static List<Probe> ParseReaded(List<EventData> history, Dictionary<string, MovableObject> objlist) { 
            List<Probe> probes = new List<Probe>();

            for(int i = 0; i < history.Count; ++i)
            {
                switch(history[i].type)
                {
                    case EventTypes.Probe_start:
                        {
                            probes.Add(new Probe());
                            probes[probes.Count - 1].name_attemp = history[i].obj;
                            probes[probes.Count - 1].totaltime = -history[i].time;
                            break;
                        }
                    case EventTypes.Probe_end:
                        {
                            probes[probes.Count - 1].pauses.Add(history[i].time - history[i - 1].time);
                            probes[probes.Count - 1].totaltime += history[i].time;
                            foreach (MovableObject k in probes[probes.Count - 1].obj.Values)
                            {
                                for (int j = 1; j < k.pos.Count; ++j)
                                {
                                    if (k.pos[j] == k.pos[0])
                                    {
                                        ++k.ret_to_start_pos;
                                    }
                                }
                            }
                            break;
                        }
                    //case EventTypes.Mouse_up:
                    //    {

                    //    }
                    //case EventTypes.Mouse_down:
                    //    {

                    //    }
                    case EventTypes.Object_get:
                        {
                            probes[probes.Count - 1].pauses.Add(history[i].time - history[i - 1].time);
                            if(!probes[probes.Count - 1].obj.TryGetValue(history[i].obj, out MovableObject obj))
                            {
                                probes[probes.Count - 1].obj.Add(history[i].obj, new MovableObject(objlist[history[i].obj]));
                            }
                            else
                            {
                                obj.pos.Add(history[i].pos);
                            }
                            break;
                        }
                    case EventTypes.Object_loose:
                        {
                            probes[probes.Count - 1].moves.Add(history[i].time - history[i - 1].time);
                            probes[probes.Count - 1].obj[history[i].obj].pos.Add(history[i].pos);
                            break;
                        }
                }
            }

            return probes;
        }
    }
}
