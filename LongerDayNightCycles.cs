namespace LongerDayNightCycles
{
    using System;
    using Harmony;
    using JetBrains.Annotations;
    using UnityEngine;
    using UnityEngine.AzureSky;

    [ModTitle("LongerDayNightCycles")]
    [ModDescription("Gives you the ability to change the duration of a day.")]
    [ModAuthor("janniksam")]
    [ModIconUrl("https://raw.githubusercontent.com/janniksam/RaftMod.LongerDayNightCycles/master/longerdaynightcycles.png")]
    [ModWallpaperUrl("https://raw.githubusercontent.com/janniksam/RaftMod.LongerDayNightCycles/master/longerdaynightcycles.png")]
    [ModVersionCheckUrl("https://www.raftmodding.com/api/v1/mods/longerdaynightcycles/version.txt")]
    [ModVersion("1.0")]
    [RaftVersion("Update 11 (4677160)")]
    [ModIsPermanent(false)]
    public class LongerDayNightCycles : Mod
    {
        private const string ModNamePrefix = "<color=#42a7f5>LongerDay</color><color=#FF0000>NightCycles</color>";
        private const string DayLengthArgumentsAreInvalid = "daylength: Needs one parameter.\n" +
                                                            "e.g. use \"daylength 60\" sets every day to be 60 minutes long. Default length is 20 minutes.";
        private const string DayLengthArgumentAreOutOfRange = "daylength: The lenght needs to be 1 and 90.";

        private float m_dayLength;
        private float m_elapsed;

        [UsedImplicitly]
        public void Start()
        {
            RConsole.Log(string.Format("{0} has been loaded!", ModNamePrefix));
            RConsole.registerCommand(typeof(LongerDayNightCycles),
                "Gives you the ability to change the duration of a day.",
                "daylength",
                SetDaylength);
        }

        private void SetDaylength()
        {
            var args = RConsole.lcargs;
            if (args.Length != 2)
            {
                RConsole.Log(DayLengthArgumentsAreInvalid);
                return;
            }

            float length;
            if ((!float.TryParse(args[1], out length)))
            {
                RConsole.Log(DayLengthArgumentAreOutOfRange);
                return;
            }

            if (length < 1 ||
                length > 90)
            {
                RConsole.Log(DayLengthArgumentAreOutOfRange);
                return;
            }

            m_dayLength = length;
            RConsole.Log(string.Format("{0}: Sucessfully set daylength to {1} minutes.", ModNamePrefix, m_dayLength));
        }

        [UsedImplicitly]
        public void Update()
        {
            if (Semih_Network.InLobbyScene)
            {
                return;
            }

            m_elapsed += Time.deltaTime;
            if (m_elapsed < 2)
            {
                return;
            }

            m_elapsed = 0;

            var skyController = FindObjectOfType<AzureSkyController>();
            if (skyController == null || 
                skyController.timeOfDay == null ||
                !(Math.Abs(skyController.timeOfDay.dayCycle - m_dayLength) > 0.01))
            {
                return;
            }

            skyController.timeOfDay.dayCycle = m_dayLength;
            Traverse.Create(skyController).Field<float>("m_timeProgression").Value =
                skyController.timeOfDay.GetDayLength();
        }

        [UsedImplicitly]
        public void OnModUnload()
        {
            RConsole.Log(string.Format("{0} has been unloaded!", ModNamePrefix));
            Destroy(gameObject);
        }
    }
}
