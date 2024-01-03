using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkolSystem.Models;

public class Klass
{
    // kolumner av table klass
    public int KlassID { get; } // primery key 
    public string KlassNamn { get; set; }

    public List<Student> Studenter { get; set; } = new List<Student>(); // pekas av
}
