namespace EmployeeService.Models
{
    public class Department
    {
        public string Name { get; set; }
        public string Phone { get; set; }

        public override string ToString()
        {
            return string.Join(',', Name, Phone);
        }

        public static Department FromString(string value)
        {
            string[] fields = value.Split(',');

            Department result = new Department() { Name = fields[0], Phone = fields[1] };

            return result;
        }
    }
}
