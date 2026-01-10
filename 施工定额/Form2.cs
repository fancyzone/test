using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using static 施工定额.Program;

namespace 施工定额
{
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();
            // 设置默认选中的索引，这里选择第一个选项
            comboBox2.SelectedIndex = 0;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            tabControl1.SelectedIndex = 0;
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 获取当前选中的选项卡的索引
            int selectedIndex = tabControl1.SelectedIndex;
            // 获取当前选中的选项卡的名称
            string selectedTabName;
            // 显示消息，告知用户当前选中的选项卡
            if (selectedIndex == 0)//清单界面
            {
                treeView1.Nodes.Clear();
                string connectionString = "Data Source=systemDB.db;Version=3;";
                SQLiteConnection connection = new(connectionString);
                try
                {
                    connection.Open();
                    // 在这里可以执行数据库操作，如查询、插入、更新等
                    // 定义 SQL 查询语句
                    string query = "SELECT * FROM 清单";
                    DataAdapter DA = new SQLiteDataAdapter(query, connection);
                    DataSet DS = new();
                    // 填充数据集
                    DA.Fill(DS);
                    // 将数据集绑定到 DataGridView
                    dataGridView1.DataSource = DS.Tables[0];

                    // 定义 SQL 查询语句
                    string query1 = "SELECT 标准号 FROM 清单 GROUP BY 标准号";
                    DataAdapter DA1 = new SQLiteDataAdapter(query1, connection);
                    DataSet DS1 = new();
                    // 填充数据集
                    DA1.Fill(DS1);
                    // 将数据集绑定到 comboBox1
                    comboBox1.DataSource = DS1.Tables[0];
                    comboBox1.DisplayMember = "标准号";

                    // 定义 SQL 查询语句
                    string query2 = "SELECT 标准号,专业类别 FROM 清单 GROUP BY 专业类别";
                    string query3 = "SELECT 标准号,分部工程,专业类别 FROM 清单 GROUP BY 分部工程";
                    string query4 = "SELECT 标准号,分项工程,分部工程,专业类别 FROM 清单 GROUP BY 分项工程";
                    DataAdapter DA2 = new SQLiteDataAdapter(query2, connection);
                    DataAdapter DA3 = new SQLiteDataAdapter(query3, connection);
                    DataAdapter DA4 = new SQLiteDataAdapter(query4, connection);
                    DataSet DS2 = new();
                    DataSet DS3 = new();
                    DataSet DS4 = new();
                    // 填充数据集
                    DA2.Fill(DS2);
                    DataTable dataTable2 = DS2.Tables[0];
                    DA3.Fill(DS3);
                    DataTable dataTable3 = DS3.Tables[0];
                    DA4.Fill(DS4);
                    DataTable dataTable4 = DS4.Tables[0];

                    //将数据库中数据显示在treeview
                    foreach (DataRow row2 in dataTable2.Rows)
                    {
                        // 获取列的值并显示
                        TreeNode rootNode = new(row2["专业类别"].ToString());
                        // 筛选出满足条件的行，并存储在 selectedRows 数组中
                        DataRow[] selectedRows = dataTable3.Select($"标准号 = '{comboBox1.Text}' AND 专业类别 = '{row2["专业类别"]}'");
                        foreach (DataRow row3 in selectedRows)
                        {
                            TreeNode childNode1 = new(row3["分部工程"].ToString());
                            rootNode.Nodes.Add(childNode1);

                            DataRow[] selectedRows4 = dataTable4.Select($"标准号 = '{comboBox1.Text}' AND 分部工程 = '{row3["分部工程"]}' AND 专业类别 = '{row2["专业类别"]}'");
                            foreach (DataRow row4 in selectedRows4)
                            {
                                TreeNode grandChildNode1 = new(row4["分项工程"].ToString());
                                childNode1.Nodes.Add(grandChildNode1);
                            }
                        }
                        treeView1.Nodes.Add(rootNode);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("连接数据库时出错: " + ex.Message);
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }

            if (selectedIndex == 1) //定额界面
            {
                treeView2.Nodes.Clear();
                string connectionString = "Data Source=systemDB.db;Version=3;";
                SQLiteConnection connection = new(connectionString);
                try
                {
                    connection.Open();
                    // 在这里可以执行数据库操作，如查询、插入、更新等
                    // 定义 SQL 查询语句
                    string query = $"SELECT * FROM {comboBox2.Text}";
                    DataAdapter DA = new SQLiteDataAdapter(query, connection);
                    DataSet DS = new();
                    // 填充数据集
                    DA.Fill(DS);
                    // 将数据集绑定到 DataGridView
                    dataGridView2.DataSource = DS.Tables[0];

                    // 定义 SQL 查询语句
                    string query2 = $"SELECT 册 FROM {comboBox2.Text} GROUP BY 册";
                    string query3 = $"SELECT 章,册 FROM {comboBox2.Text} GROUP BY 章";
                    string query4 = $"SELECT 节,章,册 FROM {comboBox2.Text} GROUP BY 节";
                    DataAdapter DA2 = new SQLiteDataAdapter(query2, connection);
                    DataAdapter DA3 = new SQLiteDataAdapter(query3, connection);
                    DataAdapter DA4 = new SQLiteDataAdapter(query4, connection);
                    DataSet DS2 = new();
                    DataSet DS3 = new();
                    DataSet DS4 = new();
                    // 填充数据集
                    DA2.Fill(DS2);
                    DataTable dataTable2 = DS2.Tables[0];
                    DA3.Fill(DS3);
                    DataTable dataTable3 = DS3.Tables[0];
                    DA4.Fill(DS4);
                    DataTable dataTable4 = DS4.Tables[0];

                    //将数据库中数据显示在treeview
                    foreach (DataRow row2 in dataTable2.Rows)
                    {
                        // 获取列的值并显示
                        TreeNode rootNode = new(row2["册"].ToString());
                        // 筛选出满足条件的行，并存储在 selectedRows 数组中
                        DataRow[] selectedRows = dataTable3.Select($"册 = '{row2["册"]}'");
                        foreach (DataRow row3 in selectedRows)
                        {
                            TreeNode childNode1 = new(row3["章"].ToString());
                            rootNode.Nodes.Add(childNode1);

                            DataRow[] selectedRows4 = dataTable4.Select($"章 = '{row3["章"]}' AND 册 = '{row2["册"]}'");
                            foreach (DataRow row4 in selectedRows4)
                            {
                                TreeNode grandChildNode1 = new(row4["节"].ToString());
                                childNode1.Nodes.Add(grandChildNode1);
                            }
                        }
                        treeView2.Nodes.Add(rootNode);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("连接数据库时出错: " + ex.Message);
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // 确保点击的是有效的行
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                //读取系统数据库（数据库A）
                int ID号 = 0;
                string 标准号 = selectedRow.Cells["标准号"].Value?.ToString() ?? "";
                string 专业类别 = selectedRow.Cells["专业类别"].Value?.ToString() ?? "";
                string 分部工程 = selectedRow.Cells["分部工程"].Value?.ToString() ?? "";
                string 分项工程 = selectedRow.Cells["分项工程"].Value?.ToString() ?? "";
                string 清单编码 = selectedRow.Cells["清单编码"].Value?.ToString() ?? "";
                string 清单名称 = "";
                string 项目特征 = "";
                string 单位 = "";
                float 工程量 = 1;
                float 综合单价 = 1;
                float 综合合价 = 1;
                string Level = "0";

                string connectionStringA = "Data Source=systemDB.db;Version=3;";
                using (SQLiteConnection connectionA = new(connectionStringA))
                {
                    string query = $"SELECT * FROM 清单 WHERE 标准号 = '{标准号}' AND 专业类别 = '{专业类别}' AND 分部工程 = '{分部工程}' AND 分项工程 = '{分项工程}' AND 清单编码 = '{清单编码}'";
                    SQLiteCommand command = new(query, connectionA);
                    try
                    {
                        connectionA.Open();
                        SQLiteDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            // 读取记录的各个字段值
                            ID号 = reader.GetInt32(0);
                            标准号 = reader.GetString(1);
                            专业类别 = reader.GetString(2);
                            分部工程 = reader.GetString(3);
                            分项工程 = reader.GetString(4);
                            清单编码 = reader.GetString(5);
                            清单名称 = reader.GetString(6);
                            项目特征 = reader.GetString(7);
                            单位 = reader.GetString(8);
                            工程量 = reader.GetFloat(9);
                            综合单价 = reader.GetFloat(10);
                            综合合价 = reader.GetFloat(11);
                            // 其他字段依此类推
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("查询A数据库出错：" + ex.Message);
                    }
                }

                //将数据写入用户数据库（数据库B）
                string connectionString1 = "Data Source=userDB.db;Version=3;";
                try
                {
                    using SQLiteConnection connection1 = new(connectionString1);
                    connection1.Open();

                    // 定义 SQL 插入语句，明确指定列名
                    string query1 = "INSERT INTO 清单 (ID号,标准号,专业类别,分部工程,分项工程,清单编码,清单名称,项目特征,单位,工程量,综合单价,综合合价,Level) " +
                        "VALUES (@ID号, @标准号, @专业类别, @分部工程, @分项工程, @清单编码, @清单名称, @项目特征, @单位, @工程量, @综合单价, @综合合价, @Level);";
                    using SQLiteCommand command = new(query1, connection1);
                    // 使用参数化查询，避免 SQL 注入
                    command.Parameters.AddWithValue("@ID号", ID号);
                    command.Parameters.AddWithValue("@标准号", 标准号);
                    command.Parameters.AddWithValue("@专业类别", 专业类别);
                    command.Parameters.AddWithValue("@分部工程", 分部工程);
                    command.Parameters.AddWithValue("@分项工程", 分项工程);
                    command.Parameters.AddWithValue("@清单编码", 清单编码);
                    command.Parameters.AddWithValue("@清单名称", 清单名称);
                    command.Parameters.AddWithValue("@项目特征", 项目特征);
                    command.Parameters.AddWithValue("@单位", 单位);
                    command.Parameters.AddWithValue("@工程量", 工程量);
                    command.Parameters.AddWithValue("@综合单价", 综合单价);
                    command.Parameters.AddWithValue("@综合合价", 综合合价);
                    command.Parameters.AddWithValue("@Level", Level);
                    // 执行插入操作
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        //MessageBox.Show("数据插入成功！");
                    }
                    else
                    {
                        MessageBox.Show("数据插入失败！");
                    }
                }
                catch (SQLiteException ex)
                {
                    // 处理 SQLite 异常
                    MessageBox.Show($"SQLite 异常: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // 处理其他异常
                    MessageBox.Show($"发生未知异常: {ex.Message}");
                }
            }

            // 查找子窗口实例
            foreach (Form form in Application.OpenForms)
            {
                if (form is Form1 childForm)
                {
                    childForm.UpdateDisplay("qingdan");
                    break;
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            // 当用户点击节点后，获取选中的节点
            TreeNode selectedNode = e.Node;   //一般不会为null
            // 获取节点层级
            int level = selectedNode.Level;
            // 可以在此处添加更多逻辑，比如根据节点的不同执行不同操作
            if (level == 0)
            {
                // 执行特定操作
                string connectionString = "Data Source=systemDB.db;Version=3;";
                SQLiteConnection connection = new(connectionString);
                try
                {
                    connection.Open();
                    // 在这里可以执行数据库操作，如查询、插入、更新等
                    // 定义 SQL 查询语句
                    string query = $"SELECT * FROM 清单 WHERE 标准号 = '{comboBox1.Text}' AND 专业类别 = '{selectedNode.Text}'";
                    DataAdapter DA = new SQLiteDataAdapter(query, connection);
                    DataSet DS = new();
                    // 填充数据集
                    DA.Fill(DS);
                    // 将数据集绑定到 DataGridView
                    dataGridView1.DataSource = DS.Tables[0];
                }
                catch (Exception ex)
                {
                    MessageBox.Show("连接数据库时出错: " + ex.Message);
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            if (level == 1)
            {
                // 执行特定操作

            }
            if (level == 2)
            {
                // 执行特定操作

            }
        }

        //定额处理
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // 从静态变量中获取值，若清单编码为空，直接返回
            if (string.IsNullOrEmpty(ValueStorage.SharedValue))
            {
                MessageBox.Show("清单编码为空！");
                return;
            }

            if (e.RowIndex >= 0) // 确保点击的是有效的行
            {
                DataGridViewRow selectedRow = dataGridView2.Rows[e.RowIndex];
                //读取系统数据库（数据库A，这里是定额表）
                string 定额编码 = selectedRow.Cells["定额编码"].Value?.ToString() ?? "";
                string 定额名称 = selectedRow.Cells["定额名称"].Value?.ToString() ?? "";
                string 定额单位 = selectedRow.Cells["定额单位"].Value?.ToString() ?? "";
                string Level = "1";

                string connectionStringA = "Data Source=systemDB.db;Version=3;";
                using (SQLiteConnection connectionA = new(connectionStringA))
                {
                    string query = $"SELECT * FROM {comboBox2.Text} WHERE 定额编码 = '{定额编码}'";
                    SQLiteCommand command = new(query, connectionA);
                    try
                    {
                        connectionA.Open();
                        SQLiteDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            // 读取记录的各个字段值
                            定额编码 = reader.GetString(4);
                            定额名称 = reader.GetString(5);
                            定额单位 = reader.GetString(6);
                            // 其他字段依此类推
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("查询A数据库出错：" + ex.Message);
                    }
                }

                //将数据写入用户数据库（数据库B，这里是定额表）
                // 执行并获取自增主键
                long newId = 0;
                string connectionString1 = "Data Source=userDB.db;Version=3;";
                try
                {
                    using SQLiteConnection connection1 = new(connectionString1);
                    connection1.Open();

                    // 定义 SQL 插入语句，明确指定列名
                    string query1 = $"INSERT INTO {comboBox2.Text} (清单编码,定额编码,定额名称,定额单位,Level) " +
                        "VALUES (@清单编码, @定额编码, @定额名称, @定额单位, @Level);";
                    using SQLiteCommand command = new(query1, connection1);
                    // 使用参数化查询，避免 SQL 注入
                    command.Parameters.AddWithValue("@清单编码", ValueStorage.SharedValue);
                    command.Parameters.AddWithValue("@定额编码", 定额编码);
                    command.Parameters.AddWithValue("@定额名称", 定额名称);
                    command.Parameters.AddWithValue("@定额单位", 定额单位);
                    command.Parameters.AddWithValue("@Level", Level);
                    // 执行插入操作
                    int rowsAffected = command.ExecuteNonQuery();
                    // 利用连接对象的属性获取主键
                    newId = connection1.LastInsertRowId;
                    if (rowsAffected > 0)
                    {
                        //MessageBox.Show("数据插入成功！");
                    }
                    else
                    {
                        MessageBox.Show("数据插入失败！");
                    }
                }
                catch (SQLiteException ex)
                {
                    // 处理 SQLite 异常
                    MessageBox.Show($"SQLite 异常: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // 处理其他异常
                    MessageBox.Show($"发生未知异常: {ex.Message}");
                }

                //读取系统数据库后写入用户数据库（消耗量表），有多条记录
                try
                {
                    // 连接到数据库 A
                    using SQLiteConnection connectionA = new(connectionStringA);
                    // 使用参数化查询，避免 SQL 注入
                    string query = "SELECT * FROM 消耗量 WHERE 定额编码 = @定额编码";
                    using SQLiteCommand command = new(query, connectionA);
                    command.Parameters.AddWithValue("@定额编码", 定额编码);
                    connectionA.Open();

                    // 使用 using 语句管理 SQLiteDataReader
                    using SQLiteDataReader reader = command.ExecuteReader();
                    bool hasRecords = false;
                    while (reader.Read())
                    {
                        hasRecords = true;
                        // 读取记录的各个字段值
                        string consumptionCategory = reader.GetString(1);
                        string consumptionCode = reader.GetString(2);
                        string consumptionName = reader.GetString(3);
                        string consumptionUnit = reader.GetString(4);
                        float content = (float)Math.Round(reader.GetFloat(5), 2);
                        float quotaBasePrice = (float)Math.Round(reader.GetFloat(6), 2);

                        // 连接到数据库 B
                        using SQLiteConnection connection1 = new(connectionString1);
                        connection1.Open();

                        // 定义 SQL 插入语句，明确指定列名
                        string query1 = "INSERT INTO 消耗量 (ID号,清单编码,定额编码,消耗量类别,消耗量编码,消耗量名称,消耗量单位,含量,定额基价) " +
                            "VALUES (@ID号, @清单编码, @定额编码, @消耗量类别, @消耗量编码, @消耗量名称, @消耗量单位, @含量, @定额基价);";
                        using SQLiteCommand insertCommand = new(query1, connection1);
                        // 使用参数化查询，避免 SQL 注入
                        insertCommand.Parameters.AddWithValue("@ID号", newId);
                        insertCommand.Parameters.AddWithValue("@清单编码", ValueStorage.SharedValue);
                        insertCommand.Parameters.AddWithValue("@定额编码", 定额编码);
                        insertCommand.Parameters.AddWithValue("@消耗量类别", consumptionCategory);
                        insertCommand.Parameters.AddWithValue("@消耗量单位", consumptionUnit);
                        insertCommand.Parameters.AddWithValue("@消耗量编码", consumptionCode);
                        insertCommand.Parameters.AddWithValue("@消耗量名称", consumptionName);
                        insertCommand.Parameters.AddWithValue("@含量", Math.Round(content, 2));
                        insertCommand.Parameters.AddWithValue("@定额基价", Math.Round(quotaBasePrice, 2));

                        // 执行插入操作
                        int rowsAffected = insertCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            //MessageBox.Show("数据插入成功！");
                        }
                        else
                        {
                            MessageBox.Show("数据插入失败！");
                        }
                    }

                    if (!hasRecords)
                    {
                        MessageBox.Show("系统数据库中，无消耗量记录！");
                        return;
                    }
                }
                catch (SQLiteException ex)
                {
                    // 处理 SQLite 异常
                    MessageBox.Show($"SQLite 异常: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // 处理其他异常
                    MessageBox.Show($"发生未知异常: {ex.Message}");
                }
            }

            // 查找子窗口实例
            foreach (Form form in Application.OpenForms)
            {
                if (form is Form1 childForm)
                {
                    childForm.UpdateDisplay("qingdan");
                    break;
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}