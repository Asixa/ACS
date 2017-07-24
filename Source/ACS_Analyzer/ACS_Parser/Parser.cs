using ACS_Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ACS_Analyzer.ACS_Parser
{
    class Parser
    {
        public static Parser instance;
        public List<Token> q;
        public int now_run_id = 0;
        //public List<string> var_name

        //public List<string> var_data = new List<string>();

        public Parser(List<Token> _q)
        {
            instance = this;
            q = _q;
        }

        public void Start()
        {
            SetVar("true", "true"); SetVar("false", "false");
            for (now_run_id = 0; now_run_id < q.Count - 1;)
           {
                //Console.WriteLine(">> [ 变量"+q[now_run_id].GetValue()+ "赋值" + GetExper(q[now_run_id]).Calculate()+ "]");
                GetExper(q[now_run_id]).Calculate();
            }
        }

        public string Test()
        {
          return  GetExper(q[now_run_id]).Calculate();
        }

        public BinaryExper GetExper(Token t)
        {
            if (t == null) //如果t是空则返回空值
            {
                return new BinaryExper("");
            }

          //  Console.WriteLine("<<<" + t.GetValue());

            #region 保留字段的函数检测
            if (t.GetValue() == "print")
            {
                int TokenDis;
                BinaryExper v = GetResultFromBrackets(q[t.seq + 1],out TokenDis);

                if (q[t.seq + 3].GetValue() == ")" && q[t.seq + 4].GetValue() == ";")
                {
                    if (q[t.seq + 2].type == Types.Identifier)
                    {
                        Console.WriteLine(GetVar(q[t.seq + 2].GetValue()));
                    }
                    else {
                        Console.WriteLine(q[t.seq + 2].GetValue().Replace("\"", ""));
                    }
                    now_run_id = t.seq + 5;
                    return new BinaryExper("");
                }
                else {

                    Console.WriteLine(v.Calculate().Replace("\"", ""));
                    now_run_id = t.seq + TokenDis + 4;
                    return new BinaryExper("");
                }

            }
            else if (t.GetValue() == "if")
            {
                int TokenDis;
                BinaryExper v = GetResultFromBrackets(q[t.seq + 1], out TokenDis);
                if (v.Calculate() == "true")
                {
                    int tokenDis2;
                    GetFieldResult(q[t.seq + TokenDis + 3],out tokenDis2);
                    now_run_id = t.seq + TokenDis + 3+ tokenDis2 + 2;
                    //Console.WriteLine("TRUE>>>" + q[now_run_id].GetValue());
                    return new BinaryExper("");
                }
                else
                {
                    int LP_count = 1;
                    int tokenDis = 0;
                    for(int i = 1; i < q.Count; i++)
                    {
                        tokenDis++;
                        if(q[t.seq + TokenDis + 3 + i].GetValue()=="}")
                        {
                            LP_count--;
                            if (LP_count == 0)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if(q[t.seq + TokenDis + 3 + i].GetValue() == "{")
                            {
                                LP_count++;
                            }
                        }

                    }
                    now_run_id = t.seq + TokenDis + 4+tokenDis;
                 // Console.WriteLine("FASLE>>>"+q[now_run_id-2].GetValue() + q[now_run_id-1].GetValue() + q[now_run_id].GetValue() + q[now_run_id+1].GetValue() );
                    return new BinaryExper("");
                }
            }
            else if (t.GetValue() == "[" && q[t.seq + 2].GetValue() == "]" && q[t.seq + 3].GetValue() == ";")
            {
                SetVar(q[t.seq + 1].GetValue(), (t.seq + 2).ToString());
                now_run_id = t.seq + 4;
                return new BinaryExper("");
            }
            else if (t.GetValue() == "goto" && q[t.seq + 2].GetValue() == ";")
            {
                now_run_id =int.Parse(GetVar(q[t.seq + 1].GetValue()));
                return new BinaryExper("");
            }
            else if (t.GetValue() == "quit" && q[t.seq + 1].GetValue() == "（"&&q[t.seq + 2].GetValue() == ")" && q[t.seq + 3].GetValue() == ";")
            {
                Environment.Exit(0);
                return new BinaryExper("");
            }
            else if (t.GetValue() == "clear" && q[t.seq + 1].GetValue() == "（" && q[t.seq + 2].GetValue() == ")" && q[t.seq + 3].GetValue() == ";")
            {
                Console.Clear();
                return new BinaryExper("");
            }
            else if (t.GetValue() == "input")
            {
                if (q[t.seq + 1].GetValue() == "(")
                {
                    if (q[t.seq + 3].GetValue() == ")" && q[t.seq + 4].GetValue() == ";")
                    {
                        if (q[t.seq + 2].type == Types.Identifier) {
                            string name = q[t.seq + 2].GetValue();
                         if (!VariableList.instance.var_name.Contains(name))
                            {
                                VariableList.instance.var_name.Add(name);
                                VariableList.instance.var_data.Add("");
                            }

                            VariableList.instance.var_data[VariableList.instance.var_name.IndexOf(name)] = Console.ReadLine(); ;
                            now_run_id = t.seq + 5;
                            return new BinaryExper("");
                        }
                    }
                }
            }

            #endregion

            #region 四则运算
            if (t.seq < q.Count)
            {
                if (q[t.seq + 1].GetValue() == ";")
                {
                    now_run_id = t.seq + 2;
                    BinaryExper e = new BinaryExper(t);
                    return e;
                }
            }
            else
            {
                BinaryExper e = new BinaryExper(t);
                return new BinaryExper(e);
            }
            BinaryExper now_exper=new BinaryExper();

            #region 括号
            if (t.GetValue() == "(")
            {

                int LP_count = 1;
                List<Token> inter_tokens=new List<Token>();
                inter_tokens.Add(new OperatorToken(0, inter_tokens.Count, "KEEP"));
                inter_tokens.Add(new OperatorToken(0, inter_tokens.Count, "="));


                for (int i = 1;i<q.Count-1; i++)
                {
                    string v = q[t.seq + i].GetValue();
                    if (v != ")")
                    {
                        if (v == "(")
                        {
                            LP_count++;
                        }
                        Token nt = q[t.seq + i];
                        nt.seq = inter_tokens.Count;
                        inter_tokens.Add(nt);
                    } 
					else                                            
                    {
                        LP_count--;
                        Token nt = q[t.seq + i];
                        nt.seq = inter_tokens.Count;
                       
                        if (LP_count == 0)
                        {
                            break;
                        }
                        else {
                            inter_tokens.Add(nt);
                        }
                    }
                }
                Token nt2 = new OperatorToken(0, inter_tokens.Count,";");nt2.seq = inter_tokens.Count;inter_tokens.Add(nt2);

                BinaryExper vvv = new BinaryExper(GetValueFromTokens(inter_tokens));
                vvv.Calculate();
               
                if (q.Count > (t.seq + inter_tokens.Count + 1))
                {
                    now_exper.Left = vvv;
                    now_exper.Operator = q[t.seq + inter_tokens.Count -1].GetValue();
                    if (now_exper.Operator == ";")
                    {
                        now_run_id = t.seq + inter_tokens.Count;
                        now_exper = vvv;
                    }
                    else {
                        now_exper.Right = GetExper(q[t.seq + inter_tokens.Count]);
                    }
                    return now_exper;
                }
                else
                {
                    now_exper = vvv;
                         return now_exper;
                }
               
            }
            #endregion
            now_exper.Left = new BinaryExper(t);
            now_exper.Operator = q[t.seq + 1].GetValue();
            if (now_exper.Operator == "*" | now_exper.Operator == "/" | now_exper.Operator == "%")
            {
                if (q[t.seq + 2].GetValue() != "(")
                {
                    now_exper.Right = new BinaryExper(q[t.seq + 2]);
                    if (q[t.seq + 3].GetValue() == ";")
                    {
                        now_run_id = t.seq + 4;
                        return now_exper;
                    }
                    //检测右子树再右边的内容
                    BinaryExper new_exper = new BinaryExper();
                    new_exper.Left = now_exper;
                    new_exper.Operator = q[t.seq + 3].GetValue();
                    new_exper.Right = GetExper(q[t.seq + 4]);
                    return new_exper;
                }
                else
                {
                    int tokenDis = 0;
                    now_exper.Right = GetResultFromBrackets(q[t.seq + 2],out tokenDis);
                    if (q[t.seq + 4 + tokenDis].GetValue() == ";")
                    {
                        now_run_id = t.seq + 4+1 + tokenDis;
                        return now_exper;
                    }
                    BinaryExper new_exper = new BinaryExper();
                    new_exper.Left = now_exper;
                    new_exper.Operator = q[t.seq + 4 + tokenDis].GetValue();
                    new_exper.Right = GetExper(q[t.seq + 5 + tokenDis]);
                    return new_exper;
                }
            }
            else
            {
               
                now_exper.Right = GetExper(q[t.seq + 2]);
                return now_exper;
            }
            //递归走起
            #endregion
        }

        public string GetVar(string s)
        {
            if (!VariableList.instance.var_name.Contains(s)) return "ERROR";
           return VariableList.instance.var_data[VariableList.instance.var_name.IndexOf(s)];
        }

        public void SetVar(string name,string value)
        {
            if (VariableList.instance.var_name.Contains(name))
            {
                VariableList.instance.var_data[VariableList.instance.var_name.IndexOf(name)]=value;
            }
            else
            {
                VariableList.instance.var_name.Add(name);
                VariableList.instance.var_data.Add(value);
            }
        }


        public string GetValueFromTokens(List<Token> t)
        {
                Parser p = new Parser(t);
                 string result = p.Test();
                instance = this;
                 return result;
        }


        public BinaryExper GetResultFromBrackets(Token t, out int number)
        {
            BinaryExper now_exper = new BinaryExper();


                int LP_count = 1;
                List<Token> inter_tokens = new List<Token>();
                inter_tokens.Add(new OperatorToken(0, inter_tokens.Count, "KEEP"));
                inter_tokens.Add(new OperatorToken(0, inter_tokens.Count, "="));

                for (int i = 1; i < q.Count - 1; i++)
                {
                    string v = q[t.seq + i].GetValue();
                    if (v != ")")
                    {
                        if (v == "(")
                        {
                            LP_count++;
                        }
                        Token nt = q[t.seq + i];
                        nt.seq = inter_tokens.Count;
                        inter_tokens.Add(nt);
                    }
                    else
                    {
                        LP_count--;
                        Token nt = q[t.seq + i];
                        nt.seq = inter_tokens.Count;

                        if (LP_count == 0)
                        {
                            break;
                        }
                        else {
                            inter_tokens.Add(nt);
                        }
                    }
                }
                Token nt2 = new OperatorToken(0, inter_tokens.Count, ";"); nt2.seq = inter_tokens.Count; inter_tokens.Add(nt2);

                BinaryExper vvv = new BinaryExper(GetValueFromTokens(inter_tokens));
                vvv.Calculate();
                 number = inter_tokens.Count - 3;
                    return vvv;
                }

        public BinaryExper GetFieldResult(Token t, out int number)
        {
            BinaryExper now_exper = new BinaryExper();

                int LP_count = 1;
                List<Token> inter_tokens = new List<Token>();
                inter_tokens.Add(new OperatorToken(0, inter_tokens.Count, "KEEP"));
                inter_tokens.Add(new OperatorToken(0, inter_tokens.Count, "="));
                for (int i = 1; i < q.Count - 1; i++)
                {
                    string v = q[t.seq + i].GetValue();
                    if (v != "}")
                    {
                        if (v == "{")
                        {
                            LP_count++;
                        }
                        Token nt = q[t.seq + i];
                        nt.seq = inter_tokens.Count;
                        inter_tokens.Add(nt);
                    }
                    else
                    {
                        LP_count--;
                        Token nt = q[t.seq + i];
                        nt.seq = inter_tokens.Count;

                        if (LP_count == 0)
                        {
                            break;
                        }
                        else {
                            inter_tokens.Add(nt);
                        }
                    }
                }
                Token nt2 = new OperatorToken(0, inter_tokens.Count, ";"); nt2.seq = inter_tokens.Count; inter_tokens.Add(nt2);

            //    BinaryExper vvv = new BinaryExper(GetValueFromTokens(inter_tokens));
            //    vvv.Calculate();

            Parser p = new Parser(inter_tokens);p.Start();
            BinaryExper vvv = new BinaryExper();

                number = inter_tokens.Count - 3;
                now_exper = vvv;
                return now_exper;
        }


    }

    class VariableList
    {
        public static VariableList instance;
        public List<string> var_name = new List<string>();
        public List<string> var_data = new List<string>();
        public VariableList()
        {
            instance = this;
        }

    }

    class BinaryExper : Token
    {
        public string var_name;
        public string Operator = "";
        public BinaryExper Left, Right;
        public string value = "";
        #region 构造重载
        public BinaryExper(string v = "", BinaryExper l = null, BinaryExper r = null)
        {
            Left = l;
            Right = r;
            value = v;
        }

        public BinaryExper(string v = "")
        {
            Left = null;
            Right = null;
            Operator = "";
            value = v;
        }

        public BinaryExper(Token t)
        {
            Left = null;
            Right = null;
            Operator = "";
            if (t.type == Types.Number)
            {
                value = t.GetNumber().ToString();
            }
            else if (t.type == Types.Identifier)
            {
                string name = t.GetValue();
                if (VariableList.instance.var_name.Contains(name))
                {
                    var_name = name;
                    value = VariableList.instance.var_data[VariableList.instance.var_name.IndexOf(name)];
                }
                else
                {
                    var_name = name;
                    VariableList.instance.var_name.Add(name);
                    VariableList.instance.var_data.Add("");
                    value = "";
                }
            }

            else
            {
                value = t.GetValue();
                //value = "";
            }
            //  = t.GetValue();
        }

        public BinaryExper()
        {
            Left = null;
            Right = null;
            Operator = "";
            value = "";
        }
        #endregion
        public string Calculate()
        {
            if (Left != null)
            {
                Left.Calculate();
            }
            else
            {

            }
            if (Right != null)
            {
                Right.Calculate();
            }
            switch (Operator)
            {
                case "+":
                    {
                        if (IsNumeric(Left.value))
                        {
                            if (IsNumeric(Right.value))
                            {
                                value = (float.Parse(Left.value) + float.Parse(Right.value)).ToString();
                            }
                            else
                            {
                                value = Left.value + Right.value;
                            }
                        }
                        else
                        {
                            value = Left.value + Right.value;
                        }
                        break;
                    }
                case "==":
                    {
                        if (Left.value == Right.value)
                        {
                            value = "true";
                        }
                        else
                        {
                            value = "false";
                        }
                        break;
                    }
                case "!=":
                    {
                        if (Left.value != Right.value)
                        {
                            value = "true";
                        }
                        else
                        {
                            value = "false";
                        }
                        break;
                    }
                case ">=":
                    {
                        if (IsNumeric(Left.value))
                        {
                            if (IsNumeric(Right.value))
                            {
                                if (Left.value == "") Left.value = "0";
                                if (Right.value == "") Right.value = "0";
                                value = (float.Parse(Left.value) >= float.Parse(Right.value)) ? "true" : "false";
                                break;
                            }
                        }
                        value = Left.value + Right.value;
                        break;
                    }
                case "<=":
                    {
                        if (IsNumeric(Left.value))
                        {
                            if (IsNumeric(Right.value))
                            {
                                if (Left.value == "") Left.value = "0";
                                if (Right.value == "") Right.value = "0";
                                value = (float.Parse(Left.value) <= float.Parse(Right.value)) ? "true" : "false";
                                break;
                            }
                        }
                        value = Left.value + Right.value;
                        break;
                    }
                case "<":
                    {
                        if (IsNumeric(Left.value))
                        {
                            if (IsNumeric(Right.value))
                            {
                                if (Left.value == "") Left.value = "0";
                                if (Right.value == "") Right.value = "0";
                                value = (float.Parse(Left.value) < float.Parse(Right.value)) ? "true" : "false";
                                break;
                            }
                        }
                        value = Left.value + Right.value;
                        break;
                    }
                case ">":
                    {
                        if (IsNumeric(Left.value))
                        {
                            if (IsNumeric(Right.value))
                            {
                                if (Left.value == "") Left.value = "0";
                                if (Right.value == "") Right.value = "0";
                                value = (float.Parse(Left.value) > float.Parse(Right.value)) ? "true" : "false";
                                break;
                            }
                        }
                        value = Left.value + Right.value;
                        break;
                    }
                case "*":
                    {
                        if (IsNumeric(Left.value))
                        {
                            if (IsNumeric(Right.value))
                            {
                                if (Left.value == "") Left.value = "0";
                                if (Right.value == "") Right.value = "0";

                                value = (float.Parse(Left.value) * float.Parse(Right.value)).ToString();
                            }
                            else
                            {
                                value = Left.value + Right.value;
                            }
                        }
                        else
                        {
                            value = Left.value + Right.value;
                        }
                        break;
                    }
                case "%":
                    {
                        if (IsNumeric(Left.value))
                        {
                            if (IsNumeric(Right.value))
                            {
                                if (Left.value == "") Left.value = "0";
                                if (Right.value == "") Right.value = "0";
                                value = (float.Parse(Left.value) % float.Parse(Right.value)).ToString();
                            }
                            else
                            {
                                value = Left.value + Right.value;
                            }
                        }
                        else
                        {
                            value = Left.value + Right.value;
                        }
                        break;
                    }
                case "/":
                    {
                        if (IsNumeric(Left.value))
                        {
                            if (IsNumeric(Right.value))
                            {
                                if (Left.value == "") Left.value = "0";
                                if (Right.value == "") Right.value = "0";
                                value = (float.Parse(Left.value) / float.Parse(Right.value)).ToString();
                            }
                            else
                            {
                                value = Left.value + Right.value;
                            }
                        }
                        else
                        {
                            value = Left.value + Right.value;
                        }
                        break;
                    }
                case "-":
                    {
                        if (IsNumeric(Left.value))
                        {
                            if (IsNumeric(Right.value))
                            {
                                if (Left.value == "") Left.value = "0";
                                if (Right.value == "") Right.value = "0";
                                value = (float.Parse(Left.value) - float.Parse(Right.value)).ToString();
                            }
                            else
                            {
                                value = Left.value + Right.value;
                            }
                        }
                        else
                        {
                            value = Left.value + Right.value;
                        }
                        break;
                    }
                case "=":
                    {
                        value = Left.value = Right.value;
                        if (Left.var_name != null)
                        {
                            if (VariableList.instance.var_name.Contains(Left.var_name))
                            {
                                VariableList.instance.var_data[VariableList.instance.var_name.IndexOf(Left.var_name)] = value;
                            }
                        }
                        break;
                    }
                case "":
                    {

                        if (Left != null)
                            value = Left.value;
                        break;
                    }
            }
            return value;
        }
        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }


    }

}

