using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ACS.Lexer;
using ACS.Parser;
using ACS.Variable_Register;

namespace ACS.Interpreter
{
    internal class Interpreter
    {
        public static Interpreter instance;

        public static void Init()
        {
            instance=new Interpreter();
        }

        public static void Run(Result r)
        {
            switch (r.name)
            {
                case "Print2":
                case "Print1":
                    switch (r.commands[0].type)
                    {
                        case "identifier":
                            Console.WriteLine(Register.Contain(r.commands[0].value.ToString())
                                ? Register.Get(r.commands[0].value.ToString()).ToString()
                                : "NULL");
                            break;
                        case "int":
                        case "float":
                            Console.WriteLine(r.commands[0].value.ToString());
                            break;
                        default:
                                   
                            break;
                    }
                    break;
                case "Print3":
                    Console.WriteLine(r.commands[0].type.ToString());
                    break;
                case "Assignment2":
                    Register.Set(r.commands[0].value.ToString(), r.commands[1].value);
                    break;
                default:
                    break;
            }
        }

        public static void Caculate(Result r)
        {

            var tree = new BinaryTree
            {
                left = new BinaryTree(r.commands[0]),
                Operator = r.commands[1].value.ToString()              
            };

            foreach (var item in r.commands)
            {
                
            }

        }
    }

    public class BinaryTree
    {
        public BinaryTree left;
        public BinaryTree right;
        public string Operator;

        public BinaryTree(object v)
        {
            Value = v;
        }

        public BinaryTree()
        {

        }

        private object _v;

        public object Value
        {
            set => _v = value;
            get => GetValue();
        }

        public object Caculate()
        {
            switch (Operator)
            {
                case "+":
                    return float.Parse(left.Value.ToString()) + float.Parse(right.Value.ToString());
                case "-":
                    return float.Parse(left.Value.ToString()) - float.Parse(right.Value.ToString());
                case "*":
                    return float.Parse(left.Value.ToString()) * float.Parse(right.Value.ToString());
                case "/":
                    return float.Parse(left.Value.ToString()) / float.Parse(right.Value.ToString());
                default:
                    break;
            }
            return new object();
        }

        public object GetValue()
        {
            if(left==null||right==null)return Value;
            return Caculate();
        }

    }


}
