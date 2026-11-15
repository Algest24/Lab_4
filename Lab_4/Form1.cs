using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_4
{
    public partial class Form1 : Form
    {
        List<Product> products;
        private List<Product> LoadProducts(string v)
        {
            if (File.Exists(v))
            {
                var x = File.ReadAllLines(v).Skip(1).Select(str =>
                    new Product(str)).ToList();
                return x;
            }
            else
                return new List<Product>();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //загрузка данных о товарах из файла
            products = LoadProducts("Товары.csv");
            //вывод товаров в dataGridView1
            dataGridView1.DataSource = products;//привязка данных к таблице dataGridView1
                                                //автоматически сгенерировать столбцы таблицы по свойствам привязанных объектов
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.AutoResizeColumns(); //подогнать размер столбцов таблицы под содержимое

            //автогенерировать столбцы по свойствам привязанных объектов
            dataGridView2.AutoGenerateColumns = true;
            //заполнить список категориями товаров
            var query = (from p in products orderby p.CategoryName select p.CategoryName).Distinct().ToArray();
            toolStripComboBox1.Items.AddRange(query); //задать список товаров для toolStripComboBox1
            //задание ширины выпадающего списка под самую длинную строку категории
            toolStripComboBox1.DropDownWidth = query.Max(x => TextRenderer.MeasureText(x, toolStripComboBox1.Font).Width);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //Запрос - категории товаров без дублирования
            var query = (from p in products orderby p.CategoryName select new { Name = p.CategoryName }).Distinct().ToList();
            dataGridView2.DataSource = query; //привязка данных к таблице dataGridView2
            dataGridView2.AutoResizeColumns(); //подогнать размер столбцов таблицы под содержимое
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            // Запрос - Группировка по поставщикам
            var query = (from p in products
                         group p by p.CompanyName into g
                         orderby g.Key descending
                         select new
                         {
                             Name = g.Key,
                             Count = g.Count(),
                             Min = g.Min(x => x.Cost),
                             Aver = g.Average(x => x.Cost),
                             Max = g.Max(x => x.Cost),
                         }).ToList();

            dataGridView2.DataSource = query;  //привязка данных к таблице dataGridView2
            dataGridView2.AutoResizeColumns(); //подогнать размер столбцов таблицы под содержимое
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string category = toolStripComboBox1.Text;

            var query = (from p in products
                         where p.CategoryName == category
                         orderby p.Count descending
                         select p).ToList();

            dataGridView2.DataSource = query;
            dataGridView2.AutoResizeColumns();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            var query = (from p in products
                         where p.Count < p.DesiredCount
                         orderby (p.DesiredCount - p.Count) descending
                         select p).ToList();
            dataGridView2.DataSource = query;  //привязка данных к таблице dataGridView2
            dataGridView2.AutoResizeColumns(); //подогнать размер столбцов таблицы под содержимое
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            var query = (from p in products
                         orderby p.Price descending
                         select p).Take(5).ToList();
            dataGridView2.DataSource = query;  //привязка данных к таблице dataGridView2
            dataGridView2.AutoResizeColumns(); //подогнать размер столбцов таблицы под содержимое
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            var query = (from p in products
                         group p by p.CategoryName into g
                         select new
                         {
                             Category = g.Key,
                             AveragePrice = g.Average(x => x.Price),
                             ProductCount = g.Count()
                         }).ToList();
            dataGridView2.DataSource = query;  //привязка данных к таблице dataGridView2
            dataGridView2.AutoResizeColumns(); //подогнать размер столбцов таблицы под содержимое
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            var query = (from p in products
                         select new
                         {
                             Наименование = p.ProductName,
                             Наценка = p.Price - p.Cost,
                             ОбщаяСтоимость = p.Price * p.Count
                         }).ToList();
            dataGridView2.DataSource = query;  //привязка данных к таблице dataGridView2
            dataGridView2.AutoResizeColumns(); //подогнать размер столбцов таблицы под содержимое
        }
    }
}
