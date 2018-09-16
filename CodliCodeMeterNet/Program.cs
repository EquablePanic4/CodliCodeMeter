using CodliDevelopment;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CodliCodeMeterNet
{
    class Program
    {
        #region Główne komponenty aplikacji
        public static bool MierzenieSzczegolwe;

        static void Main(string[] args)
        {
            MierzenieSzczegolwe = false;
            Start();
        }

        private static void Start()
        {
            //Ustawiamy tytuł okna konsoli
            Console.Title = "Codli Code Meter [v1.1]";
            wl("\r\nWitaj w aplikacji Codli Code Meter!");
            wl("Wciśnij ENTER aby kontynuuowac...");
            var k = Console.ReadLine();

            //Przechodzimy do następnego etapu.
            ChooseExtensionsToView();
        }

        private static void ChooseExtensionsToView()
        {
            Console.Clear();
            wl("\r\nWypisz rozszerzenia plików (rozdzielając je przecinkiem), które powinny być brane pod uwagę podczas wyliczania metryki kodu. Jeżeli chcesz zebrać pliki o wszystkich rozszerzeniach, wpisz '*' - gwiazdkę.");
            Console.Write("\r\nRozszerzenia: ");

            string extensionsString = Console.ReadLine();

            //Jeszcze prosimy o to, czy mają zostać wyświetlone szczegóły
            Console.Write("\r\nCzy program ma wyświetlać wszystkie szczegóły operacji? [T/N]: ");
            string szczegoly = Console.ReadLine();
            if (szczegoly.Length < 1)
                MierzenieSzczegolwe = false;
            else
            {
                if (szczegoly[0].ToString().ToUpper() == "T")
                    MierzenieSzczegolwe = true;
                else
                    MierzenieSzczegolwe = false;
            }

            //Teraz z otrzymanego stringa eliminujemy spacje
            extensionsString = extensionsString.Replace(" ", null);

            //Oraz dzielimy go do tablicy rozszerzeń
            var extensionArr = extensionsString.Split(',');

            //Oraz przechodzimy do następnego etapu...
            ChooseDirectionDirectory(extensionArr);
        }

        private static void ChooseDirectionDirectory(string[] extArr)
        {
            long commentsChars = 0;
            long commentsLines = 0;
            long codeLines = 0;
            long codeChars = 0;

            long filesSkiped = 0;
            long fileScaned = 0;

            long miliseconds = 0;

            var q = '"'.ToString();
            Console.Clear();
            Console.Write("\r\nWskaż katalog w którym znajdują się pliki: ");
            string directoryPath = Console.ReadLine();
            directoryPath = directoryPath.Replace(q, null);

            Console.Write("\r\n\r\nPodaj wartość identifikującą pliki które mają zostać zliczone: ");
            string identifier = Console.ReadLine();
            var Mourinho = 1;

            //Mając te wartości tworzymy obiekt, na którym będzie operował główny silnik programu, ale żeby to zrobić musimy zebrać wszystkie pliki...
            foreach (var e in extArr)
            {
                string searchPattern = "*." + e;
                var pliki = Directory.GetFileSystemEntries(directoryPath, searchPattern, SearchOption.AllDirectories);

                var obiektUstawien = new SettingsObject()
                {
                    Identyfikator = identifier,
                    Pliki = pliki
                };

                //I w tym momencie przechodzimy dalej...
                var result = Main_ENGINE(obiektUstawien);

                //Teraz sprawdzamy i wyświetlamy wyniki:
                Console.WriteLine("KROK [" + Mourinho + " Z " + extArr.Length + "]");
                Mourinho++;
                Console.WriteLine("Wyniki dla plików z rozszerzeniem (*." + e + "): \r\n");
                Console.WriteLine("Przeskanowane pliki: " + result.fileScaned);
                Console.WriteLine("Pominięte pliki: " + result.filesSkiped);
                Console.WriteLine("Wiersze komentarzy: " + result.commentsLines);
                Console.WriteLine("Znaki komentarzy: " + result.commentsChars);
                Console.WriteLine("Wiersze kodu: " + result.codeLines);
                Console.WriteLine("Znaki kodu: " + result.codeChars);
                Console.WriteLine("\r\n\r\n");

                //I jeszcze uzupełnimy sobie globalne wartości
                fileScaned += result.fileScaned;
                filesSkiped += result.filesSkiped;
                commentsLines += result.commentsLines;
                commentsChars += result.commentsChars;
                codeLines += result.codeLines;
                codeChars += result.codeChars;
                miliseconds += result.miliseconds;
            }

            //Skanowanie ukończone!
            wl("\r\n\r\nUkończono skanowanie plików w katalogu!");

            //Teraz sprawdzamy czy programista chce poznać statystyki godzinowe
            Console.Write("\r\nCzy chcesz wprowadzić więcej danych, aby poznać dokładne statystyki? [T/N]: ");
            string readed = Console.ReadLine();
            if (readed.ToUpper()[0] == 'T' || readed.ToUpper()[0] == 'Y')
            {
                Console.Write("\r\nWprowadź liczbę przepracowanych godzin: ");
                float workedHours = int.Parse(Console.ReadLine());

                Console.Write("\r\nWprowadź swoją stawkę godzinową: ");
                float pricePerHour = int.Parse(Console.ReadLine());

                wl("Przeskanowane pliki: " + fileScaned);
                wl("Pominięte pliki: " + filesSkiped);
                wl("Liczba wierszy komentarza: " + commentsLines);
                wl("Liczba znaków komentarza: " + commentsChars);
                wl("Liczba wierszy kodu: " + codeLines);
                wl("Liczba znaków kodu: " + codeChars);
                wl("Średnia liczba znaków w wierszu kodu: " + codeChars / codeLines + " znaki");

                //I teraz podajemy dane statystyczne
                wl("Liczba linii kodu na godzinę: " + Math.Round((codeLines / workedHours), 2) + " linii");
                wl("Liczba linii komentarza na godzinę: " + Math.Round((commentsLines / workedHours), 2) + " linii");
                wl("Liczba linii na godzinę: " + Math.Round((commentsLines + codeLines) / workedHours, 2) + " linii");
                wl("Średnia cena za linię kodu: " + Math.Round((workedHours * pricePerHour) / (codeLines), 2) + " zł");
                wl("Średnia cena za linię komentarza: " + Math.Round((workedHours * pricePerHour) / (commentsLines), 2) + " zł");
                wl("Średnia cena za linię: " + Math.Round((workedHours * pricePerHour) / (commentsLines + codeLines), 2) + " zł");
                wl("Średnia cena za znak kodu: " + Math.Round((workedHours * pricePerHour) / codeChars, 2) + " zł");

                wl("\r\n\r\nDziękujemy za skorzystanie z programu!");
                wl("http://www.codli.eu\r\n");
            }

            else
            {
                wl("Przeskanowane pliki: " + fileScaned);
                wl("Pominięte pliki: " + filesSkiped);
                wl("Liczba wierszy komentarza: " + commentsLines);
                wl("Liczba znaków komentarza: " + commentsChars);
                wl("Liczba wierszy kodu: " + codeLines);
                wl("Liczba znaków kodu: " + codeChars);
                wl("Średnia liczba znaków w wierszu kodu: " + codeChars / codeLines + " znaki");

                wl("\r\n\r\nDziękujemy za skorzystanie z programu!");
                wl("http://www.codli.eu\r\n");
            }

            Console.ReadLine();
        }

        private static MainEngineResult Main_ENGINE(SettingsObject info)
        {
            if (MierzenieSzczegolwe == true)
                wl("\r\nRozpoczonam pomiary kodów...\r\n");

            //Tworzenie zmiennych pomiaru metryki
            long commentsChars = 0;
            long commentsLines = 0;
            long codeLines = 0;
            long codeChars = 0;

            long filesSkiped = 0;
            long fileScaned = 0;

            long commentsCharsBefore = 0;
            long commentsLinesBefore = 0;
            long codeLinesBefore = 0;
            long codeCharsBefore = 0;

            var listaMetryczna = new List<MetricObject>();

            //I teraz bardzo prosty silnik, działający na typach IEnumerable
            foreach (var plik in info.Pliki)
            {
                //Zerujemy wartości dla pliku
                commentsCharsBefore = commentsChars;
                commentsLinesBefore = commentsLines;
                codeLinesBefore = codeLines;
                codeCharsBefore = codeChars;

                if (MierzenieSzczegolwe == true)
                    wl("Sprawdzam plik: " + plik + "...");

                var lines = File.ReadAllLines(plik);

                //Zanim przejdziemy do pętli iteracyjnej, sprawdzamy czy plik kwalifikuje się do sprawdzenia
                if (lines.Length == 0)
                    continue;

                if (lines[0].Contains(info.Identyfikator) == false)
                {
                    //Niestety, plik nie kwalifikuje się do sprawdzenia
                    if (MierzenieSzczegolwe == true)
                        wl("Plik został pominięty, ponieważ nie zawierał odpowiedniego identyfikatora!\r\n");

                    filesSkiped++;
                    continue;
                }

                //Jeżeli doszliśmy tutaj, znaczy że plik nadaje się do sprawdzenia
                if (MierzenieSzczegolwe == true)
                    wl("Plik poprawny! Rozpoczynam wyliczanie metryki kodu...");

                fileScaned++;

                foreach (var wiersz in lines)
                {
                    var clearLine = wiersz;
                    /*Jeżeli linia jest pusta, po prostu ją pomijamy.
                    if (String.IsNullOrEmpty(wiersz) == true)
                        continue;

                    //Tworzymy linię bez spacji na końcu i początku
                    var clearLine = RemoveSpacesAtBeginAndEnd(wiersz);*/

                    if (String.IsNullOrEmpty(wiersz) == true || String.IsNullOrWhiteSpace(wiersz) == true)
                        continue;

                    //Teraz sprawdzamy czy wiersz nie jest w całości komentarzem
                    string twoFirst = String.Empty;
                    if (clearLine.Length > 1)
                        twoFirst = clearLine[0].ToString() + clearLine[1].ToString();
                    else
                        twoFirst = clearLine[0].ToString();

                    if (twoFirst == "//" || twoFirst == "/*" || twoFirst == "<!" || wiersz[0] == '*')
                    {
                        //Spotykamy się z komentarzem w całym wierszu...
                        commentsChars += clearLine.Length;
                        commentsLines++;

                        continue;
                    }

                    //Jeżeli jeszcze tu jesteśmy, całą linię liczymy jako kod
                    codeChars += clearLine.Length;
                    codeLines++;
                }

                //Teraz dodajemy do listy metrycznej dane o pliku
                listaMetryczna.Add(new MetricObject()
                {
                    FileName = Path.GetFileName(plik),
                    codeChars = codeChars - codeCharsBefore,
                    codeLines = codeLines - codeLinesBefore,
                    commentChars = commentsChars - commentsCharsBefore,
                    commentLines = commentsLines - commentsLinesBefore
                });

                //I wyświetlamy informację dla użytkownika
                if (MierzenieSzczegolwe == true)
                {
                    wl("Liczba znaków komentarza w pliku: " + (commentsChars - commentsCharsBefore));
                    wl("Liczba wierszy komentarza w pliku: " + (commentsLines - commentsLinesBefore));
                    wl("Liczba znaków kodu w pliku: " + (codeChars - codeCharsBefore));
                    wl("Liczba wierszy kodu w pliku: " + (codeLines - codeLinesBefore));
                    wl("Liczba znaków komentarza dotychczas: " + commentsChars);
                    wl("Liczba wierszy komentarza dotychczas: " + commentsLines);
                    wl("Liczba znaków kodu dotychczas: " + codeChars);
                    wl("Liczba wierszy kodu dotychczas: " + codeLines);
                    wl("Przechodzę do kolejnego pliku... \r\n");
                }
            }

            //I zwracamy wynik...
            var wynik = new MainEngineResult()
            {
                codeChars = codeChars,
                codeLines = codeLines,
                commentsChars = commentsChars,
                commentsLines = commentsLines,
                fileScaned = fileScaned,
                filesSkiped = filesSkiped,
                miliseconds = 0
            };

            return wynik;
        }

        #endregion

        #region Aliasy metod z innych klas

        private static void wl(string value)
        {
            //ConsoleX.SmartWriteLine(value);
            Console.WriteLine(value);
        }

        #endregion

        #region Metody prywatne

        private static string RemoveSpacesAtBeginAndEnd(string line)
        {
            string firstStr = String.Empty;
            int Conte = 0;
            bool logical = false;
            while (Conte < line.Length)
            {
                if (logical == false)
                {
                    if (line[Conte] == ' ')
                    {
                        Conte++;
                        continue;
                    }

                    else
                    {
                        logical = true;
                        continue;
                    }
                }

                else
                {
                    firstStr += line[Conte].ToString();
                    Conte++;
                }
            }

            //I teraz to samo ale od końca
            Conte = firstStr.Length - 1;
            logical = false;
            string secondStr = String.Empty;

            while (Conte >= 0)
            {
                if (logical == false)
                {
                    if (firstStr[Conte] == ' ')
                    {
                        Conte--;
                        continue;
                    }

                    else
                    {
                        logical = true;
                        continue;
                    }
                }

                else
                {
                    secondStr = firstStr[Conte] + secondStr;
                    Conte--;
                }
            }

            //I zwracamy naszą wartość
            return secondStr;
        }

        private static bool DoesContainCommentInside(string value)
        {
            if (value.Contains("//") || value.Contains("/*"))
                return true;
            else
                return false;
        }

        private static string longMilisecondsToReadableSecondsString(long miliseconds)
        {
            return (miliseconds + " ms");

            string str = "" + miliseconds;
            var Kepa = String.Empty;

            switch (str.Length)
            {
                case 0:
                    return "0 sek.";

                case 1:
                    Kepa = "000" + miliseconds;
                    break;

                case 2:
                    Kepa = "00" + miliseconds;
                    break;

                case 3:
                    Kepa = "0" + miliseconds;
                    break;

                default:
                    Kepa = "" + miliseconds;
                    break;
            }

            //Teraz operujemy już tylko na zmiennej imienia Kepy Arrizabalagi, dodając przecinek w odpowiednie miejsce
            var Hazard = String.Empty;
            var Conte = 0;
            foreach (var e in Kepa)
            {
                if (Conte == Kepa.Length - 3)
                    Hazard += "," + e;
                else
                    Hazard += e;

                Conte++;
            }

            Hazard = Hazard + " sek.";
            return Hazard;
        }

        #endregion
    }
}
