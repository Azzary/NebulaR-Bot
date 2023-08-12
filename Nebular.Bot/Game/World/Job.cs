using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nebular.Bot.Game.World
{
    public class Job
    {
        public int Id { get; private set; }
        public byte Level { get; set; }
        public string Name { get; private set; }
        public uint BaseExperience { get; private set; }
        public uint CurrentExperience { get; private set; }
        public uint NextLevelExperience { get; private set; }
        public List<JobSkill> Skills { get; private set; }

        public Job(int _id)
        {
            Id = _id;
            Name = GetJobName(Id);
            Skills = new List<JobSkill>();
        }

        private string GetJobName(int jobId) => XElement.Parse(Properties.Resources.oficios).Elements("OFICIO").Where(e => int.Parse(e.Element("id").Value) == jobId).Elements("nombre").Select(e => e.Value).FirstOrDefault();
        public double ExperiencePercentage => CurrentExperience == 0 ? 0 : Math.Round((double)(CurrentExperience - BaseExperience) / (NextLevelExperience - BaseExperience) * 100, 2);

        public void UpdateJob(byte _level, uint _baseExperience, uint _currentExperience, uint _nextLevelExperience)
        {
            Level = _level;
            BaseExperience = _baseExperience;
            CurrentExperience = _currentExperience;
            NextLevelExperience = _nextLevelExperience;
        }
    }
}
