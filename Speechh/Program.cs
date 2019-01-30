using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace Speechh
{
    class Program
    {

        private static SpeechSynthesizer speaker = new SpeechSynthesizer();

        static void Main(string[] args)
        {
            VoicePlayer vp = new VoicePlayer();
            Console.Read();
        }
    }
}
