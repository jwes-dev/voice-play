using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace Speechh
{
    class VoicePlayer
    {
        private List<IWMPMedia> songs = new List<IWMPMedia>();

        WindowsMediaPlayer p = new WindowsMediaPlayer();
        IWMPPlaylistArray pl;

        int i;
        bool playall;


        public VoicePlayer()
        {
            Mouth.Load();
            Ear.Load();
            pl = p.playlistCollection.getAll();
            p.settings.volume = 20;
            playall = false;
            i = 0;
            loadSongs();
        }

        public void loadSongs()
        {
            Choices ch = new Choices();
            for (int i = 0; i < pl.Item(0).count; i++)
            {
                try
                {
                    StreamReader sr = new StreamReader(pl.Item(0).Item[i].sourceURL);
                    songs.Add(pl.Item(0).Item[i]);
                }
                catch
                {
                }
                ch.Add(Path.GetFileNameWithoutExtension(songs.Last().sourceURL));
                Console.WriteLine(Path.GetFileNameWithoutExtension(songs.Last().sourceURL));
            }
            string[] choices = new string[] { "next song", "loop this", "play silently", "play in full sound", "previous song", "play all", "play music", "pause music", "stop music", "Show volume control", "stop listening" };
            ch.Add(choices);
            Ear.WatchForAndRaise(ch, Identified);
            Mouth.Say("$>Song name please: ");
        }

        /// <summary>
        /// Song recognised
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Identified(string resp)
        {
            Console.WriteLine(resp);

            if (resp == "play all")
            {
                p.currentPlaylist = pl.Item(0);
                Mouth.Say(p.currentMedia.name);
                p.controls.play();
                playall = true;
                p.CurrentItemChange += p_CurrentItemChange;
                return;
            }
            if (resp == "pause music")
            {
                p.controls.pause();
            }
            else if (resp == "stop music")
            {
                p.controls.stop();
            }
            else if (resp == "play music")
            {
                p.controls.play();
            }
            else if (resp == "next song")
            {
                if (playall)
                    p.controls.next();
                else if (i < songs.Count() - 1)
                {
                    i++;
                    p.URL = songs.ElementAt(i).sourceURL;
                    Mouth.Say("playing " + Path.GetFileNameWithoutExtension(songs.ElementAt(i).sourceURL));
                    p.controls.play();
                }
            }
            else if (resp == "previous song")
            {
                if (playall)
                    p.controls.previous();
                else if (i > 0)
                {
                    i--;
                    p.URL = songs.ElementAt(i).sourceURL;
                    Mouth.Say("playing " + Path.GetFileNameWithoutExtension(songs.ElementAt(i).sourceURL));
                    p.controls.play();
                }
            }
            else if (resp == "loop this")
            {
                Mouth.Say("This song has been looped");
                p.settings.playCount = 50;
            }
            else if (resp == "play silently")
            {
                p.settings.volume = 20;
            }
            else if (resp == "play in full sound")
            {
                p.settings.volume = 100;
            }
            else if (resp == "stop listening")
            {
                Ear.StopListeningExcept(new Choices(new string[] { "start listening" }), Identified);
            }
            else if (resp == "start listening")
            {
                loadSongs();
            }
            else
            {
                for (i = 0; i < pl.Item(0).count; i++)
                {
                    if (Path.GetFileNameWithoutExtension(songs.ElementAt(i).sourceURL) == resp)
                    {
                        p.URL = songs.ElementAt(i).sourceURL;
                        Mouth.Say("playing " + resp);
                        p.controls.play();
                        break;
                    }
                }
                playall = false;
            }

        }
        /// <summary>
        /// When song changes
        /// </summary>
        /// <param name="pdispMedia"></param>
        void p_CurrentItemChange(object pdispMedia)
        {
            Mouth.Say("playing " + p.currentMedia.name);
        }

    }
}
