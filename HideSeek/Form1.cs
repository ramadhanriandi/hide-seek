using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Glee.Drawing;

// Tubes2StrAlgo_Ahboy
// Eka Novendra Wahyunadi / 13517011
// Marsa Thoriq Ahmada / 13517071
// Mgs.Muhammad Riandi Ramadhan / 13517080

namespace HideSeek
{
    public partial class Form1 : Form
    {
        private const int Mx = 500009;		//Kapasitas maksimal array
        private static int[] Ukuran = new int[Mx];	// Merupakan array yang merepresentasikan banyaknya tetangga dari suatu node
        private static int[] Visited = new int[Mx]; // Array yang menunjukan node sudah / belum dikunjungi
        private static int[] Ans = new int[Mx]; // Array untuk menyimpan node sementara
        private static List<int> Path = new List<int>(); // Merupakan list yang menyimpan jarul DFS yang dilakukan fungsi solve
        private static List<int>[] L = new List<int>[Mx]; // Merupakan adjecent list
        private static String[] _arrBool = new string[Mx]; //menyimpan jalur ferdiant
        private static int _countBool = 0; //sebagai counter _arrBool
        private static String[] _arrPath = new string[Mx];
        static int _x, _y, _n, _q, _a, _b, _c, _cnt; // counter dan input ke program
        static bool _stop, _end; //penanda kondisi akhir DFS
        private bool _direct;

        // konstruktor korm
        public Form1()
        {
            // menginisialisasi komponen
            InitializeComponent();
        }

        // ketika button 'Choose' diklik, muncul openFileDialog untuk membaca file graf
        private void button1_Click(object sender, EventArgs e)
        {
            // openFileDialog muncul
            openFileDialog1.ShowDialog();

            // membaca isi file graf
            string graphFile = openFileDialog1.FileName;
            string readFile = File.ReadAllText(graphFile);
            richTextBox1.Text = readFile;
        }

        // ketika button 'Generate' diklik, membangun graf dari hasil bacaan file
        private void button2_Click(object sender, EventArgs e)
        {
            // membuat objek graf
            Microsoft.Glee.Drawing.Graph graph = new Microsoft.Glee.Drawing.Graph("graph");

            // membuat isi graf
            int numberOfNodes = Convert.ToInt32(richTextBox1.Lines[0]);
            _n = numberOfNodes;
            Init(_n);               // inisialisasi graf
            string[] edge;
            for (int i = 1; i < _n; i++)                // baca input graf dari richTextBox1
            {
                edge = richTextBox1.Lines[i].Split(' ');
                _x = Convert.ToInt32(edge[0]);
                _y = Convert.ToInt32(edge[1]);
                // membangun sisi
                graph.AddEdge(edge[0], edge[1]);
                Node node = graph.FindNode(edge[0]);
                // graf berbentuk lingkaran
                node.Attr.Shape = Microsoft.Glee.Drawing.Shape.Circle;
                node = graph.FindNode(edge[1]);
                // membuat adjecent list
                Ukuran[_x] += 1;
                L[_x].Add(_y);
                // graf berbentuk lingkaran
                node.Attr.Shape = Microsoft.Glee.Drawing.Shape.Circle;
                // menghilangkan panah
                graph.GraphAttr.EdgeAttr.ArrowHeadAtTarget = ArrowStyle.None;
            };
            // graf dimasukkan ke viewer 
            gViewer1.Graph = graph;
        }

        // ketika button 'Choose' diklik, memilih file query
        private void button3_Click(object sender, EventArgs e)
        { 
            // openFileDialog muncul
            openFileDialog2.ShowDialog();

            // membaca isi file query
            string queryFile = openFileDialog2.FileName;
            string readFile = File.ReadAllText(queryFile);
            richTextBox2.Text = readFile;

            // menyatakan input dari file eksternal
            _direct = false;
            listBox1.Items.Clear();
            _countBool = 0;
        }

