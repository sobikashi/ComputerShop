﻿using ComputerShop.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerShop
{
    public partial class Main_form : Form
    {

        ComputersShopContainer1 db;

        protected const string regText = @"(?m)^.[a-zA-Zа-яА-Я0-9 -]{2,30}(?=\r?$)";
        protected const string regCode = @"(?m)^[A-Z0-9]{13}$";

        public Main_form()
        {
            InitializeComponent();

            this.Load += Main_form_Load;
        }

        private void Main_form_Load(object sender, EventArgs e)
        {
            db = new ComputersShopContainer1();

            FillData();

            mf_addCategoryBtn.Click += Mf_addCategoryBtn_Click;
            mf_CreateComponent.Click += Mf_CreateComponent_Click;
            mf_createComputer.Click += Mf_createComputer_Click;
            mf_creatSelling.Click += Mf_creatSelling_Click;
            mf_cb_period.CheckedChanged += Mf_cb_period_CheckedChanged;
            mf_dtp_from.ValueChanged += Mf_dtp_from_ValueChanged;
            mf_dtp_to.ValueChanged += Mf_dtp_from_ValueChanged;
        }

        /// <summary>
        /// Обработка изменения даты для фильтрации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mf_dtp_from_ValueChanged(object sender, EventArgs e)
        {
            FillData();
        }

        /// <summary>
        /// Обработка выбора чекбокса период
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mf_cb_period_CheckedChanged(object sender, EventArgs e)
        {
            if (mf_cb_period.Checked == true)
            {
                mf_dtp_from.Enabled = true;
                mf_dtp_to.Enabled = true;
            }
            else
            {
                mf_dtp_from.Enabled = false;
                mf_dtp_to.Enabled = false;
            }
        }

        /// <summary>
        /// Обработка нажатия кнопки создать продажу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mf_creatSelling_Click(object sender, EventArgs e)
        {
            SellingForm sf = new SellingForm();

            if (sf.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Продажа прошла успешно");
                FillData();
            }
        }

        /// <summary>
        /// Добавление компьютера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mf_createComputer_Click(object sender, EventArgs e)
        {
            ComputerForm cf = new ComputerForm();

            if (cf.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Новый комьютер добавлен");
            }
        }

        /// <summary>
        /// Обработка нажатия кнопки добавить компонент
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mf_CreateComponent_Click(object sender, EventArgs e)
        {
            ComponentForm cf = new ComponentForm();

            cf.c_cb_category.DataSource = db.Category.ToList();
            cf.c_cb_category.ValueMember = "Id";
            cf.c_cb_category.DisplayMember = "Title";

            if (cf.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (cf.c_cb_category.SelectedIndex == -1)
                    {
                        throw new Exception("Вы не выбрали категорию");
                    }

                    int categoryId = Convert.ToInt32(cf.c_cb_category.SelectedValue);

                    if (!Regex.IsMatch(cf.c_tb_title.Text, regText))
                    {
                        throw new Exception("Ошибка в наименовании!");
                    }

                    if (!Regex.IsMatch(cf.c_tb_code.Text, regCode))
                    {
                        throw new Exception("Ошибка в артикуле!\n" +
                            "Только заглавные английские буквы\n" +
                            "Только цифры!\nДлина 13 символов");
                    }

                    decimal price = (decimal)(Convert.ToDouble(cf.c_tb_price.Text));
                    int quantity = Convert.ToInt32(cf.c_nud_quantity.Value);

                    Component component = new Component();
                    component.Title = cf.c_tb_title.Text;
                    component.CategoryId = categoryId;
                    component.Vendor_code = cf.c_tb_code.Text;
                    component.Price = price;
                    component.Quantity = (short)quantity;
                    component.Description = cf.cf_tb_description.Text;
                    db.Component.Add(component);
                    db.SaveChanges();

                    MessageBox.Show("Новый компонент добавлен!");
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message);
                }
            }
        }

        /// <summary>
        /// Обработка нажатия кнопки Добавление категории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mf_addCategoryBtn_Click(object sender, EventArgs e)
        {
            CategoryForm cf = new CategoryForm();

            if (cf.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (Regex.IsMatch(cf.categoryName.Text, regText))
                    {
                        Category category = new Category();
                        category.Title = cf.categoryName.Text;
                        db.Category.Add(category);
                        db.SaveChanges();

                        MessageBox.Show("Новая категория добавлена");
                    }
                    else
                    {
                        throw new Exception("Ошибка в названии категории");
                    }
                    
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message);
                }
                
            }
        }

        /// <summary>
        /// Отображение сообщения
        /// </summary>
        /// <param name="msg"></param>
        private void ShowMessage(string msg)
        {
            MessageBox.Show(msg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Заполнение данными DataGridView
        /// </summary>
        private void FillData()
        {
            try
            {
                if (mf_cb_period.Checked == true)
                {
                    mf_data.DataSource = db.Check.Where(check => check.Date >= mf_dtp_from.Value && check.Date <= mf_dtp_to.Value).Select(check =>
                    new {
                        ID = check.Id,
                        Фамилия_покупателя = check.Buyer.LastName,
                        Имя_покупателя = check.Buyer.FirstName,
                        Дата_продажи = check.Date,
                        Сумма = check.CheckCoast
                    }).ToList();
                }
                else
                {
                    mf_data.DataSource = db.Check.Select(check =>
                    new {
                    ID = check.Id,
                    Фамилия_покупателя = check.Buyer.LastName,
                    Имя_покупателя = check.Buyer.FirstName,
                    Дата_продажи = check.Date,
                    Сумма = check.CheckCoast
                    }).ToList();
                }
                

                GetTotalCheckCoast();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        /// <summary>
        /// Подсчет общей суммы продаж
        /// </summary>
        private void GetTotalCheckCoast()
        {
            double total = 0;

            foreach (DataGridViewRow item in mf_data.Rows)
            {
                total += (double)item.Cells[4].Value;
            }

            mf_sb_totalCoast.Text = total.ToString();
        }
    }
}
