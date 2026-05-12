namespace 施工定额
{
    public class QingdanDingeXiaohaoliang
    {
        public Qingdan qingdan { get; set; } = new Qingdan();
        public List<Dinge> dinge { get; set; } = new List<Dinge>();
        public List<Xiaohaoliang> xiaohaoliang { get; set; } = new List<Xiaohaoliang>();
        // 【核心优化】：定义一个统一的计算方法
        public void Calculate()
        {
            // 1. 计算所有消耗量的合价
            foreach (var x in xiaohaoliang)
            {
                // 找到该消耗量对应的定额，获取定额工程量
                var parentDinge = dinge.FirstOrDefault(d => d.ID号 == x.ID号);
                decimal dAmount = parentDinge?.定额工程量 ?? 0;

                x.数量 = x.含量 * dAmount;
                x.市场价合计 = x.数量 * x.市场价;
            }

            // 2. 计算定额的合价与单价
            foreach (var d in dinge)
            {
                d.定额合价 = xiaohaoliang.Where(x => x.ID号 == d.ID号).Sum(x => x.市场价合计);
                if (d.定额工程量 != 0)
                    d.定额单价 = d.定额合价 / d.定额工程量;
            }

            // 3. 计算清单的合价与单价
            qingdan.综合合价 = dinge.Sum(d => d.定额合价);
            if (qingdan.工程量 != 0)
                qingdan.综合单价 = qingdan.综合合价 / qingdan.工程量;
        }
    }
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
    public void Calculate(decimal dingeWorkAmount)
    {
        数量 = 含量 * dingeWorkAmount;
        市场价合计 = 市场价 * 数量;
    }
    public override string ToString()
    {
        return $"{ID号} - {消耗量类别} - {消耗量编码} - {消耗量名称} - {规格型号} - {消耗量单位} - {含量} - {数量} - {定额基价} - {市场价} - {市场价合计}";
    }
}