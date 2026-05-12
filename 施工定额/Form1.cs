using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using static 施工定额.Program;

namespace 施工定额
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // 全局启用换行
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // 自动调整行高
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            UpdateDisplay("qingdan");
            UpdateDisplay("dinge");
            UpdateDisplay("xiaohaoliang");
        }

        //更新dataGridView的显示
        public void UpdateDisplay(string s)
        {
            if (s == "qingdan")
            {
                // 从更新清单显示
                List<Qingdan> qingdans = GetQingdansFromDatabase();
                dataGridView1.DataSource = qingdans;
            }
            if (s == "dinge")
            {
                //更新定额显示
                string connectionString = "Data Source=userDB.db;Version=3;";
                SQLiteConnection connection = new(connectionString);
                SQLiteConnection connection1 = new(connectionString);
                try
                {
                    connection1.Open();
                    // 在这里可以执行数据库操作，如查询、插入、更新等
                    // 定义 SQL 查询语句
                    string query = $"SELECT * FROM 定额_市政工程 WHERE 清单编码 = '{ValueStorage.SharedValue}'";
                    DataAdapter DA = new SQLiteDataAdapter(query, connection1);
                    DataSet DS = new();
                    // 填充数据集
                    DA.Fill(DS);
                    // 将数据集绑定到 DataGridView
                    DataGridView_dinge.DataSource = DS.Tables[0];
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
            if (s == "xiaohaoliang")
            {
                //更新消耗量显示
                string connectionString = "Data Source=userDB.db;Version=3;";
                SQLiteConnection connection1 = new(connectionString);
                try
                {
                    connection1.Open();
                    // 在这里可以执行数据库操作，如查询、插入、更新等
                    // 定义 SQL 查询语句
                    string query = $"SELECT * FROM 消耗量 WHERE 清单编码 = '{ValueStorage.SharedValue}' AND 定额编码 = '{ValueStorage.SharedValue2}' AND ID号 = '{ValueStorage.SharedValue3}'";
                    DataAdapter DA = new SQLiteDataAdapter(query, connection1);
                    DataSet DS = new();
                    // 填充数据集
                    DA.Fill(DS);
                    // 将数据集绑定到 DataGridView
                    dataGridView2.DataSource = DS.Tables[0];
                }
                catch (Exception ex)
                {
                    MessageBox.Show("连接数据库时出错: " + ex.Message);
                }
                finally
                {
                    if (connection1.State == System.Data.ConnectionState.Open)
                    {
                        connection1.Close();
                    }
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)//点击到了标题栏
            {
                return;
            }
            string 清单编码 = "";
            清单编码 = dataGridView1.Rows[e.RowIndex].Cells["清单编码"].Value.ToString() ?? "";
            // 将值存储到静态变量中
            ValueStorage.SharedValue = 清单编码;
            UpdateDisplay("dinge");
            //清空消耗量显示
            // 第一步：解除数据源绑定（核心）
            dataGridView2.DataSource = null;
            // 第二步：清除残留行，双重保险
            dataGridView2.Rows.Clear();
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)//点击到了标题栏
            {
                return;
            }
            // 查找子窗口实例
            Form2 childForm = null;
            foreach (Form form in Application.OpenForms)
            {
                if (form is Form2)
                {
                    childForm = (Form2)form;
                    break;
                }
            }

            if (childForm == null)
            {
                // 若不存在 Form2 实例，则创建一个新的实例
                childForm = new Form2();
                childForm.Show();
            }
            else
            {
                // 若已存在 Form2 实例，则将其激活
                childForm.Activate();
            }

            // 将 Form2 窗口置顶
            childForm.TopMost = true;

        }

        // 定义一个标志变量，用于控制是否处理 CellValueChanged 事件

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            QingdanDingeXiaohaoliang qingdanDingeXiaohaoliang = QingdansDingeXiaohaoliang(dataGridView1.Rows[e.RowIndex].Cells["清单编码"].Value.ToString(), sender, e);

            UpdateDatabase(qingdanDingeXiaohaoliang);
            UpdateDisplay("qingdan");
            UpdateDisplay("dinge");
            UpdateDisplay("xiaohaoliang");
        }

        private void UpdateDatabase(QingdanDingeXiaohaoliang QDX)
        {
            string connectionString = "Data Source=userDB.db;Version=3;";
            using SQLiteConnection connection = new(connectionString);
            //更新清单表
            try
            {
                connection.Open();
                // 构建更新 SQL 语句
                string updateQuery = "UPDATE 清单 SET 清单名称 = @清单名称, 项目特征 = @项目特征, 单位 = @单位, 工程量 = @工程量, 综合单价 = @综合单价, 综合合价 = @综合合价 WHERE 清单编码 = @清单编码";
                using SQLiteCommand command = new(updateQuery, connection);
                command.Parameters.AddWithValue("@清单名称", QDX.qingdan.清单名称);
                command.Parameters.AddWithValue("@项目特征", QDX.qingdan.项目特征);
                command.Parameters.AddWithValue("@单位", QDX.qingdan.单位);
                command.Parameters.AddWithValue("@工程量", QDX.qingdan.工程量.ToString("0.00"));//保留两位小数
                command.Parameters.AddWithValue("@综合单价", QDX.qingdan.综合单价.ToString("0.00"));
                command.Parameters.AddWithValue("@综合合价", QDX.qingdan.综合合价.ToString("0.00"));
                command.Parameters.AddWithValue("@清单编码", QDX.qingdan.清单编码);
                // 执行更新操作
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新数据库时出现错误：{ex.Message}");
            }
            //更新定额表
            try
            {
                // 构建更新 SQL 语句
                foreach (Dinge dinge in QDX.dinge)
                {
                    string updateQuery = "UPDATE 定额_市政工程 SET 定额名称 = @定额名称, 定额单位 = @定额单位, 定额工程量 = @定额工程量, 定额单价 = @定额单价, 定额合价 = @定额合价 WHERE 定额编码 = @定额编码 AND 清单编码 = @清单编码 AND ID号 = @ID号";
                    using SQLiteCommand command = new(updateQuery, connection);
                    command.Parameters.AddWithValue("@定额名称", dinge.定额名称);
                    command.Parameters.AddWithValue("@定额单位", dinge.定额单位);
                    command.Parameters.AddWithValue("@定额工程量", dinge.定额工程量.ToString("F2"));//保留两位小数
                    command.Parameters.AddWithValue("@定额单价", dinge.定额单价.ToString("F2"));
                    command.Parameters.AddWithValue("@定额合价", dinge.定额合价.ToString("F2"));
                    command.Parameters.AddWithValue("@定额编码", dinge.定额编码);
                    command.Parameters.AddWithValue("@清单编码", QDX.qingdan.清单编码);
                    command.Parameters.AddWithValue("@ID号", dinge.ID号);
                    // 执行更新操作
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新数据库时出现错误：{ex.Message}");
            }
            //更新消耗量表
            try
            {
                // 构建更新 SQL 语句
                foreach (Xiaohaoliang xhl in QDX.xiaohaoliang)
                {
                    string updateQuery = "UPDATE 消耗量 SET 含量 = @含量, 数量 = @数量, 定额基价 = @定额基价, 市场价 = @市场价, 市场价合计 = @市场价合计 WHERE 定额编码 = @定额编码 AND 清单编码 = @清单编码 AND ID号 = @ID号 AND 消耗量编码 = @消耗量编码 ";
                    using SQLiteCommand command = new(updateQuery, connection);
                    command.Parameters.AddWithValue("@含量", xhl.含量.ToString("F4"));//保留四位小数
                    command.Parameters.AddWithValue("@数量", xhl.数量.ToString("F4"));
                    command.Parameters.AddWithValue("@定额基价", xhl.定额基价.ToString("F2"));
                    command.Parameters.AddWithValue("@市场价", xhl.市场价.ToString("F2"));
                    command.Parameters.AddWithValue("@市场价合计", xhl.市场价合计.ToString("F2"));
                    command.Parameters.AddWithValue("@定额编码", xhl.定额编码);
                    command.Parameters.AddWithValue("@清单编码", QDX.qingdan.清单编码);
                    command.Parameters.AddWithValue("@ID号", xhl.ID号);
                    command.Parameters.AddWithValue("@消耗量编码", xhl.消耗量编码);
                    // 执行更新操作
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新数据库时出现错误：{ex.Message}");
            }


        }

        /// <summary>
        /// 从SQLite读取数据并转换为User对象列表
        /// </summary>
        /// <returns>User对象列表</returns>
        private List<Qingdan> GetQingdansFromDatabase()
        {
            List<Qingdan> qingdanList = new List<Qingdan>();
            string connectionString = "Data Source=userDB.db;Version=3;";
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string querySql = "SELECT 清单编码,清单名称,项目特征,单位,工程量,综合单价,综合合价,Level FROM 清单";
                    using (SQLiteCommand cmd = new SQLiteCommand(querySql, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            // 遍历查询结果，封装成User对象
                            while (reader.Read())
                            {
                                Qingdan qingdan = new Qingdan
                                {
                                    清单编码 = reader["清单编码"].ToString(),
                                    清单名称 = reader["清单名称"].ToString(),
                                    项目特征 = reader["项目特征"].ToString(),
                                    单位 = reader["单位"].ToString(),
                                    工程量 = Convert.ToDecimal(reader["工程量"]),
                                    综合单价 = Convert.ToDecimal(reader["综合单价"]),
                                    综合合价 = Convert.ToDecimal(reader["综合合价"]),
                                    Level = Convert.ToInt32(reader["Level"])
                                };
                                qingdanList.Add(qingdan);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"读取数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return qingdanList;
        }

        //清单定额消耗量
        private QingdanDingeXiaohaoliang QingdansDingeXiaohaoliang(string QingdanBianma, object sender, DataGridViewCellEventArgs e)
        {
            QingdanDingeXiaohaoliang result = new QingdanDingeXiaohaoliang();
            //对清单对象进行赋值
            List<Qingdan> qingdanList = new List<Qingdan>();
            string connectionString = "Data Source=userDB.db;Version=3;";
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string querySql = "SELECT 清单编码,清单名称,项目特征,单位,工程量,综合单价,综合合价,Level FROM 清单";
                    using (SQLiteCommand cmd = new SQLiteCommand(querySql, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            // 遍历查询结果，封装成User对象
                            while (reader.Read())
                            {
                                Qingdan qingdan1 = new Qingdan
                                {
                                    清单编码 = reader["清单编码"].ToString(),
                                    清单名称 = reader["清单名称"].ToString(),
                                    项目特征 = reader["项目特征"].ToString(),
                                    单位 = reader["单位"].ToString(),
                                    工程量 = Convert.ToDecimal(reader["工程量"]),
                                    综合单价 = Convert.ToDecimal(reader["综合单价"]),
                                    综合合价 = Convert.ToDecimal(reader["综合合价"]),
                                    Level = Convert.ToInt32(reader["Level"])
                                };
                                qingdanList.Add(qingdan1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"读取数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            result.qingdan = qingdanList.Find(q => q.清单编码 == QingdanBianma);

            // 核心：将sender强转为DataGridView，得到【被点击的那个DataGridView】
            DataGridView currentDgv = sender as DataGridView;
            // 方式1：通过 控件Name 属性判断（最常用，推荐）
            if (currentDgv.Name == "dataGridView1")
            {
                result.qingdan.工程量 = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["工程量"].Value);
            }

            //对定额对象进行赋值
            foreach (DataGridViewRow row in DataGridView_dinge.Rows)
            {
                // 跳过“新行”（DataGridView默认最后一行是空白新行）
                if (row.IsNewRow) continue;
                // 创建实体类对象
                Dinge item = new Dinge();
                // 读取单元格值并赋值（注意处理空值）
                //ID号
                item.ID号 = row.Cells["ID号"].Value?.ToString() ?? string.Empty;
                //定额编码
                item.定额编码 = row.Cells["定额编码"].Value?.ToString() ?? string.Empty;
                //定额名称
                item.定额名称 = row.Cells["定额名称"].Value?.ToString() ?? string.Empty;
                //定额单位
                item.定额单位 = row.Cells["定额单位"].Value?.ToString() ?? string.Empty;
                //定额工程量
                if (row.Cells["定额工程量"].Value != DBNull.Value)
                {
                    item.定额工程量 = Convert.ToDecimal(row.Cells["定额工程量"].Value);
                }
                //定额单价
                if (row.Cells["定额单价"].Value != DBNull.Value)
                {
                    item.定额单价 = item.定额单价;
                }
                //定额合价
                if (row.Cells["定额合价"].Value != DBNull.Value)
                {
                    item.定额合价 = Convert.ToDecimal(row.Cells["定额合价"].Value);
                }
                //将对象添加到List
                result.dinge.Add(item);
            }
            /*
            dinge.定额工程量 = qingdan.工程量;
            if (dinge.定额单位.Contains("10"))
            {
                dinge.定额工程量 = Convert.ToSingle(qingdan.工程量 * 0.1);
            }
            */

            //对消耗量对象进行赋值
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                // 跳过“新行”（DataGridView默认最后一行是空白新行）
                if (row.IsNewRow) continue;
                // 创建实体类对象
                Xiaohaoliang item = new Xiaohaoliang();
                // 读取单元格值并赋值（注意处理空值）
                //清单编码
                item.清单编码 = row.Cells["清单编码"].Value?.ToString() ?? string.Empty;
                //定额编码
                item.定额编码 = row.Cells["定额编码"].Value?.ToString() ?? string.Empty;
                //ID号
                item.ID号 = row.Cells["ID号"].Value?.ToString() ?? string.Empty;
                //消耗量类别
                item.消耗量类别 = row.Cells["消耗量类别"].Value?.ToString() ?? string.Empty;
                //消耗量编码
                item.消耗量编码 = row.Cells["消耗量编码"].Value?.ToString() ?? string.Empty;
                //消耗量名称
                item.消耗量名称 = row.Cells["消耗量名称"].Value?.ToString() ?? string.Empty;
                //规格型号
                //item.规格型号 = row.Cells["规格型号"].Value?.ToString() ?? string.Empty;
                //消耗量单位
                item.消耗量单位 = row.Cells["消耗量单位"].Value?.ToString() ?? string.Empty;

                //含量
                if (row.Cells["含量"].Value != DBNull.Value)
                {
                    item.含量 = Convert.ToDecimal(row.Cells["含量"].Value);
                }
                //数量
                item.数量 = item.含量 * Convert.ToDecimal(result.dinge.FirstOrDefault(p => p.ID号 == item.ID号)?.定额工程量);
                //定额基价
                if (row.Cells["定额基价"].Value != DBNull.Value)
                {
                    item.定额基价 = Convert.ToDecimal(row.Cells["定额基价"].Value);
                }
                //市场价
                if (row.Cells["市场价"].Value != DBNull.Value)
                {
                    item.市场价 = Convert.ToDecimal(row.Cells["市场价"].Value);
                }
                //市场价合计
                item.市场价合计 = item.市场价 * item.数量;
                //将对象添加到List
                result.xiaohaoliang.Add(item);

                //计算定额综合合价
                result.dinge.FirstOrDefault(p => p.ID号 == item.ID号).定额合价 += item.市场价合计;
            }
            //计算清单综合合价、定额综合单价
            result.qingdan.综合合价 = 0.0M;
            foreach (Dinge dinge in result.dinge)
            {
                dinge.定额单价 = dinge.定额合价 / dinge.定额工程量;
                result.qingdan.综合合价 += dinge.定额合价;
            }
            result.qingdan.综合单价 = result.qingdan.综合合价 / result.qingdan.工程量;

            return result;
        }

        private QingdanDingeXiaohaoliang GetCurrentModel()
        {
            QingdanDingeXiaohaoliang model = new QingdanDingeXiaohaoliang();

            // 1. 从 DataGridView 读取清单数据（假设取当前选中的那一行）
            if (dataGridView1.CurrentRow != null)
            {
                var row = dataGridView1.CurrentRow;
                model.qingdan.清单编码 = row.Cells["清单编码"].Value?.ToString();
                model.qingdan.清单名称 = row.Cells["清单名称"].Value?.ToString();
                model.qingdan.项目特征 = row.Cells["项目特征"].Value?.ToString();
                model.qingdan.单位 = row.Cells["单位"].Value?.ToString();
                model.qingdan.工程量 = Convert.ToDecimal(row.Cells["工程量"].Value ?? 0);
                model.qingdan.综合单价 = Convert.ToDecimal(row.Cells["综合单价"].Value ?? 0);
                model.qingdan.综合合价 = Convert.ToDecimal(row.Cells["综合合价"].Value ?? 0);
            }
            // 2. 从 DataGridView 读取定额列表
            foreach (DataGridViewRow row in DataGridView_dinge.Rows)
            {
                if (row.IsNewRow) continue;
                model.dinge.Add(new Dinge
                {
                    ID号 = row.Cells["ID号"].Value?.ToString(),
                    定额编码 = row.Cells["定额编码"].Value?.ToString(),
                    定额工程量 = Convert.ToDecimal(row.Cells["定额工程量"].Value ?? 0),
                    定额单价 = Convert.ToDecimal(row.Cells["定额单价"].Value ?? 0),
                    定额合价 = Convert.ToDecimal(row.Cells["定额合价"].Value ?? 0)
                });
            }
            // 3. 从 DataGridView 读取消耗量列表
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.IsNewRow) continue;
                model.xiaohaoliang.Add(new Xiaohaoliang
                {
                    ID号 = row.Cells["ID号"].Value?.ToString(),
                    含量 = Convert.ToDecimal(row.Cells["含量"].Value ?? 0),
                    数量 = Convert.ToDecimal(row.Cells["数量"].Value ?? 0),
                    定额基价 = Convert.ToDecimal(row.Cells["定额基价"].Value ?? 0),
                    市场价 = Convert.ToDecimal(row.Cells["市场价"].Value ?? 0)                    
                });
            }
            return model;
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show("Content被点击");
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 获取当前选中的选项卡的索引
            int selectedIndex = tabControl1.SelectedIndex;
            // 获取当前选中的选项卡的名称
            string selectedTabName = tabControl1.TabPages[selectedIndex].Name;
            dataGridView4.Rows.Clear();
            if (selectedIndex == 3)//人材机汇总界面
            {
                string connectionString = "Data Source=userDB.db;Version=3;";
                SQLiteConnection connection = new(connectionString);
                try
                {
                    connection.Open();
                    // 在这里可以执行数据库操作，如查询、插入、更新等
                    // 定义 SQL 查询语句
                    string query = $"SELECT * FROM 消耗量 GROUP BY 消耗量类别";
                    DataAdapter DA = new SQLiteDataAdapter(query, connection);
                    DataSet DS = new();
                    // 填充数据集
                    DA.Fill(DS);
                    // 将数据集绑定到 DataGridView
                    dataGridView3.DataSource = DS.Tables[0];
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
            if (selectedIndex == 4)//造价汇总界面
            {
                dataGridView4.Rows.Add("分部分项费");
                dataGridView4.Rows.Add("措施项目费");
                dataGridView4.Rows.Add("其他项目费");
                dataGridView4.Rows.Add("规费");
                dataGridView4.Rows.Add("增值税");
                dataGridView4.Rows.Add("工程造价");
                float 不含税工程造价 = 0.0f;
                try
                {
                    string connectionString = "Data Source=userDB.db;Version=3;";
                    // 使用 using 语句管理 SQLiteConnection
                    using SQLiteConnection connection = new(connectionString);
                    connection.Open();
                    // 定义 SQL 查询语句，使用 ROUND 函数保留两位小数
                    string query = $"SELECT ROUND(SUM(综合合价), 2) FROM 清单";
                    using SQLiteCommand command = new(query, connection);
                    // 执行查询并获取结果
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        // 确保 DataGridView 有足够的行和列
                        if (dataGridView4.Rows.Count == 0)
                        {
                            dataGridView4.Rows.Add();
                        }
                        if (dataGridView4.Columns.Count < 2)
                        {
                            dataGridView4.Columns.Add("Column1", "Column 1");
                            dataGridView4.Columns.Add("Column2", "Column 2");
                        }
                        不含税工程造价 += Convert.ToSingle(result);
                        // 将结果赋值给第一行第二列
                        dataGridView4.Rows[0].Cells[1].Value = result;
                    }
                    else
                    {
                        MessageBox.Show("查询结果为空，没有数据可显示。");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("连接数据库时出错: " + ex.Message);
                }

                float 增值税 = 不含税工程造价 * 0.09f;
                float 工程造价 = 不含税工程造价 + 增值税;
                dataGridView4.Rows[4].Cells[1].Value = 增值税.ToString("F2");
                dataGridView4.Rows[5].Cells[1].Value = 工程造价.ToString("F2");
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // 当用户点击节点后，获取选中的节点
            TreeNode selectedNode = e.Node;   //一般不会为null

            string connectionString = "Data Source=userDB.db;Version=3;";
            SQLiteConnection connection = new(connectionString);
            try
            {
                connection.Open();
                // 在这里可以执行数据库操作，如查询、插入、更新等
                // 定义 SQL 查询语句
                string query = $"SELECT * FROM 消耗量 WHERE 消耗量类别 = '{selectedNode.Text}' GROUP BY 消耗量名称";
                DataAdapter DA = new SQLiteDataAdapter(query, connection);
                DataSet DS = new();
                // 填充数据集
                DA.Fill(DS);
                // 将数据集绑定到 DataGridView
                dataGridView3.DataSource = DS.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接数据库时出错: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 1. 获取当前所有数据（模型化）
            var model = GetCurrentModel();

            // 2. 调用模型自带的计算逻辑（对象自己算自己）
            model.Calculate();

            // 3. 保存回数据库（你可以直接把整个 model 传给 UpdateDatabase）
            UpdateDatabase(model);

            UpdateDisplay("qingdan");
            UpdateDisplay("dinge");
            UpdateDisplay("xiaohaoliang");
        }

        private void DataGridView_dinge_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            QingdanDingeXiaohaoliang qingdanDingeXiaohaoliang = QingdansDingeXiaohaoliang(ValueStorage.SharedValue, sender, e);
            UpdateDatabase(qingdanDingeXiaohaoliang);
            UpdateDisplay("qingdan");
            UpdateDisplay("dinge");
            UpdateDisplay("xiaohaoliang");
        }

        private void DataGridView_dinge_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)//点击到了标题栏
            {
                return;
            }
            string 定额编码 = DataGridView_dinge.Rows[e.RowIndex].Cells["定额编码"].Value.ToString() ?? "";
            string ID号 = DataGridView_dinge.Rows[e.RowIndex].Cells["ID号"].Value.ToString() ?? "";
            // 将值存储到静态变量中
            ValueStorage.SharedValue2 = 定额编码;
            ValueStorage.SharedValue3 = ID号;
            UpdateDisplay("xiaohaoliang");
        }
    }
}