        // ketika isi dari listBox diklik dan mengalami perubahan indeks
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // jika input secara langsung, bukan file eksternal
            if (_direct)
            {
                // ambil indeks item list
                int idx = listBox1.SelectedIndex;
                string[] passed_path = _arrPath[idx].Split(' ');
                bool found = false;

                // membangun objek graf 
                Microsoft.Glee.Drawing.Graph graph = new Microsoft.Glee.Drawing.Graph("graph");

                // membuat isi graf
                int numberOfNodes = Convert.ToInt32(richTextBox1.Lines[0]);
                string[] edge;
                for (int i = 1; i < numberOfNodes; i++)
                {
                    edge = richTextBox1.Lines[i].Split(' ');
                    graph.AddEdge(edge[0], edge[1]);
                    Node node = graph.FindNode(edge[0]);
                    // mencari node yang ada pada path yang ditemukan
                    for (int j = 0; j < passed_path.Length; j++)
                    {
                        if (passed_path[j] == edge[0])
                        {
                            found = true;
                        }
                    }
                    if (found)
                    {
                        // ubah tampilan node yang ditemukan untuk menandakan jalur
                        node.Attr.Shape = Microsoft.Glee.Drawing.Shape.Diamond;
                        node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Yellow;
                        found = false;
                    }
                    else
                    {
                        // seperti bentuk node pada graf awal
                        node.Attr.Shape = Microsoft.Glee.Drawing.Shape.Circle;
                    }
                    node = graph.FindNode(edge[1]);

                    // mencari node yang ada pada path yang ditemukan
                    for (int j = 0; j < passed_path.Length; j++)
                    {
                        if (passed_path[j] == edge[1])
                        {
                            found = true;
                        }
                    }
                    if (found)
                    {
                        // ubah tampilan node yang ditemukan untuk menandakan jalur
                        node.Attr.Shape = Microsoft.Glee.Drawing.Shape.Diamond;
                        node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Yellow;
                        found = false;
                    }
                    else
                    {
                        // seperti bentuk node pada graf awal
                        node.Attr.Shape = Microsoft.Glee.Drawing.Shape.Circle;
                    }
                    graph.GraphAttr.EdgeAttr.ArrowHeadAtTarget = ArrowStyle.None;
                };
                // masukkan graf ke dalam viewer
                gViewer1.Graph = graph;

            }
            else // jika input dari file eksternal
            {
                // ambil indeks item list
                int idx = listBox1.SelectedIndex - 1;
                string[] passed_path = _arrPath[idx].Split(' ');
                bool found = false;

                // membangun objek graf
                Microsoft.Glee.Drawing.Graph graph = new Microsoft.Glee.Drawing.Graph("graph");

                // membuat isi graf 
                int numberOfNodes = Convert.ToInt32(richTextBox1.Lines[0]);
                string[] edge;
                for (int i = 1; i < numberOfNodes; i++)
                {
                    edge = richTextBox1.Lines[i].Split(' ');
                    graph.AddEdge(edge[0], edge[1]);
                    Node node = graph.FindNode(edge[0]);
                    // mencari node yang ada pada path yang ditemukan
                    for (int j = 0; j < passed_path.Length; j++)
                    {
                        if (passed_path[j] == edge[0])
                        {
                            found = true;
                        }
                    }
                    if (found)
                    {
                        // ubah tampilan node yang ditemukan untuk menandakan jalur
                        node.Attr.Shape = Microsoft.Glee.Drawing.Shape.Diamond;
                        node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Yellow;
                        found = false;
                    }
                    else
                    {
                        // seperti bentuk node pada graf awal
                        node.Attr.Shape = Microsoft.Glee.Drawing.Shape.Circle;
                    }
                    node = graph.FindNode(edge[1]);
                    for (int j = 0; j < passed_path.Length; j++)
                    {
                        if (passed_path[j] == edge[1])
                        {
                            found = true;
                        }
                    }
                    if (found)
                    {
                        // ubah tampilan node yang ditemukan untuk menandakan jalu
                        node.Attr.Shape = Microsoft.Glee.Drawing.Shape.Diamond;
                        node.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Yellow;
                        found = false;
                    }
                    else
                    {
                        // seperti bentuk node pada graf awal
                        node.Attr.Shape = Microsoft.Glee.Drawing.Shape.Circle;
                    }
                    graph.GraphAttr.EdgeAttr.ArrowHeadAtTarget = ArrowStyle.None;
                };
                // memasukkan graf ke dalam viewer
                gViewer1.Graph = graph;

            }
        }

        // ketika button 'Solve' diklik, menyelesaikan query yang diberikan
        private void button4_Click(object sender, EventArgs e)
        {
            // inputan query
            _cnt = 0;
            string[] query_complete;
            if (_direct)
            {
                _q = 1;
                query_complete = richTextBox2.Lines[0].Split(' ');
                _a = Convert.ToInt32(query_complete[0]);
                _b = Convert.ToInt32(query_complete[1]);
                _c = Convert.ToInt32(query_complete[2]);
                // Memulai dengan posisi node awal 1
                Visited[1] = 1;
                _stop = false;
                _end = false;
                if (_a != 0)
                {
                    Solve(1, _b, _c);
                }
                else
                {
                    Solve(1, _c, _b);
                }
                // mereset kembali beberapa nilai
                Reset();
                listBox1.Items.Add(_arrBool[0]);
            }
            else
            { 
                _q = Convert.ToInt32(richTextBox2.Lines[0]);
                for (var i = 1; i <= _q; i++)
                {
                    query_complete = richTextBox2.Lines[i].Split(' ');
                    _a = Convert.ToInt32(query_complete[0]);
                    _b = Convert.ToInt32(query_complete[1]);
                    _c = Convert.ToInt32(query_complete[2]);
                    // Memulai dengan posisi node awal 1
                    Visited[1] = 1;
                    _stop = false;
                    _end = false;
                    if (_a != 0)
                    {
                        Solve(1, _b, _c);
                    }
                    else
                    {
                        Solve(1, _c, _b);
                    }
                    // mereset kembali beberapa nilai
                    Reset();
                }
                listBox1.Items.Add(" ");
                for (var i = 0; i < _countBool; i++)
                {
                    listBox1.Items.Add(_arrBool[i]);
                }
            }
        }

