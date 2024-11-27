using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewcoreTestTool.Mysql
{
    public class HardwareInfoDto
    {
        // 产测主键
        public int ChanceId { get; set; }

        // SN编号
        public string SNNumber { get; set; }

        // 条目创建时间
        public DateTime? CreateTime { get; set; }

        // 条目最后更新时间
        public DateTime? UpdateTime { get; set; }

        // 删除标记
        public bool IsDeleted { get; set; }

        // 创建人id
        public long StaffId { get; set; }

        // JSON数据包
        public string JSONData { get; set; }
    }
}
