using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Entity
{
    public class CAQ_Reply : DbContext
    {
        [Key]
        public string ID//键值
        {
            get; set;
        }

        [Required]
        public string CardName//名称
        {
            get; set;
        }

        [Required]
        public string Count//数量
        {
            get; set;
        }

        public string Descript//描述
        {
            get; set;
        }
    }
}
