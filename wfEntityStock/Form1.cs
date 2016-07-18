using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wfEntityStock
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        StockEntities ent = new StockEntities();

        private void Form1_Load(object sender, EventArgs e)
        {
            GetAllCategories();
        }
        private void GetAllCategories()
        {
            var Kategoriler = (from k in ent.Categories
                               select k.name).ToList();
            cbCategories.DataSource = Kategoriler;
        }
        private void cbCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            string Kategori = cbCategories.SelectedItem.ToString();
            int CatID = (from k in ent.Categories
                         where k.name == Kategori
                         select k.id).First();
            txtCategoryID.Text = CatID.ToString();
            GetAllProductsByCategory(CatID);
        }
        private void GetAllProductsByCategory(int CategoryID)
        {
            var Urunler = (from u in ent.Products
                           where u.CategoryID == CategoryID
                           select new { u.Id, u.Name, CategoryName=u.Categories.name, u.Price, u.UnitsInStock, u.CategoryID }).ToList();
            dgvProducts.DataSource = Urunler;
            dgvProducts.Columns[1].Width = 150;
            dgvProducts.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvProducts.Columns["UnitsInStock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvProducts.Columns["CategoryID"].Visible = false;
        }
        private void dgvProducts_DoubleClick(object sender, EventArgs e)
        {
            txtProductID.Text = dgvProducts.SelectedRows[0].Cells[0].Value.ToString();
            txtProductName.Text = dgvProducts.SelectedRows[0].Cells[1].Value.ToString();
            txtPrice.Text = dgvProducts.SelectedRows[0].Cells[3].Value.ToString();
            txtUnitsInStock.Text = dgvProducts.SelectedRows[0].Cells[4].Value.ToString();
            txtCategoryID.Text = dgvProducts.SelectedRows[0].Cells[5].Value.ToString();
            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;
            btnSave.Enabled = false;
            txtProductName.Focus();
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            btnSave.Enabled = true;
            Temizle();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtProductName.Text.Trim() != "")
            {
                if (ProductControlByName(txtProductName.Text))
                {
                    MessageBox.Show("Bu ürün zaten kayıtlı!");
                    txtProductName.Focus();
                }
                else
                {
                    Products p = new Products();
                    p.Name = txtProductName.Text;
                    p.Price = Convert.ToDecimal(txtPrice.Text);
                    p.UnitsInStock = Convert.ToInt32(txtUnitsInStock.Text);
                    p.CategoryID = Convert.ToInt32(txtCategoryID.Text);
                    ent.Products.Add(p); //Yeni ürün nesnesi arakatmana eklenir.
                    try
                    {
                        ent.SaveChanges(); //Arakatmana göre veritabanı güncellenir.
                        MessageBox.Show("Ürün bilgileri eklendi.");
                        btnSave.Enabled = false;
                        GetAllProductsByCategory(Convert.ToInt32(txtCategoryID.Text));
                        Temizle();
                    }
                    catch (Exception ex)
                    {
                        string hata = ex.Message;
                    }
                }
            }
        }
        private bool ProductControlByName(string ProductName)
        {
            int sayisi = (from p in ent.Products
                          where p.Name == ProductName
                          select p).Count();
            return Convert.ToBoolean(sayisi);
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int degisenID = Convert.ToInt32(txtProductID.Text);
            var degisen = (from p in ent.Products   //değiştirilecek kayda odaklanır.
                           where p.Id == degisenID
                           select p).FirstOrDefault();
            degisen.Name = txtProductName.Text;
            degisen.Price = Convert.ToDecimal(txtPrice.Text);
            degisen.UnitsInStock = Convert.ToInt32(txtUnitsInStock.Text);
            degisen.CategoryID = Convert.ToInt32(txtCategoryID.Text);
            try
            {
                ent.SaveChanges(); //Arakatmana göre veritabanı güncellenir.
                MessageBox.Show("Ürün bilgileri güncellendi.");
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
                GetAllProductsByCategory(Convert.ToInt32(txtCategoryID.Text));
                Temizle();
            }
            catch (Exception ex)
            {
                string hata = ex.Message;
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Silmek İstiyor musunuz?", "SİLİNSİN Mİ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                int silinenID = Convert.ToInt32(txtProductID.Text);
                var silinen = (from p in ent.Products  //Silinecek kayıt gösterilir.
                               where p.Id == silinenID
                               select p).FirstOrDefault();
                ent.Products.Remove(silinen); //Arakatmandan siler.
                try
                {
                    ent.SaveChanges(); //Arakatmana göre veritabanı güncellenir.
                    MessageBox.Show("Ürün bilgileri silindi.");
                    btnUpdate.Enabled = false;
                    btnDelete.Enabled = false;
                    GetAllProductsByCategory(Convert.ToInt32(txtCategoryID.Text));
                    Temizle();
                }
                catch (Exception ex)
                {
                    string hata = ex.Message;
                }
            }
        }
        private void Temizle()
        {
            txtProductName.Clear();
            txtPrice.Text = "0";
            txtUnitsInStock.Text = "1";
            txtProductName.Focus();
        }






    }
}
