using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSharePassword.Data
{
    public class Image
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Password { get; set; }
        public int Views { get; set; }
        public int UserId { get; set; }
    }
}
