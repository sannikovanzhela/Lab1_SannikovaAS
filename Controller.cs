using DataBaseFunctional;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationApp_Test
{
    public class Controller
    {
        IView _ViewInterface;
        DatabaseRepository db = new DatabaseRepository();
        IMessage _MessageInterface;

        public Controller(IView ViewInterface, IMessage MessageInterface)
        {
            _ViewInterface = ViewInterface;
            _MessageInterface = MessageInterface;
            _ViewInterface.runCheck += new Action<(string, string, string)>(Check);
        }

        public void Check((string,string,string)data)
        {
            (string, string) result;

            string[] dataInfo = db.GetData(data.Item1, data.Item2, data.Item3);

            if (dataInfo != null)
            {
                _MessageInterface.SentMessage(data.Item1, true);

                if (dataInfo[3] == "1") result.Item1 = "True";
                else result.Item1 = "False";

                result.Item2 = dataInfo[4];

                _ViewInterface.ShowResult(result);
            }
            else
            {
                _MessageInterface.SentMessage(data.Item1, false);

                CheckRegistrationData check = new CheckRegistrationData();
                (string, string) resultReg = check.CheckRegistration(data.Item1, data.Item2, data.Item3);

                if (resultReg.Item1 == "True")
                {
                    db.AddData(data.Item1, data.Item2, data.Item3, true, resultReg.Item2);

                    _ViewInterface.ShowResult(resultReg);
                }
                else
                {
                    db.AddData(data.Item1, data.Item2, data.Item3, false, resultReg.Item2);

                    _ViewInterface.ShowResult(resultReg);
                }
            }
        }
    }
}
