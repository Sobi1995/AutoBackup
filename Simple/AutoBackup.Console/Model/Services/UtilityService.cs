using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup.ConsoleApp.Model.Services
{
public    class UtilityService
    {
     Task<bool> checkConnectinString(string connectionString)
        {
            try
            {
                IPAddress ipAddress = Dns.GetHostEntry(connectionString).AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 1433);

                using (TcpClient tcpClient = new TcpClient())
                {
                    tcpClient.Connect(ipEndPoint);
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
          throw new Exception($"TestViaTcp to server {connectionString} failed '{ex.GetType().Name}': {ex.Message}");
            }

        }
    }
}
