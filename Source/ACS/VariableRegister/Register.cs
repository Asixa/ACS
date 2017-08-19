using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACS.Variable_Register
{
   
    internal class Register
    {

        public static void Init()
        {
            instance=new Register();
        }

        public static Register instance;
        List<Vars> list=new List<Vars>();
        public Register()
        {
            instance = this;
        }

        public static void Add(string name,object value)
        {
            instance.list.Add(new Vars(name,value));
        }

        public static void Set(string name, object value)
        {
            if (Contain(name))
            {
                foreach (var t in instance.list)
                {
                    if(t.name == name)
                    {
                        t.value = value;
                    }
                }
            }
            else
            {
                Add(name,value);
            }
        }

        public static object Get(string name)
        {
            foreach (var t in instance.list)
            {
                if (t.name == name) return t.value;
            }
            throw new ArgumentNullException($"变量不存在的");
        }

        public static bool Contain(string name) =>  instance.list.Any(t => t.name == name);
    }

    public class Vars
    {
        public string name;
        public object value;

        public Vars(string name, object value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
