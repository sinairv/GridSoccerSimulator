using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MRLDM
{
    class Program
    {
        private static MRLDMClient client;

        private static void ClientMainMethod()
        {
            client.Start();
        }

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                client = new MRLDMClient(args[0], Int32.Parse(args[1]));
            }
            else
            {
                client = new MRLDMClient("MRLDM", 1);
            }

            Thread clientThread = new Thread(ClientMainMethod);
            clientThread.Start();

            ControllerForm frmController = new ControllerForm(client);
            frmController.Show();
            Application.Run(frmController);
        }
    }
}
