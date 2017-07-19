using System;
using System.Collections;
//using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACS_Lexer
{
    public abstract class ASTree : IEnumerable<ASTree>
    {
        public IEnumerator<ASTree> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
