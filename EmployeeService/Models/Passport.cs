using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeService.Models
{
    public class Passport
    {
        public string Type { get; set; }
        public string Number { get; set; }

        public override string ToString()
        {
            return string.Join(',', Type, Number);
        }

        public static Passport FromString(string value)
        {
            string[] fields = value.Split(',');

            Passport result = new Passport() { Type = fields[0], Number = fields[1] };

            return result;
        }
    }
}
