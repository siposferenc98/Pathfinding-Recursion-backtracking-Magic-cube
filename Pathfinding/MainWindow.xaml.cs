using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pathfinding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<List<int>> matrix;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void general(object sender, RoutedEventArgs e)
        {
            matrixLB.Items.Clear();
            int s = int.Parse(sor.Text);
            int o = int.Parse(oszlop.Text);
            matrix = new();
            for (int i = 0; i < s; i++)
            {
                List<int> sor = new();
                for (int j = 0; j < o; j++)
                {
                    sor.Add(0);
                }
                matrix.Add(sor);
            }
            bool cel = false;
            int edb = 0;
            Random rnd = new();
            for(int i = 0; i < s; i++)
            {
                for (int j = 0; j < o; j++)
                {
                    int ek = 0;
                    int mk = 0;

                    if (!cel)
                    {
                        matrix[rnd.Next(s)][rnd.Next(o)] = 2;
                        cel = true;
                    }
                    else if (edb < s * o * 0.3)
                    {
                        do
                        {
                            ek = rnd.Next(s);
                            mk = rnd.Next(o);
                        } while (matrix[ek][mk] == 2 || matrix[ek][mk] == 1);
                        matrix[ek][mk] = 1;
                        edb++;
                    }
                }
            }
            listboxFeltolt(matrix,s, o);

        }

        private void keres(object sender, RoutedEventArgs e)
        {
            List<Mezo> nyitott = new();
            List<Mezo> zart = new();
            Mezo indul = new();
            indul.x = int.Parse(Hsor.Text);
            indul.y = int.Parse(Hoszlop.Text);
            Mezo cel = new();
            cel.x = matrix.FindIndex(x => x.Contains(2));
            cel.y = matrix[cel.x].IndexOf(2);
            indul.tavolsagBeallit(cel.x, cel.y);

            nyitott.Add(indul);

            while (nyitott.Any())
            {
                Mezo legkozelebb = nyitott.OrderByDescending(x => x.tavolsag).Last();

                //megoldva
                if (legkozelebb.x == cel.x && legkozelebb.y == cel.y)
                {
                    matrixLB.Items.Add("");
                    Mezo kesz = legkozelebb;
                    List<List<int>> masolat = new();
                    foreach (var sor in matrix)
                    {
                        masolat.Add(sor.ToList());
                    }
                    while(true)
                    {
                        masolat[kesz.x][kesz.y] = 3;
                        kesz = kesz.parent;
                        if(kesz == null)
                        {
                            listboxFeltolt(masolat,matrix.Count(), matrix.First().Count());
                            return;
                        }
                    }

                }

                zart.Add(legkozelebb);
                nyitott.Remove(legkozelebb);
                List<Mezo> szomszedok = generateMezok(legkozelebb, cel);
                foreach (Mezo mezo in szomszedok)
                {
                    if (zart.Any(x => x.x == mezo.x && x.y == mezo.y))
                        continue;
                    if (nyitott.Any(x => x.x == mezo.x && x.y == mezo.y))
                    {
                        Mezo letezoMezo = nyitott.First(x => x.x == mezo.x && x.y == mezo.y);
                        if (letezoMezo.tavolsag > legkozelebb.tavolsag)
                        {
                            nyitott.Remove(letezoMezo);
                            nyitott.Add(mezo);
                        }
                    }
                    else
                    {
                        nyitott.Add(mezo);
                    }
                }
            }
            MessageBox.Show("Nincs megoldás!");


        }


        private List<Mezo> generateMezok(Mezo adott, Mezo cel)
        {
            List<Mezo> lehetseges = new List<Mezo>
            {
                new Mezo{x = adott.x, y = adott.y-1, parent = adott},
                new Mezo{x = adott.x, y = adott.y+1, parent = adott},
                new Mezo{x = adott.x-1, y = adott.y, parent = adott},
                new Mezo{x = adott.x+1, y = adott.y, parent = adott},

            };

            lehetseges.ForEach(mezo => mezo.tavolsagBeallit(cel.x, cel.y));

            int maxX = matrix.Count()-1;
            int maxY = matrix.First().Count() - 1;
            return lehetseges
                .Where(mezo => mezo.x >= 0 && mezo.x <= maxX)
                .Where(mezo => mezo.y >= 0 && mezo.y <= maxY)
                .Where(mezo => matrix[mezo.x][mezo.y] == 0 || matrix[mezo.x][mezo.y] == 2)
                .ToList();
        }

        private void listboxFeltolt(List<List<int>> matrix, int s, int o)
        {
            for (int i = 0; i < s; i++)
            {
                string sor = "";
                for (int j = 0; j < o; j++)
                {
                    sor += matrix[i][j] + " ";
                }
                matrixLB.Items.Add(sor);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changeUI(cb.SelectedIndex);
        }

        private void changeUI(int index)
        {
            switch(index)
            {
                case 0:
                    honnanLabel.Content = "Honnan?";
                    Hoszlop.Visibility = Visibility.Visible;
                    buvosNBTN.Visibility = Visibility.Hidden;
                    utkeresBTN1.Visibility = Visibility.Visible;
                    utkeresBTN2.Visibility = Visibility.Visible;
                    oszlop.IsEnabled = true;
                    sorLabel.Content = "Sor:";
                    break;
                case 1:
                    honnanLabel.Content = "Összeg?";
                    utkeresBTN1.Visibility = Visibility.Hidden;
                    utkeresBTN2.Visibility = Visibility.Hidden;
                    buvosNBTN.Visibility = Visibility.Visible;
                    Hoszlop.Visibility = Visibility.Hidden;
                    oszlop.IsEnabled = false;
                    sorLabel.Content = "Sor/oszlop:";
                    break;
            }
        }

        private void buvosNegyzetGen(object sender, RoutedEventArgs e)
        {
            int n = int.Parse(sor.Text);
            if (n > 3)
                MessageBox.Show("Rekurzív backtracking nem működik 3x3nál nagyobbra!");
            else
            {
                int[,] buvosErtekek = new int[n,n];
                if (buvosNegyzetBacktrack(buvosErtekek, 0, 0, n, int.Parse(Hsor.Text)))
                    listboxFeltolt2(buvosErtekek, n);
                else
                    MessageBox.Show("Nincs megoldása!");
            }

        }

        //3x3nál nagyobbra a rekurzív megoldás nem működik, túl sok lehetőséget kell végignézzen.
        private bool buvosNegyzetBacktrack(int[,] ertekek, int sor, int oszlop, int n, int e)
        {
            if (n == 1)
            {
                ertekek[0, 0] = e;
                return true;
            }
            //base casek
            if (sor == n - 1 && oszlop == n)
            {
                int atlo1 = 0;
                int atlo2 = 0;
                for (int i = 0; i < n ; i++)
                {
                    int sorO = 0;
                    int oszlopO = 0;
                    for (int j = 0; j < n; j++)
                    {
                        sorO += ertekek[i, j];
                        oszlopO += ertekek[j, i];
                    }
                    atlo1 += ertekek[i, i];
                    atlo2 += ertekek[i, n - i - 1];
                    if (sorO != e || oszlopO != e)
                        return false;
                }
                if (atlo1 != e || atlo2 != e)
                    return false;
                return true;
            }

            if (oszlop == n)
            {
                sor++;
                oszlop = 0;
            }
            if (!tulmentE(ertekek, sor, oszlop, n, e))
                return false;


            //probálgatás
            for (int i = 1; i <= n*n; i++)
            {
                if (i > e)
                    break;
                if (safePlace(ertekek, sor, oszlop, i, n, e))
                {
                    ertekek[sor, oszlop] = i;
                    if (buvosNegyzetBacktrack(ertekek, sor, oszlop + 1, n, e))
                        return true;
                }
                ertekek[sor, oszlop] = 0;

            }
            return false;

            
        }

        private void listboxFeltolt2(int[,] ertekek, int n)
        {
            matrixLB.Items.Clear();
            for (int i = 0; i < n; i++)
            {
                string sor = "";
                for (int j = 0; j < n; j++)
                {
                    sor += ertekek[i, j] + " ";
                }
                matrixLB.Items.Add(sor);
            }
        }

        private bool safePlace(int[,] ertekek, int sor, int oszlop, int szam, int n, int e)
        {
            
            for (int i = 0; i < n; i++)
            {
                if (ertekek[sor, i] == szam || ertekek[i,oszlop] == szam || ertekek[i,i] == szam)
                    return false;

               
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (ertekek[i, j] == szam)
                        return false;
                }
            }

            return true;

        }

        private bool tulmentE(int[,] ertekek, int sor, int oszlop, int n, int e)
        {
            int sorO = 0;
            int oszlopO = 0;
            int atloO = 0;
            for (int i = 0; i < n; i++)
            {
                sorO += ertekek[sor, i];
                oszlopO += ertekek[i, oszlop];
                atloO += ertekek[i, i];
                if (sorO > e || oszlopO > e || atloO > e)
                    return false;
            }
            return true;
        }
    }
}
