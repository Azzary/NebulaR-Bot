using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebular.Bot.Game.World.Interactive
{
    public class InteractiveObjectModel
    {
        public short[] Gfxs { get; private set; }
        public bool IsWalkable { get; private set; }
        public short[] Abilities { get; private set; }
        public string Name { get; private set; }
        public bool IsCollectible { get; private set; }

        private static List<InteractiveObjectModel> loadedInteractiveModels = new List<InteractiveObjectModel>();

        public InteractiveObjectModel(string name, string gfx, bool isWalkable, string abilities, bool isCollectible)
        {
            Name = name;

            if (!gfx.Equals("-1") && !string.IsNullOrEmpty(gfx))
            {
                string[] separator = gfx.Split(',');
                Gfxs = new short[separator.Length];

                for (byte i = 0; i < Gfxs.Length; i++)
                    Gfxs[i] = short.Parse(separator[i]);
            }

            IsWalkable = isWalkable;

            if (!abilities.Equals("-1") && !string.IsNullOrEmpty(abilities))
            {
                string[] separator = abilities.Split(',');
                Abilities = new short[separator.Length];

                for (byte i = 0; i < Abilities.Length; ++i)
                    Abilities[i] = short.Parse(separator[i]);
            }

            IsCollectible = isCollectible;
            loadedInteractiveModels.Add(this);
        }

        public static InteractiveObjectModel GetModelByGfx(short gfxId)
        {
            foreach (InteractiveObjectModel interactive in loadedInteractiveModels)
            {
                if (interactive.Gfxs.Contains(gfxId))
                    return interactive;
            }
            return null;
        }

        public static InteractiveObjectModel GetModelByAbility(short abilityId)
        {
            IEnumerable<InteractiveObjectModel> interactiveList = loadedInteractiveModels.Where(i => i.Abilities != null);

            foreach (InteractiveObjectModel interactive in interactiveList)
            {
                if (interactive.Abilities.Contains(abilityId))
                    return interactive;
            }
            return null;
        }

        public static List<InteractiveObjectModel> GetLoadedInteractiveModels() => loadedInteractiveModels;
    }

}
