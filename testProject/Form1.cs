using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bunifu.Data.Helper;
namespace testProject
{
    public partial class Form1 : Form
    {
        Mysql DB = new Mysql("company");

        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string querry = "Select * from customers";


            dataGridView1.DataSource = DB.GetRows(querry);

            
            List<Customer> customers =   BunifuMapper.MapToList<Customer>(DB.GetRows("Select * from customers"));

       
            Customer c = customers.Where(r=>r.ContactName.Contains("too")).FirstOrDefault();

            MessageBox.Show(c.CustomerID.ToString());
           
        }

        private void bunifuTextBox1_TextChange(object sender, EventArgs e)
        {
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bunifuTextBox1.AutoCompleteCustomSource = DB.GetAutoCompleteStringCollection("Select contactName from customers");

              
        }

     

        private void button2_Click(object sender, EventArgs e)
        {
            ModelGenerator.GenerateModel(DB.GetRows("Select * from customers").ToTable(), "Customer");
            ModelGenerator.GenerateModel(DB.GetRows("Select * from categories").ToTable(), "Category", true);
            ModelGenerator.GenerateModel(DB.GetRows("Select * from employees").ToTable(), "Employee", true);
            ModelGenerator.GenerateModel(DB.GetRows("Select * from orders").ToTable(), "Order", true);
            ModelGenerator.GenerateModel(DB.GetRows("Select * from Products").ToTable(), "Product", true);
            ModelGenerator.GenerateModel(DB.GetRows("Select * from Shippers").ToTable(), "Shipper", true);
            MessageBox.Show("Done");
        }

        /****CUSTOMER MODEL****/
        public class Customer
        {
            public Int32 CustomerID { get; set; }
            public String CustomerName { get; set; }
            public String ContactName { get; set; }
            public String Address { get; set; }
            public String City { get; set; }
            public String PostalCode { get; set; }
            public String Country { get; set; }
        }


        /****CATEGORY MODEL****/
        public class Category
        {
            public Int32 CategoryID { get; set; }
            public String CategoryName { get; set; }
            public String Description { get; set; }
        }


        /****EMPLOYEE MODEL****/
        public class Employee
        {
            public Int32 EmployeeID { get; set; }
            public String LastName { get; set; }
            public String FirstName { get; set; }
            public DateTime BirthDate { get; set; }
            public String Photo { get; set; }
            public String Notes { get; set; } 
        }


        /****ORDER MODEL****/
        public class Order
        {
            public Int32 OrderID { get; set; }
            public Int32 CustomerID { get; set; }
            public Int32 EmployeeID { get; set; }
            public DateTime OrderDate { get; set; }
            public Int32 ShipperID { get; set; }
        }


        /****PRODUCT MODEL****/
        public class Product
        {
            public Int32 ProductID { get; set; }
            public String ProductName { get; set; }
            public Int32 SupplierID { get; set; }
            public Int32 CategoryID { get; set; }
            public String Unit { get; set; }
            public Double Price { get; set; }
        }


        /****SHIPPER MODEL****/
        public class Shipper
        {
            public Int32 ShipperID { get; set; }
            public String ShipperName { get; set; }
            public String Phone { get; set; }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(BunifuMapper.GenerateInsert<Customer>(new Customer() {
                ContactName="Kim Too Flex",
                Address="001",
                City="Nairobi",
                Country="Kenya", 
                CustomerName="Tiondo",
                PostalCode="00100" 
            },"Customers"));
        }
    }
}
