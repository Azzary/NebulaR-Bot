using Nebular.Bot.Game.World;
using Nebular.Core.ProcessHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.Extension
{
    public class JobsExtension
    {
        public List<Job> Jobs { get; private set; }
        private bool JobsInitialized;
        private bool disposed;
        private Dofus Client { get; set; }
        public JobsExtension(Dofus client)
        {
            Client = client;
            Jobs = new List<Job>();
        }
        public bool HasSkillWithId(int id) => Jobs.FirstOrDefault(j => j.Skills.FirstOrDefault(s => s.Id == id) != null) != null;
        public IEnumerable<short> GetAvailableGatheringSkills() => Jobs.SelectMany(job => job.Skills.Where(skill => !skill.IsCraft).Select(skill => skill.Id));
        public IEnumerable<JobSkill> GetAvailableSkills() => Jobs.SelectMany(job => job.Skills.Select(skill => skill));


        public async Task CloseLvlupWindow()
        {
            Client.WindowManager.Click(370, 240);
            Client.WindowManager.Click(370, 240);
        }

        public async void UpdateJobSkills(string packet)
        {
            try
            {
                await CloseLvlupWindow();
                JobsInitialized = false;

                string[] separatorSkill;
                Job job;
                JobSkill skill = null;
                short jobId, skillId;
                byte minQuantity, maxQuantity;
                float time;

                foreach (string jobData in packet.Substring(3).Split('|'))
                {
                    jobId = short.Parse(jobData.Split(';')[0]);
                    job = Jobs.Find(x => x.Id == jobId);

                    if (job == null)
                    {
                        job = new Job(jobId);
                        Jobs.Add(job);
                    }

                    foreach (string skillData in jobData.Split(';')[1].Split(','))
                    {
                        separatorSkill = skillData.Split('~');
                        skillId = short.Parse(separatorSkill[0]);
                        minQuantity = byte.Parse(separatorSkill[1]);
                        maxQuantity = byte.Parse(separatorSkill[2]);
                        time = float.Parse(separatorSkill[4]);
                        skill = job.Skills.Find(activity => activity.Id == skillId);

                        if (skill != null)
                            skill.Update(skillId, minQuantity, maxQuantity, time);
                        else
                            job.Skills.Add(new JobSkill(skillId, minQuantity, maxQuantity, time));
                    }
                }

                JobsInitialized = true;
            }
            catch (Exception)
            {
            }
        }

        public async Task UpdateJobExperience(string packet)
        {
            while (!JobsInitialized)
                await Task.Delay(50);

            string[] separatorJobExperience = packet.Substring(3).Split('|');
            uint currentExperience, baseExperience, nextLevelExperience;
            short id;
            byte level;

            foreach (string job in separatorJobExperience)
            {
                id = short.Parse(job.Split(';')[0]);
                level = byte.Parse(job.Split(';')[1]);
                baseExperience = uint.Parse(job.Split(';')[2]);
                currentExperience = uint.Parse(job.Split(';')[3]);

                if (level < 100)
                    nextLevelExperience = uint.Parse(job.Split(';')[4]);
                else
                    nextLevelExperience = 0;

                Jobs.Find(x => x.Id == id).UpdateJob(level, baseExperience, currentExperience, nextLevelExperience);
            }
        }

    }
}
