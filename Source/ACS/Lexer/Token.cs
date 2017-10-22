namespace ACS.Lexer
{
    public class Token
    {
        public Token(int line,int seq,object value,int type)
        {
            line_number = line;
            this.seq = seq;
            this.Value = value;
            this.type = type;
        }

        public int type;
        public int line_number;
        public int seq;
        public object Value { get; set; }
    }
}
