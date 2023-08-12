using Nebular.Bot.Network;
using Nebular.Core.Network.Messages;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Linq;
using Nebular.Bot.Game.Spells;
using Nebular.Core.Network;
using Nebular.Bot.Game.World.Interactive;
using Nebular.Bot.Interface;
using System.Threading;
using System.Management.Instrumentation;
using Nebular.Bot.Game.World.PathFinder;

namespace Nebular.Bot
{
    internal static class Program
    {

        [STAThread]

        static void Main()
        {
            Task.Run(WorldPathfinder.LoadScriptedCells);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }

        public static void LoadData()
        {
            Task.Run(() =>
            {
                XElement.Parse(Properties.Resources.interactivos).Descendants("SKILL").ToList().ForEach(i => new InteractiveObjectModel(i.Element("nombre").Value, i.Element("gfx").Value, bool.Parse(i.Element("caminable").Value), i.Element("habilidades").Value, bool.Parse(i.Element("recolectable").Value)));
            }).ContinueWith(t =>
            {
                XElement.Parse(Properties.Resources.lista_hechizos).Descendants("HECHIZO").ToList().ForEach(mapa =>
                {
                    Spell spell = new Spell(short.Parse(mapa.Attribute("ID").Value), mapa.Element("NOMBRE").Value);

                    mapa.Descendants("NIVEL").ToList().ForEach(stats =>
                    {
                        SpellStats spellStats = new SpellStats
                        {
                            CostPA = byte.Parse(stats.Attribute("COSTE_PA").Value),
                            MinRange = byte.Parse(stats.Attribute("RANGO_MINIMO").Value),
                            MaxRange = byte.Parse(stats.Attribute("RANGO_MAXIMO").Value),

                            IsLineCast = bool.Parse(stats.Attribute("LANZ_EN_LINEA").Value),
                            RequiresLineOfSight = bool.Parse(stats.Attribute("NECESITA_VISION").Value),
                            RequiresFreeCell = bool.Parse(stats.Attribute("NECESITA_CELDA_LIBRE").Value),
                            IsModifiableRange = bool.Parse(stats.Attribute("RANGO_MODIFICABLE").Value),

                            CastsPerTurn = byte.Parse(stats.Attribute("MAX_LANZ_POR_TURNO").Value),
                            CastsPerTarget = byte.Parse(stats.Attribute("MAX_LANZ_POR_OBJETIVO").Value),
                            Interval = byte.Parse(stats.Attribute("COOLDOWN").Value)
                        };

                        stats.Descendants("EFECTO").ToList().ForEach(efecto => spellStats.AddEffect(new SpellEffect(int.Parse(efecto.Attribute("TIPO").Value), Zones.Parse(efecto.Attribute("ZONA").Value)), bool.Parse(efecto.Attribute("ES_CRITICO").Value)));
                        spell.AddSpellStats(byte.Parse(stats.Attribute("NIVEL").Value), spellStats);

                    });
                });
            }).Wait();
        }
    }
}