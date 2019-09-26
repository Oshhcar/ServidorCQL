using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.entorno
{
    class Time
    {
        public Time(string time)
        {
            time = time.Replace("\'", "");
            string[] d = time.Split(':');

            Hours = 0;
            Minutes = 0;
            Seconds = 0;
            Correcto = false;

            try
            {
                Hours = Convert.ToInt32(d[0]);
                Minutes = Convert.ToInt32(d[1]);
                Seconds = Convert.ToInt32(d[2]);

                if (Hours >= 0 && Hours <= 24)
                    if (Minutes >= 0 && Minutes <= 60)
                        if (Seconds >= 0 && Seconds <= 60)
                            Correcto = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Time: " + ex.Message);
            }

        }

        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public bool Correcto { get; set; }

        public override string ToString()
        {
            return Hours + ":" + Minutes + ":" + Seconds;
        }

        public string ToString2()
        {
            return "'" + Hours + ":" + Minutes + ":" + Seconds + "'";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            if(obj is Time time)
            {
                if (Hours == time.Hours)
                {
                    if (Minutes == time.Minutes)
                    {
                        if (Seconds == time.Seconds)
                        {
                            return 0;
                        }
                        else
                        {
                            if (Seconds > time.Seconds)
                                return 1;
                            else
                                return -1;
                        }
                    }
                    else
                    {
                        if (Minutes > time.Minutes)
                            return 1;
                        else
                            return -1;
                    }
                }
                else
                {
                    if (Hours > time.Hours)
                        return 1;
                    else
                        return -1;
                }
            }
            return 2;
        }
    }
}
