using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class FormPanelMessageBridge : IFormMessageBridge
    {
        Form1 f;
        Panel panel;

        public FormPanelMessageBridge(Form1 _f, Panel _p)
        {
            f = _f;
            panel = _p;
            panel.mesBridge = this;
        }

        public void SendCommandToForm(enumFormMessageCommand _com)
        {
            switch(_com)
            {
                case enumFormMessageCommand.RenewImage:
                    f.RenewImage();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void SendMessageToForm(string _mes)
        {
            f.RecieveMessage(_mes);
        }

        public void SendMessageToForm(string _mes, string _caption)
        {
            f.RecieveMessage(_mes, _caption);
        }

        public void SendMessageToObject(string _mes)
        {
            throw new NotImplementedException();
        }

        public void SendMessageToObject(string _mes, string _caption)
        {
            throw new NotImplementedException();
        }
    }
}
