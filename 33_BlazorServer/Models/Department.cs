using System.Collections.Generic;
using System.Diagnostics;

namespace Advanced.Models
{
    public class Department
    {
        public long DepartmentId { get; set; }
        public string Name { get; set; }

        public IEnumerable<Person> People { get; set; }
    }
}