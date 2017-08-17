namespace ACS.Lexer
{
    public class Token
    {
        public Token(int line,int seq,object value,string type)
        {
            line_number = line;
            this.seq = seq;
            this.Value = value;
            this.type = type;
        }

        public string type;
        public int line_number;
        public int seq;
        public object Value { get; set; }
    }
}
