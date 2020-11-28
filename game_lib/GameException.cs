using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace game_lib
{
    [Serializable()]
    public class GameException : Exception
    {
        public int ErrorCategory { get; }

        public GameException(string message)
           : base(message)
        {

        }
        public GameException(string message, int category)
          : base(message)
        {
            ErrorCategory = category;
        }


        protected GameException(SerializationInfo info,
                                    StreamingContext context)
           : base(info, context)
        { }

        public override string ToString()
        {
            return base.Message;
        }
    }
}
