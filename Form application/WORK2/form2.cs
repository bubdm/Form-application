using MySql.Data.MySqlClient;
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
using WORK2.SerializeTree;
namespace WORK2
{
    public partial class form2 : Form
    {
        String connetStr = "server=127.0.0.1/localhost;port=3306;user=root;password=root@; database=mysql;";
        // server=127.0.0.1/localhost 代表本机，端口号port默认是3306可以不写
        MySqlConnection conn;
        String Base;
        String Form;
        DataTable Dt;
        public form2(String Base ,String Form)
        {
            InitializeComponent();
            this.Form = Form;
            this.Base = Base;
            conn = new MySqlConnection(connetStr);
            label1.Text = Base + "." + Form;
            conn.Open();
            try
            {
                String sql = "select * from "+Base+"."+Form;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                //BindingSource bs = new BindingSource();
                Dt = new DataTable();

                Dt.Load(reader);
                //bs.DataSource = reader;
             
                this.dataGridView1.DataSource = Dt;
                dataGridView1.ReadOnly = false;

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

        private void form2_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataTableToXml(Dt);
            MessageBox.Show("任务完成");
        }
        public  void DataTableToXml(DataTable vTable)
        {
            string savePath = Application.StartupPath.ToString();
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            string xml = savePath + @"\"+Base+"."+Form+".xml";
            //如果文件DataTable.xml存在则直接删除
            if (File.Exists(xml))
            {
                File.Delete(xml);
            }
            vTable.WriteXml(savePath + @"\" + Base + "." + Form + ".xml");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String path;
            OpenFileDialog file = new OpenFileDialog();
            file.ShowDialog();
            path = file.FileName;
            TreeViewDataAccess.SaveTreeViewData(treeView1, path);
            MessageBox.Show("任务完成");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String path;
            OpenFileDialog file = new OpenFileDialog();
            file.ShowDialog();
            path = file.SafeFileName;
            TreeViewDataAccess.LoadTreeViewData(treeView1, path);
        }
        private void InitModuleTree(DataTable dt)
        {
            //清空treeview上所有节点
            this.treeView1.Nodes.Clear();
            int[] gen = new int[dt.Rows.Count]; //用于存储父节点Tag
            int[] zi = new int[dt.Rows.Count];  //用于存储子节点Tag
            for (int i = 0; i < gen.Length; i++)
            {
                string zhi = dt.Rows[i][3].ToString();//获取节点Tag值   eg：1-2
                if (zhi.Length > 1)   //表示是子节点   eg：1-2
                {
                    gen[i] = int.Parse(zhi.Substring(0, zhi.IndexOf('-')));
                    zi[i] = int.Parse(zhi.Substring(zhi.IndexOf('-') + 1));
                }
                else    //表示是根节点   eg：2
                {
                    //将所有父节点加到treeview上
                    zi[i] = int.Parse(zhi);
                    TreeNode nodeParent = new TreeNode();
                    nodeParent.Tag = (zi[i]).ToString();
                    nodeParent.Text = dt.Rows[i][1].ToString();
                    treeView1.Nodes.Add(nodeParent);
                }
            }
            bindChildNote(dt, gen, zi);
        }

        //绑定子节点
        private void bindChildNote(DataTable dt, int[] gen, int[] zi)
        {
            for (int i = 0; i < gen.Length; i++)
            {
                if (gen[i] != 0 && zi[i] != 0)        //便利所有节点，找到所有子节点
                {
                    TreeNode childNode = new TreeNode();
                    foreach (TreeNode item in treeView1.Nodes)   //便历treeview上所有父节点
                    {
                        if (item.Tag.ToString() == gen[i].ToString())  //找到当前子节点的父节点
                        {
                            childNode.Tag = zi[i].ToString();
                            childNode.Text = dt.Rows[i][1].ToString();
                            item.Nodes.Add(childNode);
                        }
                    }
                }
            }
            treeView1.ExpandAll();      //展开整棵树
        }

        private void button5_Click(object sender, EventArgs e)
        {
            InitModuleTree(Dt);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            TreeNode newP = new TreeNode(textBox1.Text);
            TreeNode nowS = treeView1.SelectedNode;
            nowS.Nodes.Add(newP);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TreeNode n= treeView1.SelectedNode;
            n.Remove();
        }
    }
}
