using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuazalWV
{
    /// <summary>
    /// Model which maps 'stats' db data for TTLStats list values.
    /// </summary>
    /// <see href="https://gitlab.com/phoenix-network-ltd/25-to-life-docs/-/wikis/RMC-custom-data-objects#values-list-indexes"/>
    public class LadderStats : SqliteModel
    {
        /// <summary>
        /// Player's 'Ranking' value.
        /// </summary>
        public float Ranking { get; set; }
        /// <summary>
        /// Unknown.
        /// </summary>
        public float Stat2 { get; set; }
        /// <summary>
        /// Unknown.
        /// </summary>
        public float Stat3 { get; set; }
        /// <summary>
        /// Rounds won as law enforcement, summed with RoundsWonCriminal
        /// </summary>
        public float RoundsWonLaw { get; set; }
        /// <summary>
        /// Sessions (not sure, played or won) as law enforcement, summed with SessionsCriminal.
        /// </summary>
        public float SessionsLaw { get; set; }
        /// <summary>
        /// Unknown.
        /// </summary>
        public float Stat6 { get; set; }
        /// <summary>
        /// Rounds won as criminals, summed with RoundsWonLaw.
        /// </summary>
        public float RoundsWonCriminal { get; set; }
        /// <summary>
        /// Sessions (not sure, played or won) as criminals, summed with SessionsLaw.
        /// </summary>
        public float SessionsCriminal { get; set; }
        /// <summary>
        /// Unknown.
        /// </summary>
        public float Stat9 { get; set; }
        /// <summary>
        /// Kills as law enforcement, summed with KillsCriminal.
        /// </summary>
        public float KillsLaw { get; set; }
        /// <summary>
        /// Deaths as law enforcement, summed with DeathsCriminal.
        /// </summary>
        public float DeathsLaw { get; set; }
        /// <summary>
        /// Kills as criminals, summed with KillsLaw.
        /// </summary>
        public float KillsCriminal { get; set; }
        /// <summary>
        /// Deaths as criminals, summed with DeathsLaw.
        /// </summary>
        public float DeathsCriminal { get; set; }
        /// <summary>
        /// MVPs scored as law enforcement, summed with MvpsCriminal.
        /// </summary>
        public float MvpsLaw { get; set; }
        /// <summary>
        /// MVPs scored as criminals, summed with MvpsLaw.
        /// </summary>
        public float MvpsCriminal { get; set; }
        /// <summary>
        /// Player's leaderboard rank.
        /// </summary>
        public float Rank { get; set; }
        /// <summary>
        /// Unknown.
        /// </summary>
        public float Stat17 { get; set; }
        /// <summary>
        /// Rounds lost as law enforcement, summed with RoundsLostCriminal.
        /// </summary>
        public float RoundsLostLaw { get; set; }
        /// <summary>
        /// Unknown.
        /// </summary>
        public float Stat19 { get; set; }
        /// <summary>
        /// Unknown.
        /// </summary>
        public float Stat20 { get; set; }
        /// <summary>
        /// Rounds lost as criminals, summed with RoundsLostCriminal.
        /// </summary>
        public float RoundsLostCriminal { get; set; }
        /// <summary>
        /// Unknown.
        /// </summary>
        public float Stat22 { get; set; }
        /// <summary>
        /// Unknown.
        /// </summary>
        public float Stat23 { get; set; }
        /// <summary>
        /// Unknown.
        /// </summary>
        public float Stat24 { get; set; }
        /// <summary>
        /// Duplicated law enforcement deaths for K/D ratio, summed with DeathsCriminalKdRatio. KillsLaw is reused for K/D ratio.
        /// </summary>
        public float DeathsLawKdRatio { get; set; }
        /// <summary>
        /// Duplicated criminals deaths for K/D ratio, summed with DeathsLawKdRatio. KillsCriminal is reused for K/D ratio.
        /// </summary>
        public float DeathsCriminalKdRatio { get; set; }
        /// <summary>
        /// Suicides as law enforcement, summed with SuicidesCriminal.
        /// </summary>
        public float SuicidesLaw { get; set; }
        /// <summary>
        /// Suicides as criminals, summed with SuicidesLaw.
        /// </summary>
        public float SuicidesCriminal { get; set; }
        /// <summary>
        /// All headshots as law enforcement, summed with HeadshotsCriminal.
        /// </summary>
        public float HeadshotsLaw { get; set; }
        /// <summary>
        /// All headshots as criminals, summed with HeadshotsLaw.
        /// </summary>
        public float HeadshotsCriminal { get; set; }
        /// <summary>
        /// Friendly-fire kills as law enforcement, summed with TeamKillsCriminal.
        /// </summary>
        public float TeamKillsLaw { get; set; }
        /// <summary>
        /// Friendly-fire kills as criminals, summed with TeamKillsLaw.
        /// </summary>
        public float TeamKillsCriminal { get; set; }
        /// <summary>
        /// Friendly-fire deaths as law enforcement.
        /// </summary>
        public float TeamKilledLaw { get; set; }
        /// <summary>
        /// Friendly-fire deaths as criminals.
        /// </summary>
        public float TeamKilledCriminal { get; set; }
        /// <summary>
        /// Hostage kills as law enforcement, summed with NpcKillsCriminal.
        /// </summary>
        public float NpcKillsLaw { get; set; }
        /// <summary>
        /// Hostage kills as criminal, summed with NpcKillsLaw.
        /// </summary>
        public float NpcKillsCriminal { get; set; }
        /// <summary>
        /// Apprehensions done (law enforcement).
        /// </summary>
        public float ApprehensionsLaw { get; set; }
        /// <summary>
        /// Apprehensions taken (criminals).
        /// </summary>
        public float ApprehendedCriminal { get; set; }
        /// <summary>
        /// Shots fired as law enforcement (melee attacks included), summed with ShotsFiredCriminal.
        /// </summary>
        public float ShotsFiredLaw { get; set; }
        /// <summary>
        /// Shots fired as criminals (melee attacks included), summed with ShotsFiredLaw.
        /// </summary>
        public float ShotsFiredCriminal { get; set; }
        /// <summary>
        /// Shots hit as law enforcement (melee attacks included), summed with ShotsHitCriminal.
        /// </summary>
        public float ShotsHitLaw { get; set; }
        /// <summary>
        /// Shots hit as criminals (melee attacks included), summed with ShotsHitLaw.
        /// </summary>
        public float ShotsHitCriminal { get; set; }
        /// <summary>
        /// Unknown.
        /// </summary>
        public float Stat43 { get; set; }
        /// <summary>
        /// Unknown.
        /// </summary>
        public float Stat44 { get; set; }
        /// <summary>
        /// Goods deliveries as criminals in Robbery mode.
        /// </summary>
        public float RobberyEscapes { get; set; }
        /// <summary>
        /// Evidence deliveries as law enforcement in Raid mode.
        /// </summary>
        public float RaidCaptures { get; set; }
        /// <summary>
        /// Tag captures in Tag mode (for both teams).
        /// </summary>
        public float Tags { get; set; }

        /// <summary>
        /// Returns a list of TTLStats.Values format.
        /// </summary>
        /// <returns></returns>
        public List<float> ToList()
        {
            List<float> result = new List<float>
            {
                Ranking,
                Stat2,
                Stat3,
                RoundsWonLaw,
                SessionsLaw,
                Stat6,
                RoundsWonCriminal,
                SessionsCriminal,
                Stat9,
                KillsLaw,
                DeathsLaw,
                KillsCriminal,
                DeathsCriminal,
                MvpsLaw,
                MvpsCriminal,
                Rank,
                Stat17,
                RoundsLostLaw,
                Stat19,
                Stat20,
                RoundsLostCriminal,
                Stat22,
                Stat23,
                Stat24,
                DeathsLawKdRatio,
                DeathsCriminalKdRatio,
                SuicidesLaw,
                SuicidesCriminal,
                HeadshotsLaw,
                HeadshotsCriminal,
                TeamKillsLaw,
                TeamKillsCriminal,
                TeamKilledLaw,
                TeamKilledCriminal,
                NpcKillsLaw,
                NpcKillsCriminal,
                ApprehensionsLaw,
                ApprehendedCriminal,
                ShotsFiredLaw,
                ShotsFiredCriminal,
                ShotsHitLaw,
                ShotsHitCriminal,
                Stat43,
                Stat44,
                RobberyEscapes,
                RaidCaptures,
                Tags,
                1f,
                1f,
                1f
            };
            return result;
        }
    }
}
