using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Nebular.Zaap.Network;

namespace Nebular.Zaap.ZaapAPI.Protocol
{
    [Obfuscation(Exclude = true)]

    public class MessagesHandler
    {
        public static void Handle(ZaapClient client, ZaapMessage message)
        {
            switch (message)
            {
                case ConnectArgs connectArgs:
                    HandleConnectArgs(client, connectArgs);
                    break;
                case SettingsGet settingsGet:
                    HandleSettingsGet(client, settingsGet);
                    break;
                case UserInfoGet userInfoGet:
                    HandleUserInfoGet(client, userInfoGet);
                    break;
                case AuthGetGameToken authGetGameToken:
                    HandleAuthGetGameToken(client, authGetGameToken);
                    break;
                default:
                    break;
            }
        }

        private static async void HandleAuthGetGameToken(ZaapClient client, AuthGetGameToken message)
        {
            string apiKey = await ZaapConnect.get_ApiKey(client.Account.Email, client.Account.Password, client.Account.Key);
            string token = await ZaapConnect.get_Token(apiKey);
            client.Send(new AuthGetGameTokenResult(token));
        }

        private static void HandleUserInfoGet(ZaapClient client, UserInfoGet message)
        {
            client.Send(new UserInfosGetResult(client.Account.Speudo));
        }

        private static void HandleSettingsGet(ZaapClient client, SettingsGet message)
        {
            string result = null;

            switch (message.Key)
            {
                case "autoConnectType":
                    result = "false";
                    break;
                case "language":
                    result = "fr";
                    break;
                case "connectionPort":
                    result = "443";
                    break;
                default:
                    break;
            }

            client.Send(new SettingsGetResult(result));
        }

        private static void HandleConnectArgs(ZaapClient client, ConnectArgs message)
        {
            client.Send(new ConnectResult());
        }
    }
}
