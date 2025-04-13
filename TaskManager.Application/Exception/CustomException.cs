using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.Exception
{
    public class CustomException : System.Exception
    {
        public CustomException(string message) : base(message) { }
    }
}
