using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace RemoteInterface
{
    [Serializable]
   public class RemoteException:Exception
    {
       public RemoteException(string message):base(message)
       {
       }
       RemoteException(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
        }


       public override void GetObjectData(SerializationInfo info, StreamingContext context)
       {
           base.GetObjectData(info, context);
          // info.AddValue("additionalMessage", additionalMessage_);
       }

    }
}
