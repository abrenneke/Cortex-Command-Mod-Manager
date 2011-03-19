using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CortexCommandModManager.MVVM.Utilities;
using CortexCommandModManager.MVVM.WindowViewModel.GameSettingsTab;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Reflection;

namespace CortexCommandModManager.MVVM.WindowViewModel
{
    public class SettingsTabViewModel : ViewModel
    {
        public GameSettings Settings { get; set; }

        private GameSettingsManager gameSettings;
        private SkirmishSettingsManager skirmishSettings;

        public SettingsTabViewModel(GameSettingsManager gameSettings, SkirmishSettingsManager skirmishSettings)
        {
            this.gameSettings = gameSettings;
            this.skirmishSettings = skirmishSettings;

            Settings = new GameSettings();

            BindSkirmishSetting(x => x.EasiestEnemySpawnRate, x => x.SpawnIntervalEasiest);
            BindSkirmishSetting(x => x.HardestEnemySpawnRate, x => x.SpawnIntervalHardest);
            BindSkirmishSetting(x => x.Player1StartingGold, x => x.InitialP1Money);
            BindSkirmishSetting(x => x.Player2StartingGold, x => x.InitialP2Money);

            BindGameSetting(x => x.GameXResolution, x => x.ResolutionX);
            BindGameSetting(x => x.GameYResolution, x => x.ResolutionY);
            BindGameSetting(x => x.IsFullscreen, x => x.Fullscreen);

            Settings.LoadFrom(gameSettings);
            Settings.LoadFrom(skirmishSettings);
        }

        private void BindGameSetting<T1>(Expression<Func<GameSettings, T1>> settingsExpression, Expression<Func<GameSettingsManager, T1>> managerExpression)
        {
            Bind<T1, GameSettingsManager>(settingsExpression, managerExpression, gameSettings);
        }

        private void BindSkirmishSetting<T1>(Expression<Func<GameSettings, T1>> settingsExpression, Expression<Func<SkirmishSettingsManager, T1>> managerExpression)
        {
            Bind<T1, SkirmishSettingsManager>(settingsExpression, managerExpression, skirmishSettings);
        }

        private void Bind<T1, T2>(Expression<Func<GameSettings, T1>> settingsExpression, Expression<Func<T2, T1>> managerExpression, T2 invokeObject)
        {
            var settingsPropertyName = ((MemberExpression)settingsExpression.Body).Member.Name;

            var managerProperty = (PropertyInfo)((MemberExpression)managerExpression.Body).Member;
            var setMethod = managerProperty.GetSetMethod();

            var paramA = Expression.Parameter(typeof(object), "sender");
            var paramB = Expression.Parameter(typeof(PropertyChangedEventArgs), "e");
            
            Expression<Func<PropertyChangedEventArgs, string>> getNameExpression = x => x.PropertyName;

            var lambda = Expression.Lambda<PropertyChangedEventHandler>(
                    Expression.IfThen(
                        Expression.Equal(
                            Expression.Invoke(getNameExpression, paramB),
                            Expression.Constant(settingsPropertyName)),
                        Expression.Call(
                            Expression.Constant(invokeObject),
                            setMethod,
                            Expression.Invoke(settingsExpression,
                                Expression.Constant(Settings))
                            )),
                    paramA,
                    paramB);

            Settings.PropertyChanged += lambda.Compile();
        }
    }
}
