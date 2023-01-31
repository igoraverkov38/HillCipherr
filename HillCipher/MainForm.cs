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

namespace HillCipher
{
    public partial class MainForm : Form
    {
        class Vector
        {
            int[] item;
            int size;
            public Vector(int[] item)
            {
                this.item = item;size = item.Length;
            }
            public Vector(Vector x)
            {
                item = (int[])x.item.Clone(); size = item.Length;
            }
            public static int operator*(Vector x, Vector y)
            {
                int res = 0;int size = x.size;
                int[] X = x.item, Y = y.item;
                for (int i = 0; i < size; i++)
                    res += X[i] * Y[i];
                return res;
            }
            public static Vector operator *(Vector x, int k)
            {
                int size = x.size;
                int[] item = x.item, item_k=new int[size];
                for (int i = 0; i < size; i++)
                    item_k[i] = k * item[i];
                return new Vector(item_k);
            }
            public static Vector operator %(Vector x, int m)
            {
                int size = x.size;
                int[] item = x.item, item_k = new int[size];
                for (int i = 0; i < size; i++)
                    item_k[i] = item[i]%m;
                return new Vector(item_k);
            }
            public static Vector operator +(Vector x, Vector y)
            {
                int size = x.size;
                int[] X = x.item, Y = y.item, item = new int[size];
                for (int i = 0; i < size; i++)
                   item[i]=X[i]+Y[i];
                return new Vector(item);
            }
            public static Vector operator -(Vector x, Vector y)
            {
                int size = x.size;
                int[] X = x.item, Y = y.item, item = new int[size];
                for (int i = 0; i < size; i++)
                    item[i] = X[i] - Y[i];
                return new Vector(item);
            }
            public int this[int i]
            {
                get=> item[i];
                set
                {
                    item[i]=value;
                }
            }

