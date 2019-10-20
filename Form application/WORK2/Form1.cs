using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace WORK2
{
    public partial class Form1 : Form
    {
        String now_DataBase;
        String now_Form;
        String connetStr = "server=127.0.0.1/localhost;port=3306;user=root;password=root@; database=mysql;";
        // server=127.0.0.1/localhost 代表本机，端口号port默认是3306可以不写
        MySqlConnection conn;
        public Form1()
        {
            InitializeComponent();
            setBaseHead();
            setFormHead();
            try
            {
                conn = new MySqlConnection(connetStr);
                conn.Open();//打开通道，建立连接，可能出现异常,使用try catch语句
                //在这里使用代码对数据库进行增删查改
                String sql = "show databases";
                //  this.listView1.Columns.Add("数据库");
               

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())//初始索引是-1，执行读取下一行数据，返回值是bool
                {
                    //Console.WriteLine(reader[0].ToString() + reader[1].ToString() + reader[2].ToString());
                    //Console.WriteLine(reader.GetInt32(0)+reader.GetString(1)+reader.GetString(2));
                    listView1.Items.Add( reader.GetString("Database")) ;//"userid"是数据库对应的列名，推荐这种方式
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                now_DataBase = listView1.SelectedItems[0].Text;
                String name= listView1.SelectedItems[0].Text;
                setFormHead();
               
                try
                {
                    conn.Open();
                    String sql = "select table_name from information_schema.tables where table_schema= "+"'"+name+"'";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
               
                    while (reader.Read())//初始索引是-1，执行读取下一行数据，返回值是bool
                    {
                        //Console.WriteLine(reader[0].ToString() + reader[1].ToString() + reader[2].ToString());
                        //Console.WriteLine(reader.GetInt32(0)+reader.GetString(1)+reader.GetString(2));
                        listView2.Items.Add(reader.GetString("table_name"));//"userid"是数据库对应的列名，推荐这种方式
                    }

                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            conn.Open();
            int result = 0;
            try
            {
                String sql;
                String name = textBox1.Text;
                sql = "create database " + name;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                result= cmd.ExecuteNonQuery();
               
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            if (result == 1)
            {
                MessageBox.Show("任务完成");

                refreshBase();
            }
        }
        private void refreshBase()
        {
            
            try
            {
                conn.Open();
                String sql = "show databases";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                setBaseHead();
                while (reader.Read())//初始索引是-1，执行读取下一行数据，返回值是bool
                {
                    //Console.WriteLine(reader[0].ToString() + reader[1].ToString() + reader[2].ToString());
                    //Console.WriteLine(reader.GetInt32(0)+reader.GetString(1)+reader.GetString(2));
                    listView1.Items.Add(reader.GetString("Database"));//"userid"是数据库对应的列名，推荐这种方式
                }

            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        private void refreshForm()
        {
            conn.Open();
            setFormHead();
            String name = now_DataBase;
            try
            {
                String sql = "select table_name from information_schema.tables where table_schema= " + "'" + name + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())//初始索引是-1，执行读取下一行数据，返回值是bool
                {
                    //Console.WriteLine(reader[0].ToString() + reader[1].ToString() + reader[2].ToString());
                    //Console.WriteLine(reader.GetInt32(0)+reader.GetString(1)+reader.GetString(2));
                    listView2.Items.Add(reader.GetString("table_name"));//"userid"是数据库对应的列名，推荐这种方式
                }

            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        private void setBaseHead()
        {
            listView1.Clear();
            ColumnHeader ch = new ColumnHeader();

            ch.Text = "数据库";   //设置列标题

            ch.Width = 140;    //设置列宽度

            ch.TextAlign = HorizontalAlignment.Left;   //设置列的对齐方式

            this.listView1.Columns.Add(ch);    //将列头添加到ListView控件。
        }
        private void setFormHead()
        {
            listView2.Clear();
            ColumnHeader ch2 = new ColumnHeader();

            ch2.Text = "表";   //设置列标题

            ch2.Width = 140;    //设置列宽度

            ch2.TextAlign = HorizontalAlignment.Left;   //设置列的对齐方式

            this.listView2.Columns.Add(ch2);    //将列头添加到ListView控件。
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            conn.Open();
            int res = -1;
            String name = now_DataBase;
            try
            {
                String sql = "drop database " + name;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                res = cmd.ExecuteNonQuery();


            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            if (res != -1)
            {
                refreshBase();
                if (this.listView1.Items.Count > 0)
                {
                    this.listView1.Items[0].Selected = true;
                    now_DataBase = listView1.SelectedItems[0].Text;

                }
                else
                {
                    now_DataBase = "";
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (this.listView1.Items.Count > 0&&now_DataBase!="")
            {

                conn.Open();
                int res = -1;
                String name = listView2.SelectedItems[0].Text;
                try
                {
                    String sql = "drop table "+ now_DataBase+"."+name ;
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                 
                
                    res = cmd.ExecuteNonQuery();


                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                if (res != -1)
                {
                    refreshForm();
                  
                }
            }
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.listView1.Items.Count > 0 && now_DataBase != "")
            {

                conn.Open();
                int res = -1;
                String name = textBox2.Text;
                try
                {
                    String sql = "create table " + now_DataBase + "." + name+"(id int)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);


                    res = cmd.ExecuteNonQuery();


                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("添加失败，检查是否重名");
                }
                finally
                {
                    conn.Close();
                }
                if (res != -1)
                {
                    refreshForm();

                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            String name = listView1.SelectedItems[0].Text;
            String newName = textBox3.Text;
            int res = -1;
            int rrr = -1;
            List<String> nameList = new List<string>();
            conn.Open();
            try
            {
                String sql = "create database  if not exists " + newName; 
                MySqlCommand cmd = new MySqlCommand(sql, conn);
               
                res=cmd.ExecuteNonQuery();

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            if(res==1)
            {
                
                try
                {
                    conn.Open();
                    String sql = "select table_name from information_schema.tables where table_schema= " + "'" + name + "'";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())//初始索引是-1，执行读取下一行数据，返回值是bool
                    {
                        //Console.WriteLine(reader[0].ToString() + reader[1].ToString() + reader[2].ToString());
                        //Console.WriteLine(reader.GetInt32(0)+reader.GetString(1)+reader.GetString(2));
                        nameList.Add( reader.GetString("table_name"));//"userid"是数据库对应的列名，推荐这种方式
                        
                    }
                  
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                for (int i = 0; i < nameList.Count; i++)
                {
                    int k = -1;
                 
                    try
                    {
                        conn.Open();
                        String sql = "rename table " + now_DataBase + "." + nameList[i] + " to " + newName + "." + nameList[i] ;
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        k = cmd.ExecuteNonQuery();
                        conn.Close();



                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                       conn.Close();
                    }
                }
                
                try
                {
                    conn.Open();
                    String sql = "drop database " + name;
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    int k = cmd.ExecuteNonQuery();
                    
                    



                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close();
                    refreshBase();
                    if (this.listView1.Items.Count > 0)
                    {
                        this.listView1.Items[0].Selected = true;
                        now_DataBase = listView1.SelectedItems[0].Text;

                    }
                    else
                    {
                        now_DataBase = "";
                    }
                }
                
               
                
            }
            else
            {
                MessageBox.Show("创建新数据库时失败，是否存在重名？");
            }
            
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count != 0)
            {
                now_Form = listView2.SelectedItems[0].Text;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            conn.Open();
            int res = -1;
            String name = textBox4.Text;
            try
            {
                String sql = "rename table " + now_DataBase+"."+ now_Form + " to " + now_DataBase+"." +name ;
                MySqlCommand cmd = new MySqlCommand(sql, conn);


                res = cmd.ExecuteNonQuery();


            }
            catch (MySqlException ex)
            {
                MessageBox.Show("添加失败，检查是否重名");
            }
            finally
            {
                conn.Close();
            }
            if (res != -1)
            {
                refreshForm();
                textBox4.Clear();

            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            form2 work = new form2(now_DataBase,now_Form);
            work.Show();
        }
    }
}
