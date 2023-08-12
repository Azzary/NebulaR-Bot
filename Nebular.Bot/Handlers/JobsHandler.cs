using Nebular.Bot.Network;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Handlers
{
    public class JobsHandler
    {

        [Packet("JS")]
        public void GetJobSkills(Dofus client, string packet)
        {
            client.Account.Character.Jobs.UpdateJobSkills(packet);
        }

        [Packet("JX")]
        public Task GetJobExperience(Dofus client, string packet) => Task.Run(async () => await client.Account.Character.Jobs.UpdateJobExperience(packet));



    }
}
