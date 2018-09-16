using System;
using System.Collections.Generic;
using System.Text;

namespace CodliDevelopment
{
    static class ConsoleX
    {
        public static void SmartWriteLine(string value)
        {
            //Musimy podzielić naszą wartość na sekwencje nie dłuższe niż 80 znaków
            var words = value.Split(' ');
            var lines = new List<string>();
            
            string lineBuffer = String.Empty;
            
            foreach (var e in words)
            {
                if (lineBuffer.Length + e.Length <= 140 - 1) // <-- -1 ponieważ jeszcze spacja
                {
                    lineBuffer += ' ' + e;
                }

                else
                {
                    lines.Add(lineBuffer);
                    lineBuffer = String.Empty;
                    lineBuffer = e;
                }
            }

            //Teraz dla każdej linii, po prostu ją wypisujemy
            foreach (var e in lines)
                Console.WriteLine(e);
        }
    }
}
