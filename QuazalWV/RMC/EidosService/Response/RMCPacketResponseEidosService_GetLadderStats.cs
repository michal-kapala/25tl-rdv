using System.Collections.Generic;
using System.IO;

namespace QuazalWV
{
    public class RMCPacketResponseEidosService_GetLadderStats : RMCPResponse
    {
        public List<TTLStats> Stats { get; set; }

        public RMCPacketResponseEidosService_GetLadderStats(ClientInfo client, uint nbPlayers)
        {
            // Currently the view is limited to 4 players
            Stats = StatsHelper.GetLadderPart(client);
            /*Stats = new List<TTLStats>
            {
                new TTLStats(client)
                {
                    // Queries db for the stats for 1 player
                    Values = StatsHelper.Get(client.PID).ToList()
                    *//*Values = new List<float> 
                    { 
                        011f, 022f, 033f, 044f, 055f, 066f, 077f, 088f, 099f, 1000f,
                        111f, 122f, 133f, 144f, 155f, 166f, 177f, 188f, 199f, 200f,
                        211f, 001f, 002f, 002f, 110f, 002f, 277f, 288f, 299f, 300f,
                        311f, 322f, 333f, 344f, 355f, 366f, 377f, 388f, 399f, 400f,
                        411f, 422f, 433f, 444f, 455f, 466f, 477f, 488f, 499f, 500f,
                        511f, 522f, 533f, 544f, 555f, 566f, 577f, 588f, 599f, 600f,
                        611f, 622f, 633f, 644f, 655f, 666f, 677f, 688f, 699f, 700f,
                    }*//*
                }
            };*/
        }

        public override byte[] ToBuffer()
        {
            MemoryStream m = new MemoryStream();
            Helper.WriteU32(m, (uint)Stats.Count);
            foreach (TTLStats s in Stats)
                s.ToBuffer(m);
            return m.ToArray();
        }

        public override string ToString()
        {
            return $"[GetLadderStats Response, count={Stats.Count}]";
        }

        public override string PayloadToString()
        {
            return "";
        }

    }
}
