using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Speech.Recognition;

namespace Speechh
{
    public static class Mouth
    {
        static SpeechSynthesizer ss = new SpeechSynthesizer();
        static PromptStyle raised = new PromptStyle();
        static PromptStyle normal = new PromptStyle();
        static PromptStyle reduced = new PromptStyle();

        public static void Load()
        {
            ss.Volume = 100;
            raised.Emphasis = PromptEmphasis.Strong;
            raised.Volume = PromptVolume.Loud;
            normal.Emphasis = PromptEmphasis.Moderate;
            normal.Volume = PromptVolume.Medium;
            reduced.Emphasis = PromptEmphasis.Reduced;
            reduced.Volume = PromptVolume.Soft;
        }

        public static void SetSpeaker(string name)
        {
            ss.SelectVoice(name);
        }

        public static void Say(string question)
        {
            PromptBuilder pb = new PromptBuilder();
            pb.StartStyle(normal);
            for (int i = 0; i < question.Length; i++)
            {
                if (question[i] == '$' && question[i + 1] == '>')
                {
                    pb.EndStyle();
                    pb.StartStyle(raised);
                    i += 1;
                }
                else if (question[i] == '$' && question[i + 1] == '<')
                {
                    pb.EndStyle();
                    pb.StartStyle(reduced);
                    i += 1;
                }
                else if (question[i] == '$' && question[i + 1] == '|')
                {
                    pb.EndStyle();
                    pb.StartStyle(normal);
                    i += 1;
                }
                else
                {
                    pb.AppendText(question[i] + "");
                }
            }
            pb.EndStyle();
            ss.SpeakAsyncCancelAll();
            ss.SpeakAsync(pb);
        }


        public static void Speak(string question)
        {
            PromptBuilder pb = new PromptBuilder();
            pb.StartStyle(normal);
            for (int i = 0; i < question.Length; i++)
            {
                if (question[i] == '$' && question[i + 1] == '>')
                {
                    pb.EndStyle();
                    pb.StartStyle(raised);
                    i += 1;
                }
                else if (question[i] == '$' && question[i + 1] == '<')
                {
                    pb.EndStyle();
                    pb.StartStyle(reduced);
                    i += 1;
                }
                else if (question[i] == '$' && question[i + 1] == '|')
                {
                    pb.EndStyle();
                    pb.StartStyle(normal);
                    i += 1;
                }
                else
                {
                    pb.AppendText(question[i] + "");
                }
            }
            pb.EndStyle();
            ss.SpeakAsyncCancelAll();
            ss.Speak(pb);
        }
    }


    public static class Ear
    {
        static SpeechRecognitionEngine sre = new SpeechRecognitionEngine();
        public delegate void Func(string x);
        private static Func Recognised;
        public static void Load()
        {
            sre.UnloadAllGrammars();
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += sre_SpeechRecognized;
            sre.SpeechRecognitionRejected += sre_SpeechRecognitionRejected;
        }

        public static void StopListeningExcept(Choices ch, Func RaiseFunc)
        {
            sre.UnloadAllGrammars();
            ch.Add(new string[] { "Hello David", "Hello Zira" });
            WatchForAndRaise(ch, RaiseFunc);
            Mouth.Speak("here when you need me");
        }

        public static void WatchFor(string[] Sentences)
        {
            sre.UnloadAllGrammars();
            Choices choice = new Choices(Sentences);
            choice.Add(new string[] { "Hello David", "Hello Zira" });
            sre.LoadGrammar(new Grammar(new GrammarBuilder()));
            sre.RecognizeAsync(RecognizeMode.Single);
        }

        public static void WatchForAndRaise(Choices ch, Func RaiseFunction)
        {
            Recognised = RaiseFunction;
            sre.UnloadAllGrammars();
            ch.Add(new string[] { "Hello David", "Hello Zira" });
            sre.Dispose();
            sre = new SpeechRecognitionEngine();
            Load();
            sre.LoadGrammar(new Grammar(new GrammarBuilder(ch)));
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        static void sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Mouth.Say("Pardon!?");
        }

        static void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "Hello David")
            {
                Mouth.SetSpeaker("Microsoft David Desktop");
                Mouth.Speak("Hello " + Environment.MachineName);
                return;
            }
            else if (e.Result.Text == "Hello Zira")
            {
                Mouth.SetSpeaker("Microsoft Zira Desktop");
                Mouth.Speak("Hello " + Environment.MachineName);
                return;
            }
            Recognised(e.Result.Text);
        }

    }
}
