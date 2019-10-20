#

#实现说明
本Solution位于\WORK2下，由4个文件组成，Form1.cs,form2.cs,Program.cs和save.cs。
Program.cs为主程序。
Form1.cs 为主窗体，负责数据库和表的相关操作，如重命名删除，添加功能。
form2.cs 为子窗体 负责具体选定的数据表的编辑修改保存等功能。
save.cs 类，主要工作是完成树形结构的序列化与反序列化。

dataTable 保存位置：\WORK2\bin\Debug\当前数据库.当前数据表.xml 或者\WORK2\bin\realease\当前数据库.当前数据表.xml。
树形结构保存位置：用户在运行程序时自行选择。
树形结构的序列化装载：用户自行选择文件。

下面是程序中较为重要的一些过程
```重命名数据库
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
                        
                        nameList.Add( reader.GetString("table_name"));
                        
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
```
    

        
```打开新窗口，显示表的具体数据
        private void button8_Click(object sender, EventArgs e)
        {
            form2 work = new form2(now_DataBase,now_Form);
            work.Show();
        }
```
```treeview的序列化与反序列化
            public static void LoadTreeViewData(TreeView treeView, string path)
            {
                BinaryFormatter ser = new BinaryFormatter();
                Stream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                TreeViewData treeData = ((TreeViewData)(ser.Deserialize(file)));
                treeData.PopulateTree(treeView);
                file.Close();
            }
            /// <summary>
            /// 保存TreeView到文件
            /// </summary>
            /// <param name="treeView"></param>
            /// <param name="path"></param>
            public static void SaveTreeViewData(TreeView treeView, string path)
            {
                BinaryFormatter ser = new BinaryFormatter();
                Stream file = new FileStream(path, FileMode.Create);
                ser.Serialize(file, new TreeViewData(treeView));
                file.Close();
            }
```
# 运行说明

## 使用VS运行

1. 需要Visual Studio 2017版本和.NET Framework 4.6.1或以上的.NET SDK
2. 通过NuGet管理包安装Google.Protobuf和MySql.Data
3. 使用VS打开整个解决方案，并选择Build -> Build Solution
4. 运行主程序。

## 独立运行

1. 需要.NET Framework 4.6.1或以上的.NET SDK
2. 通过https://dev.mysql.com/downloads/connector/net/8.0.html下载Connector / NET并配置到本项目地址
3. 运行主程序。

# References

How to Serialize an Object to XML
https://stackoverflow.com/questions/4123590/serialize-an-object-to-xml

How to Serialize treeView
https://blog.csdn.net/joetao/article/details/2835676

How to save dataTable as xml
https://docs.microsoft.com/en-us/dotnet/api/system.data.datatable.writexml?view=netframework-4.7.2

How to do CRUD work on MYSQL with C#
https://dev.mysql.com/doc/connector-net/en/connector-net-tutorials-sql-command.html

How to rename database Mysql5.7
https://stackoverflow.com/questions/67093/how-do-i-quickly-rename-a-mysql-database-change-schema-name