        // ketika button 'OK' diklik, memasukkan input query ke dalam richTextBox2
        private void button5_Click(object sender, EventArgs e)
        {
            // memindahkan isinya
            richTextBox2.Text = textBox1.Text;
            // menyatakan input secara langsung
            _direct = true;
            listBox1.Items.Clear();
            _countBool = 0;
        }

        private static void Init(int input)
        {
            //inisialisasi program dengan mengeset 0
            for (int i = 0; i < input; i++)
            {
                Ukuran[i] = 0;
                Visited[i] = 0;
                L[i+1] = new List<int>();
            }
            //isi path
            Path.Add(0);
        }

        // fungsi yang menunjukan apakah tidak ada jalur lagi dari n
        private static int NoPath(int n)
        {
            var bol = 1;
            for (var i = 0; i < Ukuran[n]; i++)
            {
                var next = L[n][i];
                if (Visited == null) continue;
                if (Visited[next]==0)
                {
                    bol = 0;
                }
            }
            return bol;
        }

        //mencari dengan DFS jalur yang dilewati ferdiant dari posisi node
        private static void CariJalur(int node)
        {
            Path.Add(node);
            //cek apakah masih ada jalur dan apakah sudah menemukan jalan buntu
            if ((NoPath(node) != 0) && (_end != true))
            {
                _end = true;
                Ans[_cnt] = node;
                _cnt++;
                for (var i = 1; i < Path.Count; i++)
                {
                    _arrPath[_countBool] += Path[i] + " ";
                }
            }
            else //untuk kasus ada jalur
            {
                if (_end) return; //langsung keluar rekursif
                for (var i = 0; i < Ukuran[node]; i++)
                {
                    var next = L[node][i];
                    if (Visited[next] != 0) continue;
                    Visited[next] = 1;
                    CariJalur(next); //rekursif ke node selanjutnya
                    //backtrack
                    Path.RemoveAt(Path.Count - 1);
                    Visited[next] = 0;
                }
            }
        }

        private static void Solve(int nodes, int last, int pos)
        {
            Path.Add(nodes);

            if (last == nodes && (_stop != true)) // cek apakah menemukan node tujuan dan apakah sudah saatnya stop
            {
                _stop = true; // stop ketika menemukan node tujuan
                TulisJawaban(pos); //menulis jalur dan jawaban
            }
            else
            {
                if (_stop) return; //langsung keluar rekursif / pruning
                for (var i = 0; i < Ukuran[nodes]; i++)
                {
                    var next = L[nodes][i]; //menuju ke node tetangga
                    if (Visited[next] != 0) continue;
                    Visited[next] = 1;
                    Solve(next, last, pos); // rekursif
                    //backtrack
                    Path.RemoveAt(Path.Count - 1);
                    Visited[next] = 0;
                }
            }
        }

        //menulis jawaban
        private static void TulisJawaban(int post)
        {
            bool found; //cek apakah ada post didalam list path
            found = false;
            _arrPath[_countBool] = ""; //menyimpan jalur ferdiant
            if (_a == 0) // jika mendekati istana
            {
                var i = Path.Count - 1;
                while (i >= 1 && !found) //pencarian dari array path apakah ada post dan berhenti jika ketemu
                {
                    _arrPath[_countBool] += Path[i] + " "; //tulis jalurnya dalam string
                    if (Path[i] == post)
                    {
                        found = true;
                    }
                    else 
                    {
                        i--;
                    }
                }
            }
            else //jika menjauhi istana
            {
                var i = Path.Count - 1;
                while (i >= 1 && !found) //mencari posisi indeks ke berapa post berada dalam array path
                {
                    if (Path[i] == post)
                    {
                        found = true;
                    }
                    else
                    {
                        i--;
                    }
                }

                if (found) //jika ditemukan
                {
                    for (var j = i; j < Path.Count; j++)
                    {
                        _arrPath[_countBool] += Path[j] + " "; //tulis jalur ferdiant dan disimpan dalam array
                    }
                }
                else //Jika tidak ditemukan di DFS lagi dengan kondisi akhir tidak ada jalur lagi 
                {
                    Reset(); //reset agar bisa di-DFS
                    CariJalur((post)); //Mencari jalur yang dilewati ferdiant
                }
            }
            _arrBool[_countBool] = found ? "YA" : "TIDAK"; //menyimpan hasil jawaban
            _countBool++;
        }

        private static void Reset()
        {
            //mereset nilai visited
            for (var i = 1 ; i <= _n ; i++){
                Visited[i]=0;
            }

            // dan mereset path
            Path = new List<int>();
            Path.Add(0);
        }
    }
}
