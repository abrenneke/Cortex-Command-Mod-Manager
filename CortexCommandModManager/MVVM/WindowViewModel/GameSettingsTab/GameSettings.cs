using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CortexCommandModManager.MVVM.Utilities;

namespace CortexCommandModManager.MVVM.WindowViewModel.GameSettingsTab
{
    public class GameSettings : ViewModel
    {
        /// <summary>Gets or sets the starting gold of player 1</summary>
        public int Player1StartingGold { get { return player1StartingGold; } set { player1StartingGold = value; OnPropertyChanged(x => Player1StartingGold); } }
        private int player1StartingGold;

        /// <summary>Gets or sets the starting gold of player 2.</summary>
        public int Player2StartingGold { get { return player2StartingGold; } set { player2StartingGold = value; OnPropertyChanged(x => Player2StartingGold); } }
        private int player2StartingGold;

        
        /// <summary>Gets or sets the rate of the easiest difficulty.</summary>
        public int EasiestEnemySpawnRate { get { return easiestEnemySpawnRate; } set { easiestEnemySpawnRate = value; OnPropertyChanged(x => EasiestEnemySpawnRate); } }
        private int easiestEnemySpawnRate;

        /// <summary>Gets or sets the rate of the hardest difficulty.</summary>
        public int HardestEnemySpawnRate { get { return hardestEnemySpawnRate; } set { hardestEnemySpawnRate = value; OnPropertyChanged(x => HardestEnemySpawnRate); } }
        private int hardestEnemySpawnRate;


        /// <summary>Gets or sets the X resolution of the game.</summary>
        public int GameXResolution { get { return gameXResolution; } set { gameXResolution = value; OnPropertyChanged(x => GameXResolution); } }
        private int gameXResolution;

        /// <summary>Gets or sets the Y resolution of the game.</summary>
        public int GameYResolution { get { return gameYResolution; } set { gameYResolution = value; OnPropertyChanged(x => GameYResolution); } }
        private int gameYResolution;


        /// <summary>Gets or sets whether the game is to run in fullscreen.</summary>
        public bool IsFullscreen { get { return isFullscreen; } set { isFullscreen = value; OnPropertyChanged(x => IsFullscreen); } }
        private bool isFullscreen;

        public void LoadFrom(SkirmishSettingsManager skirmishSettings)
        {
            Player1StartingGold = skirmishSettings.InitialP1Money;
            Player2StartingGold = skirmishSettings.InitialP2Money;
            EasiestEnemySpawnRate = skirmishSettings.SpawnIntervalEasiest;
            HardestEnemySpawnRate = skirmishSettings.SpawnIntervalHardest;
        }

        public void LoadFrom(GameSettingsManager gameSettings)
        {
            IsFullscreen = gameSettings.Fullscreen;
            GameXResolution = gameSettings.ResolutionX;
            GameYResolution = gameSettings.ResolutionY;
        }
    }
}
