using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACS
{
    class ADK
    {
        public static object[] SplitArray(object[] Source, int StartIndex, int EndIndex)
        {
            try
            {
                var result = new object[EndIndex - StartIndex + 1];
                for (var i = 0; i <= EndIndex - StartIndex; i++) result[i] = Source[i + StartIndex];
                return result;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
