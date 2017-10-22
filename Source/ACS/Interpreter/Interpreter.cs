using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

        public static void Run(List<Result> tree)
        {
            foreach (var command in tree)
            {
                Run_Result(command);
            }
        }

        public static void Run_Result(Result r)
        {
            switch (r.name)
            {
                case "Print1": //string
                    Console.WriteLine(r.commands[0].value.ToString());
                    break;
                case "Print2": // 变量
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
                    Console.WriteLine(Caculate((Result)r.commands[0].value));
                    break;
                case "Assignment2":
                    Register.Set(r.commands[0].value.ToString(), r.commands[1].value);
                    break;
                case "Assignment1":
                    Register.Set(r.commands[0].value.ToString(), Caculate((Result)r.commands[1].value));
                    break;
                case "if1":
                    var result = GetValue_boolExpression((Result) r.commands[0].value);
                    if (!result)break;
                    var field_list=new List<Result>();
                    for (var i = 1; i < r.commands.Count; i++)
                    {
                        field_list.Add((Result)r.commands[i].value);
                    }
                    Run(field_list);
                    break;
                case "Input":
                        Register.Set(r.commands[0].value.ToString(), Console.ReadLine());
                        //r.commands[0].value.ToString(), Caculate((Result)r.commands[1].value));
                    break;
                default:
                    break;
            }
        }

        public static object Caculate(Result r)
        {

            var tree = GetTree(r.commands, 0);
            //ADK.Print("A: "+tree.Value.ToString());
            //ADK.Pause();
            return tree.Value;
        }

        private static BinaryTree GetTree(List<SingleResult> list,int id)
        {
           
            if(id==list.Count-1)return new BinaryTree(list[id].value);
        
            var tree = new BinaryTree();
            if (list[id].value.ToString() != "(")
            {
              
                tree = new BinaryTree()
                {
                    left = new BinaryTree(list[id].value),
                    Operator = list[id + 1].value.ToString(),
                };
                var _operator = list[id + 1].value.ToString();
                if (_operator == "*" || _operator == "/")
                {
                        var b_Length = 0;

                        tree.right = new BinaryTree(list[id + 2].value);
                        if (id+b_Length + 2 >= list.Count - 1)
                        {
                            return tree;
                        }
                        var new_tree = new BinaryTree
                        {
                            left = tree,
                            Operator = list[id + 3 + b_Length].value.ToString(),
                            right = GetTree(list, id + 4 + b_Length)
                        };
                    
                    return new_tree;
                }
                else if (_operator == "+" || _operator == "-")
                {
                    tree.right = GetTree(list, id + 2);
                }
            }
            else
            {
                var B_list = GetBrackets(list, id);
                var bracket_value = B_list.Count==1 ? new BinaryTree(B_list[0].value) : new BinaryTree(GetValueInBrackets(B_list));
                

                if (list.Count > (id + B_list.Count + 1))
                {
                    tree.left = bracket_value;
                    tree.Operator = list[id + B_list.Count + 2].value.ToString();
                    if (tree.Operator == ";")
                    {
                        tree = bracket_value;
                    }
                    else
                    {
                        tree.right = GetTree(list, B_list.Count+id+3);
                    }
                    return tree;
                }
                else
                {
                    tree = bracket_value;
                    return tree;
                }
            }
            return tree;
        }

        private static object GetValueInBrackets(List<SingleResult> list)
        {
            var result = GetTree(list, 0);
            return result.Caculate();
        }

        public static List<SingleResult> GetBrackets(List<SingleResult> list,int id)
        {
            var LP = 1;
            var new_result = new List<SingleResult>();
            for (var i = id + 1; i < list.Count; i++)
            {
                if (list[i].value.ToString() == "(") LP++;
                if (list[i].value.ToString() == ")") LP--;

                if (LP == 0)
                {

                    break;
                }
                new_result.Add(list[i]);
            }

            return new_result;
        }

        public static bool GetValue_boolExpression(Result r)
        {
            object v1 = r.commands[0].type == "identifier" ? Register.Get(r.commands[0].value.ToString()) : r.commands[0].value;
            object v2 = r.commands[2].type == "identifier" ? Register.Get(r.commands[2].value.ToString()) : r.commands[2].value;
            switch (r.commands[1].value.ToString())
            {
                case "==": return v1.ToString() == v2.ToString();
                case "<=": return  Math.Abs((float)v1 - (float)v2) < 0.0000001f;
                case ">=": return Math.Abs((float)v1 - (float)v2) > 0.0000001f;
                case "!=": return v1.ToString() != v2.ToString();
                default: return false;
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
            set { _v = value; }
            get { return GetValue(); }
        }

        public object Caculate()
        {
            if (left == null)
            {
                ADK.Print("左值空");
            }
            if (right == null)
            {
                ADK.Print("右值空");
            }
            //ADK.Print("左 " + left.Value+"操作 "+Operator+ " 右 " + right.Value);
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
                case "":
                    return _v;
                default:
                    break;
            }
            return new object();
        }

        public object GetValue()
        {
            if(left==null||right==null)return _v;
            return Caculate();
        }

    }


}
