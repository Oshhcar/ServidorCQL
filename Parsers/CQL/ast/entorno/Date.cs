using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramaticasCQL.Parsers.CQL.ast.entorno
{
    class Date
    {
        public Date(string date)
        {
            date = date.Replace("\'", "");
            date = date.Trim();
            string[] d = date.Split('-');

            Year = 0;
            Month = 0;
            Day = 0;

            Correcto = false;

            try
            {
                Year = Convert.ToInt32(d[0]);
                Month = Convert.ToInt32(d[1]);
                Day = Convert.ToInt32(d[2]);

                if (Year >= 0 && Year <= 2999)
                    if (Month >= 0 && Month <= 12)
                        if (Day >= 0 && Day <= 31)
                            Correcto = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Date: " + ex.Message);
            }
        }

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public bool Correcto { get; set; }

        public override string ToString()
        {
            return Year + "-" + Month + "-" + Day;
        }

        public string ToString2()
        {
            return "'" + Year + "-" + Month + "-" + Day + "'";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            if (obj is Date date)
            {
                if (Year == date.Year)
                {
                    if (Month == date.Month)
                    {
                        if (Day == date.Day)
                        {
                            return 0;
                        }
                        else
                        {
                            if (Day > date.Day)
                                return 1;
                            else
                                return -1;
                        }
                    }
                    else
                    {
                        if (Month > date.Month)
                            return 1;
                        else
                            return -1;
                    }
                }
                else
                {
                    if (Year > date.Year)
                        return 1;
                    else
                        return -1;
                }
            }

            return 2;
        }
    }
}