            public int Size => size;
        }
        static int GCD(int a, int b)
        {
            if (a == 0) return 1;
            if ((b % a) == 0) return a;
            else return GCD(b, a % b);
        }
        int LCM(int a, int b)
        {
            return a * b / GCD(a, b);
        }
        static int Mod(int x, int m)
        {
            return (x+(-x/m+1)*m)%m;
        }
        static Vector Mod(Vector x, int m)
        {
            int size = x.Size;
            int[] item = new int[size];
            for (int i = 0; i < size; i++)
                item[i] = Mod(x[i], m);
            return new Vector(item);
        }
        static int GCD(int a, int b, ref int  x, ref int y)
        {
            if (a == 0)
            {
                x = 0; y = 1;
                return b;
            }
            int x1=0, y1=0;
            int d = GCD(b % a, a, ref x1,ref y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return d;
        }
        class Matrix
        {
            int size;
            Vector[] item;
            public Matrix(Vector[] item)
            {
                this.item = item;
                size = item.Length;
            }
            public Matrix(int size)
            {
                this.size = size;
                item = new Vector[size];
                for (int i = 0; i < size; i++)
                {
                    item[i] = new Vector(new int[size]);
                    item[i][i] = 1;
                }
            }
            public Matrix(Matrix A)
            {
                size = A.size;
                item = new Vector[size];
                Vector[] A_item=A.item;
                for (int i = 0; i < size; i++)
                    item[i] = new Vector(A_item[i]);
            }
            public static Vector operator *(Matrix A, Vector x)
            {
                
                int size = A.size;
                int[] res = new int[size];
                Vector[] item = A.item;
                for (int i = 0; i < size; i++)
                    res[i] = item[i] * x;
                return new Vector(res);
            }
            public static Matrix operator *(Matrix A, int x)
            {

                int size = A.size;
                Vector[] res = new Vector[size];
                Vector[] item = A.item;
                for (int i = 0; i < size; i++)
                    res[i] = item[i] * x;
                return new Matrix(res);
            }
            public static Matrix operator %(Matrix A, int m)
            {
                int size = A.size;
                Vector[] res = new Vector[size];
                Vector[] item = A.item;
                for (int i = 0; i < size; i++)
                    res[i] = Mod(item[i], m);
                return new Matrix(res);
            }
            public int Det()
            {
                Matrix A = new Matrix(this);
                Vector[] item_A = A.item;
                int det = 1,d=1;
                for (int i = size - 1; i >= 0; i--)
                {
                    Vector row_i = item_A[i], row_0=row_i;
                    int a_ii = row_i[i], k=i;
                    for (int j = i-1; j >= 0 && a_ii==0; j--)
                    {
                        row_i = item_A[j];
                        a_ii = row_i[i];
                        k = j; 
                    }
                    item_A[i] = row_i;
                    item_A[k] = row_0;
                    int sign = 1-2*(i != k).GetHashCode();
                    det *= a_ii*sign;
                    for (int j = 0; j < i; j++)
                    {
                        Vector row_j = item_A[j];
                        int a_ji = row_j[i];
                        if (a_ji!=0)
                        {
                            Vector res = row_j * a_ii - row_i * a_ji;
                            item_A[j] = res;
                            d *= a_ii;
                        }
                    }
                    
                }
                det /= d;
                return det;
            }
            public int this[int i,int j]
            {
                get => item[i][j];
                set
                {
                    item[i][j] = value;
                }
            }
            public void Show(DataGridView dgv)
            {
                dgv.ColumnCount = size;
                dgv.RowCount = size;
                for (int i = 0; i < size; i++)
                {
                    Vector vec = item[i];
                    for (int j = 0; j < size; j++)
                        dgv[j, i].Value = vec[j] + "";
                }
            }
            public int Size => size;
            public int Minor(int i,int j)
            {
                Matrix M = new Matrix(size - 1);
                int p = 0;
                for (int k = 0;k<size;k++)
                {
                    if (k != i)
                    {
                        Vector row = item[k];
                        int q = 0;
                        for (int t = 0; t < size; t++)
                        {
                            if (t != j)
                            {
                                M[p,q] = row[t];
                                q++;
                            }
                        }
                        p++;
                    }
                }
                return M.Det();
            }
            public int Cofactor(int i, int j)
            {
                return (1 - 2 * ((i + j) % 2))*Minor(i,j);
            }
            public Matrix TransposeCofactorMatrix()//Транспонированная матрица алгебраических дополнений
            {
                Matrix A = new Matrix(size);
                for (int i = 0; i < size; i++)
                {
                    Vector vec = A.item[i];
                    for (int j = 0; j < size; j++)
                        vec[j]=Cofactor(j,i);
                }
                return A;
            }
        }
        class CipherHill
        {
            Matrix matrix;
            public CipherHill(Matrix matrix)
            {
                this.matrix = matrix;
            }
            public Vector Crypt(Vector x, int m)
            {
                return matrix * x % m;
            }
            public string Crypt(String message, char s0, int m)
            {
                int len = message.Length;
                int size = matrix.Size;
                int alpha_count = 0; for (int i = 0; i < len; i++) alpha_count +=(message[i] >= 'a' && message[i] <='z').GetHashCode();
                int tail_size = (size - (alpha_count % size))% size;
                string tail = "";for (int i = 0; i < tail_size; i++) tail += (s0);

                int[] item = new int[size];
                int block_i = 0;
                StringBuilder sb = new StringBuilder(message+tail);
                int[] number_sym=new int[size];
                for (int i=0;i<len+tail_size;i++)
                {
                    char sym = sb[i];
                    int code = sym - s0;
                    if (code >= 0 && code < m)
                    {
                        item[block_i] = code;
                        number_sym[block_i] = i;
                        if (block_i==size-1)
                        {
                            Vector input_vector = new Vector(item);
                            Vector output_vector = Crypt(input_vector, m);
                            for (int j = 0; j <= block_i; j++)
                            {
                                item[j] = 0;
                                sb[number_sym[j]] = (char)(output_vector[j] + s0);
                            }
                        }
                        block_i = (block_i + 1) % size;
                    }
                }
                return sb.ToString();
            }
        }

        class Board
        {
            DataGridView dgv;
            Label info;
            RichTextBox input, output;
            int m,current_det=1;
            char s0;
            Matrix current_matrix;
            CipherHill hill_encrypt, hill_decrypt;
            int current_size=3;
            public Board(DataGridView dgv, int m,char s0, RichTextBox input, RichTextBox output,Label info)
            {
                this.dgv = dgv;this.m = m; this.input = input; this.output = output;
                this.info = info;
                current_matrix = new Matrix(new Vector[] { new Vector(new int[] { 1, 0, 0 }), new Vector(new int[] { 0, 1, 0 }), new Vector(new int[] { 0, 0, 1 }) });
                current_matrix.Show(dgv);this.s0 = s0;
                hill_encrypt = new CipherHill(current_matrix);
                hill_decrypt = new CipherHill(current_matrix);
                dgv.CellValidating += CellValidating;
                dgv.CellEndEdit += CellEndEdit;
            }
            private void CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
            {
                string str = e.FormattedValue + "";
                int res = -1;
                e.Cancel = !(int.TryParse(str, out res) && res >= 0);
            }
            private void CellEndEdit(object sender, DataGridViewCellEventArgs e)
            {
                if(!opening_file)
                {
                    int i = e.RowIndex, j = e.ColumnIndex;
                    current_matrix[i, j] = int.Parse(dgv[j,i].Value+"");
                    current_det = current_matrix.Det();
                    int y = 0, inverse_det=0;
                    GCD(current_det, m, ref inverse_det, ref y);
                    Matrix Inverse = (current_matrix.TransposeCofactorMatrix() % m) * inverse_det;
                    hill_decrypt = new CipherHill(Inverse);
                }
            }
            public void Encrypt()
            {
                info.Text = "Сообщение: ";
                if (current_det != 0 && GCD(current_det, m) == 1)
                {
                    string message = input.Text;
                    output.Text = hill_encrypt.Crypt(message.ToLower(), s0, m);
                }
                else
                    info.Text ="Сообщение: матрица не является обратимой, невозможно шифрование";
                
            }
            public void Decrypt()
            {
                info.Text = "Сообщение: ";
                if (current_det != 0 && GCD(current_det, m) == 1)
                {
                    string message = input.Text;
                    output.Text = hill_decrypt.Crypt(message.ToLower(), s0, m);
                }
                else
                    info.Text = "Сообщение: матрица не является обратимой, невозможно шифрование";

            }
            public void Save()
            {
                SaveFileDialog SFD = new SaveFileDialog();
                SFD.InitialDirectory = Application.StartupPath;
                SFD.Filter = "Матрица(*.dat)|*.dat";
                if (SFD.ShowDialog() == DialogResult.OK)
                {
                    string filename = SFD.FileName;
                    StreamWriter SW = new StreamWriter(filename);
                    int size = dgv.RowCount;
                    SW.WriteLine(size);
                    Vector[] vec = new Vector[size];
                    for (int i = 0; i < size; i++)
                    {
                        int[] item = new int[size];
                        for (int j = 0; j < size; j++)
                        {
                            string str = dgv[j, i].Value + "";
                            SW.Write(str+"\t");
                        }
                        SW.WriteLine();
                    }
                    SW.Close();
                }
            }
            bool opening_file = false;
            public void OpenFile()
            {
                OpenFileDialog OFD = new OpenFileDialog();
                OFD.InitialDirectory = Application.StartupPath;
                OFD.Filter = "Матрица(*.dat)|*.dat";
                if (OFD.ShowDialog() == DialogResult.OK)
                {
                    opening_file = true;
                    string filename = OFD.FileName;
                    StreamReader SR = new StreamReader(filename);
                    int size = int.Parse(SR.ReadLine());
                    current_size=size;
                    current_matrix = new Matrix(size);
                    //dgv.ColumnCount = size; dgv.RowCount = size;
                    for (int i = 0; i < size; i++)
                    {
                        string str = SR.ReadLine();
                        string[] Str = str.Split((char)9);
                        for (int j = 0; j < size; j++)
                        {
                            //dgv[j, i].Value = Str[j];
                            current_matrix[i, j] = int.Parse(Str[j]);
                        }
                    }
                    SR.Close();
                    hill_encrypt = new CipherHill(current_matrix);
                    current_det = current_matrix.Det();
                    int y = 0, inverse_det = 0;
                    GCD(current_det, m, ref inverse_det, ref y);
                    if ((current_det != 0 && GCD(current_det, m) == 1))
                    {
                        inverse_det = Mod(inverse_det, m);
                        Matrix Inverse = ((current_matrix.TransposeCofactorMatrix() % m) * inverse_det) % m;
                        hill_decrypt = new CipherHill(Inverse);
                    }
                    current_matrix.Show(dgv);
                    opening_file = false;
                }
            }
            public void IncreaseSize()
            {
                if(current_size<10)
                {
                    opening_file = true;
                    dgv.Rows.Add();
                    dgv.Columns.Add("","");
                    for (int i = 0; i < current_size; i++)
                    {
                        dgv[i, current_size].Value = 0;
                        dgv[current_size, i].Value = 0;
                    }
                    dgv[current_size, current_size].Value = 1;
                    current_size++;
                    current_matrix = new Matrix(current_size);
                    for (int i = 0; i < current_size; i++)
                        for (int j = 0; j < current_size; j++)
                        {
                            string str = dgv[j, i].Value+"";
                            current_matrix[i, j] = int.Parse(str);
                        }
                    hill_encrypt = new CipherHill(current_matrix);
                    current_det = current_matrix.Det();
                    int y = 0, inverse_det = 0;
                    GCD(current_det, m, ref inverse_det, ref y);
                    if ((current_det != 0 && GCD(current_det, m) == 1))
                    {
                        inverse_det = Mod(inverse_det, m);
                        Matrix Inverse = ((current_matrix.TransposeCofactorMatrix() % m) * inverse_det) % m;
                        hill_decrypt = new CipherHill(Inverse);
                    }
                    opening_file = false;
                }
            }
            public void DecreaseSize()
            {
                if (current_size > 1)
                {
                    opening_file = true; current_size--;
                    dgv.ColumnCount--;
                    dgv.RowCount--;
                    current_matrix = new Matrix(current_size);
                    for (int i = 0; i < current_size; i++)
                        for (int j = 0; j < current_size; j++)
                        {
                            string str = dgv[j, i].Value + "";
                            current_matrix[i, j] = int.Parse(str);
                        }
                    hill_encrypt = new CipherHill(current_matrix);
                    current_det = current_matrix.Det();
                    int y = 0, inverse_det = 0;
                    GCD(current_det, m, ref inverse_det, ref y);
                    if ((current_det != 0 && GCD(current_det, m) == 1))
                    {
                        inverse_det = Mod(inverse_det, m);
                        Matrix Inverse = ((current_matrix.TransposeCofactorMatrix() % m) * inverse_det) % m;
                        hill_decrypt = new CipherHill(Inverse);
                    }
                    opening_file = false;
                }
            }

        }
        Board board;
        public MainForm()
        {
            InitializeComponent();
            board = new Board(dataGridView1, 26,'a', richTextBox1, richTextBox2,label4);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            board.Encrypt();
        }

        private void открытьКлючToolStripMenuItem_Click(object sender, EventArgs e)
        {
            board.OpenFile();
        }

        private void сохранитьКлючToolStripMenuItem_Click(object sender, EventArgs e)
        {
            board.Save();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            board.Decrypt();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            board.IncreaseSize();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            board.DecreaseSize();
        }

        private void расшифроватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            board.Decrypt();
        }

        private void зашифроватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            board.Encrypt();
        }

        private void увеличитьРазмерностьМатрицыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            board.IncreaseSize();
        }

        private void уменьшитьРазмерностьМатрицыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            board.DecreaseSize();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Программа для работы с шифром Хилла. Для успешного пользования необходимо ввести" +
                "\nобратимую матрицу, т.е. определитель должен быть неравен нулю и определитель не должен иметь общие множители с 26");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
