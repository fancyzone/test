namespace 施工定额
{
    public class QingdanDingeXiaohaoliang
    {
        public Qingdan qingdan { get; set; } = new Qingdan();
        public List<Dinge> dinge { get; set; } = new List<Dinge>();
        public List<Xiaohaoliang> xiaohaoliang { get; set; } = new List<Xiaohaoliang>();
    }
    public class Qingdan
    {
        public string 清单编码 { get; set; }
        public string 清单名称 { get; set; }
        public string 项目特征 { get; set; }
        public string 单位 { get; set; }
        public decimal 工程量 { get; set; }
        public decimal 综合单价 { get; set; }
        public decimal 综合合价 { get; set; }
        public int Level { get; set; }
        public override string ToString()
        {
            return $"{清单编码} - {清单名称} - {项目特征} - {单位} - {工程量} - {综合单价} - {综合合价} - {Level}";
        }
    }
    public class Dinge
    {
        public string ID号 { get; set; }
        public string 定额编码 { get; set; }
        public string 定额名称 { get; set; }
        public string 定额单位 { get; set; }
        public decimal 定额工程量 { get; set; }
        public decimal 定额单价 { get; set; }
        public decimal 定额合价 { get; set; }
        public override string ToString()
        {
            return $"{ID号} - {定额编码} - {定额名称} - {定额工程量} - {定额单价} - {定额合价}";
        }
    }
    public class Xiaohaoliang
    {
        public string ID号 { get; set; }
        public string 清单编码 { get; set; }
        public string 定额编码 { get; set; }
        public string 消耗量类别 { get; set; }
        public string 消耗量编码 { get; set; }
        public string 消耗量名称 { get; set; }
        public string 规格型号 { get; set; }
        public string 消耗量单位 { get; set; }
        public decimal 含量 { get; set; }
        public decimal 数量 { get; set; }
        public decimal 定额基价 { get; set; }
        public decimal 市场价 { get; set; }
        public decimal 市场价合计 { get; set; }
        public override string ToString()
        {
            return $"{ID号} - {消耗量类别} - {消耗量编码} - {消耗量名称} - {规格型号} - {消耗量单位} - {含量} - {数量} - {定额基价} - {市场价} - {市场价合计}";
        }
    }
}
