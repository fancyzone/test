using Dapper;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using static 施工定额.Program;

namespace 施工定额
{
    public partial class Form1 : Form
    {
        private string connectionString = "Data Source=userDB.db;Version=3;";
        private List<Qingdan> myMemoryQingdanList = new List<Qingdan>();

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            myMemoryQingdanList = LoadTreeDataToMemory();

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
                //更新清单显示
                dataGridView1.DataSource = DbHelper.QueryList<Qingdan>("SELECT * FROM 清单");
            }
            if (s == "dinge")
            {
                //更新定额显示
                DataGridView_dinge.DataSource = DbHelper.QueryList<Dinge>($"SELECT * FROM 定额_市政工程 WHERE 清单编码 = '{ValueStorage.SharedValue}'");
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

            // 当点击清单时，直接去内存列表里过滤
            var currentQd = myMemoryQingdanList.FirstOrDefault(q => q.清单编码 == ValueStorage.SharedValue);
            DataGridView_dinge.DataSource = currentQd.定额列表; // 秒开，完全不卡
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

        private void UpdateDatabase(QingdanDingeXiaohaoliang QDX)
        {
            DbHelper.Execute("UPDATE 清单 SET 清单名称 = @清单名称, 项目特征 = @项目特征, 单位 = @单位, 工程量 = @工程量, 综合单价 = @综合单价, 综合合价 = @综合合价 WHERE 清单编码 = @清单编码", QDX.qingdan);
            DbHelper.Execute($"UPDATE 定额_市政工程 SET 定额名称 = @定额名称, 定额单位 = @定额单位, 定额工程量 = @定额工程量, 定额单价 = @定额单价, 定额合价 = @定额合价 WHERE 定额编码 = @定额编码 AND 清单编码 = {ValueStorage.SharedValue} AND ID号 = @ID号", QDX.dinge);
            DbHelper.Execute($"UPDATE 消耗量 SET 含量 = @含量, 数量 = @数量, 定额基价 = @定额基价, 市场价 = @市场价, 市场价合计 = @市场价合计 WHERE 定额编码 = @定额编码 AND 清单编码 = {ValueStorage.SharedValue} AND ID号 = @ID号 AND 消耗量编码 = @消耗量编码", QDX.xiaohaoliang);
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
                    定额名称 = row.Cells["定额名称"].Value?.ToString(),
                    定额单位 = row.Cells["定额单位"].Value?.ToString(),
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
                    清单编码 = row.Cells["清单编码"].Value?.ToString(),
                    定额编码 = row.Cells["定额编码"].Value?.ToString(),
                    消耗量类别 = row.Cells["消耗量类别"].Value?.ToString(),
                    消耗量编码 = row.Cells["消耗量编码"].Value?.ToString(),
                    消耗量名称 = row.Cells["消耗量名称"].Value?.ToString(),
                    //规格型号 = row.Cells["规格型号"].Value?.ToString(),
                    消耗量单位 = row.Cells["消耗量单位"].Value?.ToString(),
                    含量 = Convert.ToDecimal(row.Cells["含量"].Value ?? 0),
                    数量 = Convert.ToDecimal(row.Cells["数量"].Value ?? 0),
                    定额基价 = Convert.ToDecimal(row.Cells["定额基价"].Value ?? 0),
                    市场价 = Convert.ToDecimal(row.Cells["市场价"].Value ?? 0),
                    市场价合计 = Convert.ToDecimal(row.Cells["市场价合计"].Value ?? 0)
                });
            }
            return model;
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
                decimal 不含税工程造价 = 0.0M;
                try
                {

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
                        不含税工程造价 += Convert.ToDecimal(result);
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

                decimal 增值税 = 不含税工程造价 * 0.09M;
                decimal 工程造价 = 不含税工程造价 + 增值税;
                dataGridView4.Rows[4].Cells[1].Value = 增值税.ToString("F2");
                dataGridView4.Rows[5].Cells[1].Value = 工程造价.ToString("F2");
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // 当用户点击节点后，获取选中的节点
            TreeNode selectedNode = e.Node;   //一般不会为null

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
            model.Calculate(ValueStorage.SharedValue3);

            // 3. 保存回数据库（你可以直接把整个 model 传给 UpdateDatabase）
            UpdateDatabase(model);

            UpdateDisplay("qingdan");
            UpdateDisplay("dinge");
            UpdateDisplay("xiaohaoliang");
        }

        private void DataGridView_dinge_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 1. 获取当前所有数据（模型化）
            var model = GetCurrentModel();

            // 2. 调用模型自带的计算逻辑（对象自己算自己）
            model.Calculate(ValueStorage.SharedValue3);

            // 3. 保存回数据库（你可以直接把整个 model 传给 UpdateDatabase）
            UpdateDatabase(model);
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
        public static List<Qingdan> LoadTreeDataToMemory()
        {
            string sql = @"
            SELECT * FROM 清单;
            SELECT * FROM 定额_市政工程;
            SELECT * FROM 消耗量;";

            using (var conn = new SQLiteConnection("Data Source=userDB.db;Version=3;"))
            {
                using (var multi = conn.QueryMultiple(sql))
                {
                    var qingdanList = multi.Read<Qingdan>().ToList();
                    var dingeList = multi.Read<Dinge>().ToList();
                    var xiaohaoliangList = multi.Read<Xiaohaoliang>().ToList();

                    // 1. 消耗量按【ID号】分组（Lookup类似于超快的只读字典）
                    var xhlLookup = xiaohaoliangList.ToLookup(x => x.ID号);

                    // 2. 先把【消耗量】全部塞进对应的【定额】里（这一步至关重要！）
                    foreach (var dg in dingeList)
                    {
                        // 根据 ID号 匹配，把属于该定额的消耗量全部倒进去
                        dg.消耗量列表 = xhlLookup[dg.ID号].ToList();
                        // 注意：如果你的 Dinge 类里定义的属性叫“消耗量列表”，这里记得改成 dg.消耗量列表
                    }

                    // 3. 再按【清单编码】将已经装好消耗量的【定额】进行分组
                    // 借用消耗量的关系（因为你的消耗量表里同时有“清单编码”和“定额编码”）
                    var dingeLookup = dingeList.ToLookup(d => {
                        var sampleXhl = xiaohaoliangList.FirstOrDefault(x => x.ID号 == d.ID号);
                        return sampleXhl?.清单编码 ?? "";
                    });

                    // 4. 最后把定额列表塞进清单
                    foreach (var qd in qingdanList)
                    {
                        qd.定额列表 = dingeLookup[qd.清单编码].ToList();
                    }

                    return qingdanList;
                }
            }
        }
    }
}