using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public class GenerateCuponService
    {
        public string GenerateCode() 
        {
            Random random = new Random();
            string code = "";

            for (int i = 0; i < 9; i++) 
            {
                if (i != 0 && i % 3 == 0) code += "-";
                code += random.Next(0, 9);
            }
            
            return code;
        }
    }
}
