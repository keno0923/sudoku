using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    enum enumFormMessageCommand
    {
        RenewImage
    }

    interface IFormMessageBridge
    {
        void SendMessageToForm(String _mes);
        void SendMessageToForm(String _mes, String _caption);
        void SendCommandToForm(enumFormMessageCommand _com);
        void SendMessageToObject(String _mes);
        void SendMessageToObject(String _mes, String _caption);
    }
}
